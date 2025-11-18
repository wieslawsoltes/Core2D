using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;

namespace Core2D;

internal static class Program
{
    [SupportedOSPlatform("browser")]
    private static Task Main(string[] args)
        => AppBuilderFactory.BuildAvaloniaApp()
            .WithInterFont()
            .StartBrowserAppAsync("out");
}
