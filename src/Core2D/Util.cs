using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Skia.Helpers;
using Avalonia.Threading;
using SkiaSharp;

namespace Core2D
{
    public static class Util
    {
        public static void RenderAsPng(Control target, Size size, string path, double dpi = 96)
        {
            var pixelSize = new PixelSize((int)size.Width, (int)size.Height);
            var dpiVector = new Vector(dpi, dpi);
            using var bitmap = new RenderTargetBitmap(pixelSize, dpiVector);
            target.Measure(size);
            target.Arrange(new Rect(size));
            bitmap.Render(target);
            bitmap.Save(path);
        }

        private class CustomRenderTarget : IRenderTarget
        {
            private readonly SKCanvas _canvas;
            private readonly double _dpi;
            
            public CustomRenderTarget(SKCanvas canvas, double dpi)
            {
                _canvas = canvas;
                _dpi = dpi;
            }

            public IDrawingContextImpl CreateDrawingContext(IVisualBrushRenderer visualBrushRenderer)
            {
                return DrawingContextHelper.WrapSkiaCanvas(_canvas, new Vector(_dpi, _dpi), visualBrushRenderer);
            }

            public void Dispose()
            {
                _canvas.Flush();
            }
        }

        public static void RenderAsSvg(Control target, Size size, string path, double dpi = 96)
        {
            using var stream = File.Create(path);
            using var wstream = new SKManagedWStream(stream);
            var bounds = SKRect.Create(new SKSize((float)size.Width, (float)size.Height));
            using var canvas = SKSvgCanvas.Create(bounds, wstream);
            using var renderer = new ImmediateRenderer(target);
            target.Measure(size);
            target.Arrange(new Rect(size));
            using var renderTarget = new CustomRenderTarget(canvas, dpi);
            ImmediateRenderer.Render(target, renderTarget);
        }

        public static void RenderAsPdf(Control target, Size size, string path, double dpi = 72)
        {
            using var stream = File.Create(path);
            using var wstream = new SKManagedWStream(stream);
            using var document = SKDocument.CreatePdf(stream, (float)dpi);
            using var canvas = document.BeginPage((float)size.Width, (float)size.Height);
            using var renderer = new ImmediateRenderer(target);
            target.Measure(size);
            target.Arrange(new Rect(size));
            using var renderTarget = new CustomRenderTarget(canvas, dpi);
            ImmediateRenderer.Render(target, renderTarget);
        }

        public static void Render(Control control, Size size, string path)
        {
            if (control is null)
            {
                return;
            }
            
            if (path.EndsWith("png", StringComparison.OrdinalIgnoreCase))
            {
                RenderAsPng(control, size, path);
            }

            if (path.EndsWith("pdf", StringComparison.OrdinalIgnoreCase))
            {
                RenderAsPdf(control, size, path);
            }

            if (path.EndsWith("svg", StringComparison.OrdinalIgnoreCase))
            {
                RenderAsSvg(control, size, path);
            }
        }

        public static async Task RunUiJob(Action action)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                action.Invoke();
                Dispatcher.UIThread.RunJobs();
            });
        }
    }
}
