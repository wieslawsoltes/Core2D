using System;
using System.IO;
using System.Threading.Tasks;
using SkiaSharp;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Rendering;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Skia.Helpers;

namespace Avalonia.Screenshot
{
    public static class CanvasRenderer
    {
        public static void Render(Control target, SKCanvas canvas, double dpi = 96, bool useDeferredRenderer = false)
        {
            var renderTarget = new CanvasRenderTarget(canvas, dpi);
            if (useDeferredRenderer)
            {
                using var renderer = new DeferredRenderer(target, renderTarget);
                renderer.Start();
                var renderLoopTask = renderer as IRenderLoopTask;
                renderLoopTask.Update(TimeSpan.Zero);
                renderLoopTask.Render();
            }
            else
            {
                ImmediateRenderer.Render(target, renderTarget);
            }
        }
    }

    public class CanvasRenderTarget : IRenderTarget
    {
        private readonly SKCanvas _canvas;
        private readonly double _dpi;

        public CanvasRenderTarget(SKCanvas canvas, double dpi)
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
        }
    }

    public static class PdfRenderer
    {
        public static void Render(Control target, Size size, Stream stream, double dpi = 72, bool useDeferredRenderer = false)
        {
            using var managedWStream = new SKManagedWStream(stream);
            using var document = SKDocument.CreatePdf(stream, (float)dpi);
            using var canvas = document.BeginPage((float)size.Width, (float)size.Height);
            target.Measure(size);
            target.Arrange(new Rect(size));
            CanvasRenderer.Render(target, canvas, dpi, useDeferredRenderer);
        }
    }

    public static class PngRenderer
    {
        public static void Render(Control target, Size size, string path, double dpi = 96)
        {
            var pixelSize = new PixelSize((int)size.Width, (int)size.Height);
            var dpiVector = new Vector(dpi, dpi);
            using var bitmap = new RenderTargetBitmap(pixelSize, dpiVector);
            target.Measure(size);
            target.Arrange(new Rect(size));
            bitmap.Render(target);
            bitmap.Save(path);
        }
    }
    
    public static class SkpRenderer
    {
        public static void Render(Control target, Size size, Stream stream, double dpi = 96, bool useDeferredRenderer = false)
        {
            var bounds = SKRect.Create(new SKSize((float)size.Width, (float)size.Height));
            using var pictureRecorder = new SKPictureRecorder();
            using var canvas = pictureRecorder.BeginRecording(bounds);
            target.Measure(size);
            target.Arrange(new Rect(size));
            CanvasRenderer.Render(target, canvas, dpi, useDeferredRenderer);
            using var picture = pictureRecorder.EndRecording();
            picture.Serialize(stream);
        }
    }

    public static class SvgRenderer
    {
        public static void Render(Control target, Size size, Stream stream, double dpi = 96, bool useDeferredRenderer = false)
        {
            using var managedWStream = new SKManagedWStream(stream);
            var bounds = SKRect.Create(new SKSize((float)size.Width, (float)size.Height));
            using var canvas = SKSvgCanvas.Create(bounds, managedWStream);
            target.Measure(size);
            target.Arrange(new Rect(size));
            CanvasRenderer.Render(target, canvas, dpi, useDeferredRenderer);
        }
    }

    public static class XpsRenderer
    {
        public static void Render(Control target, Size size, Stream stream, double dpi = 72, bool useDeferredRenderer = false)
        {
            using var managedWStream = new SKManagedWStream(stream);
            using var document = SKDocument.CreateXps(stream, (float)dpi);
            using var canvas = document.BeginPage((float)size.Width, (float)size.Height);
            target.Measure(size);
            target.Arrange(new Rect(size));
            CanvasRenderer.Render(target, canvas, dpi, useDeferredRenderer);
        }
    }

    public static class Capture
    {
        public static void AttachCapture(this TopLevel root)
        {
            AttachCapture(root, new KeyGesture(Key.F6));
        }

        public static void AttachCapture(this TopLevel root, KeyGesture gesture)
        {
            root.AddHandler(InputElement.KeyDownEvent, async (sender, args) =>
            {
                if (args.Key == Key.F6)
                {
                    await Save(root);
                }
            }, RoutingStrategies.Tunnel);
        }

        public static async Task Save(TopLevel root)
        {
            var dlg = new SaveFileDialog() { Title = "Save" };
            dlg.Filters.Add(new FileDialogFilter() { Name = "Png", Extensions = { "png" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Svg", Extensions = { "svg" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Pdf", Extensions = { "pdf" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Xps", Extensions = { "xps" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Skp", Extensions = { "skp" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
            dlg.InitialFileName = "screenshot";
            dlg.DefaultExtension = "png";
            var result = await dlg.ShowAsync(root as Window);
            if (result is { } path)
            {
                Save(root, root.Bounds.Size, path);
            }
        }

        public static void Save(Control? control, Size size, string path)
        {
            if (control is null)
            {
                return;
            }

            if (path.EndsWith("png", StringComparison.OrdinalIgnoreCase))
            {
                PngRenderer.Render(control, size, path);
            }

            if (path.EndsWith("skp", StringComparison.OrdinalIgnoreCase))
            {
                using var stream = File.Create(path);
                SkpRenderer.Render(control, size, stream, 96, false);
            }

            if (path.EndsWith("svg", StringComparison.OrdinalIgnoreCase))
            {
                using var stream = File.Create(path);
                SvgRenderer.Render(control, size, stream, 96, false);
            }

            if (path.EndsWith("pdf", StringComparison.OrdinalIgnoreCase))
            {
                using var stream = File.Create(path);
                PdfRenderer.Render(control, size, stream, 96, false);
            }
            
            if (path.EndsWith("xps", StringComparison.OrdinalIgnoreCase))
            {
                using var stream = File.Create(path);
                XpsRenderer.Render(control, size, stream, 96, false);
            }
        }
    }
}
