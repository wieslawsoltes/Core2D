// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Xml.Linq;
using Core2D.Model;
using Core2D.Model.Path;
using Core2D.Model.Style;
using Core2D.ViewModels;
using Core2D.ViewModels.Layout;
using Core2D.ViewModels.Path;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.Modules.Renderer.OpenXml;

public sealed class VisioImporter : IVisioImporter
{
    private const double PageSpacing = 48.0;
    private const double PixelsPerInch = 96.0;
    private static readonly XNamespace VisioNs = "http://schemas.microsoft.com/office/visio/2012/main";
    private static readonly XNamespace RelNs = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";

    private readonly IViewModelFactory? _viewModelFactory;
    private readonly ILog? _log;
    private TextStyleViewModel? _defaultTextStyle;
    private ArrowStyleViewModel? _arrowStyle;
    private IViewModelFactory Factory => _viewModelFactory ?? throw new InvalidOperationException("Visio importer requires a view model factory.");

    public VisioImporter(IServiceProvider? serviceProvider)
    {
        _viewModelFactory = serviceProvider?.GetService<IViewModelFactory>();
        _log = serviceProvider?.GetService<ILog>();
    }

    public VisioImportResult? Import(Stream stream)
    {
        if (_viewModelFactory is null)
        {
            return null;
        }

        var buffer = ReadAllBytes(stream);
        if (buffer.Length == 0)
        {
            return new VisioImportResult(new List<BaseShapeViewModel>(), new List<BlockShapeViewModel>(), new List<ShapeStyleViewModel>());
        }

        try
        {
            using var memory = new MemoryStream(buffer, writable: false);
            using var package = Package.Open(memory, FileMode.Open, FileAccess.Read);

            var documentInfo = LoadDocumentInfo(package);
            var masterInfos = LoadMasters(package, documentInfo).ToList();
            var masterLookup = new Dictionary<int, VisioMasterUsage>();
            var pages = LoadPages(package, documentInfo).ToList();
            if (pages.Count == 0)
            {
                return new VisioImportResult(new List<BaseShapeViewModel>(), new List<BlockShapeViewModel>(), new List<ShapeStyleViewModel>());
            }

            var shapes = new List<BaseShapeViewModel>();
            var styles = new List<ShapeStyleViewModel>();
            var styleCache = new Dictionary<VisioStyleKey, ShapeStyleViewModel>();
            var textStyleCache = new Dictionary<VisioTextStyleKey, TextStyleViewModel>();
            var styledTextCache = new Dictionary<VisioStyledTextKey, ShapeStyleViewModel>();
            var blocks = BuildBlocks(masterInfos, styleCache, textStyleCache, styledTextCache, styles, masterLookup);

            double pageOffset = 0.0;
            foreach (var page in pages)
            {
                page.StackOffsetPixels = pageOffset;
                CollectShapes(
                    page.Document.Root?.Element(VisioNs + "Shapes"),
                    page,
                    VisioTransformContext.Identity,
                    shapes,
                    styleCache,
                    textStyleCache,
                    styledTextCache,
                    styles,
                    masterLookup);

                pageOffset += (page.HeightInches * PixelsPerInch) + PageSpacing;
            }

            return new VisioImportResult(shapes, blocks, styles);
        }
        catch (Exception ex)
        {
            _log?.LogError($"Visio importer failed: {ex.Message}");
            return null;
        }
    }

    private IList<BlockShapeViewModel> BuildBlocks(
        IEnumerable<VisioMasterInfo> masters,
        IDictionary<VisioStyleKey, ShapeStyleViewModel> styleCache,
        IDictionary<VisioTextStyleKey, TextStyleViewModel> textStyleCache,
        IDictionary<VisioStyledTextKey, ShapeStyleViewModel> styledTextCache,
        IList<ShapeStyleViewModel> styles,
        IDictionary<int, VisioMasterUsage> masterLookup)
    {
        var blocks = new List<BlockShapeViewModel>();

        foreach (var master in masters)
        {
            var shapes = new List<BaseShapeViewModel>();
            var pageInfo = new VisioPageInfo(master.Name, master.WidthInches, master.HeightInches, master.Document, master.DocumentInfo)
            {
                StackOffsetPixels = 0.0
            };

            var container = master.Document.Root?.Element(VisioNs + "Shapes");
            if (container is null)
            {
                continue;
            }

            foreach (var shapeElement in container.Elements(VisioNs + "Shape"))
            {
                foreach (var shape in CreateShapes(shapeElement, pageInfo, VisioTransformContext.Identity, styleCache, textStyleCache, styledTextCache, styles, null))
                {
                    shapes.Add(shape);
                }
            }

            if (shapes.Count == 0)
            {
                continue;
            }

            NormalizeBlockShapes(shapes);
            var block = Factory.CreateBlockShape(SanitizeName(master.Name, $"Master {master.Id}"));
            foreach (var shape in shapes)
            {
                block.AddShape(shape);
            }

            blocks.Add(block);
            masterLookup[master.Id] = new VisioMasterUsage(block, master.WidthPixels, master.HeightPixels);
        }

        return blocks;
    }

    private static void NormalizeBlockShapes(IList<BaseShapeViewModel> shapes)
    {
        if (shapes.Count == 0)
        {
            return;
        }

        var boxes = shapes.Select(s => new ShapeBox(s)).ToList();
        if (boxes.Count == 0)
        {
            return;
        }

        var minX = boxes.Min(b => b.Bounds.Left);
        var minY = boxes.Min(b => b.Bounds.Top);

        if (minX == 0m && minY == 0m)
        {
            return;
        }

        foreach (var shape in shapes)
        {
            shape.Move(null, -minX, -minY);
        }
    }

    private void CollectShapes(
        XElement? container,
        VisioPageInfo page,
        VisioTransformContext context,
        IList<BaseShapeViewModel> shapes,
        IDictionary<VisioStyleKey, ShapeStyleViewModel> styleCache,
        IDictionary<VisioTextStyleKey, TextStyleViewModel> textStyleCache,
        IDictionary<VisioStyledTextKey, ShapeStyleViewModel> styledTextCache,
        IList<ShapeStyleViewModel> styles,
        IDictionary<int, VisioMasterUsage>? masterLookup)
    {
        if (container is null)
        {
            return;
        }

        foreach (var shapeElement in container.Elements(VisioNs + "Shape"))
        {
            if (string.Equals(shapeElement.Attribute("Del")?.Value, "1", StringComparison.Ordinal))
            {
                continue;
            }

            foreach (var shape in CreateShapes(shapeElement, page, context, styleCache, textStyleCache, styledTextCache, styles, masterLookup))
            {
                shapes.Add(shape);
            }

            var childContainer = shapeElement.Element(VisioNs + "Shapes");
            if (childContainer is not null)
            {
                var childCells = CreateCellLookup(shapeElement);
                var childPlacement = CreatePlacement(childCells, context);
                var childContext = childPlacement?.CreateChildContext() ?? context;
                CollectShapes(childContainer, page, childContext, shapes, styleCache, textStyleCache, styledTextCache, styles, masterLookup);
            }
        }
    }

    private IEnumerable<BaseShapeViewModel> CreateShapes(
        XElement shapeElement,
        VisioPageInfo page,
        VisioTransformContext context,
        IDictionary<VisioStyleKey, ShapeStyleViewModel> styleCache,
        IDictionary<VisioTextStyleKey, TextStyleViewModel> textStyleCache,
        IDictionary<VisioStyledTextKey, ShapeStyleViewModel> styledTextCache,
        IList<ShapeStyleViewModel> styles,
        IDictionary<int, VisioMasterUsage>? masterLookup)
    {
        var cells = CreateCellLookup(shapeElement);
        var shapeStyleInfo = CreateShapeStyleInfo(cells);
        var styleKey = CreateStyleKey(shapeStyleInfo);
        var style = ResolveStyle(shapeStyleInfo, styleKey, styleCache, styles);
        var shapeName = shapeElement.Attribute("Name")?.Value
                        ?? shapeElement.Attribute("NameU")?.Value
                        ?? "Shape";

        if (masterLookup is { } && TryCreateInsertFromMaster(shapeElement, cells, page, masterLookup, shapeName, out var insertShape))
        {
            if (insertShape is { })
            {
                yield return insertShape;
            }
            yield break;
        }

        if (HasLineEndpoints(cells) && TryCreateLine(cells, page, context, style, shapeStyleInfo, shapeName) is { } line)
        {
            yield return line;
            yield break;
        }

        var placement = CreatePlacement(cells, context);
        if (placement is null)
        {
            yield break;
        }

        var placementValue = placement.Value;

        var path = TryCreatePathShape(shapeElement, page, context, placementValue, style, shapeStyleInfo, shapeName);
        if (path is not null)
        {
            yield return path;
        }
        else
        {
            foreach (var primitive in CreatePrimitiveShapes(shapeElement, page, placementValue, style, shapeStyleInfo, shapeName))
            {
                yield return primitive;
            }
        }

        var text = ExtractText(shapeElement);
        if (!string.IsNullOrWhiteSpace(text))
        {
            var textInfo = CreateTextStyleInfo(shapeElement, page, cells);
            var textStyle = textInfo is null
                ? style
                : ResolveTextAwareStyle(style, styleKey, textInfo.Value, textStyleCache, styledTextCache, styles);

            var textShape = Factory.CreateTextShape(
                ToPixelX(placementValue.Left),
                ToPixelY(page, placementValue.Top),
                ToPixelX(placementValue.Right),
                ToPixelY(page, placementValue.Bottom),
                textStyle,
                text.Trim(),
                isStroked: false,
                name: $"{shapeName}_Text");
            textShape.IsFilled = false;
            textShape.IsStroked = false;
            yield return textShape;
        }
    }

    private IEnumerable<BaseShapeViewModel> CreatePrimitiveShapes(
        XElement shapeElement,
        VisioPageInfo page,
        VisioShapePlacement placement,
        ShapeStyleViewModel style,
        VisioShapeStyleInfo styleInfo,
        string shapeName)
    {
        var geometryType = DetermineGeometry(shapeElement);
        var left = ToPixelX(placement.Left);
        var right = ToPixelX(placement.Right);
        var top = ToPixelY(page, placement.Top);
        var bottom = ToPixelY(page, placement.Bottom);

        BaseShapeViewModel? shape = geometryType switch
        {
            VisioGeometryKind.Ellipse => Factory.CreateEllipseShape(left, top, right, bottom, style, styleInfo.IsStroked, styleInfo.IsFilled, shapeName),
            _ => Factory.CreateRectangleShape(left, top, right, bottom, style, styleInfo.IsStroked, styleInfo.IsFilled, shapeName)
        };

        if (shape is not null && (styleInfo.IsFilled || styleInfo.IsStroked))
        {
            shape.IsFilled = styleInfo.IsFilled;
            shape.IsStroked = styleInfo.IsStroked;
            shape.Name = shapeName;
            yield return shape;
        }
    }

    private LineShapeViewModel? TryCreateLine(
        IDictionary<string, string> cells,
        VisioPageInfo page,
        VisioTransformContext context,
        ShapeStyleViewModel style,
        VisioShapeStyleInfo styleInfo,
        string shapeName)
    {
        if (!TryGetDouble(cells, "BeginX", out var beginX)
            || !TryGetDouble(cells, "BeginY", out var beginY)
            || !TryGetDouble(cells, "EndX", out var endX)
            || !TryGetDouble(cells, "EndY", out var endY))
        {
            return null;
        }

        var (startX, startY) = context.Apply(beginX, beginY);
        var (endXValue, endYValue) = context.Apply(endX, endY);

        var line = Factory.CreateLineShape(
            ToPixelX(startX),
            ToPixelY(page, startY),
            ToPixelX(endXValue),
            ToPixelY(page, endYValue),
            style,
            styleInfo.IsStroked,
            shapeName);
        line.IsStroked = styleInfo.IsStroked;
        line.IsFilled = false;
        return line;
    }

    private bool TryCreateInsertFromMaster(
        XElement shapeElement,
        IDictionary<string, string> cells,
        VisioPageInfo page,
        IDictionary<int, VisioMasterUsage> masterLookup,
        string shapeName,
        out InsertShapeViewModel? insert)
    {
        insert = null;
        var masterAttr = shapeElement.Attribute("Master")?.Value;
        if (string.IsNullOrWhiteSpace(masterAttr) || !int.TryParse(masterAttr, out var masterId))
        {
            return false;
        }

        if (!masterLookup.TryGetValue(masterId, out var usage))
        {
            return false;
        }

        if (!TryGetDouble(cells, "PinX", out var pinXInches)
            || !TryGetDouble(cells, "PinY", out var pinYInches))
        {
            return false;
        }

        var widthPixels = usage.WidthPixels;
        if (TryGetDouble(cells, "Width", out var widthInches))
        {
            widthPixels = widthInches * PixelsPerInch;
        }

        var heightPixels = usage.HeightPixels;
        if (TryGetDouble(cells, "Height", out var heightInches))
        {
            heightPixels = heightInches * PixelsPerInch;
        }

        var centerXPx = pinXInches * PixelsPerInch;
        var centerYPx = ToPixelY(page, pinYInches);
        var left = centerXPx - (widthPixels / 2.0);
        var top = centerYPx - (heightPixels / 2.0);

        var insertShape = Factory.CreateInsertShape(usage.Block, left, top);
        insertShape.Name = shapeName;
        insert = insertShape;
        return true;
    }

    private PathShapeViewModel? TryCreatePathShape(
        XElement shapeElement,
        VisioPageInfo page,
        VisioTransformContext context,
        VisioShapePlacement placement,
        ShapeStyleViewModel style,
        VisioShapeStyleInfo styleInfo,
        string shapeName)
    {
        var sections = shapeElement.Elements(VisioNs + "Section")
            .Where(s => string.Equals(s.Attribute("N")?.Value, "Geometry", StringComparison.OrdinalIgnoreCase))
            .ToList();
        if (sections.Count == 0)
        {
            return null;
        }

        var path = Factory.CreatePathShape(shapeName, style, ImmutableArray<PathFigureViewModel>.Empty, FillRule.Nonzero, styleInfo.IsStroked, styleInfo.IsFilled);
        var geometry = Factory.CreateGeometryContext(path);
        var hasFigure = false;

        foreach (var section in sections)
        {
            foreach (var row in section.Elements(VisioNs + "Row"))
            {
                var type = row.Attribute("T")?.Value;
                if (string.IsNullOrWhiteSpace(type))
                {
                    continue;
                }

                var isRelative = type.StartsWith("Rel", StringComparison.OrdinalIgnoreCase);
                switch (type)
                {
                    case "MoveTo":
                    case "RelMoveTo":
                    {
                        if (TryCreatePoint(row, isRelative, page, placement, context, out var movePoint))
                        {
                            geometry.BeginFigure(movePoint, isClosed: false);
                            hasFigure = true;
                        }
                        break;
                    }
                    case "LineTo":
                    case "RelLineTo":
                    {
                        if (TryCreatePoint(row, isRelative, page, placement, context, out var linePoint))
                        {
                            geometry.LineTo(linePoint);
                        }
                        break;
                    }
                    case "NURBSTo":
                    case "RelNURBSTo":
                    case "SplineKnot":
                    case "RelSplineKnot":
                    {
                        if (TryCreatePoint(row, isRelative, page, placement, context, out var nurbsPoint))
                        {
                            geometry.LineTo(nurbsPoint);
                        }
                        break;
                    }
                    case "PolylineTo":
                    case "RelPolylineTo":
                    {
                        if (TryCreatePoint(row, isRelative, page, placement, context, out var polyPoint))
                        {
                            geometry.LineTo(polyPoint);
                        }
                        break;
                    }
                }
            }
        }

        return hasFigure ? path : null;
    }

    private bool TryCreatePoint(
        XElement row,
        bool isRelative,
        VisioPageInfo page,
        VisioShapePlacement placement,
        VisioTransformContext context,
        out PointShapeViewModel point)
    {
        point = default!;
        if (!TryGetRowDouble(row, "X", out var xValue) || !TryGetRowDouble(row, "Y", out var yValue))
        {
            return false;
        }

        double absoluteX;
        double absoluteY;
        if (isRelative)
        {
            absoluteX = placement.Left + (xValue * placement.Width);
            absoluteY = placement.Bottom + (yValue * placement.Height);
        }
        else
        {
            (absoluteX, absoluteY) = context.Apply(xValue, yValue);
        }

        point = Factory.CreatePointShape(ToPixelX(absoluteX), ToPixelY(page, absoluteY));
        return true;
    }

    private ShapeStyleViewModel ResolveStyle(
        VisioShapeStyleInfo info,
        VisioStyleKey key,
        IDictionary<VisioStyleKey, ShapeStyleViewModel> cache,
        IList<ShapeStyleViewModel> styles)
    {
        if (cache.TryGetValue(key, out var existing))
        {
            return existing;
        }

        var strokeColor = ExtractArgb(info.StrokeColor);
        var fillColor = ExtractArgb(info.FillColor);

        var stroke = Factory.CreateArgbColor(strokeColor.A, strokeColor.R, strokeColor.G, strokeColor.B);
        var fill = Factory.CreateArgbColor(fillColor.A, fillColor.R, fillColor.G, fillColor.B);

        var textStyle = _defaultTextStyle ??= Factory.CreateTextStyle();
        var arrow = _arrowStyle ??= Factory.CreateArrowStyle();

        var thickness = info.StrokeThickness <= 0.0 ? 1.0 : info.StrokeThickness;

        var style = Factory.CreateShapeStyle(string.Empty, stroke, fill, thickness, textStyle, arrow, arrow);
        cache[key] = style;
        styles.Add(style);
        return style;
    }

    private ShapeStyleViewModel ResolveTextAwareStyle(
        ShapeStyleViewModel baseStyle,
        VisioStyleKey styleKey,
        VisioTextStyleInfo textInfo,
        IDictionary<VisioTextStyleKey, TextStyleViewModel> textStyleCache,
        IDictionary<VisioStyledTextKey, ShapeStyleViewModel> styledTextCache,
        IList<ShapeStyleViewModel> styles)
    {
        var textKey = new VisioTextStyleKey(textInfo.FontName, textInfo.FontSize, textInfo.FontStyle, textInfo.HorizontalAlignment, textInfo.VerticalAlignment, textInfo.Underline);
        var styledKey = new VisioStyledTextKey(styleKey, textKey);

        if (styledTextCache.TryGetValue(styledKey, out var cached))
        {
            return cached;
        }

        var textStyle = GetOrCreateTextStyle(textKey, textStyleCache);
        var clone = CloneStyle(baseStyle);
        clone.TextStyle = textStyle;
        styledTextCache[styledKey] = clone;
        styles.Add(clone);
        return clone;
    }

    private TextStyleViewModel GetOrCreateTextStyle(VisioTextStyleKey key, IDictionary<VisioTextStyleKey, TextStyleViewModel> cache)
    {
        if (cache.TryGetValue(key, out var existing))
        {
            return existing;
        }

        var textStyle = Factory.CreateTextStyle();
        textStyle.FontName = key.FontName;
        textStyle.FontSize = key.FontSize;
        textStyle.FontStyle = key.FontStyle;
        textStyle.TextHAlignment = key.HorizontalAlignment;
        textStyle.TextVAlignment = key.VerticalAlignment;
        textStyle.Underline = key.Underline;
        cache[key] = textStyle;
        return textStyle;
    }

    private ShapeStyleViewModel CloneStyle(ShapeStyleViewModel source)
    {
        var shared = new Dictionary<object, object>();
        return (ShapeStyleViewModel)source.Copy(shared);
    }

    private VisioShapePlacement? CreatePlacement(IDictionary<string, string> cells, VisioTransformContext context)
    {
        if (!TryGetDouble(cells, "PinX", out var pinX)
            || !TryGetDouble(cells, "PinY", out var pinY)
            || !TryGetDouble(cells, "Width", out var widthInches)
            || !TryGetDouble(cells, "Height", out var heightInches))
        {
            return null;
        }

        var absolutePin = context.Apply(pinX, pinY);
        var width = Math.Abs(widthInches);
        var height = Math.Abs(heightInches);
        return new VisioShapePlacement(absolutePin.X, absolutePin.Y, width, height);
    }

    private VisioShapeStyleInfo CreateShapeStyleInfo(IDictionary<string, string> cells)
    {
        var linePattern = TryGetDouble(cells, "LinePattern", out var lp) ? lp : 1.0;
        var fillPattern = TryGetDouble(cells, "FillPattern", out var fp) ? fp : 1.0;
        var hasNoLine = TryGetDouble(cells, "NoLine", out var nl) && nl > 0.5;
        var hasNoFill = TryGetDouble(cells, "NoFill", out var nf) && nf > 0.5;

        var isStroked = !hasNoLine && linePattern > 0.0;
        var isFilled = !hasNoFill && fillPattern > 0.0;

        var strokeThicknessInches = TryGetDouble(cells, "LineWeight", out var lw) ? lw : 1.0 / PixelsPerInch;
        var strokeThickness = Math.Max(strokeThicknessInches * PixelsPerInch, 0.25);

        var strokeColor = ParseColor(cells.TryGetValue("LineColor", out var lc) ? lc : null,
            alpha: 1.0 - Clamp(TryGetDouble(cells, "LineColorTrans", out var lt) ? lt : 0.0));
        var fillColor = ParseColor(cells.TryGetValue("FillForegnd", out var fc) ? fc : null,
            alpha: 1.0 - Clamp(TryGetDouble(cells, "FillForegndTrans", out var ft) ? ft : 0.0));

        return new VisioShapeStyleInfo(isFilled, isStroked, strokeThickness, strokeColor, fillColor);
    }

    private static VisioStyleKey CreateStyleKey(VisioShapeStyleInfo info)
    {
        var thicknessKey = (int)Math.Round(info.StrokeThickness * 1000.0, MidpointRounding.AwayFromZero);
        return new VisioStyleKey(info.StrokeColor, info.IsStroked, info.FillColor, info.IsFilled, thicknessKey);
    }

    private VisioTextStyleInfo? CreateTextStyleInfo(XElement shapeElement, VisioPageInfo page, IDictionary<string, string> cells)
    {
        var charSection = shapeElement.Elements(VisioNs + "Section")
            .FirstOrDefault(s => string.Equals(s.Attribute("N")?.Value, "Character", StringComparison.OrdinalIgnoreCase));
        var charRow = charSection?.Elements(VisioNs + "Row").FirstOrDefault();
        if (charRow is null)
        {
            return null;
        }

        var fontIndex = TryGetRowDouble(charRow, "Font", out var fontValue) ? (int)Math.Round(fontValue, MidpointRounding.AwayFromZero) : 0;
        var fontName = page.DocumentInfo.GetFontName(fontIndex);
        var fontSize = TryGetRowDouble(charRow, "Size", out var sizeValue) ? sizeValue * 72.0 : 12.0;
        var styleFlags = TryGetRowDouble(charRow, "Style", out var styleValue) ? (int)Math.Round(styleValue, MidpointRounding.AwayFromZero) : 0;
        var underline = TryGetRowDouble(charRow, "Underline", out var underlineValue) && underlineValue > 0.5;

        var fontStyle = FontStyleFlags.Regular;
        if ((styleFlags & 1) != 0)
        {
            fontStyle |= FontStyleFlags.Bold;
        }
        if ((styleFlags & 2) != 0)
        {
            fontStyle |= FontStyleFlags.Italic;
        }

        var paragraphSection = shapeElement.Elements(VisioNs + "Section")
            .FirstOrDefault(s => string.Equals(s.Attribute("N")?.Value, "Paragraph", StringComparison.OrdinalIgnoreCase));
        var paragraphRow = paragraphSection?.Elements(VisioNs + "Row").FirstOrDefault();
        var hAlign = TryGetRowDouble(paragraphRow, "HorzAlign", out var alignValue)
            ? MapHorizontalAlignment((int)Math.Round(alignValue))
            : TextHAlignment.Center;

        var vAlign = TryGetDouble(cells, "VerticalAlign", out var verticalValue)
            ? MapVerticalAlignment((int)Math.Round(verticalValue))
            : TextVAlignment.Center;

        return new VisioTextStyleInfo(fontName, fontSize, fontStyle, hAlign, vAlign, underline);
    }

    private static TextHAlignment MapHorizontalAlignment(int value)
    {
        return value switch
        {
            0 => TextHAlignment.Left,
            1 => TextHAlignment.Center,
            2 => TextHAlignment.Right,
            3 => TextHAlignment.Center,
            _ => TextHAlignment.Left
        };
    }

    private static TextVAlignment MapVerticalAlignment(int value)
    {
        return value switch
        {
            0 => TextVAlignment.Top,
            1 => TextVAlignment.Center,
            2 => TextVAlignment.Bottom,
            _ => TextVAlignment.Center
        };
    }

    private static bool HasLineEndpoints(IDictionary<string, string> cells)
        => cells.ContainsKey("BeginX") && cells.ContainsKey("BeginY") && cells.ContainsKey("EndX") && cells.ContainsKey("EndY");

    private static VisioGeometryKind DetermineGeometry(XElement shapeElement)
    {
        var geometrySections = shapeElement.Elements(VisioNs + "Section")
            .Where(s => string.Equals(s.Attribute("N")?.Value, "Geometry", StringComparison.OrdinalIgnoreCase));

        foreach (var section in geometrySections)
        {
            foreach (var row in section.Elements(VisioNs + "Row"))
            {
                var type = row.Attribute("T")?.Value;
                if (string.Equals(type, "Ellipse", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(type, "RelEllipse", StringComparison.OrdinalIgnoreCase))
                {
                    return VisioGeometryKind.Ellipse;
                }
            }
        }

        return VisioGeometryKind.Rectangle;
    }

    private string? ExtractText(XElement shapeElement)
    {
        var textElement = shapeElement.Element(VisioNs + "Text");
        return textElement?.Value;
    }

    private static IDictionary<string, string> CreateCellLookup(XElement shapeElement)
    {
        var lookup = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var cell in shapeElement.Elements(VisioNs + "Cell"))
        {
            var name = cell.Attribute("N")?.Value;
            if (string.IsNullOrWhiteSpace(name) || lookup.ContainsKey(name))
            {
                continue;
            }

            var value = cell.Attribute("V")?.Value ?? cell.Value;
            if (!string.IsNullOrWhiteSpace(value))
            {
                lookup[name] = value;
            }
        }

        return lookup;
    }

    private VisioDocumentInfo LoadDocumentInfo(Package package)
    {
        var fonts = new Dictionary<int, string>();
        try
        {
            var documentUri = PackUriHelper.CreatePartUri(new Uri("/visio/document.xml", UriKind.Relative));
            if (package.PartExists(documentUri))
            {
                var part = package.GetPart(documentUri);
                using var stream = part.GetStream(FileMode.Open, FileAccess.Read);
                var document = XDocument.Load(stream);
                var faceNames = document.Root?.Element(VisioNs + "FaceNames")?.Elements(VisioNs + "FaceName") ?? Enumerable.Empty<XElement>();
                var index = 0;
                foreach (var faceName in faceNames)
                {
                    var name = faceName.Attribute("NameU")?.Value
                               ?? faceName.Attribute("Name")?.Value
                               ?? $"Font{index}";
                    fonts[index++] = name;
                }
            }
        }
        catch
        {
            // Ignore font parsing failures, we'll fall back to defaults.
        }

        return new VisioDocumentInfo(fonts);
    }

    private static IEnumerable<VisioMasterInfo> LoadMasters(Package package, VisioDocumentInfo documentInfo)
    {
        var mastersUri = PackUriHelper.CreatePartUri(new Uri("/visio/masters/masters.xml", UriKind.Relative));
        if (!package.PartExists(mastersUri))
        {
            yield break;
        }

        var mastersPart = package.GetPart(mastersUri);
        var mastersDocument = LoadDocument(mastersPart);
        var relationships = mastersPart.GetRelationships().ToDictionary(r => r.Id, StringComparer.Ordinal);

        foreach (var masterElement in mastersDocument.Root?.Elements(VisioNs + "Master") ?? Enumerable.Empty<XElement>())
        {
            if (!int.TryParse(masterElement.Attribute("ID")?.Value, out var id))
            {
                continue;
            }

            var relId = masterElement.Element(VisioNs + "Rel")?.Attribute(RelNs + "id")?.Value;
            if (string.IsNullOrWhiteSpace(relId) || !relationships.TryGetValue(relId, out var relationship))
            {
                continue;
            }

            var partUri = PackUriHelper.ResolvePartUri(mastersPart.Uri, relationship.TargetUri);
            if (!package.PartExists(partUri))
            {
                continue;
            }

            var masterPart = package.GetPart(partUri);
            var document = LoadDocument(masterPart);
            var width = ReadPageDimension(masterElement, "PageWidth", 1.0);
            var height = ReadPageDimension(masterElement, "PageHeight", 1.0);
            var name = masterElement.Attribute("Name")?.Value
                       ?? masterElement.Attribute("NameU")?.Value
                       ?? $"Master {id}";

            yield return new VisioMasterInfo(id, name, width, height, width * PixelsPerInch, height * PixelsPerInch, document, documentInfo);
        }
    }

    private static IEnumerable<VisioPageInfo> LoadPages(Package package, VisioDocumentInfo documentInfo)
    {
        var pagesUri = PackUriHelper.CreatePartUri(new Uri("/visio/pages/pages.xml", UriKind.Relative));
        if (!package.PartExists(pagesUri))
        {
            yield break;
        }

        var pagesPart = package.GetPart(pagesUri);
        var pagesDocument = LoadDocument(pagesPart);
        var relationships = pagesPart.GetRelationships().ToDictionary(r => r.Id, StringComparer.Ordinal);

        foreach (var pageElement in pagesDocument.Root?.Elements(VisioNs + "Page") ?? Enumerable.Empty<XElement>())
        {
            var relId = pageElement.Attribute(RelNs + "id")?.Value;
            if (string.IsNullOrWhiteSpace(relId) || !relationships.TryGetValue(relId, out var relationship))
            {
                continue;
            }

            var partUri = PackUriHelper.ResolvePartUri(pagesPart.Uri, relationship.TargetUri);
            if (!package.PartExists(partUri))
            {
                continue;
            }

            var pagePart = package.GetPart(partUri);
            var document = LoadDocument(pagePart);
            var width = ReadPageDimension(pageElement, "PageWidth", 8.5);
            var height = ReadPageDimension(pageElement, "PageHeight", 11.0);
            var name = pageElement.Attribute("Name")?.Value
                       ?? pageElement.Attribute("NameU")?.Value
                       ?? "Page";

            yield return new VisioPageInfo(name, width, height, document, documentInfo);
        }
    }

    private static XDocument LoadDocument(PackagePart part)
    {
        using var stream = part.GetStream(FileMode.Open, FileAccess.Read);
        return XDocument.Load(stream, LoadOptions.PreserveWhitespace);
    }

    private static double ReadPageDimension(XElement pageElement, string cellName, double fallback)
    {
        var sheet = pageElement.Element(VisioNs + "PageSheet");
        var cell = sheet?.Elements(VisioNs + "Cell").FirstOrDefault(x => string.Equals(x.Attribute("N")?.Value, cellName, StringComparison.OrdinalIgnoreCase));
        return cell is null ? fallback : (TryParseDouble(cell.Attribute("V")?.Value, out var value) ? value : fallback);
    }

    private static bool TryGetDouble(IDictionary<string, string> cells, string name, out double value)
    {
        if (cells.TryGetValue(name, out var stored))
        {
            return TryParseDouble(stored, out value);
        }

        value = 0;
        return false;
    }

    private static bool TryGetRowDouble(XElement? row, string name, out double value)
    {
        if (row is null)
        {
            value = 0;
            return false;
        }

        var cell = row.Elements(VisioNs + "Cell").FirstOrDefault(c => string.Equals(c.Attribute("N")?.Value, name, StringComparison.OrdinalIgnoreCase));
        if (cell is null)
        {
            value = 0;
            return false;
        }

        return TryParseDouble(cell.Attribute("V")?.Value ?? cell.Value, out value);
    }

    private static bool TryParseDouble(string? value, out double result)
        => double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out result);

    private static double Clamp(double value)
        => Math.Max(0.0, Math.Min(1.0, value));

    private static uint ParseColor(string? value, double alpha)
    {
        if (!string.IsNullOrWhiteSpace(value) && value.StartsWith("#", StringComparison.Ordinal) && value.Length == 7)
        {
            var r = byte.Parse(value.Substring(1, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            var g = byte.Parse(value.Substring(3, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            var b = byte.Parse(value.Substring(5, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            var a = (byte)Math.Round(Clamp(alpha) * 255.0, MidpointRounding.AwayFromZero);
            return (uint)(a << 24 | r << 16 | g << 8 | b);
        }

        var defaultAlpha = (byte)Math.Round(Clamp(alpha) * 255.0, MidpointRounding.AwayFromZero);
        return (uint)(defaultAlpha << 24 | 0x000000);
    }

    private static (byte A, byte R, byte G, byte B) ExtractArgb(uint color)
    {
        var a = (byte)((color >> 24) & 0xFF);
        var r = (byte)((color >> 16) & 0xFF);
        var g = (byte)((color >> 8) & 0xFF);
        var b = (byte)(color & 0xFF);
        return (a, r, g, b);
    }

    private static double ToPixelX(double inches)
        => inches * PixelsPerInch;

    private static double ToPixelY(VisioPageInfo page, double inches)
        => (page.HeightInches - inches) * PixelsPerInch + page.StackOffsetPixels;

    private static byte[] ReadAllBytes(Stream stream)
    {
        if (stream.CanSeek)
        {
            stream.Seek(0, SeekOrigin.Begin);
        }

        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    private sealed class VisioDocumentInfo
    {
        private readonly IReadOnlyDictionary<int, string> _fonts;

        public VisioDocumentInfo(IReadOnlyDictionary<int, string> fonts)
        {
            _fonts = fonts;
        }

        public string GetFontName(int index)
        {
            if (_fonts.TryGetValue(index, out var name))
            {
                return name;
            }

            return _fonts.TryGetValue(0, out var fallback) ? fallback : "Calibri";
        }
    }

    private sealed class VisioPageInfo
    {
        public VisioPageInfo(string name, double widthInches, double heightInches, XDocument document, VisioDocumentInfo documentInfo)
        {
            Name = name;
            WidthInches = widthInches;
            HeightInches = heightInches;
            Document = document;
            DocumentInfo = documentInfo;
        }

        public string Name { get; }
        public double WidthInches { get; }
        public double HeightInches { get; }
        public XDocument Document { get; }
        public VisioDocumentInfo DocumentInfo { get; }
        public double StackOffsetPixels { get; set; }
    }

    private sealed record VisioMasterInfo(int Id, string Name, double WidthInches, double HeightInches, double WidthPixels, double HeightPixels, XDocument Document, VisioDocumentInfo DocumentInfo);

    private readonly record struct VisioTransformContext(double OffsetX, double OffsetY)
    {
        public static VisioTransformContext Identity { get; } = new(0, 0);

        public (double X, double Y) Apply(double x, double y)
            => (OffsetX + x, OffsetY + y);
    }

    private readonly record struct VisioShapePlacement(double PinX, double PinY, double Width, double Height)
    {
        public double Left => PinX - (Width * 0.5);
        public double Right => PinX + (Width * 0.5);
        public double Bottom => PinY - (Height * 0.5);
        public double Top => PinY + (Height * 0.5);

        public VisioTransformContext CreateChildContext()
            => new(Left, Bottom);
    }

    private readonly record struct VisioShapeStyleInfo(bool IsFilled, bool IsStroked, double StrokeThickness, uint StrokeColor, uint FillColor);

    private readonly record struct VisioStyleKey(uint StrokeColor, bool IsStroked, uint FillColor, bool IsFilled, int Thickness);

    private readonly record struct VisioTextStyleInfo(string FontName, double FontSize, FontStyleFlags FontStyle, TextHAlignment HorizontalAlignment, TextVAlignment VerticalAlignment, bool Underline);

    private readonly record struct VisioTextStyleKey(string FontName, double FontSize, FontStyleFlags FontStyle, TextHAlignment HorizontalAlignment, TextVAlignment VerticalAlignment, bool Underline);

    private readonly record struct VisioStyledTextKey(VisioStyleKey StyleKey, VisioTextStyleKey TextKey);

    private readonly record struct VisioMasterUsage(BlockShapeViewModel Block, double WidthPixels, double HeightPixels);

    private static string SanitizeName(string? name, string fallback)
    {
        var invalid = new[] { '/', '\\', '?', '*', '[', ']', ':' };
        var result = string.IsNullOrWhiteSpace(name) ? fallback : name.Trim();
        foreach (var ch in invalid)
        {
            result = result.Replace(ch, '-');
        }
        return string.IsNullOrWhiteSpace(result) ? fallback : result;
    }

    private enum VisioGeometryKind
    {
        Rectangle,
        Ellipse
    }
}
