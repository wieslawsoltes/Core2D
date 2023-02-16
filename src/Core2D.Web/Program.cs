using System;
using System.Runtime.Versioning;
using Avalonia;
using Avalonia.Browser;

[assembly:SupportedOSPlatform("browser")]

namespace Core2D.Web;

internal partial class Program
{
    private static void Main(string[] args)
    {
        try
        {
            BuildAvaloniaApp().SetupBrowserApp("out");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}
