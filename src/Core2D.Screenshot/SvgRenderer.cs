using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Rendering;
using SkiaSharp;

namespace Core2D.Screenshot
{
    public static class SvgRenderer
    {
        public static void Render(Control target, Size size, Stream stream, double dpi = 96)
        {
            using var wstream = new SKManagedWStream(stream);
            var bounds = SKRect.Create(new SKSize((float)size.Width, (float)size.Height));
            using var canvas = SKSvgCanvas.Create(bounds, wstream);
            target.Measure(size);
            target.Arrange(new Rect(size));
            using var renderTarget = new CanvasRenderTarget(canvas, dpi);
            ImmediateRenderer.Render(target, renderTarget);
        }
    }
}
