using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Rendering;
using SkiaSharp;

namespace Core2D.Screenshot
{
    public static class PdfRenderer
    {
        public static void Render(Control target, Size size, Stream stream, double dpi = 72)
        {
            using var wstream = new SKManagedWStream(stream);
            using var document = SKDocument.CreatePdf(stream, (float)dpi);
            using var canvas = document.BeginPage((float)size.Width, (float)size.Height);
            target.Measure(size);
            target.Arrange(new Rect(size));
            using var renderTarget = new CanvasRenderTarget(canvas, dpi);
            ImmediateRenderer.Render(target, renderTarget);
        }
    }
}
