using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Controls.Studio;
using SkiaSharp;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioSwatchTests
{
    [AvaloniaFact]
    public void SwatchPaintsItsActualColorAndUpdatesWhenEdited()
    {
        var field = new StudioColorField { Color = Color.Parse("#685DE8") };
        var window = new Window { Width = 300, Height = 60, Content = field };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var swatch = field.GetVisualDescendants().OfType<Border>().Single(x => x.Name == "PART_SwatchColor");
            Assert.True(swatch.Bounds.Width >= 16 && swatch.Bounds.Height >= 16);
            foreach (var expected in new[] { "#685DE8", "#27AE60" })
            {
                field.Color = Color.Parse(expected);
                Dispatcher.UIThread.RunJobs();
                Assert.Equal(field.Color, Assert.IsAssignableFrom<ISolidColorBrush>(swatch.Background).Color);
                var point = swatch.TranslatePoint(new Point(swatch.Bounds.Width / 2, swatch.Bounds.Height / 2), window)!.Value;
                using var frame = window.CaptureRenderedFrame();
                Assert.NotNull(frame);
                using var stream = new MemoryStream();
                frame!.Save(stream);
                stream.Position = 0;
                using var bitmap = SKBitmap.Decode(stream);
                var pixel = bitmap.GetPixel((int)point.X, (int)point.Y);
                Assert.Equal(field.Color.R, pixel.Red);
                Assert.Equal(field.Color.G, pixel.Green);
                Assert.Equal(field.Color.B, pixel.Blue);
            }
        }
        finally
        {
            window.Close();
        }
    }
}
