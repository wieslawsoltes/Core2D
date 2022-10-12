using System.Runtime.Versioning;
using Avalonia;
using Avalonia.Web;
using Core2D.Web.Base;

[assembly:SupportedOSPlatform("browser")]

namespace Core2D.Web;

internal partial class Program
{
    private static void Main(string[] args)
        => BuildAvaloniaApp().SetupBrowserApp("out");

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}
