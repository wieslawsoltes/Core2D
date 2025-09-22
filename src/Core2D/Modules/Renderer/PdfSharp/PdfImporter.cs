// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Numerics;
using Core2D.Model;
using Core2D.Model.Path;
using Core2D.Model.Style;
using Core2D.ViewModels;
using Core2D.ViewModels.Path;
using Core2D.ViewModels.Path.Segments;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.Content;
using PdfSharp.Pdf.Content.Objects;
using PdfSharp.Pdf.IO;
using SkiaSharp;
using SkiaPathConverter = Core2D.Modules.Renderer.SkiaSharp.PathGeometryConverter;

namespace Core2D.Modules.Renderer.PdfSharp;

internal sealed class PdfImporter : IPdfImporter
{
    private readonly IViewModelFactory? _viewModelFactory;
    private readonly ILog? _log;

    public PdfImporter(IServiceProvider? serviceProvider)
    {
        _viewModelFactory = serviceProvider?.GetService<IViewModelFactory>();
        _log = serviceProvider?.GetService<ILog>();
    }

    public PdfImportResult? Import(Stream stream)
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
            return new PdfImportResult(
                new List<BaseShapeViewModel>(),
                new List<ShapeStyleViewModel>(),
                new List<PdfImportedImage>(),
                0.0,
                0.0);
        }

        var document = TryLoadDocument(buffer);
        if (document is null)
        {
            _log?.LogError("PDF importer failed to load document stream (unsupported or corrupted file).");
            return null;
        }

        using (document)
        {
            try
            {
                var context = new ImportContext(_viewModelFactory, _log, document);
                return context.Import();
            }
            catch (Exception ex)
            {
                _log?.LogError($"PDF importer failed: {ex.Message}");
                return null;
            }
        }
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

    private static PdfDocument? TryLoadDocument(byte[] data)
    {
        try
        {
            using var ms = new MemoryStream(data, writable: false);
            return PdfReader.Open(ms, PdfDocumentOpenMode.Import);
        }
        catch
        {
            return null;
        }
    }

    private sealed class ImportContext
    {
        private readonly IViewModelFactory _factory;
        private readonly ILog? _log;
        private readonly PdfDocument _document;

        public ImportContext(IViewModelFactory factory, ILog? log, PdfDocument document)
        {
            _factory = factory;
            _log = log;
            _document = document;
        }

        public PdfImportResult Import()
        {
            var shapes = new List<BaseShapeViewModel>();
            var styles = new Dictionary<StyleKey, ShapeStyleViewModel>();
            var images = new Dictionary<string, PdfImportedImage>();

            double maxWidth = 0.0;
            double totalHeight = 0.0;

            for (var i = 0; i < _document.Pages.Count; i++)
            {
                var page = _document.Pages[i];
                var interpreter = new PageInterpreter(_factory, _log, page, styles, images, totalHeight);
                interpreter.Process();
                shapes.AddRange(interpreter.Shapes);
                maxWidth = Math.Max(maxWidth, interpreter.PageWidth);
                totalHeight += interpreter.PageHeight;
            }

            return new PdfImportResult(
                shapes,
                styles.Values.ToList(),
                images.Values.ToList(),
                maxWidth,
                totalHeight);
        }
    }

    private sealed class PageInterpreter
    {
        private readonly IViewModelFactory _factory;
        private readonly ILog? _log;
        private readonly PdfPage _page;
        private readonly IDictionary<StyleKey, ShapeStyleViewModel> _styleCache;
        private readonly IDictionary<string, PdfImportedImage> _imageCache;
        private readonly double _pageOffsetY;
        private readonly double _scale;
        private readonly Matrix3x2 _pageTransform;
        private readonly Matrix3x2 _pageVectorTransform;
        private readonly List<BaseShapeViewModel> _shapes = new();
        private readonly StyleBuilder _styleBuilder;
        private readonly TextInterpreter _textInterpreter;

        public PageInterpreter(
            IViewModelFactory factory,
            ILog? log,
            PdfPage page,
            IDictionary<StyleKey, ShapeStyleViewModel> styleCache,
            IDictionary<string, PdfImportedImage> imageCache,
            double pageOffsetY)
        {
            _factory = factory;
            _log = log;
            _page = page;
            _styleCache = styleCache;
            _imageCache = imageCache;
            _pageOffsetY = pageOffsetY;
            _scale = 96.0 / 72.0;
            PageWidth = page.Width.Point * _scale;
            PageHeight = page.Height.Point * _scale;
            _pageTransform = Matrix3x2.CreateScale((float)_scale, (float)-_scale) * Matrix3x2.CreateTranslation(0f, (float)(PageHeight + _pageOffsetY));
            _pageVectorTransform = Matrix3x2.CreateScale((float)_scale, (float)-_scale);
            _styleBuilder = new StyleBuilder(_factory, _styleCache, _pageVectorTransform, _scale);
            _textInterpreter = new TextInterpreter(_factory, _styleBuilder, _log, this, page);
        }

        public IReadOnlyList<BaseShapeViewModel> Shapes => _shapes;

        public double PageWidth { get; }

        public double PageHeight { get; }

        public void Process()
        {
            var sequence = ContentReader.ReadContent(_page);
            var graphicsState = new GraphicsState();
            var stateStack = new Stack<GraphicsState>();
            var pathBuilder = new PathBuilder();

            foreach (var element in sequence)
            {
                ProcessObject(element, graphicsState, stateStack, pathBuilder);
            }
        }

        private void ProcessObject(CObject element, GraphicsState state, Stack<GraphicsState> stack, PathBuilder pathBuilder)
        {
            switch (element)
            {
                case CSequence sequence:
                    foreach (var item in sequence)
                    {
                        ProcessObject(item, state, stack, pathBuilder);
                    }
                    break;
                case COperator op:
                    HandleOperator(op, state, stack, pathBuilder);
                    break;
            }
        }

        private void HandleOperator(COperator op, GraphicsState state, Stack<GraphicsState> stack, PathBuilder pathBuilder)
        {
            switch (op.OpCode.OpCodeName)
            {
                case OpCodeName.q:
                    stack.Push(state.Clone());
                    break;
                case OpCodeName.Q:
                    if (stack.Count > 0)
                    {
                        state.CopyFrom(stack.Pop());
                    }
                    break;
                case OpCodeName.cm:
                    state.ConcatenateMatrix(GetMatrix(op));
                    break;
                case OpCodeName.w:
                    state.LineWidth = GetNumber(op, 0);
                    break;
                case OpCodeName.J:
                    state.LineCap = (int)GetNumber(op, 0);
                    break;
                case OpCodeName.j:
                    state.LineJoin = (int)GetNumber(op, 0);
                    break;
                case OpCodeName.M:
                    state.MiterLimit = GetNumber(op, 0);
                    break;
                case OpCodeName.d:
                    state.SetDashPattern(GetDashArray(op), GetNumber(op, 1));
                    break;
                case OpCodeName.CS:
                    SetColorSpace(op, state, stroke: true);
                    break;
                case OpCodeName.cs:
                    SetColorSpace(op, state, stroke: false);
                    break;
                case OpCodeName.G:
                    state.SetStrokeColorSpace(PdfColorSpaceInfo.DeviceGray);
                    state.StrokeColor = PdfColor.FromGray(GetNumber(op, 0));
                    break;
                case OpCodeName.g:
                    state.SetFillColorSpace(PdfColorSpaceInfo.DeviceGray);
                    state.FillColor = PdfColor.FromGray(GetNumber(op, 0));
                    break;
                case OpCodeName.RG:
                    state.SetStrokeColorSpace(PdfColorSpaceInfo.DeviceRgb);
                    state.StrokeColor = PdfColor.FromRgb(GetNumber(op, 0), GetNumber(op, 1), GetNumber(op, 2));
                    break;
                case OpCodeName.rg:
                    state.SetFillColorSpace(PdfColorSpaceInfo.DeviceRgb);
                    state.FillColor = PdfColor.FromRgb(GetNumber(op, 0), GetNumber(op, 1), GetNumber(op, 2));
                    break;
                case OpCodeName.K:
                    state.SetStrokeColorSpace(PdfColorSpaceInfo.DeviceCmyk);
                    state.StrokeColor = PdfColor.FromCmyk(GetNumber(op, 0), GetNumber(op, 1), GetNumber(op, 2), GetNumber(op, 3));
                    break;
                case OpCodeName.k:
                    state.SetFillColorSpace(PdfColorSpaceInfo.DeviceCmyk);
                    state.FillColor = PdfColor.FromCmyk(GetNumber(op, 0), GetNumber(op, 1), GetNumber(op, 2), GetNumber(op, 3));
                    break;
                case OpCodeName.SC:
                case OpCodeName.SCN:
                    SetColor(op, state, stroke: true);
                    break;
                case OpCodeName.sc:
                case OpCodeName.scn:
                    SetColor(op, state, stroke: false);
                    break;
                case OpCodeName.m:
                    pathBuilder.MoveTo(TransformPoint(state, GetNumber(op, 0), GetNumber(op, 1)));
                    break;
                case OpCodeName.l:
                    pathBuilder.LineTo(TransformPoint(state, GetNumber(op, 0), GetNumber(op, 1)));
                    break;
                case OpCodeName.c:
                {
                    var p1 = TransformPoint(state, GetNumber(op, 0), GetNumber(op, 1));
                    var p2 = TransformPoint(state, GetNumber(op, 2), GetNumber(op, 3));
                    var p3 = TransformPoint(state, GetNumber(op, 4), GetNumber(op, 5));
                    pathBuilder.CubicBezierTo(p1, p2, p3);
                    break;
                }
                case OpCodeName.v:
                {
                    var current = pathBuilder.CurrentPoint;
                    var p2 = TransformPoint(state, GetNumber(op, 0), GetNumber(op, 1));
                    var p3 = TransformPoint(state, GetNumber(op, 2), GetNumber(op, 3));
                    pathBuilder.CubicBezierTo(current, p2, p3);
                    break;
                }
                case OpCodeName.y:
                {
                    var p1 = TransformPoint(state, GetNumber(op, 0), GetNumber(op, 1));
                    var p3 = TransformPoint(state, GetNumber(op, 2), GetNumber(op, 3));
                    pathBuilder.CubicBezierTo(p1, p3, p3);
                    break;
                }
                case OpCodeName.h:
                    pathBuilder.ClosePath();
                    break;
                case OpCodeName.re:
                {
                    var x = GetNumber(op, 0);
                    var y = GetNumber(op, 1);
                    var w = GetNumber(op, 2);
                    var h = GetNumber(op, 3);
                    var p1 = TransformPoint(state, x, y);
                    var p2 = TransformPoint(state, x + w, y);
                    var p3 = TransformPoint(state, x + w, y + h);
                    var p4 = TransformPoint(state, x, y + h);
                    pathBuilder.MoveTo(p1);
                    pathBuilder.LineTo(p2);
                    pathBuilder.LineTo(p3);
                    pathBuilder.LineTo(p4);
                    pathBuilder.ClosePath();
                    break;
                }
                case OpCodeName.S:
                    PaintPath(pathBuilder, state, stroke: true, fill: false, FillRule.Nonzero);
                    break;
                case OpCodeName.s:
                    pathBuilder.ClosePath();
                    PaintPath(pathBuilder, state, stroke: true, fill: false, FillRule.Nonzero);
                    break;
                case OpCodeName.f:
                case OpCodeName.F:
                    PaintPath(pathBuilder, state, stroke: false, fill: true, FillRule.Nonzero);
                    break;
                case OpCodeName.fx:
                    PaintPath(pathBuilder, state, stroke: false, fill: true, FillRule.EvenOdd);
                    break;
                case OpCodeName.B:
                    PaintPath(pathBuilder, state, stroke: true, fill: true, FillRule.Nonzero);
                    break;
                case OpCodeName.Bx:
                    PaintPath(pathBuilder, state, stroke: true, fill: true, FillRule.EvenOdd);
                    break;
                case OpCodeName.b:
                    pathBuilder.ClosePath();
                    PaintPath(pathBuilder, state, stroke: true, fill: true, FillRule.Nonzero);
                    break;
                case OpCodeName.bx:
                    pathBuilder.ClosePath();
                    PaintPath(pathBuilder, state, stroke: true, fill: true, FillRule.EvenOdd);
                    break;
                case OpCodeName.n:
                    pathBuilder.Reset();
                    break;
                case OpCodeName.W:
                    ApplyClipping(pathBuilder, state, FillRule.Nonzero);
                    break;
                case OpCodeName.Wx:
                    ApplyClipping(pathBuilder, state, FillRule.EvenOdd);
                    break;
                case OpCodeName.BT:
                    _textInterpreter.BeginText(state.Clone());
                    break;
                case OpCodeName.ET:
                    _textInterpreter.EndText(_shapes);
                    break;
                case OpCodeName.gs:
                    ApplyExtGraphicsState(op, state);
                    break;
                default:
                    if (!_textInterpreter.TryHandle(op, _shapes))
                    {
                        HandleXObject(op, state);
                    }
                    break;
            }
        }

        private void HandleXObject(COperator op, GraphicsState state)
        {
            if (op.OpCode.OpCodeName != OpCodeName.Do || op.Operands.Count == 0)
            {
                return;
            }

            if (op.Operands[0] is not CName name)
            {
                return;
            }

            var xObjects = _page.Resources?.Elements.GetDictionary("/XObject");
            if (xObjects is null)
            {
                return;
            }

            var item = xObjects.Elements[name.Name];
            if (item is null)
            {
                return;
            }

            object? direct = item;
            PdfReference.Dereference(ref direct);
            if (direct is not PdfDictionary xObject)
            {
                return;
            }

            var subtype = xObject.Elements.GetName("/Subtype");
            if (!string.Equals(subtype, "/Image", StringComparison.Ordinal))
            {
                return;
            }

            var stream = xObject.Stream;
            if (stream?.Value is not { Length: > 0 } rawData)
            {
                return;
            }

            var width = Math.Max(1, xObject.Elements.GetInteger("/Width"));
            var height = Math.Max(1, xObject.Elements.GetInteger("/Height"));

            var key = $"Images/ImportedPdf/{Guid.NewGuid():N}.bin";
            if (!_imageCache.ContainsKey(key))
            {
                _imageCache[key] = new PdfImportedImage(key, rawData);
            }

            var p0 = TransformPoint(state, 0, 0);
            var p1 = TransformPoint(state, width, 0);
            var p2 = TransformPoint(state, width, height);
            var p3 = TransformPoint(state, 0, height);

            var minX = Math.Min(Math.Min(p0.X, p1.X), Math.Min(p2.X, p3.X));
            var minY = Math.Min(Math.Min(p0.Y, p1.Y), Math.Min(p2.Y, p3.Y));
            var maxX = Math.Max(Math.Max(p0.X, p1.X), Math.Max(p2.X, p3.X));
            var maxY = Math.Max(Math.Max(p0.Y, p1.Y), Math.Max(p2.Y, p3.Y));

            if (!IntersectsClip(state, minX, minY, maxX, maxY))
            {
                return;
            }

            var style = _styleBuilder.CreateImageStyle();
            var imageShape = _factory.CreateImageShape(
                minX,
                minY,
                maxX,
                maxY,
                style,
                key,
                isStroked: false,
                isFilled: false);

            _shapes.Add(imageShape);
        }

        private void ApplyClipping(PathBuilder pathBuilder, GraphicsState state, FillRule clipRule)
        {
            var clipPath = pathBuilder.ToSkPath(clipRule == FillRule.EvenOdd);
            pathBuilder.Reset();
            if (clipPath is null)
            {
                return;
            }

            try
            {
                clipPath.FillType = clipRule == FillRule.EvenOdd ? SKPathFillType.EvenOdd : SKPathFillType.Winding;
                state.ApplyClip(clipPath);
            }
            finally
            {
                clipPath.Dispose();
            }
        }

        private void PaintPath(PathBuilder pathBuilder, GraphicsState state, bool stroke, bool fill, FillRule fillRule)
        {
            if (!pathBuilder.HasGeometry)
            {
                return;
            }

            try
            {
                var figures = pathBuilder.ToFigures(_factory);
                if (figures.Length == 0)
                {
                    pathBuilder.Reset();
                    return;
                }

                var style = _styleBuilder.GetOrCreatePathStyle(state, stroke, fill, fillRule);
                var shape = _factory.CreatePathShape(style, figures, fillRule, stroke, fill);
                shape = ApplyClip(shape, state);
                if (shape is { })
                {
                    _shapes.Add(shape);
                }
            }
            catch (Exception ex)
            {
                _log?.LogWarning($"PDF importer skipped path: {ex.Message}");
            }
            finally
            {
                pathBuilder.Reset();
            }
        }

        private PathShapeViewModel? ApplyClip(PathShapeViewModel shape, GraphicsState state)
        {
            if (state.ClipPath is null)
            {
                return shape;
            }

            using var path = SkiaPathConverter.ToSKPath(shape);
            if (path is null)
            {
                return shape;
            }

            using var clip = new SKPath(state.ClipPath);
            using var clipped = new SKPath();
            if (!clip.Op(path, SKPathOp.Intersect, clipped))
            {
                return shape;
            }

            if (clipped.IsEmpty)
            {
                return null;
            }

            var clippedShape = SkiaPathConverter.ToPathGeometry(clipped, _factory);
            clippedShape.IsFilled = shape.IsFilled;
            clippedShape.IsStroked = shape.IsStroked;
            clippedShape.Style = shape.Style;
            clippedShape.State = shape.State;
            clippedShape.Name = shape.Name;
            clippedShape.Properties = shape.Properties;
            clippedShape.Record = shape.Record;
            clippedShape.FillRule = shape.FillRule;
            return clippedShape;
        }

        private void SetColorSpace(COperator op, GraphicsState state, bool stroke)
        {
            if (op.Operands.Count == 0)
            {
                return;
            }

            var colorSpace = ResolveColorSpace(op.Operands[0]);
            if (stroke)
            {
                state.SetStrokeColorSpace(colorSpace);
            }
            else
            {
                state.SetFillColorSpace(colorSpace);
            }
        }

        private void SetColor(COperator op, GraphicsState state, bool stroke)
        {
            var colorSpace = stroke ? state.StrokeColorSpace : state.FillColorSpace;
            var componentCount = Math.Max(colorSpace.Components, 1);
            var components = new double[componentCount];
            var componentIndex = 0;

            foreach (var operand in op.Operands)
            {
                if (operand is CInteger or CReal)
                {
                    components[componentIndex] = Math.Clamp(GetOperandNumber(operand), 0.0, 1.0);
                    componentIndex++;
                    if (componentIndex >= componentCount)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            if (componentIndex > 0 && componentIndex < componentCount)
            {
                var last = components[componentIndex - 1];
                for (var i = componentIndex; i < componentCount; i++)
                {
                    components[i] = last;
                }
            }

            var color = ConvertToColor(colorSpace, components);
            if (stroke)
            {
                state.StrokeColor = color;
            }
            else
            {
                state.FillColor = color;
            }
        }

        private void ApplyExtGraphicsState(COperator op, GraphicsState state)
        {
            if (op.Operands.Count == 0 || op.Operands[0] is not CName name)
            {
                return;
            }

            var extStates = _page.Resources?.Elements.GetDictionary("/ExtGState");
            if (extStates is null)
            {
                return;
            }

            if (!TryGetDictionaryValue(extStates.Elements, name.Name, out var gsItem))
            {
                return;
            }

            var resolved = ResolveObject(gsItem);
            if (resolved is not PdfDictionary gsDictionary)
            {
                return;
            }

            if (gsDictionary.Elements.TryGetValue("/CA", out var strokeAlphaItem))
            {
                state.StrokeAlpha = Math.Clamp(GetDouble(strokeAlphaItem), 0.0, 1.0);
            }

            if (gsDictionary.Elements.TryGetValue("/ca", out var fillAlphaItem))
            {
                state.FillAlpha = Math.Clamp(GetDouble(fillAlphaItem), 0.0, 1.0);
            }

            if (gsDictionary.Elements.TryGetValue("/SMask", out var smaskItem))
            {
                var smask = ResolveObject(smaskItem);
                if (smask is PdfName smaskName && string.Equals(smaskName.Value, "/None", StringComparison.Ordinal))
                {
                    state.SoftMaskAlpha = 1.0;
                }
                else if (smask is PdfDictionary smaskDictionary)
                {
                    var subtype = smaskDictionary.Elements.GetName("/Subtype");
                    state.SoftMaskAlpha = string.Equals(subtype, "/Alpha", StringComparison.Ordinal)
                        ? 0.5
                        : 0.75;
                }
            }
        }

        private PdfColorSpaceInfo ResolveColorSpace(CObject operand)
        {
            if (operand is CName name)
            {
                return ResolveColorSpaceByName(name.Name);
            }

            return PdfColorSpaceInfo.DeviceRgb;
        }

        private PdfColorSpaceInfo ResolveColorSpaceByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return PdfColorSpaceInfo.DeviceRgb;
            }

            return name switch
            {
                "/DeviceRGB" => PdfColorSpaceInfo.DeviceRgb,
                "/DeviceCMYK" => PdfColorSpaceInfo.DeviceCmyk,
                "/DeviceGray" => PdfColorSpaceInfo.DeviceGray,
                "DeviceRGB" => PdfColorSpaceInfo.DeviceRgb,
                "DeviceCMYK" => PdfColorSpaceInfo.DeviceCmyk,
                "DeviceGray" => PdfColorSpaceInfo.DeviceGray,
                _ => ResolveColorSpaceFromResources(name)
            };
        }

        private PdfColorSpaceInfo ResolveColorSpaceFromResources(string name)
        {
            var colorSpaces = _page.Resources?.Elements.GetDictionary("/ColorSpace");
            if (colorSpaces is null)
            {
                return PdfColorSpaceInfo.DeviceRgb;
            }

            if (!TryGetDictionaryValue(colorSpaces.Elements, name, out var item))
            {
                return PdfColorSpaceInfo.DeviceRgb;
            }

            var resolved = ResolveObject(item);
            return ResolveColorSpaceObject(resolved);
        }

        private static PdfColorSpaceInfo ResolveColorSpaceObject(PdfItem? item)
        {
            item = ResolveObject(item);

            switch (item)
            {
                case PdfName pdfName:
                    return pdfName.Value switch
                    {
                        "/DeviceGray" => PdfColorSpaceInfo.DeviceGray,
                        "/DeviceRGB" => PdfColorSpaceInfo.DeviceRgb,
                        "/DeviceCMYK" => PdfColorSpaceInfo.DeviceCmyk,
                        _ => PdfColorSpaceInfo.DeviceRgb
                    };
                case PdfArray array when array.Elements.Count > 0:
                {
                    var first = ResolveObject(array.Elements[0]);
                    if (first is PdfName csName)
                    {
                        var value = csName.Value;
                        if (string.Equals(value, "/ICCBased", StringComparison.Ordinal))
                        {
                            var components = 3;
                            if (array.Elements.Count > 1)
                            {
                                var profile = ResolveObject(array.Elements[1]);
                                if (profile is PdfDictionary profileDictionary)
                                {
                                    components = profileDictionary.Elements.GetInteger("/N");
                                }
                            }
                            if (components <= 0)
                            {
                                components = 3;
                            }
                            return PdfColorSpaceInfo.Icc(components);
                        }

                        return value switch
                        {
                            "/DeviceGray" => PdfColorSpaceInfo.DeviceGray,
                            "/DeviceRGB" => PdfColorSpaceInfo.DeviceRgb,
                            "/DeviceCMYK" => PdfColorSpaceInfo.DeviceCmyk,
                            _ => PdfColorSpaceInfo.DeviceRgb
                        };
                    }
                    break;
                }
                case PdfDictionary:
                    return PdfColorSpaceInfo.DeviceRgb;
            }

            return PdfColorSpaceInfo.DeviceRgb;
        }

        private static PdfItem? ResolveObject(PdfItem? item)
        {
            while (item is PdfReference reference)
            {
                item = reference.Value;
            }

            return item;
        }

        private static bool TryGetDictionaryValue(PdfDictionary.DictionaryElements elements, string name, out PdfItem? item)
        {
            if (elements.TryGetValue(name, out item))
            {
                return true;
            }

            var trimmed = name.StartsWith('/') ? name.Substring(1) : "/" + name;
            return elements.TryGetValue(trimmed, out item)
                   || elements.TryGetValue(trimmed.TrimStart('/'), out item)
                   || elements.TryGetValue("/" + trimmed.TrimStart('/'), out item);
        }

        private static PdfColor ConvertToColor(PdfColorSpaceInfo colorSpace, double[] components)
        {
            static double Component(double[] source, int index)
            {
                if (index < source.Length)
                {
                    return Math.Clamp(source[index], 0.0, 1.0);
                }

                return source.Length > 0 ? Math.Clamp(source[^1], 0.0, 1.0) : 0.0;
            }

            return colorSpace.Kind switch
            {
                ColorSpaceKind.DeviceGray => PdfColor.FromGray(Component(components, 0)),
                ColorSpaceKind.DeviceRgb => PdfColor.FromRgb(Component(components, 0), Component(components, 1), Component(components, 2)),
                ColorSpaceKind.DeviceCmyk => PdfColor.FromCmyk(Component(components, 0), Component(components, 1), Component(components, 2), Component(components, 3)),
                ColorSpaceKind.Icc when colorSpace.Components == 1 => PdfColor.FromGray(Component(components, 0)),
                ColorSpaceKind.Icc when colorSpace.Components == 4 => PdfColor.FromCmyk(Component(components, 0), Component(components, 1), Component(components, 2), Component(components, 3)),
                ColorSpaceKind.Icc => PdfColor.FromRgb(Component(components, 0), Component(components, 1), Component(components, 2)),
                _ => PdfColor.FromRgb(Component(components, 0), Component(components, 1), Component(components, 2))
            };
        }

        private static double GetOperandNumber(CObject operand)
        {
            return operand switch
            {
                CInteger integer => integer.Value,
                CReal real => real.Value,
                _ => 0.0
            };
        }

        private static double GetDouble(PdfItem? item)
        {
            item = ResolveObject(item);

            return item switch
            {
                PdfReal real => real.Value,
                PdfInteger integer => integer.Value,
                _ => 0.0
            };
        }

        private static bool IntersectsClip(GraphicsState state, double minX, double minY, double maxX, double maxY)
        {
            if (state.ClipPath is null)
            {
                return true;
            }

            var left = Math.Min(minX, maxX);
            var top = Math.Min(minY, maxY);
            var width = Math.Abs(maxX - minX);
            var height = Math.Abs(maxY - minY);

            var rect = SKRect.Create((float)left, (float)top, (float)width, (float)height);
            if (rect.Width <= 0f || rect.Height <= 0f)
            {
                return false;
            }

            var bounds = state.ClipPath.TightBounds;
            return bounds.IntersectsWith(rect);
        }

        private Vector2 TransformPoint(GraphicsState state, double x, double y)
        {
            var vector = new Vector2((float)x, (float)y);
            vector = Vector2.Transform(vector, state.Transform);
            vector = Vector2.Transform(vector, _pageTransform);
            return vector;
        }

        private static Matrix3x2 GetMatrix(COperator op)
        {
            return new Matrix3x2(
                (float)GetNumber(op, 0),
                (float)GetNumber(op, 1),
                (float)GetNumber(op, 2),
                (float)GetNumber(op, 3),
                (float)GetNumber(op, 4),
                (float)GetNumber(op, 5));
        }

        private static double GetNumber(COperator op, int index)
        {
            if (index >= op.Operands.Count)
            {
                return 0.0;
            }

            return op.Operands[index] switch
            {
                CInteger integer => integer.Value,
                CReal real => real.Value,
                _ => 0.0
            };
        }

        private static double[]? GetDashArray(COperator op)
        {
            if (op.Operands.Count == 0)
            {
                return null;
            }

            if (op.Operands[0] is not CArray array || array.Count == 0)
            {
                return null;
            }

            var result = new double[array.Count];
            for (var i = 0; i < array.Count; i++)
            {
                result[i] = array[i] switch
                {
                    CInteger integer => integer.Value,
                    CReal real => real.Value,
                    _ => 0.0
                };
            }

            return result;
        }

        private sealed class PathBuilder
        {
            private readonly List<PathFigureData> _figures = new();
            private PathFigureData? _current;

            public bool HasGeometry => _figures.Any(f => f.HasGeometry);

            public Vector2 CurrentPoint => _current?.CurrentPoint ?? Vector2.Zero;

            public void MoveTo(Vector2 point)
            {
                _current = new PathFigureData(point);
                _figures.Add(_current);
            }

            public void LineTo(Vector2 point)
            {
                EnsureCurrent(point);
                _current!.Segments.Add(new LineSegmentData(point));
                _current.CurrentPoint = point;
            }

            public void CubicBezierTo(Vector2 c1, Vector2 c2, Vector2 point)
            {
                EnsureCurrent(point);
                _current!.Segments.Add(new CubicSegmentData(c1, c2, point));
                _current.CurrentPoint = point;
            }

            public void ClosePath()
            {
                _current?.Close();
            }

        public void Reset()
        {
            _figures.Clear();
            _current = null;
        }

        public SKPath? ToSkPath(bool evenOdd)
        {
            if (!HasGeometry)
            {
                return null;
            }

            var path = new SKPath
            {
                FillType = evenOdd ? SKPathFillType.EvenOdd : SKPathFillType.Winding
            };

            foreach (var figure in _figures)
            {
                if (!figure.HasGeometry)
                {
                    continue;
                }

                path.MoveTo((float)figure.StartPoint.X, (float)figure.StartPoint.Y);

                foreach (var segment in figure.Segments)
                {
                    switch (segment)
                    {
                        case LineSegmentData line:
                            path.LineTo((float)line.Point.X, (float)line.Point.Y);
                            break;
                        case CubicSegmentData cubic:
                            path.CubicTo(
                                (float)cubic.Control1.X,
                                (float)cubic.Control1.Y,
                                (float)cubic.Control2.X,
                                (float)cubic.Control2.Y,
                                (float)cubic.Point.X,
                                (float)cubic.Point.Y);
                            break;
                    }
                }

                if (figure.IsClosed)
                {
                    path.Close();
                }
            }

            if (path.IsEmpty)
            {
                path.Dispose();
                return null;
            }

            return path;
        }

        public ImmutableArray<PathFigureViewModel> ToFigures(IViewModelFactory factory)
        {
            var builder = ImmutableArray.CreateBuilder<PathFigureViewModel>();
                foreach (var figure in _figures)
                {
                    if (!figure.HasGeometry)
                    {
                        continue;
                    }

                    var start = factory.CreatePointShape(figure.StartPoint.X, figure.StartPoint.Y);
                    var segments = ImmutableArray.CreateBuilder<PathSegmentViewModel>();
                    foreach (var segment in figure.Segments)
                    {
                        switch (segment)
                        {
                            case LineSegmentData line:
                                segments.Add(factory.CreateLineSegment(factory.CreatePointShape(line.Point.X, line.Point.Y)));
                                break;
                            case CubicSegmentData cubic:
                                segments.Add(factory.CreateCubicBezierSegment(
                                    factory.CreatePointShape(cubic.Control1.X, cubic.Control1.Y),
                                    factory.CreatePointShape(cubic.Control2.X, cubic.Control2.Y),
                                    factory.CreatePointShape(cubic.Point.X, cubic.Point.Y)));
                                break;
                        }
                    }

                    var figureVm = factory.CreatePathFigure(start, figure.IsClosed);
                    figureVm.Segments = segments.ToImmutable();
                    builder.Add(figureVm);
                }

                return builder.ToImmutable();
            }

            private void EnsureCurrent(Vector2 point)
            {
                if (_current is null)
                {
                    MoveTo(point);
                }
            }

            private sealed class PathFigureData
            {
                public PathFigureData(Vector2 start)
                {
                    StartPoint = start;
                    CurrentPoint = start;
                }

                public Vector2 StartPoint { get; }

                public Vector2 CurrentPoint { get; set; }

                public bool IsClosed { get; private set; }

                public List<PathSegmentData> Segments { get; } = new();

                public bool HasGeometry => Segments.Count > 0;

                public void Close()
                {
                    IsClosed = true;
                }
            }

            private abstract class PathSegmentData
            {
            }

            private sealed class LineSegmentData : PathSegmentData
            {
                public LineSegmentData(Vector2 point)
                {
                    Point = point;
                }

                public Vector2 Point { get; }
            }

            private sealed class CubicSegmentData : PathSegmentData
            {
                public CubicSegmentData(Vector2 control1, Vector2 control2, Vector2 point)
                {
                    Control1 = control1;
                    Control2 = control2;
                    Point = point;
                }

                public Vector2 Control1 { get; }
                public Vector2 Control2 { get; }
                public Vector2 Point { get; }
            }
        }

        private sealed class TextInterpreter
        {
            private readonly IViewModelFactory _factory;
            private readonly StyleBuilder _styleBuilder;
            private readonly ILog? _log;
            private readonly PageInterpreter _owner;
            private readonly PdfPage _page;
            private readonly TextState _state = new();
            private readonly Dictionary<string, FontMetrics> _fontCache = new(StringComparer.Ordinal);

            public TextInterpreter(IViewModelFactory factory, StyleBuilder styleBuilder, ILog? log, PageInterpreter owner, PdfPage page)
            {
                _factory = factory;
                _styleBuilder = styleBuilder;
                _log = log;
                _owner = owner;
                _page = page;
            }

            public void BeginText(GraphicsState graphicsState)
            {
                _state.Reset(graphicsState.Clone());
            }

            public void EndText(ICollection<BaseShapeViewModel> shapes)
            {
                _state.Reset(null);
            }

            public bool TryHandle(COperator op, ICollection<BaseShapeViewModel> shapes)
            {
                if (_state.BaseGraphicsState is null)
                {
                    return false;
                }

                switch (op.OpCode.OpCodeName)
                {
                    case OpCodeName.Tf:
                        HandleTf(op);
                        return true;
                    case OpCodeName.Tc:
                        HandleTc(op);
                        return true;
                    case OpCodeName.Tw:
                        HandleTw(op);
                        return true;
                    case OpCodeName.Tz:
                        HandleTz(op);
                        return true;
                    case OpCodeName.TL:
                        HandleTl(op);
                        return true;
                    case OpCodeName.Tr:
                        HandleTr(op);
                        return true;
                    case OpCodeName.Ts:
                        HandleTs(op);
                        return true;
                    case OpCodeName.Td:
                        HandleTd(op);
                        return true;
                    case OpCodeName.TD:
                        HandleTd(op);
                        HandleTlFromTd(op);
                        return true;
                    case OpCodeName.Tm:
                        HandleTm(op);
                        return true;
                    case OpCodeName.Tx: // T*
                        MoveToNextLine();
                        return true;
                    case OpCodeName.Tj:
                        ShowTextOperand(op, shapes);
                        return true;
                    case OpCodeName.TJ:
                        ShowTextArray(op, shapes);
                        return true;
                    case OpCodeName.QuoteSingle:
                        MoveToNextLine();
                        ShowTextOperand(op, shapes, operandIndex: 0);
                        return true;
                    case OpCodeName.QuoteDouble:
                        HandleQuoteDouble(op, shapes);
                        return true;
                    default:
                        return false;
                }
            }

            private void HandleTf(COperator op)
            {
                if (op.Operands.Count < 2 || op.Operands[0] is not CName name)
                {
                    return;
                }

                var metrics = ResolveFont(name.Name);
                if (metrics is null)
                {
                    _log?.LogWarning($"PDF importer could not resolve font '{name.Name}'.");
                }

                _state.Font = metrics;

                var size = Math.Abs(GetNumber(op, 1));
                _state.FontSize = size > 0.0 ? size : 12.0;
            }

            private void HandleTc(COperator op)
            {
                if (op.Operands.Count == 0)
                {
                    return;
                }

                _state.CharSpacing = GetNumber(op, 0);
            }

            private void HandleTw(COperator op)
            {
                if (op.Operands.Count == 0)
                {
                    return;
                }

                _state.WordSpacing = GetNumber(op, 0);
            }

            private void HandleTz(COperator op)
            {
                if (op.Operands.Count == 0)
                {
                    return;
                }

                var scaling = GetNumber(op, 0);
                _state.HorizontalScaling = scaling / 100.0;
                if (Math.Abs(_state.HorizontalScaling) < double.Epsilon)
                {
                    _state.HorizontalScaling = 1.0;
                }
            }

            private void HandleTl(COperator op)
            {
                if (op.Operands.Count == 0)
                {
                    return;
                }

                _state.Leading = GetNumber(op, 0);
            }

            private void HandleTr(COperator op)
            {
                if (op.Operands.Count == 0)
                {
                    return;
                }

                var mode = (int)Math.Round(GetNumber(op, 0));
                if (mode < 0)
                {
                    mode = 0;
                }
                else if (mode > 7)
                {
                    mode = 7;
                }

                _state.RenderingMode = (TextRenderingMode)mode;
            }

            private void HandleTs(COperator op)
            {
                if (op.Operands.Count == 0)
                {
                    return;
                }

                _state.TextRise = GetNumber(op, 0);
            }

            private void HandleTd(COperator op)
            {
                if (op.Operands.Count < 2)
                {
                    return;
                }

                var tx = GetNumber(op, 0);
                var ty = GetNumber(op, 1);

                var translation = Matrix3x2.CreateTranslation((float)(tx * _state.HorizontalScaling), (float)ty);
                _state.LineMatrix = Matrix3x2.Multiply(_state.LineMatrix, translation);
                _state.TextMatrix = _state.LineMatrix;
            }

            private void HandleTlFromTd(COperator op)
            {
                if (op.Operands.Count < 2)
                {
                    return;
                }

                _state.Leading = -GetNumber(op, 1);
            }

            private void HandleTm(COperator op)
            {
                if (op.Operands.Count < 6)
                {
                    return;
                }

                _state.TextMatrix = new Matrix3x2(
                    (float)GetNumber(op, 0),
                    (float)GetNumber(op, 1),
                    (float)GetNumber(op, 2),
                    (float)GetNumber(op, 3),
                    (float)GetNumber(op, 4),
                    (float)GetNumber(op, 5));
                _state.LineMatrix = _state.TextMatrix;
            }

            private void HandleQuoteDouble(COperator op, ICollection<BaseShapeViewModel> shapes)
            {
                if (op.Operands.Count < 3)
                {
                    return;
                }

                _state.WordSpacing = GetNumber(op, 0);
                _state.CharSpacing = GetNumber(op, 1);

                MoveToNextLine();

                if (op.Operands[2] is CString text)
                {
                    ShowText(text.Value, shapes);
                }
            }

            private void MoveToNextLine()
            {
                var dy = _state.Leading != 0.0 ? -_state.Leading : -_state.FontSize;
                var translation = Matrix3x2.CreateTranslation(0f, (float)dy);
                _state.LineMatrix = Matrix3x2.Multiply(_state.LineMatrix, translation);
                _state.TextMatrix = _state.LineMatrix;
            }

            private void ShowTextOperand(COperator op, ICollection<BaseShapeViewModel> shapes, int operandIndex = 0)
            {
                if (op.Operands.Count <= operandIndex || op.Operands[operandIndex] is not CString str)
                {
                    return;
                }

                ShowText(str.Value, shapes);
            }

            private void ShowText(string? text, ICollection<BaseShapeViewModel> shapes)
            {
                if (string.IsNullOrEmpty(text))
                {
                    return;
                }

                if (_state.RenderingMode == TextRenderingMode.Invisible)
                {
                    AdvanceText(text);
                    return;
                }

                if (_state.Font is null)
                {
                    _log?.LogWarning("PDF importer encountered text with no active font.");
                    AdvanceText(text);
                    return;
                }

                var glyphs = EnumerateGlyphs(text, _state.Font).ToList();
                if (glyphs.Count == 0)
                {
                    return;
                }

                var glyphMatrix = _state.TextMatrix;
                var minX = double.PositiveInfinity;
                var minY = double.PositiveInfinity;
                var maxX = double.NegativeInfinity;
                var maxY = double.NegativeInfinity;

                foreach (var glyph in glyphs)
                {
                    var origin = GetBaselinePosition(glyphMatrix);
                    if (_state.TextRise != 0.0)
                    {
                        origin += TransformVector(glyphMatrix, new Vector2(0f, (float)_state.TextRise));
                    }

                    var advance = ComputeGlyphAdvance(glyph);
                    var advanceVector = TransformVector(glyphMatrix, new Vector2((float)(advance * _state.HorizontalScaling), 0f));
                    var heightVector = TransformVector(glyphMatrix, new Vector2(0f, (float)_state.FontSize));

                    var p0 = origin;
                    var p1 = origin + advanceVector;
                    var p2 = origin + heightVector;
                    var p3 = origin + advanceVector + heightVector;

                    minX = Math.Min(Math.Min(minX, p0.X), Math.Min(p1.X, Math.Min(p2.X, p3.X)));
                    maxX = Math.Max(Math.Max(maxX, p0.X), Math.Max(p1.X, Math.Max(p2.X, p3.X)));
                    minY = Math.Min(Math.Min(minY, p0.Y), Math.Min(p1.Y, Math.Min(p2.Y, p3.Y)));
                    maxY = Math.Max(Math.Max(maxY, p0.Y), Math.Max(p1.Y, Math.Max(p2.Y, p3.Y)));

                    glyphMatrix = Matrix3x2.Multiply(glyphMatrix, Matrix3x2.CreateTranslation((float)(advance * _state.HorizontalScaling), 0f));
                }

                if (double.IsPositiveInfinity(minX) || double.IsPositiveInfinity(minY))
                {
                    return;
                }

                if (_state.BaseGraphicsState is { } gs && !IntersectsClip(gs, minX, minY, maxX, maxY))
                {
                    _state.TextMatrix = glyphMatrix;
                    _state.LineMatrix = glyphMatrix;
                    return;
                }

                var style = _styleBuilder.GetOrCreateTextStyle(_state.BaseGraphicsState, _state.FontSize, _state.Font.ResourceName);
                var topLeft = _factory.CreatePointShape(minX, minY);
                var bottomRight = _factory.CreatePointShape(maxX, maxY);

                var textShape = _factory.CreateTextShape(topLeft, bottomRight, style, text, isStroked: RendersStroke(_state.RenderingMode));
                textShape.Text = text;
                textShape.IsStroked = RendersStroke(_state.RenderingMode);
                textShape.IsFilled = RendersFill(_state.RenderingMode);
                shapes.Add(textShape);

                _state.TextMatrix = glyphMatrix;
                _state.LineMatrix = glyphMatrix;
            }

            private void AdjustTextMatrix(double value)
            {
                var adjustment = -value / 1000.0 * _state.FontSize * _state.HorizontalScaling;
                _state.TextMatrix = _state.TextMatrix * Matrix3x2.CreateTranslation((float)adjustment, 0f);
                _state.LineMatrix = _state.TextMatrix;
            }

            private Vector2 GetBaselinePosition(Matrix3x2 matrix)
            {
                var vector = Vector2.Transform(Vector2.Zero, matrix);
                if (_state.BaseGraphicsState is { } graphicsState)
                {
                    vector = Vector2.Transform(vector, graphicsState.Transform);
                }

                vector = Vector2.Transform(vector, _owner._pageTransform);
                return vector;
            }

            private Vector2 TransformVector(Matrix3x2 matrix, Vector2 vector)
            {
                var transformed = new Vector2(
                    matrix.M11 * vector.X + matrix.M21 * vector.Y,
                    matrix.M12 * vector.X + matrix.M22 * vector.Y);

                if (_state.BaseGraphicsState is { } graphicsState)
                {
                    transformed = Vector2.Transform(transformed, graphicsState.GetTransformWithoutTranslation());
                }

                transformed = Vector2.Transform(transformed, _owner._pageVectorTransform);
                return transformed;
            }

            private void AdvanceText(string text)
            {
                if (string.IsNullOrEmpty(text))
                {
                    return;
                }

                double total = 0.0;
                if (_state.Font is { } font)
                {
                    foreach (var glyph in EnumerateGlyphs(text, font))
                    {
                        total += ComputeGlyphAdvance(glyph);
                    }
                }
                else
                {
                    total = text.Length * (_state.FontSize + _state.CharSpacing);
                }

                var translation = total * _state.HorizontalScaling;
                _state.TextMatrix = _state.TextMatrix * Matrix3x2.CreateTranslation((float)translation, 0f);
                _state.LineMatrix = _state.TextMatrix;
            }

            private FontMetrics? ResolveFont(string resourceName)
            {
                var key = resourceName.StartsWith('/') ? resourceName[1..] : resourceName;

                if (_fontCache.TryGetValue(key, out var metrics))
                {
                    return metrics;
                }

                var fonts = _page.Resources?.Elements.GetDictionary("/Font");
                if (fonts is null)
                {
                    return null;
                }

                if (!TryGetDictionaryValue(fonts.Elements, resourceName, out var fontItem))
                {
                    if (!TryGetDictionaryValue(fonts.Elements, key, out fontItem))
                    {
                        return null;
                    }
                }

                if (ResolveObject(fontItem) is not PdfDictionary fontDictionary)
                {
                    return null;
                }

                metrics = ParseFontMetrics(key, fontDictionary);
                _fontCache[key] = metrics;
                return metrics;
            }

            private FontMetrics ParseFontMetrics(string resourceName, PdfDictionary fontDictionary)
            {
                var metrics = new FontMetrics(resourceName);
                var subtype = fontDictionary.Elements.GetName("/Subtype");
                if (string.Equals(subtype, "/Type0", StringComparison.Ordinal))
                {
                    ParseType0Font(metrics, fontDictionary);
                }
                else
                {
                    ParseSimpleFont(metrics, fontDictionary);
                }

                return metrics;
            }

            private void ParseSimpleFont(FontMetrics metrics, PdfDictionary fontDictionary)
            {
                var firstChar = fontDictionary.Elements.GetInteger("/FirstChar");
                var widths = fontDictionary.Elements.GetArray("/Widths");
                if (widths is not null)
                {
                    for (int i = 0; i < widths.Elements.Count; i++)
                    {
                        var glyphWidth = GetDouble(widths.Elements[i]) / 1000.0;
                        metrics.Widths[firstChar + i] = glyphWidth;
                    }
                }

                var descriptor = fontDictionary.Elements.GetDictionary("/FontDescriptor");
                if (descriptor is not null)
                {
                    var missingWidth = descriptor.Elements.GetInteger("/MissingWidth");
                    if (missingWidth > 0)
                    {
                        metrics.DefaultWidth = missingWidth / 1000.0;
                    }
                }
            }

            private void ParseType0Font(FontMetrics metrics, PdfDictionary fontDictionary)
            {
                var descendants = fontDictionary.Elements.GetArray("/DescendantFonts");
                if (descendants is null || descendants.Elements.Count == 0)
                {
                    return;
                }

                if (ResolveObject(descendants.Elements[0]) is not PdfDictionary cidFont)
                {
                    return;
                }

                metrics.IsCidFont = true;

                var defaultWidth = cidFont.Elements.GetInteger("/DW");
                if (defaultWidth > 0)
                {
                    metrics.DefaultWidth = defaultWidth / 1000.0;
                }

                var wArray = cidFont.Elements.GetArray("/W");
                if (wArray is null)
                {
                    return;
                }

                var elements = wArray.Elements;
                int idx = 0;
                while (idx < elements.Count)
                {
                    var startCid = (int)Math.Round(GetDouble(elements[idx++]));
                    if (idx >= elements.Count)
                    {
                        break;
                    }

                    var next = elements[idx++];
                    if (next is PdfArray rangeArray)
                    {
                        for (int i = 0; i < rangeArray.Elements.Count; i++)
                        {
                            var width = GetDouble(rangeArray.Elements[i]) / 1000.0;
                            metrics.Widths[startCid + i] = width;
                        }
                    }
                    else
                    {
                        if (idx >= elements.Count)
                        {
                            break;
                        }

                        var endCid = (int)Math.Round(GetDouble(next));
                        var width = GetDouble(elements[idx++]) / 1000.0;
                        for (int cid = startCid; cid <= endCid; cid++)
                        {
                            metrics.Widths[cid] = width;
                        }
                    }
                }
            }

            private void ShowTextArray(COperator op, ICollection<BaseShapeViewModel> shapes)
            {
                if (op.Operands.Count == 0 || op.Operands[0] is not CArray array)
                {
                    return;
                }

                foreach (var element in array)
                {
                    switch (element)
                    {
                        case CString str:
                            ShowText(str.Value, shapes);
                            break;
                        case CReal real:
                            AdjustTextMatrix(real.Value);
                            break;
                        case CInteger integer:
                            AdjustTextMatrix(integer.Value);
                            break;
                    }
                }
            }

            private IEnumerable<GlyphInfo> EnumerateGlyphs(string text, FontMetrics font)
            {
                foreach (var ch in text)
                {
                    var code = (int)ch;
                    yield return new GlyphInfo(code, ch == ' ', font.GetWidth(code));
                }
            }

            private double ComputeGlyphAdvance(GlyphInfo glyph)
            {
                var advance = glyph.Width * _state.FontSize + _state.CharSpacing;
                if (glyph.IsSpace)
                {
                    advance += _state.WordSpacing;
                }

                return advance;
            }

            private sealed class FontMetrics
            {
                public FontMetrics(string resourceName)
                {
                    ResourceName = resourceName;
                }

                public string ResourceName { get; }
                public Dictionary<int, double> Widths { get; } = new();
                public double DefaultWidth { get; set; } = 0.5;
                public bool IsCidFont { get; set; }

                public double GetWidth(int codePoint)
                {
                    if (Widths.TryGetValue(codePoint, out var width))
                    {
                        return width;
                    }

                    if (IsCidFont)
                    {
                        // Fallback for CID fonts: use high byte as CID.
                        var cid = codePoint & 0xFFFF;
                        if (Widths.TryGetValue(cid, out width))
                        {
                            return width;
                        }
                    }

                    return DefaultWidth;
                }
            }

            private readonly record struct GlyphInfo(int Code, bool IsSpace, double Width);

            private enum TextRenderingMode
            {
                Fill = 0,
                Stroke = 1,
                FillStroke = 2,
                Invisible = 3,
                FillClip = 4,
                StrokeClip = 5,
                FillStrokeClip = 6,
                Clip = 7
            }

            private sealed class TextState
            {
                public GraphicsState? BaseGraphicsState { get; private set; }
                public Matrix3x2 TextMatrix { get; set; } = Matrix3x2.Identity;
                public Matrix3x2 LineMatrix { get; set; } = Matrix3x2.Identity;
                public double FontSize { get; set; } = 12.0;
                public double Leading { get; set; } = 0.0;
                public double CharSpacing { get; set; } = 0.0;
                public double WordSpacing { get; set; } = 0.0;
                public double HorizontalScaling { get; set; } = 1.0;
                public double TextRise { get; set; } = 0.0;
                public TextRenderingMode RenderingMode { get; set; } = TextRenderingMode.Fill;
                public FontMetrics? Font { get; set; }

                public void Reset(GraphicsState? baseState)
                {
                    BaseGraphicsState = baseState;
                    TextMatrix = Matrix3x2.Identity;
                    LineMatrix = Matrix3x2.Identity;
                    FontSize = 12.0;
                    Leading = 0.0;
                    CharSpacing = 0.0;
                    WordSpacing = 0.0;
                    HorizontalScaling = 1.0;
                    TextRise = 0.0;
                    RenderingMode = TextRenderingMode.Fill;
                    Font = null;
                }
            }

            private static bool RendersFill(TextRenderingMode mode)
            {
                return mode is TextRenderingMode.Fill or TextRenderingMode.FillStroke or TextRenderingMode.FillClip or TextRenderingMode.FillStrokeClip;
            }

            private static bool RendersStroke(TextRenderingMode mode)
            {
                return mode is TextRenderingMode.Stroke or TextRenderingMode.FillStroke or TextRenderingMode.StrokeClip or TextRenderingMode.FillStrokeClip;
            }
        }
    }

    private enum ColorSpaceKind
    {
        DeviceGray,
        DeviceRgb,
        DeviceCmyk,
        Icc
    }

    private readonly record struct PdfColorSpaceInfo(ColorSpaceKind Kind, int Components)
    {
        public static readonly PdfColorSpaceInfo DeviceGray = new(ColorSpaceKind.DeviceGray, 1);
        public static readonly PdfColorSpaceInfo DeviceRgb = new(ColorSpaceKind.DeviceRgb, 3);
        public static readonly PdfColorSpaceInfo DeviceCmyk = new(ColorSpaceKind.DeviceCmyk, 4);

        public static PdfColorSpaceInfo Icc(int components)
        {
            if (components <= 0)
            {
                components = 1;
            }

            return new PdfColorSpaceInfo(ColorSpaceKind.Icc, components);
        }
    }

    private sealed class GraphicsState
    {
        public Matrix3x2 Transform { get; private set; } = Matrix3x2.Identity;
        public PdfColor StrokeColor { get; set; } = PdfColor.Black;
        public PdfColor FillColor { get; set; } = PdfColor.Black;
        public PdfColorSpaceInfo StrokeColorSpace { get; private set; } = PdfColorSpaceInfo.DeviceGray;
        public PdfColorSpaceInfo FillColorSpace { get; private set; } = PdfColorSpaceInfo.DeviceGray;
        public double StrokeAlpha { get; set; } = 1.0;
        public double FillAlpha { get; set; } = 1.0;
        public double SoftMaskAlpha { get; set; } = 1.0;
        public double LineWidth { get; set; } = 1.0;
        public double[]? DashArray { get; private set; }
        public double DashPhase { get; private set; }
        public int LineCap { get; set; }
        public int LineJoin { get; set; }
        public double MiterLimit { get; set; } = 10.0;
        public SKPath? ClipPath { get; private set; }

        public GraphicsState Clone()
        {
            return new GraphicsState
            {
                Transform = Transform,
                StrokeColor = StrokeColor,
                FillColor = FillColor,
                StrokeColorSpace = StrokeColorSpace,
                FillColorSpace = FillColorSpace,
                StrokeAlpha = StrokeAlpha,
                FillAlpha = FillAlpha,
                SoftMaskAlpha = SoftMaskAlpha,
                LineWidth = LineWidth,
                DashArray = DashArray is null ? null : (double[])DashArray.Clone(),
                DashPhase = DashPhase,
                LineCap = LineCap,
                LineJoin = LineJoin,
                MiterLimit = MiterLimit,
                ClipPath = ClipPath is null ? null : new SKPath(ClipPath)
            };
        }

        public void CopyFrom(GraphicsState other)
        {
            Transform = other.Transform;
            StrokeColor = other.StrokeColor;
            FillColor = other.FillColor;
            StrokeColorSpace = other.StrokeColorSpace;
            FillColorSpace = other.FillColorSpace;
            StrokeAlpha = other.StrokeAlpha;
            FillAlpha = other.FillAlpha;
            SoftMaskAlpha = other.SoftMaskAlpha;
            LineWidth = other.LineWidth;
            DashArray = other.DashArray is null ? null : (double[])other.DashArray.Clone();
            DashPhase = other.DashPhase;
            LineCap = other.LineCap;
            LineJoin = other.LineJoin;
            MiterLimit = other.MiterLimit;
            ClipPath = other.ClipPath is null ? null : new SKPath(other.ClipPath);
        }

        public void ConcatenateMatrix(Matrix3x2 matrix)
        {
            Transform = Matrix3x2.Multiply(Transform, matrix);
        }

        public void SetDashPattern(double[]? dashArray, double dashPhase)
        {
            DashArray = dashArray;
            DashPhase = dashPhase;
        }

        public Matrix3x2 GetTransformWithoutTranslation()
        {
            return new Matrix3x2(Transform.M11, Transform.M12, Transform.M21, Transform.M22, 0f, 0f);
        }

        public void SetStrokeColorSpace(PdfColorSpaceInfo colorSpace)
        {
            StrokeColorSpace = colorSpace;
        }

        public void SetFillColorSpace(PdfColorSpaceInfo colorSpace)
        {
            FillColorSpace = colorSpace;
        }

        public PdfColor GetEffectiveStrokeColor()
        {
            return StrokeColor.WithAlpha(StrokeAlpha * SoftMaskAlpha);
        }

        public PdfColor GetEffectiveFillColor()
        {
            return FillColor.WithAlpha(FillAlpha * SoftMaskAlpha);
        }

        public void ApplyClip(SKPath clip)
        {
            if (ClipPath is null)
            {
                ClipPath = new SKPath(clip);
                return;
            }

            using var result = new SKPath();
            using var current = new SKPath(ClipPath);
            if (current.Op(clip, SKPathOp.Intersect, result))
            {
                ClipPath.Dispose();
                ClipPath = new SKPath(result);
            }
        }
    }

    private readonly record struct StyleKey(
        bool HasStroke,
        PdfColor StrokeColor,
        double StrokeWidth,
        int LineCap,
        string? DashKey,
        bool HasFill,
        PdfColor FillColor,
        FillRule FillRule,
        bool IsText,
        double FontSize,
        string? FontName);

    private sealed class StyleBuilder
    {
        private readonly IViewModelFactory _factory;
        private readonly IDictionary<StyleKey, ShapeStyleViewModel> _cache;
        private readonly Matrix3x2 _vectorTransform;
        private readonly double _scale;
        private int _counter;

        public StyleBuilder(IViewModelFactory factory, IDictionary<StyleKey, ShapeStyleViewModel> cache, Matrix3x2 vectorTransform, double scale)
        {
            _factory = factory;
            _cache = cache;
            _vectorTransform = vectorTransform;
            _scale = scale;
        }

        public ShapeStyleViewModel GetOrCreatePathStyle(GraphicsState state, bool stroke, bool fill, FillRule fillRule)
        {
            var strokeWidth = stroke ? ComputeStrokeWidth(state) : 0.0;
            var dashKey = stroke && state.DashArray is { Length: > 0 }
                ? StyleHelper.ConvertDoubleArrayToDashes(ToDashPattern(state, strokeWidth))
                : null;

            var strokeColor = state.GetEffectiveStrokeColor();
            var fillColor = state.GetEffectiveFillColor();

            var key = new StyleKey(
                stroke,
                strokeColor,
                strokeWidth,
                state.LineCap,
                dashKey,
                fill,
                fillColor,
                fillRule,
                false,
                0.0,
                null);

            if (_cache.TryGetValue(key, out var existing))
            {
                return existing;
            }

            var style = _factory.CreateShapeStyle();
            style.Name = $"PdfStyle{++_counter}";

            if (stroke)
            {
                style.Stroke ??= _factory.CreateStrokeStyle();
                style.Stroke.Color = strokeColor.ToArgb(_factory);
                style.Stroke.Thickness = Math.Max(strokeWidth, 0.1);
                style.Stroke.LineCap = state.LineCap switch
                {
                    1 => LineCap.Round,
                    2 => LineCap.Square,
                    _ => LineCap.Flat
                };
                style.Stroke.Dashes = dashKey;
            }
            else
            {
                style.Stroke = null;
            }

            if (fill)
            {
                style.Fill ??= _factory.CreateFillStyle();
                style.Fill.Color = fillColor.ToArgb(_factory);
            }
            else
            {
                style.Fill = null;
            }

            _cache[key] = style;
            return style;
        }

        public ShapeStyleViewModel GetOrCreateTextStyle(GraphicsState state, double fontSizePoints, string? fontName)
        {
            var fontSize = Math.Max(fontSizePoints * _scale, 0.1);
            var strokeColor = state.GetEffectiveStrokeColor();
            var key = new StyleKey(
                true,
                strokeColor,
                0.75,
                0,
                null,
                false,
                PdfColor.Transparent,
                FillRule.Nonzero,
                true,
                fontSize,
                fontName);

            if (_cache.TryGetValue(key, out var existing))
            {
                return existing;
            }

            var style = _factory.CreateShapeStyle();
            style.Name = $"PdfTextStyle{++_counter}";
            style.Fill = null;
            style.Stroke ??= _factory.CreateStrokeStyle();
            style.Stroke.Color = strokeColor.ToArgb(_factory);
            style.Stroke.Thickness = 0.75;
            if (style.TextStyle is { })
            {
                style.TextStyle.FontSize = fontSize;
                style.TextStyle.FontName = fontName ?? style.TextStyle.FontName ?? "Arial";
                style.TextStyle.TextHAlignment = TextHAlignment.Left;
                style.TextStyle.TextVAlignment = TextVAlignment.Bottom;
            }

            _cache[key] = style;
            return style;
        }

        public ShapeStyleViewModel CreateImageStyle()
        {
            var key = new StyleKey(false, PdfColor.Transparent, 0.0, 0, null, false, PdfColor.Transparent, FillRule.Nonzero, false, 0.0, null);
            if (_cache.TryGetValue(key, out var existing))
            {
                return existing;
            }

            var style = _factory.CreateShapeStyle();
            style.Name = $"PdfImageStyle{++_counter}";
            style.Stroke = null;
            style.Fill = null;
            _cache[key] = style;
            return style;
        }

        private double ComputeStrokeWidth(GraphicsState state)
        {
            var lineWidth = state.LineWidth;
            if (lineWidth <= 0.0)
            {
                lineWidth = 1.0;
            }

            var transform = state.GetTransformWithoutTranslation();
            var vx = Vector2.Transform(new Vector2((float)lineWidth, 0f), transform);
            vx = Vector2.Transform(vx, _vectorTransform);
            var vy = Vector2.Transform(new Vector2(0f, (float)lineWidth), transform);
            vy = Vector2.Transform(vy, _vectorTransform);

            var lenX = vx.Length();
            var lenY = vy.Length();

            if (lenX <= 0 && lenY <= 0)
            {
                return lineWidth * _scale;
            }

            if (lenY <= 0)
            {
                return lenX;
            }

            if (lenX <= 0)
            {
                return lenY;
            }

            return (lenX + lenY) / 2.0;
        }

        private double[]? ToDashPattern(GraphicsState state, double strokeWidth)
        {
            if (state.DashArray is null || state.DashArray.Length == 0 || strokeWidth <= double.Epsilon)
            {
                return null;
            }

            var result = new double[state.DashArray.Length];
            for (var i = 0; i < state.DashArray.Length; i++)
            {
                var length = state.DashArray[i] * _scale;
                result[i] = length / strokeWidth;
            }
            return result;
        }
    }

    private readonly record struct PdfColor(double R, double G, double B, double A)
    {
        public static readonly PdfColor Black = new(0.0, 0.0, 0.0, 1.0);
        public static readonly PdfColor Transparent = new(0.0, 0.0, 0.0, 0.0);

        public static PdfColor FromRgb(double r, double g, double b)
        {
            return new PdfColor(Math.Clamp(r, 0.0, 1.0), Math.Clamp(g, 0.0, 1.0), Math.Clamp(b, 0.0, 1.0), 1.0);
        }

        public static PdfColor FromGray(double gray)
        {
            var value = Math.Clamp(gray, 0.0, 1.0);
            return new PdfColor(value, value, value, 1.0);
        }

        public static PdfColor FromCmyk(double c, double m, double y, double k)
        {
            var r = 1.0 - Math.Min(1.0, c + k);
            var g = 1.0 - Math.Min(1.0, m + k);
            var b = 1.0 - Math.Min(1.0, y + k);
            return new PdfColor(Math.Clamp(r, 0.0, 1.0), Math.Clamp(g, 0.0, 1.0), Math.Clamp(b, 0.0, 1.0), 1.0);
        }

        public PdfColor WithAlpha(double alphaMultiplier)
        {
            return new PdfColor(R, G, B, Math.Clamp(A * alphaMultiplier, 0.0, 1.0));
        }

        public ArgbColorViewModel ToArgb(IViewModelFactory factory)
        {
            return factory.CreateArgbColor(
                (byte)Math.Round(A * 255.0),
                (byte)Math.Round(R * 255.0),
                (byte)Math.Round(G * 255.0),
                (byte)Math.Round(B * 255.0));
        }
    }
}
