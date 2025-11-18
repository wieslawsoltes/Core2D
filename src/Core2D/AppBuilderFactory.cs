using Avalonia;

namespace Core2D;

public static class AppBuilderFactory
{
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}
