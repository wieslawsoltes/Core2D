using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Avalonia.Themes.Browser;
using Xunit;

namespace Core2D.UI.Tests;

public class BrowserThemeIntegrationTests
{
    [AvaloniaFact]
    public void ApplicationInstallsBrowserThemeInsteadOfFluent()
    {
        var app = Application.Current!;
        var theme = Assert.Single(app.Styles.OfType<BrowserTheme>());
        Assert.Equal(BrowserDensity.Compact, theme.Density);
        Assert.Equal("Avalonia.Themes.Browser", theme.GetType().Assembly.GetName().Name);
        Assert.DoesNotContain(app.Styles, style => style.GetType().FullName == "Avalonia.Themes.Fluent.FluentTheme");
        foreach (var key in new[] { typeof(Button), typeof(TextBox), typeof(CheckBox), typeof(ComboBox), typeof(TabControl), typeof(ScrollBar) })
        {
            Assert.True(((IResourceNode)theme).TryGetResource(key, ThemeVariant.Light, out var resource));
            Assert.IsType<ControlTheme>(resource);
        }
    }
}
