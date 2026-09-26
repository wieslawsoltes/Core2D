using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using SkiaSharp;
using Xunit;

namespace Core2D.UI.Tests;

public class RulerPresentationTests
{
    [AvaloniaFact]
    public void OpaqueBackgroundDoesNotPaintOverTicksAndLabels()
    {
        var ruler = new Controls.Ruler
        {
            Width = 300,
            Height = 32,
            Background = Brushes.Black,
            BorderThickness = new Thickness(0),
            TickBrush = Brushes.White,
            TextBrush = Brushes.White,
            AccentBrush = Brushes.White,
            Zoom = 1,
            HighlightLength = 0
        };
        var window = new Window { Width = 300, Height = 32, Background = Brushes.Black, Content = ruler };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            using var frame = window.CaptureRenderedFrame();
            Assert.NotNull(frame);
            using var stream = new MemoryStream();
            frame!.Save(stream);
            stream.Position = 0;
            using var bitmap = SKBitmap.Decode(stream);
            Assert.NotNull(bitmap);
            var visibleMarkings = 0;
            for (var y = 2; y < 28; y++)
            {
                for (var x = 20; x < 280; x++)
                {
                    var pixel = bitmap.GetPixel(x, y);
                    if (pixel.Red > 128 && pixel.Green > 128 && pixel.Blue > 128)
                    {
                        visibleMarkings++;
                    }
                }
            }
            Assert.True(visibleMarkings > 10, "Opaque ruler backgrounds must not cover the rendered ticks and labels.");
        }
        finally
        {
            window.Close();
        }
    }
}
