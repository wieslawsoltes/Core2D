using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Controls.Studio;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioGlyphTests
{
    [AvaloniaTheory]
    [InlineData(false, "#242424")]
    [InlineData(true, "#F5F5F5")]
    public void SharedImagesUseTheLocalThemeWithoutMutatingTheirDrawing(bool dark, string expectedColor)
    {
        var drawing = new GeometryDrawing { Brush = Brushes.Black, Geometry = Geometry.Parse("M0,0 L16,0 L16,16 L0,16 Z") };
        var image = new DrawingImage { Drawing = drawing };
        var button = new StudioToolButton { Icon = image };
        var window = new Window
        {
            Width = 100,
            Height = 100,
            Content = button,
            RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light
        };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var glyph = button.GetVisualDescendants().OfType<StudioGlyph>().Single();
            Assert.Same(image, glyph.Source);
            Assert.Equal(Color.Parse(expectedColor), Assert.IsAssignableFrom<ISolidColorBrush>(glyph.Foreground).Color);
            Assert.Equal(Colors.Black, Assert.IsAssignableFrom<ISolidColorBrush>(drawing.Brush).Color);
            using var frame = window.CaptureRenderedFrame();
            Assert.NotNull(frame);
        }
        finally
        {
            window.Close();
        }
    }
}
