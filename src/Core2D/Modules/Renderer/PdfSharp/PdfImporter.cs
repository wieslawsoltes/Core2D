// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core2D.Model;
using Core2D.ViewModels;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using Core2D.Modules.Renderer.SkiaSharp;
using SkiaPathConverter = Core2D.Modules.Renderer.SkiaSharp.PathGeometryConverter;
using SkiaSharp;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Rendering.Skia;

namespace Core2D.Modules.Renderer.PdfSharp;

internal sealed class PdfImporter : IPdfImporter
{
    private const double Scale = 96.0 / 72.0;

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
                var collector = new SkiaRenderCollector(
                    _viewModelFactory,
                    styleBuilder,
                    shapes,
                    images,
                    Scale,
                    totalHeight,
                    _log);

                using (SkiaRenderCapture.Begin(collector))
                using (document.GetPage<SKPicture>(pageNumber))
                {
                    // Rendering side-effects populate the collector via SkiaRenderCapture.
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
        private readonly IViewModelFactory _factory;
        private readonly StyleBuilder _styleBuilder;
        private readonly IList<BaseShapeViewModel> _shapes;
        private readonly IDictionary<string, PdfImportedImage> _images;
        private readonly double _scale;
        private readonly double _pageOffset;
        private readonly ILog? _log;

        public SkiaRenderCollector(
            IViewModelFactory factory,
            StyleBuilder styleBuilder,
            IList<BaseShapeViewModel> shapes,
            IDictionary<string, PdfImportedImage> images,
            double scale,
            double pageOffset,
            ILog? log)
        {
            _factory = factory;
            _styleBuilder = styleBuilder;
            _shapes = shapes;
            _images = images;
            _scale = scale;
            _pageOffset = pageOffset;
            _log = log;
        }

        public double PageWidth { get; private set; }

        public double PageHeight { get; private set; }

        public void BeginPage(SkiaRenderPageInfo pageInfo)
        {
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

                    _shapes.Add(geometry);
                }
                catch (Exception ex)
                {
                    _log?.LogError($"PDF importer path conversion failed: {ex.Message}");
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

        public void EndPage()
        {
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
        bool IsImage);

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
                false);
        }


        private static uint ToUint(SKColor color)
        {
            return (uint)((uint)color.Alpha << 24 | (uint)color.Red << 16 | (uint)color.Green << 8 | color.Blue);
        }
        private ShapeStyleViewModel CreateImageStyleInternal()
        {
            var key = new StyleKey(false, 0u, 0.0, Core2D.Model.Style.LineCap.Flat, null, 0.0, false, 0u, true);
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
