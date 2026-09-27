using Avalonia;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using global::BrowserTheme.Catalog;
using Xunit;
[assembly: AvaloniaTestApplication(typeof(Avalonia.Themes.Browser.Tests.TestAppBuilder))]
[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace Avalonia.Themes.Browser.Tests;
public static class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<CatalogApp>().UseSkia()
        .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false });
}
