using System;
using Avalonia;
namespace BrowserTheme.Catalog;
internal static class Program
{
    [STAThread]
    public static void Main(string[] args) => AppBuilder.Configure<CatalogApp>().UsePlatformDetect().LogToTrace().StartWithClassicDesktopLifetime(args);
}
