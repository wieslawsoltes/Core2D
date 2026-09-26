using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioDockIndicatorTests
{
    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void NativeThemeLessResizePreviewLookupUsesTheBrowserAccent(bool dark)
    {
        var window = new Window { Width = 180, Height = 100,
            RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            var expected = Assert.IsAssignableFrom<ISolidColorBrush>(window.FindResource(window.ActualThemeVariant, "BrowserAccentBrush"));
            // The installed SplitterPreviewAdorner.Render uses this theme-less overload.
            var preview = Assert.IsAssignableFrom<ISolidColorBrush>(window.FindResource("DockApplicationAccentBrushIndicator"));
            Assert.Equal(expected.Color, preview.Color);
            Assert.Same(preview, window.FindResource(window.ActualThemeVariant, "DockApplicationAccentBrushIndicator"));
            var custom = new SolidColorBrush(Colors.OrangeRed);
            window.Resources["DockApplicationAccentBrushIndicator"] = custom;
            Assert.Same(custom, window.FindResource("DockApplicationAccentBrushIndicator"));
            Assert.Same(custom, window.FindResource(window.ActualThemeVariant, "DockApplicationAccentBrushIndicator"));
        }
        finally { window.Close(); }
    }
}
