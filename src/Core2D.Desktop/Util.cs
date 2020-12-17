using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Rendering;
using Avalonia.Skia.Helpers;
using Avalonia.Threading;
using SkiaSharp;

namespace Core2D.Desktop
{
    public static class Util
    {
        public static void RenderAsPng(Control? target, Size size, string path, double dpi = 96)
        {
            var pixelSize = new PixelSize((int)size.Width, (int)size.Height);
            var dpiVector = new Vector(dpi, dpi);
            using var bitmap = new RenderTargetBitmap(pixelSize, dpiVector);
            target?.Measure(size);
            target?.Arrange(new Rect(size));
            bitmap.Render(target);
            bitmap.Save(path);
        }

        public static void RenderAsSvg(Control? target, Size size, string path, double dpi = 96)
        {
            using var stream = File.Create(path);
            using var wstream = new SKManagedWStream(stream);
            var bounds = SKRect.Create(new SKSize((float)size.Width, (float)size.Height));
            using var canvas = SKSvgCanvas.Create(bounds, wstream);
            using var impl = DrawingContextHelper.WrapSkiaCanvas(canvas, new Vector(dpi, dpi));
            using var context = new DrawingContext(impl);
            target?.Measure(size);
            target?.Arrange(new Rect(size));
            target?.Render(context);
            canvas.Flush();
        }
        
        public static void RenderAsPdf(Control? target, Size size, string path, double dpi = 96)
        {
            using var stream = File.Create(path);
            using var wstream = new SKManagedWStream(stream);
            using var document = SKDocument.CreatePdf(stream, (float)dpi);
            using var canvas = document.BeginPage((float)size.Width, (float)size.Height);
            using var impl = DrawingContextHelper.WrapSkiaCanvas(canvas, new Vector(dpi, dpi));
            using var context = new DrawingContext(impl);
            target?.Measure(size);
            target?.Arrange(new Rect(size));
            target?.Render(context);
            canvas.Flush();
        }
        
        public static void Render(Control? control, Size size, string path)
        {
            if (path.EndsWith("png", StringComparison.OrdinalIgnoreCase))
            {
                Util.RenderAsPng(control, size, path);
            }

            if (path.EndsWith("pdf", StringComparison.OrdinalIgnoreCase))
            {
                Util.RenderAsPdf(control, size, path);
            }

            if (path.EndsWith("svg", StringComparison.OrdinalIgnoreCase))
            {
                Util.RenderAsSvg(control, size, path);
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
