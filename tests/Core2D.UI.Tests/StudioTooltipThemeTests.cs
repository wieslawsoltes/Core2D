using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioTooltipThemeTests
{
    [AvaloniaTheory]
    [InlineData(false)] [InlineData(true)]
    public void TooltipContrastAndWrappingOverrideLegacyBackground(bool dark)
    {
        var tooltip = new ToolTip { Content = "This contextual description is long enough to wrap without extending beyond the compact tooltip surface." };
        var window = new Window { Width = 400, Height = 200, Content = tooltip,
            RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            Assert.Equal(Color.Parse("#222222"), Assert.IsAssignableFrom<ISolidColorBrush>(tooltip.Background).Color);
            Assert.Equal(Colors.White, Assert.IsAssignableFrom<ISolidColorBrush>(tooltip.Foreground).Color);
            var text = tooltip.GetVisualDescendants().OfType<TextBlock>().Single();
            Assert.Equal(Colors.White, Assert.IsAssignableFrom<ISolidColorBrush>(text.Foreground).Color);
            Assert.Equal(TextWrapping.Wrap, text.TextWrapping);
            Assert.True(tooltip.Bounds.Width <= 320);
            window.RequestedThemeVariant = dark ? ThemeVariant.Light : ThemeVariant.Dark;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(Color.Parse("#222222"), Assert.IsAssignableFrom<ISolidColorBrush>(tooltip.Background).Color);
            Assert.Equal(Colors.White, Assert.IsAssignableFrom<ISolidColorBrush>(text.Foreground).Color);
        }
        finally { window.Close(); }
    }
}
