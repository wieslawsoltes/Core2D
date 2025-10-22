// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Core2D.Model;
using Core2D.ViewModels;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using Core2D.Model.Style;
using Core2D.Modules.Renderer.SkiaSharp;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Graphics.Colors;
using UglyToad.PdfPig.Graphics;
using SkiaPathConverter = Core2D.Modules.Renderer.SkiaSharp.PathGeometryConverter;
using SkiaSharp;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Rendering.Skia;
using UglyToad.PdfPig.Rendering.Skia.Helpers;

namespace Core2D.Modules.Renderer.PdfSharp;

internal sealed class PdfImporter : IPdfImporter
{
    private const double Scale = 96.0 / 72.0;

    private const bool PreferLetterExtraction = true;

    private static readonly PdfImportResult EmptyResult = new(
        new List<BaseShapeViewModel>(),
        new List<ShapeStyleViewModel>(),
        new List<PdfImportedImage>(),
        0.0,
        0.0);

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
            return EmptyResult;
        }

        try
        {
            using var memoryStream = new MemoryStream(buffer, writable: false);
            using var document = PdfDocument.Open(memoryStream);

            document.AddSkiaPageFactory();

            var shapes = new List<BaseShapeViewModel>();
            var images = new Dictionary<string, PdfImportedImage>(StringComparer.Ordinal);
            var styleBuilder = new StyleBuilder(_viewModelFactory, Scale);

            double maxWidth = 0.0;
            double totalHeight = 0.0;

            for (int pageNumber = 1; pageNumber <= document.NumberOfPages; pageNumber++)
            {
                var captureGlyphs = !PreferLetterExtraction;
                var collector = new SkiaRenderCollector(
                    _viewModelFactory,
                    styleBuilder,
                    shapes,
                    images,
                    Scale,
                    totalHeight,
                    _log,
                    captureGlyphs);

                using (SkiaRenderCapture.Begin(collector))
                using (document.GetPage<SKPicture>(pageNumber))
                {
                    // Rendering side-effects populate the collector via SkiaRenderCapture.
                }

                if (PreferLetterExtraction)
                {
                    var page = document.GetPage(pageNumber);
                    collector.ProcessLetters(page.Letters, page.Height);
                }

                maxWidth = Math.Max(maxWidth, collector.PageWidth * Scale);
                totalHeight += collector.PageHeight * Scale;
            }

            return new PdfImportResult(
                shapes,
                styleBuilder.Styles.ToList(),
                images.Values.ToList(),
                maxWidth,
                totalHeight);
        }
        catch (Exception ex)
        {
            _log?.LogError($"PDF importer failed: {ex.Message}");
            return null;
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

    private sealed class SkiaRenderCollector : ISkiaPageRenderListener
    {
        private const double BaselineTolerance = 0.75;
        private const double FontSizeTolerance = 0.5;
        private const double ScaleTolerance = 0.01;
        private const float AxisTolerance = 1e-3f;

        private readonly IViewModelFactory _factory;
        private readonly StyleBuilder _styleBuilder;
        private readonly IList<BaseShapeViewModel> _shapes;
        private readonly IDictionary<string, PdfImportedImage> _images;
        private readonly double _scale;
        private readonly double _pageOffset;
        private readonly ILog? _log;
        private readonly bool _captureGlyphs;

        private readonly Queue<PendingTextPath> _textPaths = new();
        private GlyphRunBuilder? _currentRun;

        public SkiaRenderCollector(
            IViewModelFactory factory,
            StyleBuilder styleBuilder,
            IList<BaseShapeViewModel> shapes,
            IDictionary<string, PdfImportedImage> images,
            double scale,
            double pageOffset,
            ILog? log,
            bool captureGlyphs)
        {
            _factory = factory;
            _styleBuilder = styleBuilder;
            _shapes = shapes;
            _images = images;
            _captureGlyphs = captureGlyphs;
            _scale = scale;
            _pageOffset = pageOffset;
            _log = log;
        }

        public double PageWidth { get; private set; }

        public double PageHeight { get; private set; }

        public void BeginPage(SkiaRenderPageInfo pageInfo)
        {
            FlushCurrentRun();
            FlushPendingTextPaths();
            PageWidth = pageInfo.Width;
            PageHeight = pageInfo.Height;
        }

        public void OnPath(SkiaRenderPath path)
        {
            using (path)
            {
                try
                {
                    if (path.Stroke is null && path.Fill is null)
                    {
                        return;
                    }

                    using var transformed = new SKPath(path.Path);
                    ApplyPageTransform(transformed);

                    var geometry = SkiaPathConverter.ToPathGeometry(transformed, _factory);
                    if (geometry is null)
                    {
                        return;
                    }

                    geometry.IsStroked = path.Stroke is not null;
                    geometry.IsFilled = path.Fill is not null;

                    var style = _styleBuilder.GetOrCreate(path.Stroke, path.Fill);
                    geometry.Style = style;

                    if (path.IsText)
                    {
                        _textPaths.Enqueue(new PendingTextPath(geometry, geometry.IsFilled, geometry.IsStroked));
                        return;
                    }

                    _shapes.Add(geometry);
                }
                catch (Exception ex)
                {
                    _log?.LogError($"PDF importer path conversion failed: {ex.Message}");
                }
            }
        }

        public void OnGlyph(SkiaRenderGlyph glyph)
        {
            if (!_captureGlyphs)
            {
                return;
            }
            if (glyph.Unicode == "\n" || glyph.Unicode == "\r")
            {
                FlushCurrentRun();
                var newlinePaths = DequeueGlyphPaths(glyph, out _);
                FlushGlyphPaths(newlinePaths);
                return;
            }

            var glyphPaths = DequeueGlyphPaths(glyph, out var pathMismatch);

            if (!CanConvertGlyph(glyph, pathMismatch))
            {
                FlushCurrentRun();
                FlushGlyphPaths(glyphPaths);
                return;
            }

            var glyphInfo = BuildGlyphInfo(glyph);
            if (glyphInfo is null)
            {
                FlushCurrentRun();
                FlushGlyphPaths(glyphPaths);
                return;
            }

            _currentRun ??= new GlyphRunBuilder(BaselineTolerance, FontSizeTolerance, ScaleTolerance);

            if (!_currentRun.TryAdd(glyphInfo.Value))
            {
                FlushCurrentRun();
                _currentRun ??= new GlyphRunBuilder(BaselineTolerance, FontSizeTolerance, ScaleTolerance);

                if (!_currentRun.TryAdd(glyphInfo.Value))
                {
                    FlushCurrentRun();
                    FlushGlyphPaths(glyphPaths);
                }
            }
        }

        public void OnImage(SkiaRenderImage image)
        {
            try
            {
                var key = $"Images/ImportedPdf/{Guid.NewGuid():N}.png";
                if (!_images.ContainsKey(key))
                {
                    _images[key] = new PdfImportedImage(key, image.Data);
                }

                var style = _styleBuilder.GetOrCreateImageStyle();

                var left = image.Destination.Left * _scale;
                var top = image.Destination.Top * _scale + _pageOffset;
                var right = image.Destination.Right * _scale;
                var bottom = image.Destination.Bottom * _scale + _pageOffset;

                var imageShape = _factory.CreateImageShape(
                    left,
                    top,
                    right,
                    bottom,
                    style,
                    key,
                    isStroked: false,
                    isFilled: false);

                _shapes.Add(imageShape);
            }
            catch (Exception ex)
            {
                _log?.LogError($"PDF importer image conversion failed: {ex.Message}");
            }
        }

        public void ProcessLetters(IReadOnlyList<Letter> letters, double pageHeight)
        {
            if (_captureGlyphs)
            {
                return;
            }

            if (letters is null || letters.Count == 0)
            {
                FlushPendingTextPaths();
                return;
            }

            var builder = new GlyphRunBuilder(BaselineTolerance, FontSizeTolerance, ScaleTolerance);
            var pendingRuns = new List<TextRun>();
            var fallbackToGeometry = false;

            void FlushBuilder()
            {
                var run = builder.ToRun();
                builder = new GlyphRunBuilder(BaselineTolerance, FontSizeTolerance, ScaleTolerance);
                if (run is null || string.IsNullOrEmpty(run.Value.Text))
                {
                    return;
                }

                pendingRuns.Add(run.Value);
            }

            foreach (var letter in letters)
            {
                var glyphInfo = BuildGlyphInfo(letter, pageHeight);
                if (glyphInfo is null)
                {
                    fallbackToGeometry = true;
                    break;
                }

                if (!builder.TryAdd(glyphInfo.Value))
                {
                    if (builder.HasGlyphs)
                    {
                        FlushBuilder();
                    }

                    if (!builder.TryAdd(glyphInfo.Value))
                    {
                        fallbackToGeometry = true;
                        break;
                    }
                }
            }

            if (!fallbackToGeometry && builder.HasGlyphs)
            {
                FlushBuilder();
            }

            if (fallbackToGeometry)
            {
                while (_textPaths.Count > 0)
                {
                    AddPathToShapes(_textPaths.Dequeue());
                }
                return;
            }

            foreach (var run in pendingRuns)
            {
                var style = _styleBuilder.GetOrCreateTextStyle(run.Fill, run.Stroke, run.FontName, run.FontSize);
                var topLeft = _factory.CreatePointShape(run.Left, run.Top);
                var bottomRight = _factory.CreatePointShape(run.Right, run.Bottom);
                var textShape = _factory.CreateTextShape(topLeft, bottomRight, style, run.Text, isStroked: run.Stroke is not null);
                textShape.IsFilled = run.Fill is not null;
                textShape.Text = run.Text;
                _shapes.Add(textShape);
            }

            FlushPendingTextPaths();
        }

        public void EndPage()
        {
            if (_captureGlyphs)
            {
                FlushCurrentRun();
                FlushPendingTextPaths();
            }
        }

        private List<PendingTextPath> DequeueGlyphPaths(SkiaRenderGlyph glyph, out bool mismatch)
        {
            var expected = 0;
            if (glyph.Fill is not null)
            {
                expected++;
            }
            if (glyph.Stroke is not null)
            {
                expected++;
            }

            var collected = new List<PendingTextPath>(expected);
            mismatch = false;

            for (var i = 0; i < expected; i++)
            {
                if (_textPaths.Count == 0)
                {
                    mismatch = expected > 0;
                    break;
                }

                collected.Add(_textPaths.Dequeue());
            }

            if (expected > 0 && collected.Count != expected)
            {
                mismatch = true;
            }

            return collected;
        }
        private void FlushGlyphPaths(IEnumerable<PendingTextPath> paths)
        {
            foreach (var pending in paths)
            {
                AddPathToShapes(pending);
            }
        }
        private bool CanConvertGlyph(SkiaRenderGlyph glyph, bool pathMismatch)
        {
            if (pathMismatch)
            {
                return false;
            }

            if (string.IsNullOrEmpty(glyph.Unicode))
            {
                return false;
            }

            if (!IsAxisAligned(glyph.Transform))
            {
                return false;
            }

            if (glyph.Fill is null && glyph.Stroke is null)
            {
                return string.IsNullOrWhiteSpace(glyph.Unicode);
            }

            return true;
        }

        private GlyphInfo? BuildGlyphInfo(Letter letter, double pageHeight)
        {
            var rect = letter.GlyphRectangle;
            var left = Math.Min(rect.Left, rect.Right) * _scale;
            var right = Math.Max(rect.Left, rect.Right) * _scale;
            var top = (pageHeight - Math.Max(rect.Top, rect.Bottom)) * _scale + _pageOffset;
            var bottom = (pageHeight - Math.Min(rect.Top, rect.Bottom)) * _scale + _pageOffset;

            if (double.IsNaN(left) || double.IsNaN(top) || double.IsInfinity(left) || double.IsInfinity(top))
            {
                return null;
            }

            var fill = new SkiaFillStyle(SKColors.Black, SKColors.Black.Alpha / 255f);

            return new GlyphInfo(
                letter.Value,
                left,
                top,
                right,
                bottom,
                1.0,
                1.0,
                fill,
                null,
                letter.PointSize * _scale,
                letter.Font?.Name ?? string.Empty);
        }

        private GlyphInfo? BuildGlyphInfo(SkiaRenderGlyph glyph)
        {
            var bounds = glyph.Bounds;
            var left = Math.Min(bounds.Left, bounds.Right) * _scale;
            var right = Math.Max(bounds.Left, bounds.Right) * _scale;
            var top = Math.Min(bounds.Top, bounds.Bottom) * _scale + _pageOffset;
            var bottom = Math.Max(bounds.Top, bounds.Bottom) * _scale + _pageOffset;

            if (double.IsNaN(left) || double.IsNaN(top) || double.IsInfinity(left) || double.IsInfinity(top))
            {
                return null;
            }

            return new GlyphInfo(
                glyph.Unicode,
                left,
                top,
                right,
                bottom,
                glyph.Transform.ScaleX,
                glyph.Transform.ScaleY,
                glyph.Fill,
                glyph.Stroke,
                glyph.FontSize * _scale,
                glyph.FontName);
        }

        private void FlushCurrentRun()
        {
            if (_currentRun is null)
            {
                return;
            }

            var run = _currentRun.ToRun();
            _currentRun = null;

            if (run is null || string.IsNullOrEmpty(run.Value.Text))
            {
                return;
            }

            var result = run.Value;

            var style = _styleBuilder.GetOrCreateTextStyle(result.Fill, result.Stroke, result.FontName, result.FontSize);
            var topLeft = _factory.CreatePointShape(result.Left, result.Top);
            var bottomRight = _factory.CreatePointShape(result.Right, result.Bottom);
            var textShape = _factory.CreateTextShape(topLeft, bottomRight, style, result.Text, isStroked: result.Stroke is not null);
            textShape.IsFilled = result.Fill is not null;
            textShape.Text = result.Text;
            _shapes.Add(textShape);
        }

        private void FlushPendingTextPaths()
        {
            while (_textPaths.Count > 0)
            {
                var pending = _textPaths.Dequeue();
                if (_captureGlyphs)
                {
                    AddPathToShapes(pending);
                }
            }
        }

        private void AddPathToShapes(PendingTextPath pending)
        {
            _shapes.Add(pending.Geometry);
        }

        private static bool IsAxisAligned(SKMatrix matrix)
        {
            return Math.Abs(matrix.SkewX) <= AxisTolerance && Math.Abs(matrix.SkewY) <= AxisTolerance;
        }

        private static bool ApproximatelyEqual(double a, double b, double tolerance)
        {
            return Math.Abs(a - b) <= tolerance;
        }

        private static bool AreFillStylesEquivalent(SkiaFillStyle? a, SkiaFillStyle? b)
        {
            if (a is null || b is null)
            {
                return a is null && b is null;
            }

            return a.Color == b.Color && Math.Abs(a.Alpha - b.Alpha) <= 0.001f;
        }

        private static bool AreStrokeStylesEquivalent(SkiaStrokeStyle? a, SkiaStrokeStyle? b)
        {
            if (a is null || b is null)
            {
                return a is null && b is null;
            }

            return a.Color == b.Color && Math.Abs(a.Alpha - b.Alpha) <= 0.001f && Math.Abs(a.Width - b.Width) <= 0.1f;
        }

        private void ApplyPageTransform(SKPath path)
        {
            var scaleMatrix = SKMatrix.CreateScale((float)_scale, (float)_scale);
            path.Transform(scaleMatrix);

            if (_pageOffset > 0.0)
            {
                var offsetMatrix = SKMatrix.CreateTranslation(0f, (float)_pageOffset);
                path.Transform(offsetMatrix);
            }
        }

        private readonly record struct GlyphInfo(
            string Text,
            double Left,
            double Top,
            double Right,
            double Bottom,
            double ScaleX,
            double ScaleY,
            SkiaFillStyle? Fill,
            SkiaStrokeStyle? Stroke,
            double FontSize,
            string? FontName);

        private sealed record class PendingTextPath(PathShapeViewModel Geometry, bool IsFill, bool IsStroke);

        private readonly record struct TextRun(
            string Text,
            double Left,
            double Top,
            double Right,
            double Bottom,
            SkiaFillStyle? Fill,
            SkiaStrokeStyle? Stroke,
            double FontSize,
            string? FontName);

        private sealed class GlyphRunBuilder
        {
            private readonly double _baselineTolerance;
            private readonly double _fontSizeTolerance;
            private readonly double _scaleTolerance;

            private readonly List<GlyphInfo> _glyphs = new();

            public bool HasGlyphs => _glyphs.Count > 0;


            private double _baseline;
            private double _scaleX;
            private double _scaleY;
            private SkiaFillStyle? _fill;
            private SkiaStrokeStyle? _stroke;
            private string? _fontName;
            private double _fontSize;
            private double _minX;
            private double _minY;
            private double _maxX;
            private double _maxY;
            private bool _initialized;

            public GlyphRunBuilder(double baselineTolerance, double fontSizeTolerance, double scaleTolerance)
            {
                _baselineTolerance = baselineTolerance;
                _fontSizeTolerance = fontSizeTolerance;
                _scaleTolerance = scaleTolerance;
            }

            public bool TryAdd(GlyphInfo glyph)
            {
                if (!_initialized)
                {
                    Initialize(glyph);
                    return true;
                }

                if (!ApproximatelyEqual(_scaleX, glyph.ScaleX, _scaleTolerance) ||
                    !ApproximatelyEqual(_scaleY, glyph.ScaleY, _scaleTolerance))
                {
                    return false;
                }

                if (!ApproximatelyEqual(_baseline, glyph.Bottom, _baselineTolerance))
                {
                    return false;
                }

                if (!AreFillStylesEquivalent(_fill, glyph.Fill) || !AreStrokeStylesEquivalent(_stroke, glyph.Stroke))
                {
                    return false;
                }

                if (!string.Equals(_fontName, glyph.FontName, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                if (!ApproximatelyEqual(_fontSize, glyph.FontSize, _fontSizeTolerance))
                {
                    return false;
                }

                _glyphs.Add(glyph);
                _minX = Math.Min(_minX, glyph.Left);
                _minY = Math.Min(_minY, glyph.Top);
                _maxX = Math.Max(_maxX, glyph.Right);
                _maxY = Math.Max(_maxY, glyph.Bottom);
                return true;
            }

            public TextRun? ToRun()
            {
                if (_glyphs.Count == 0)
                {
                    return null;
                }

                var builder = new StringBuilder();
                foreach (var glyph in _glyphs)
                {
                    builder.Append(glyph.Text);
                }

                return new TextRun(builder.ToString(), _minX, _minY, _maxX, _maxY, _fill, _stroke, _fontSize, _fontName);
            }

            private void Initialize(GlyphInfo glyph)
            {
                _glyphs.Clear();
                _glyphs.Add(glyph);
                _baseline = glyph.Bottom;
                _scaleX = glyph.ScaleX;
                _scaleY = glyph.ScaleY;
                _fill = glyph.Fill;
                _stroke = glyph.Stroke;
                _fontName = glyph.FontName;
                _fontSize = glyph.FontSize;
                _minX = glyph.Left;
                _minY = glyph.Top;
                _maxX = glyph.Right;
                _maxY = glyph.Bottom;
                _initialized = true;
            }
        }
    }
    private readonly record struct StyleKey(
        bool HasStroke,
        uint StrokeColor,
        double StrokeWidth,
        Core2D.Model.Style.LineCap StrokeCap,
        string? DashKey,
        double DashOffset,
        bool HasFill,
        uint FillColor,
        bool IsImage,
        bool IsText,
        double FontSize,
        string? FontName);

    private sealed class StyleBuilder
    {
        private readonly IViewModelFactory _factory;
        private readonly double _scale;
        private readonly Dictionary<StyleKey, ShapeStyleViewModel> _cache = new();
        private ShapeStyleViewModel? _imageStyle;
        private int _index;

        public StyleBuilder(IViewModelFactory factory, double scale)
        {
            _factory = factory;
            _scale = scale;
        }

        public IEnumerable<ShapeStyleViewModel> Styles => _cache.Values;

        public ShapeStyleViewModel GetOrCreate(SkiaStrokeStyle? stroke, SkiaFillStyle? fill)
        {
            var key = CreateKey(stroke, fill);
            if (_cache.TryGetValue(key, out var style))
            {
                return style;
            }

            style = _factory.CreateShapeStyle();
            style.Name = $"PdfStyle{++_index}";

            if (stroke is not null)
            {
                style.Stroke ??= _factory.CreateStrokeStyle();
                style.Stroke.Color = CreateColor(stroke.Color);
                style.Stroke.Thickness = Math.Max(stroke.Width * _scale, 0.1);
                style.Stroke.LineCap = MapLineCap(stroke.Cap);
                style.Stroke.Dashes = stroke.DashArray is { Length: > 0 }
                    ? StyleHelper.ConvertFloatArrayToDashes(stroke.DashArray)
                    : null;
                style.Stroke.DashOffset = stroke.DashPhase * _scale;
                style.Stroke.DashScale = 1.0;
            }
            else
            {
                style.Stroke = null;
            }

            if (fill is not null)
            {
                style.Fill ??= _factory.CreateFillStyle();
                style.Fill.Color = CreateColor(fill.Color);
            }
            else
            {
                style.Fill = null;
            }

            _cache[key] = style;
            return style;
        }

        public ShapeStyleViewModel GetOrCreateImageStyle()
        {
            _imageStyle ??= CreateImageStyleInternal();
            return _imageStyle;
        }

        public ShapeStyleViewModel GetOrCreateTextStyle(SkiaFillStyle? fill, SkiaStrokeStyle? stroke, string? fontName, double fontSize)
        {
            var textColorSource = fill?.Color ?? stroke?.Color ?? SKColors.Black;
            var strokeColor = ToUint(textColorSource);
            var strokeWidth = stroke is not null ? Math.Max(stroke.Width * _scale, 0.1) : Math.Max(1.0 * _scale, 0.1);
            var strokeCap = stroke is not null ? MapLineCap(stroke.Cap) : Core2D.Model.Style.LineCap.Flat;
            var hasFill = fill is not null;
            var fillColor = hasFill ? ToUint(fill!.Color) : 0u;

            var key = new StyleKey(
                true,
                strokeColor,
                strokeWidth,
                strokeCap,
                null,
                0.0,
                hasFill,
                fillColor,
                false,
                true,
                fontSize,
                fontName ?? string.Empty);

            if (_cache.TryGetValue(key, out var style))
            {
                return style;
            }

            style = _factory.CreateShapeStyle();
            style.Name = $"PdfTextStyle{++_index}";

            style.Stroke ??= _factory.CreateStrokeStyle();
            style.Stroke.Color = CreateColor(textColorSource);
            style.Stroke.Thickness = strokeWidth;
            style.Stroke.LineCap = strokeCap;
            style.Stroke.Dashes = null;
            style.Stroke.DashOffset = 0.0;
            style.Stroke.DashScale = 1.0;

            if (hasFill)
            {
                style.Fill ??= _factory.CreateFillStyle();
                style.Fill.Color = CreateColor(fill!.Color);
            }
            else
            {
                style.Fill = null;
            }

            if (style.TextStyle is { } textStyle)
            {
                if (!string.IsNullOrEmpty(fontName))
                {
                    textStyle.FontName = fontName;
                }

                if (fontSize > 0.0)
                {
                    textStyle.FontSize = fontSize;
                }

                textStyle.TextHAlignment = TextHAlignment.Left;
                textStyle.TextVAlignment = TextVAlignment.Top;
                textStyle.UseTextBackground = false;
            }

            _cache[key] = style;
            return style;
        }

        private StyleKey CreateKey(SkiaStrokeStyle? stroke, SkiaFillStyle? fill)
        {
            var hasStroke = stroke is not null;
            var strokeColor = hasStroke ? ToUint(stroke!.Color) : 0u;
            var strokeWidth = hasStroke ? Math.Max(stroke!.Width * _scale, 0.1) : 0.0;
            var strokeCap = hasStroke ? MapLineCap(stroke!.Cap) : Core2D.Model.Style.LineCap.Flat;
            var dashKey = hasStroke && stroke!.DashArray is { Length: > 0 }
                ? StyleHelper.ConvertFloatArrayToDashes(stroke.DashArray)
                : null;
            var dashOffset = hasStroke ? stroke!.DashPhase * _scale : 0.0;

            var hasFill = fill is not null;
            var fillColor = hasFill ? ToUint(fill!.Color) : 0u;

            return new StyleKey(
                hasStroke,
                strokeColor,
                strokeWidth,
                strokeCap,
                dashKey,
                dashOffset,
                hasFill,
                fillColor,
                false,
                false,
                0.0,
                null);
        }


        private static uint ToUint(SKColor color)
        {
            return (uint)((uint)color.Alpha << 24 | (uint)color.Red << 16 | (uint)color.Green << 8 | color.Blue);
        }
        private ShapeStyleViewModel CreateImageStyleInternal()
        {
            var key = new StyleKey(false, 0u, 0.0, Core2D.Model.Style.LineCap.Flat, null, 0.0, false, 0u, true, false, 0.0, null);
            if (_cache.TryGetValue(key, out var existing))
            {
                return existing;
            }

            var style = _factory.CreateShapeStyle();
            style.Name = $"PdfImageStyle{++_index}";
            style.Stroke = null;
            style.Fill = null;
            _cache[key] = style;
            return style;
        }

        private ArgbColorViewModel CreateColor(SKColor color)
        {
            return _factory.CreateArgbColor(color.Alpha, color.Red, color.Green, color.Blue);
        }

        private static Core2D.Model.Style.LineCap MapLineCap(SKStrokeCap cap)
        {
            return cap switch
            {
                SKStrokeCap.Round => Core2D.Model.Style.LineCap.Round,
                SKStrokeCap.Square => Core2D.Model.Style.LineCap.Square,
                _ => Core2D.Model.Style.LineCap.Flat
            };
        }
    }
}
