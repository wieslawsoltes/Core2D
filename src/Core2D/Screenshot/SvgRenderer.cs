using System.IO;
using Avalonia;
using Avalonia.Controls;
using SkiaSharp;

namespace Core2D.Screenshot
{
    public static class SvgRenderer
    {
        public static void Render(Control target, Size size, Stream stream, double dpi = 96, bool useDeferredRenderer = false)
        {
            using var wstream = new SKManagedWStream(stream);
            var bounds = SKRect.Create(new SKSize((float)size.Width, (float)size.Height));
            using var canvas = SKSvgCanvas.Create(bounds, wstream);
            target.Measure(size);
            target.Arrange(new Rect(size));
            CanvasRenderer.Render(target, canvas, dpi, useDeferredRenderer);
        }
    }
}
