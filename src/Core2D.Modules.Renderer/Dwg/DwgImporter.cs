// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.IO;
using ACadSharp.Tables;
using Core2D.Model;
using Core2D.Model.Path;
using Core2D.Model.Style;
using Core2D.ViewModels;
using Core2D.ViewModels.Path;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using CSMath;
using CSUtilities.Extensions;

namespace Core2D.Modules.Renderer.Dwg;

public sealed class DwgImporter : IDwgImporter
{
    private readonly IServiceProvider? _serviceProvider;
    private readonly IViewModelFactory? _viewModelFactory;
    private readonly ILog? _log;

    public DwgImporter(IServiceProvider? serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _viewModelFactory = serviceProvider?.GetService<IViewModelFactory>();
        _log = serviceProvider?.GetService<ILog>();
    }

    public DwgImportResult? Import(Stream stream)
    {
        if (_viewModelFactory is null)
        {
            return null;
        }

        if (stream is null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        var buffer = ReadAllBytes(stream);
        if (buffer.Length == 0)
        {
            return new DwgImportResult(new List<BaseShapeViewModel>(), new List<BlockShapeViewModel>(), new List<ShapeStyleViewModel>());
        }

        var document = TryLoadDocument(buffer);
        if (document is null)
        {
            _log?.LogError("DWG importer failed to load document stream (unsupported or corrupted file).");
            return null;
        }

        var context = new ImportContext(_viewModelFactory, _log, document);

        List<BlockShapeViewModel> blocks;
        try
        {
            var builtBlocks = context.BuildBlocks();
            blocks = builtBlocks is List<BlockShapeViewModel> blockList
                ? blockList
                : new List<BlockShapeViewModel>(builtBlocks);
        }
        catch (Exception ex)
        {
            _log?.LogError($"DWG importer failed while building block library: {ex.Message}");
            blocks = new List<BlockShapeViewModel>();
        }

        List<BaseShapeViewModel> shapes;
        try
        {
            var builtShapes = context.BuildModelSpace();
            shapes = builtShapes is List<BaseShapeViewModel> shapeList
                ? shapeList
                : new List<BaseShapeViewModel>(builtShapes);
        }
        catch (Exception ex)
        {
            _log?.LogError($"DWG importer failed while building model space: {ex.Message}");
            shapes = new List<BaseShapeViewModel>();
        }

        var styles = context.CollectStyles();

        return new DwgImportResult(shapes, blocks, styles);
    }

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

    private static CadDocument? TryLoadDocument(byte[] data)
    {
        static CadDocument? Try(Func<Stream, CadDocument> loader, byte[] payload)
        {
            try
            {
                using var ms = new MemoryStream(payload, writable: false);
                return loader(ms);
            }
            catch
            {
                return null;
            }
        }

        return Try(stream => DwgReader.Read(stream), data)
               ?? Try(stream => DxfReader.Read(stream), data);
    }

    private sealed class ImportContext
    {
        private readonly IViewModelFactory _factory;
        private readonly ILog? _log;
        private readonly CadDocument _document;
        private readonly Dictionary<StyleKey, ShapeStyleViewModel> _styleCache = new();
        private readonly double _lineweightFactor = 96.0 / 2540.0;
        private readonly Dictionary<BlockRecord, BlockShapeViewModel> _blockCache = new();

        public ImportContext(IViewModelFactory factory, ILog? log, CadDocument document)
        {
            _factory = factory;
            _log = log;
            _document = document;
        }

        public IList<BaseShapeViewModel> BuildModelSpace()
        {
            var transform = CadTransform.FromEntities(_document.Entities);
            var result = new List<BaseShapeViewModel>();

            foreach (var entity in _document.Entities)
            {
                if (!IsRenderable(entity))
                {
                    continue;
                }

                try
                {
                    foreach (var shape in ConvertEntity(entity, transform, depth: 0, blockChain: null))
                    {
                        if (shape is { })
                        {
                            result.Add(shape);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log?.LogWarning($"DWG importer skipped entity {DescribeEntity(entity)}: {ex.Message}");
                }
            }

            return result;
        }

        public IList<BlockShapeViewModel> BuildBlocks()
        {
            foreach (var record in _document.BlockRecords)
            {
                ResolveBlock(record, new HashSet<BlockRecord>());
            }

            return _blockCache.Values.ToList();
        }

        public IList<ShapeStyleViewModel> CollectStyles()
        {
            return _styleCache.Values.ToList();
        }

        private static bool ShouldSkip(BlockRecord record)
        {
            if (record.Name is null)
            {
                return true;
            }

            if (record.Name.Equals(BlockRecord.ModelSpaceName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (record.Name.Equals(BlockRecord.PaperSpaceName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (record.IsAnonymous)
            {
                return true;
            }

            return false;
        }

        private static string DescribeEntity(Entity entity)
        {
            return $"{entity.ObjectName} (handle {entity.Handle:X})";
        }

        private IEnumerable<BaseShapeViewModel> ConvertEntity(Entity entity, CadTransform transform, int depth, HashSet<BlockRecord>? blockChain)
        {
            if (depth > 8)
            {
                yield break;
            }

            switch (entity)
            {
                case Line line:
                {
                    var style = ResolveStyle(line, isFilled: false);
                    var shape = CreateLine(line, style, transform);
                    if (shape is { })
                    {
                        yield return shape;
                    }
                    yield break;
                }

                case PolyfaceMesh mesh:
                {
                    _log?.LogWarning($"DWG importer skipped unsupported PolyfaceMesh entity (handle {mesh.Handle:X}).");
                    yield break;
                }

                case Polyline polyline when polyline is not PolyfaceMesh:
                {
                    foreach (var child in polyline.Explode())
                    {
                        foreach (var shape in ConvertEntity(child, transform, depth + 1, blockChain))
                        {
                            yield return shape;
                        }
                    }
                    yield break;
                }

                case LwPolyline lwPolyline:
                {
                    foreach (var child in lwPolyline.Explode())
                    {
                        foreach (var shape in ConvertEntity(child, transform, depth + 1, blockChain))
                        {
                            yield return shape;
                        }
                    }
                    yield break;
                }

                case Insert insert:
                {
                    foreach (var insertShape in CreateInsert(insert, transform, blockChain))
                    {
                        yield return insertShape;
                    }
                    yield break;
                }

                case Hatch hatch:
                {
                    foreach (var edgeEntity in hatch.Explode())
                    {
                        foreach (var shape in ConvertEntity(edgeEntity, transform, depth + 1, blockChain))
                        {
                            yield return shape;
                        }
                    }
                    yield break;
                }

                case Arc arc:
                {
                    var style = ResolveStyle(arc, isFilled: false);
                    var shape = CreateArc(arc, style, transform);
                    if (shape is { })
                    {
                        yield return shape;
                    }
                    yield break;
                }

                case Circle circle:
                {
                    var style = ResolveStyle(circle, isFilled: false);
                    var shape = CreateCircle(circle, style, transform);
                    if (shape is { })
                    {
                        yield return shape;
                    }
                    yield break;
                }

                case Ellipse ellipse:
                {
                    var isClosed = Math.Abs(ellipse.EndParameter - ellipse.StartParameter) >= Math.PI * 2.0 - 1e-3;
                    var style = ResolveStyle(ellipse, isFilled: isClosed);
                    var shape = CreateEllipse(ellipse, style, transform);
                    if (shape is { })
                    {
                        yield return shape;
                    }
                    yield break;
                }

                case Spline spline:
                {
                    var style = ResolveStyle(spline, isFilled: spline.IsClosed);
                    BaseShapeViewModel? splineShape = null;
                    try
                    {
                        splineShape = CreateSpline(spline, style, transform);
                    }
                    catch (Exception ex)
                    {
                        _log?.LogWarning($"DWG importer skipped spline (handle {spline.Handle:X}): {ex.Message}");
                    }

                    if (splineShape is { })
                    {
                        yield return splineShape;
                    }
                    yield break;
                }

                case TextEntity textEntity:
                {
                    if (!string.IsNullOrEmpty(textEntity.Value))
                    {
                        var style = ResolveStyle(textEntity, isFilled: false);
                        var shape = CreateText(textEntity, style, transform);
                        if (shape is { })
                        {
                            yield return shape;
                        }
                    }
                    yield break;
                }

                case Solid solid:
                {
                    var style = ResolveStyle(solid, isFilled: true);
                    var shape = CreateSolid(solid, style, transform);
                    if (shape is { })
                    {
                        yield return shape;
                    }
                    yield break;
                }

                case Face3D face:
                {
                    var style = ResolveStyle(face, isFilled: true);
                    var shape = CreateFace(face, style, transform);
                    if (shape is { })
                    {
                        yield return shape;
                    }
                    yield break;
                }

                default:
                {
                    var fallback = TryConvertByPolygon(entity, transform);
                    if (fallback is { })
                    {
                        yield return fallback;
                    }
                    yield break;
                }
            }
        }

        private IEnumerable<BaseShapeViewModel> CreateInsert(Insert insert, CadTransform transform, HashSet<BlockRecord>? blockChain)
        {
            var blockVm = ResolveBlock(insert.Block, blockChain);
            if (blockVm is null)
            {
                yield break;
            }

            var rowCount = Math.Max(1, (int)insert.RowCount);
            var colCount = Math.Max(1, (int)insert.ColumnCount);
            var rowSpacing = insert.RowSpacing;
            var colSpacing = insert.ColumnSpacing;

            for (int row = 0; row < rowCount; row++)
            {
                for (int col = 0; col < colCount; col++)
                {
                    var dx = insert.InsertPoint.X + col * colSpacing;
                    var dy = insert.InsertPoint.Y + row * rowSpacing;
                    var point = transform.Apply(dx, dy);
                    var name = (rowCount > 1 || colCount > 1)
                        ? $"{BuildEntityName(insert)}_{row}_{col}"
                        : BuildEntityName(insert);
                    var insertShape = _factory.CreateInsertShape(blockVm, point.X, point.Y, name);
                    yield return insertShape;
                }
            }
        }

        private BaseShapeViewModel? CreateLine(Line line, ShapeStyleViewModel style, CadTransform transform)
        {
            var start = transform.Apply(line.StartPoint);
            var end = transform.Apply(line.EndPoint);

            var shape = _factory.CreateLineShape(start.X, start.Y, end.X, end.Y, style);
            shape.Style = style;
            shape.Name = BuildEntityName(line);
            return shape;
        }

        private BaseShapeViewModel? CreateCircle(Circle circle, ShapeStyleViewModel style, CadTransform transform)
        {
            var minX = circle.Center.X - circle.Radius;
            var maxX = circle.Center.X + circle.Radius;
            var minY = circle.Center.Y - circle.Radius;
            var maxY = circle.Center.Y + circle.Radius;

            var tl = transform.Apply(minX, maxY);
            var br = transform.Apply(maxX, minY);

            var shape = _factory.CreateEllipseShape(tl.X, tl.Y, br.X, br.Y, style, isStroked: true, isFilled: false);
            shape.Style = style;
            shape.Name = BuildEntityName(circle);
            return shape;
        }

        private BaseShapeViewModel? CreateArc(Arc arc, ShapeStyleViewModel style, CadTransform transform)
        {
            arc.GetEndVertices(out var startPoint, out var endPoint);
            var start = transform.Apply(startPoint);
            var end = transform.Apply(endPoint);

            var path = _factory.CreatePathShape(style, ImmutableArray<PathFigureViewModel>.Empty, FillRule.Nonzero, isStroked: true, isFilled: false);
            var context = _factory.CreateGeometryContext(path);
            context.BeginFigure(_factory.CreatePointShape(start.X, start.Y), isClosed: false);

            var size = _factory.CreatePathSize(arc.Radius, arc.Radius);
            var sweep = NormalizeSweep(arc.StartAngle, arc.EndAngle);
            var sweepDirection = sweep >= 0 ? SweepDirection.Counterclockwise : SweepDirection.Clockwise;
            var isLargeArc = Math.Abs(sweep) > Math.PI;

            context.ArcTo(_factory.CreatePointShape(end.X, end.Y), size, rotationAngle: 0, isLargeArc, sweepDirection);
            context.SetClosedState(false);

            path.Style = style;
            path.Name = BuildEntityName(arc);
            path.IsFilled = false;
            return path;
        }

        private BaseShapeViewModel? CreateEllipse(Ellipse ellipse, ShapeStyleViewModel style, CadTransform transform)
        {
            var vertices = ellipse.PolygonalVertexes(128);
            var shape = CreatePathFromVertices(vertices, style, ellipse.IsClosed(), transform, name: BuildEntityName(ellipse));
            return shape;
        }

        private BaseShapeViewModel? CreateSpline(Spline spline, ShapeStyleViewModel style, CadTransform transform)
        {
            List<XYZ> vertices;

            try
            {
                if (spline.ControlPoints.Count == 0)
                {
                    _log?.LogWarning($"DWG importer skipped spline without control points (handle {spline.Handle:X}).");
                    return null;
                }

                if (spline.Knots.Count < 2)
                {
                    _log?.LogWarning($"DWG importer skipped spline with insufficient knots (handle {spline.Handle:X}).");
                    return null;
                }

                vertices = spline.PolygonalVertexes(128);
            }
            catch (Exception ex)
            {
                _log?.LogWarning($"DWG importer skipped spline (handle {spline.Handle:X}): {ex.Message}");
                return null;
            }

            var shape = CreatePathFromVertices(vertices, style, spline.IsClosed, transform, name: BuildEntityName(spline));
            return shape;
        }

        private BaseShapeViewModel? CreateSolid(Solid solid, ShapeStyleViewModel style, CadTransform transform)
        {
            var vertices = new List<XYZ>
            {
                solid.FirstCorner,
                solid.SecondCorner,
                solid.ThirdCorner,
                solid.FourthCorner
            };

            return CreatePathFromVertices(vertices.Where(IsValidVertex).ToList(), style, isClosed: true, transform, forceFill: true, name: BuildEntityName(solid));
        }

        private BaseShapeViewModel? CreateFace(Face3D face, ShapeStyleViewModel style, CadTransform transform)
        {
            var vertices = new List<XYZ>
            {
                face.FirstCorner,
                face.SecondCorner,
                face.ThirdCorner,
                face.FourthCorner
            };

            return CreatePathFromVertices(vertices.Where(IsValidVertex).ToList(), style, isClosed: true, transform, forceFill: true, name: BuildEntityName(face));
        }

        private BaseShapeViewModel? TryConvertByPolygon(Entity entity, CadTransform transform)
        {
            try
            {
                switch (entity)
                {
                    case IPolyline polyline:
                    {
                        var vertices = polyline.Vertices
                            .Select(v => v.Location.Convert<XYZ>())
                            .ToList();
                        var style = ResolveStyle(entity, polyline.IsClosed);
                        return CreatePathFromVertices(vertices, style, polyline.IsClosed, transform, name: BuildEntityName(entity));
                    }
                }
            }
            catch
            {
                // Ignore conversion failures for unsupported entities.
            }

            return null;
        }

        private PathShapeViewModel? CreatePathFromVertices(IList<XYZ> vertices, ShapeStyleViewModel style, bool isClosed, CadTransform transform, bool forceFill = false, string? name = null)
        {
            if (vertices.Count < 2)
            {
                return null;
            }

            var transformed = vertices
                .Where(IsValidVertex)
                .Select(transform.Apply)
                .ToList();

            if (transformed.Count < 2)
            {
                return null;
            }

            var path = _factory.CreatePathShape(style, ImmutableArray<PathFigureViewModel>.Empty, FillRule.Nonzero, isStroked: true, isFilled: isClosed || forceFill);
            var context = _factory.CreateGeometryContext(path);
            context.BeginFigure(_factory.CreatePointShape(transformed[0].X, transformed[0].Y), isClosed);

            for (int i = 1; i < transformed.Count; i++)
            {
                var point = transformed[i];
                context.LineTo(_factory.CreatePointShape(point.X, point.Y));
            }

            context.SetClosedState(isClosed);

            path.Style = style;
            path.Name = name ?? "path";
            path.IsFilled = isClosed || forceFill;
            return path;
        }

        private ShapeStyleViewModel ResolveStyle(Entity entity, bool isFilled)
        {
            var color = entity.GetActiveColor();
            var strokeAlpha = ResolveAlpha(entity);
            var thickness = ResolveThickness(entity);

            var fillColor = isFilled ? color : (Color?)null;
            var fillAlpha = isFilled ? strokeAlpha : (byte)0;

            var key = StyleKey.Create(color, strokeAlpha, thickness, fillColor, fillAlpha, isFilled);
            if (!_styleCache.TryGetValue(key, out var style))
            {
                style = _factory.CreateShapeStyle("CAD Style");
                style.Name = key.ToString();

                if (style.Stroke is { })
                {
                    style.Stroke.Color = _factory.CreateArgbColor(strokeAlpha, color.R, color.G, color.B);
                    style.Stroke.Thickness = thickness;
                    style.Stroke.LineCap = LineCap.Flat;
                }

                if (style.Fill is { })
                {
                    if (fillColor.HasValue)
                    {
                        var fc = fillColor.Value;
                        style.Fill.Color = _factory.CreateArgbColor(fillAlpha, fc.R, fc.G, fc.B);
                    }
                    else
                    {
                        style.Fill.Color = _factory.CreateArgbColor(0, 0, 0, 0);
                    }
                }

                if (style.TextStyle is { })
                {
                    style.TextStyle.FontSize = 12.0;
                    style.TextStyle.FontName = "Arial";
                }

                _styleCache[key] = style;
            }

            return style;
        }

        private BlockShapeViewModel? ResolveBlock(BlockRecord? record, HashSet<BlockRecord>? chain)
        {
            if (record is null)
            {
                return null;
            }

            if (ShouldSkip(record))
            {
                return null;
            }

            if (_blockCache.TryGetValue(record, out var cached))
            {
                return cached;
            }

            chain ??= new HashSet<BlockRecord>();
            if (!chain.Add(record))
            {
                return null;
            }

            try
            {
                var transform = CadTransform.FromEntities(record.Entities);
                var shapes = new List<BaseShapeViewModel>();

                foreach (var entity in record.Entities)
                {
                    if (!IsRenderable(entity))
                    {
                        continue;
                    }

                    try
                    {
                        foreach (var shape in ConvertEntity(entity, transform, depth: 0, chain))
                        {
                            if (shape is { })
                            {
                                shapes.Add(shape);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _log?.LogWarning($"DWG importer skipped block entity {DescribeEntity(entity)}: {ex.Message}");
                    }
                }

                if (shapes.Count == 0)
                {
                    return null;
                }

                var block = _factory.CreateBlockShape(SanitizeName(record.Name));
                block.Shapes = ImmutableArray<BaseShapeViewModel>.Empty;
                foreach (var shape in shapes)
                {
                    block.AddShape(shape);
                }

                _blockCache[record] = block;
                return block;
            }
            finally
            {
                chain.Remove(record);
            }
        }

        private BaseShapeViewModel? CreateText(TextEntity text, ShapeStyleViewModel style, CadTransform transform)
        {
            if (string.IsNullOrWhiteSpace(text.Value))
            {
                return null;
            }

            var height = text.Height > 0 ? text.Height : 12.0;
            var widthFactor = text.WidthFactor > 0 ? text.WidthFactor : 1.0;
            var estimatedWidth = Math.Max(height, text.Value.Length * height * widthFactor * 0.6);

            var insert = text.InsertPoint;

            double minX;
            double maxX;

            switch (text.HorizontalAlignment)
            {
                case TextHorizontalAlignment.Center:
                case TextHorizontalAlignment.Middle:
                    minX = insert.X - estimatedWidth / 2.0;
                    maxX = insert.X + estimatedWidth / 2.0;
                    break;
                case TextHorizontalAlignment.Right:
                    minX = insert.X - estimatedWidth;
                    maxX = insert.X;
                    break;
                default:
                    minX = insert.X;
                    maxX = insert.X + estimatedWidth;
                    break;
            }

            double minY;
            double maxY;

            switch (text.VerticalAlignment)
            {
                case TextVerticalAlignmentType.Top:
                    maxY = insert.Y;
                    minY = insert.Y - height;
                    break;
                case TextVerticalAlignmentType.Middle:
                    maxY = insert.Y + height / 2.0;
                    minY = insert.Y - height / 2.0;
                    break;
                case TextVerticalAlignmentType.Bottom:
                case TextVerticalAlignmentType.Baseline:
                default:
                    maxY = insert.Y;
                    minY = insert.Y - height;
                    break;
            }

            var topLeft = transform.Apply(minX, maxY);
            var bottomRight = transform.Apply(maxX, minY);

            var textShape = _factory.CreateTextShape(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y, style, text.Value, isStroked: true, name: BuildEntityName(text));
            textShape.IsFilled = false;

            if (style.TextStyle is { } textStyle)
            {
                textStyle.FontSize = height;
                if (text.Style is { } cadTextStyle)
                {
                    var fontName = !string.IsNullOrWhiteSpace(cadTextStyle.Filename)
                        ? Path.GetFileNameWithoutExtension(cadTextStyle.Filename)
                        : cadTextStyle.Name;

                    if (!string.IsNullOrWhiteSpace(fontName))
                    {
                        textStyle.FontName = fontName;
                    }
                }

                textStyle.TextHAlignment = MapHorizontalAlignment(text.HorizontalAlignment);
                textStyle.TextVAlignment = MapVerticalAlignment(text.VerticalAlignment);
            }

            return textShape;
        }

        private static TextHAlignment MapHorizontalAlignment(TextHorizontalAlignment alignment)
        {
            return alignment switch
            {
                TextHorizontalAlignment.Center or TextHorizontalAlignment.Middle => TextHAlignment.Center,
                TextHorizontalAlignment.Right => TextHAlignment.Right,
                _ => TextHAlignment.Left
            };
        }

        private static TextVAlignment MapVerticalAlignment(TextVerticalAlignmentType alignment)
        {
            return alignment switch
            {
                TextVerticalAlignmentType.Top => TextVAlignment.Top,
                TextVerticalAlignmentType.Middle => TextVAlignment.Center,
                _ => TextVAlignment.Bottom
            };
        }

        private double ResolveThickness(Entity entity)
        {
            var lw = entity.GetActiveLineWeightType();
            var value = (short)lw;

            if (value <= 0)
            {
                return 1.0;
            }

            var thickness = value * _lineweightFactor;
            return thickness <= 0 ? 1.0 : thickness;
        }

        private static byte ResolveAlpha(Entity entity)
        {
            var transparency = entity.Transparency;
            if (transparency.IsByLayer || transparency.IsByBlock)
            {
                return 255;
            }

            var value = transparency.Value;
            value = Math.Clamp(value, (short)0, (short)90);
            var opaque = 100 - value;
            return (byte)Math.Clamp((int)Math.Round(opaque * 255.0 / 100.0), 0, 255);
        }

        private static double NormalizeSweep(double start, double end)
        {
            var sweep = end - start;
            if (sweep > Math.PI * 2)
            {
                sweep = sweep % (Math.PI * 2);
            }
            else if (sweep < -Math.PI * 2)
            {
                sweep = sweep % (Math.PI * 2);
            }

            if (sweep > Math.PI*2 - 1e-6)
            {
                sweep = Math.PI * 2;
            }
            else if (sweep < -Math.PI*2 + 1e-6)
            {
                sweep = -Math.PI * 2;
            }

            return sweep;
        }

        private static bool IsRenderable(Entity entity)
        {
            if (entity.IsInvisible)
            {
                return false;
            }

            if (entity.Layer is { } layer && !layer.IsOn)
            {
                return false;
            }

            return true;
        }

        private static bool IsValidVertex(XYZ vertex)
        {
            return !(double.IsNaN(vertex.X) || double.IsNaN(vertex.Y) || double.IsInfinity(vertex.X) || double.IsInfinity(vertex.Y));
        }

        private static string BuildEntityName(Entity entity)
        {
            if (entity.Handle != 0)
            {
                return $"e{entity.Handle:X}";
            }

            return entity.ObjectName;
        }

        private static string SanitizeName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "Block";
            }

            var filtered = new string(name.Select(ch => char.IsLetterOrDigit(ch) ? ch : '_').ToArray());
            return string.IsNullOrWhiteSpace(filtered) ? "Block" : filtered;
        }
    }

    private readonly record struct StyleKey(int StrokeArgb, int FillArgb, int ThicknessMilli, bool IsFilled)
    {
        public static StyleKey Create(Color stroke, byte strokeAlpha, double thickness, Color? fill, byte fillAlpha, bool isFilled)
        {
            var strokeArgb = PackArgb(strokeAlpha, stroke.R, stroke.G, stroke.B);
            var fillArgb = isFilled && fill.HasValue
                ? PackArgb(fillAlpha, fill.Value.R, fill.Value.G, fill.Value.B)
                : 0;
            var thicknessMilli = (int)Math.Round(thickness * 1000.0);
            return new StyleKey(strokeArgb, fillArgb, thicknessMilli, isFilled);
        }

        private static int PackArgb(byte a, byte r, byte g, byte b)
        {
            return (a << 24) | (r << 16) | (g << 8) | b;
        }

        public override string ToString()
        {
            return $"CAD-{StrokeArgb:X8}-{FillArgb:X8}-{ThicknessMilli}-{IsFilled}";
        }
    }

    private sealed class CadTransform
    {
        private readonly double _minX;
        private readonly double _maxY;
        private readonly bool _hasBounds;

        private CadTransform(double minX, double maxY, bool hasBounds)
        {
            _minX = minX;
            _maxY = maxY;
            _hasBounds = hasBounds;
        }

        public static CadTransform FromEntities(IEnumerable<Entity> entities)
        {
            var bbox = ComputeBoundingBox(entities);
            if (bbox is null)
            {
                return new CadTransform(0, 0, false);
            }

            return new CadTransform(bbox.Value.Min.X, bbox.Value.Max.Y, true);
        }

        public Point Apply(XYZ point)
        {
            return Apply(point.X, point.Y);
        }

        public Point Apply(double x, double y)
        {
            if (!_hasBounds)
            {
                return new Point(x, y);
            }

            var tx = x - _minX;
            var ty = _maxY - y;
            return new Point(tx, ty);
        }

        private static BoundingBox? ComputeBoundingBox(IEnumerable<Entity> entities)
        {
            BoundingBox box = BoundingBox.Null;
            bool has = false;

            foreach (var entity in entities)
            {
                try
                {
                    var entityBox = entity.GetBoundingBox();
                    if (entityBox.Extent == BoundingBoxExtent.Null)
                    {
                        continue;
                    }

                    if (!has)
                    {
                        box = entityBox;
                        has = true;
                    }
                    else
                    {
                        box = box.Merge(entityBox);
                    }
                }
                catch
                {
                    // Skip entities that fail to produce a bounding box.
                }
            }

            return has ? box : null;
        }
    }

    private readonly record struct Point(double X, double Y);
}

internal static class EllipseExtensions
{
    public static bool IsClosed(this Ellipse ellipse)
    {
        var diff = Math.Abs(ellipse.EndParameter - ellipse.StartParameter);
        return diff >= Math.PI * 2.0 - 1e-3;
    }
}
