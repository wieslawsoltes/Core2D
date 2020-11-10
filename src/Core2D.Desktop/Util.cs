using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;

namespace Core2D
{
    public static class Util
    {
        public static void Screenshot(Control target, Size size, string path, double dpi = 96)
        {
            var pixelSize = new PixelSize((int)size.Width, (int)size.Height);
            var dpiVector = new Vector(dpi, dpi);
            using var bitmap = new RenderTargetBitmap(pixelSize, dpiVector);
            target.Measure(size);
            target.Arrange(new Rect(size));
            bitmap.Render(target);
            bitmap.Save(path);
        }

        public static async Task RunUIJob(Action action)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                action.Invoke();
                Dispatcher.UIThread.RunJobs();
            });
        }
    }
}
