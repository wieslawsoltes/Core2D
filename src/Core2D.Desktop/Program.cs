using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Dialogs;
using Avalonia.Headless;
using Avalonia.Threading;
using Avalonia.Win32;
using Core2D.ViewModels.Editor;
using Core2D.Views;

namespace Core2D.Desktop;

internal static class Program
{
    private static void Log(Exception ex)
    {
        Console.WriteLine(ex.Message);
        Console.WriteLine(ex.StackTrace);
        if (ex.InnerException is { })
        {
            Log(ex.InnerException);
        }
    }

    private static async Task ProcessSettings(Settings settings)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var applicationLifetime = (IClassicDesktopStyleApplicationLifetime?)Application.Current?.ApplicationLifetime;
            var mainWindow = applicationLifetime?.MainWindow;
            var mainView = mainWindow?.Content as MainView;
            var editor = mainView?.DataContext as ProjectEditorViewModel;

            if (mainView is { })
            {
                if (settings.Scripts is { })
                {
                    foreach (var script in settings.Scripts)
                    {
                        var stream = File.OpenRead(script.FullName);
                        editor?.OnExecuteScript(stream);
                        Dispatcher.UIThread.RunJobs();
                    }
                }

                if (settings.Project is { })
                {
                    var name = Path.GetFileNameWithoutExtension(settings.Project.FullName); 
                    using var stream = File.OpenRead(settings.Project.FullName);
                    editor?.OnOpenProject(stream, name);
                    Dispatcher.UIThread.RunJobs();
                }
            }
        });
    }

    private static void StartAvaloniaApp(Settings settings, string[] args)
    {
        var builder = BuildAvaloniaApp();

        try
        {
            if (settings.Theme is { })
            {
                App.DefaultTheme = settings.Theme;
            }

#if ENABLE_DIRECT2D1
            if (settings.UseDirect2D1)
            {
                builder.UseDirect2D1();
            }
#endif

            if (settings.UseSkia)
            {
                builder.UseSkia();
            }

            builder.With(new X11PlatformOptions
            {
                UseGpu = settings.UseGpu,
            });

            builder.With(new Win32PlatformOptions
            {
                AllowEglInitialization = settings.AllowEglInitialization,
                UseWgl = settings.UseWgl,
#if ENABLE_DIRECT2D1
                UseWindowsUIComposition = !settings.UseDirect2D1 && settings.UseWindowsUIComposition
#else
                UseWindowsUIComposition = settings.UseWindowsUIComposition
#endif
            });

            if (settings.UseDirectX11)
            {
                builder.With(new AngleOptions()
                {
                    AllowedPlatformApis = new List<AngleOptions.PlatformApi>
                    {
                        AngleOptions.PlatformApi.DirectX11
                    }
                });
            }

            if (settings.UseManagedSystemDialogs)
            {
                builder.UseManagedSystemDialogs();
            }

            if (settings.UseHeadless)
            {
                builder.UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = settings.UseHeadlessDrawing});
            }

            if (settings.UseHeadlessVnc)
            {
                builder.AfterSetup(async _ => await ProcessSettings(settings))
                    .StartWithHeadlessVncPlatform(settings.VncHost, settings.VncPort, args, ShutdownMode.OnMainWindowClose);
                return;
            }

            builder.AfterSetup(async _ => await ProcessSettings(settings))
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Log(ex);
        }
    }

    private static RootCommand CreateRootCommand()
    {
        var rootCommand = new RootCommand()
        {
            Description = "A multi-platform data driven 2D diagram editor."
        };

        rootCommand.AddOption(new Option(new[] { "--theme", "-t" }, "Set application theme", typeof(string)));
        rootCommand.AddOption(new Option(new[] { "--scripts", "-s" }, "The relative or absolute path to the script files", typeof(FileInfo[])));
        rootCommand.AddOption(new Option(new[] { "--project", "-p" }, "The relative or absolute path to the project file", typeof(FileInfo)));
        rootCommand.AddOption(new Option(new[] { "--repl" }, "Run scripting repl", typeof(bool)));
        rootCommand.AddOption(new Option(new[] { "--useManagedSystemDialogs" }, "Use managed system dialogs", typeof(bool)));
#if ENABLE_DIRECT2D1
        rootCommand.AddOption(new Option(new[] { "--useDirect2D1" }, "Use Direct2D1 renderer", typeof(bool)));
#endif
        rootCommand.AddOption(new Option(new[] { "--useSkia" }, "Use Skia renderer", typeof(bool)));
        rootCommand.AddOption(new Option(new[] { "--enableMultiTouch" }, "Enable multi-touch", typeof(bool), () => true));
        rootCommand.AddOption(new Option(new[] { "--useGpu" }, "Use Gpu", typeof(bool), () => true));
        rootCommand.AddOption(new Option(new[] { "--allowEglInitialization" }, "Allow EGL initialization", typeof(bool), () => true));
        rootCommand.AddOption(new Option(new[] { "--useWgl" }, "Use Windows GL", typeof(bool)));
        rootCommand.AddOption(new Option(new[] { "--useDeferredRendering" }, "Use deferred rendering", typeof(bool), () => true));
        rootCommand.AddOption(new Option(new[] { "--useWindowsUIComposition" }, "Use Windows UI composition", typeof(bool),  () => true));
        rootCommand.AddOption(new Option(new[] { "--useDirectX11" }, "Use DirectX11 platform api", typeof(bool)));
        rootCommand.AddOption(new Option(new[] { "--useHeadless" }, "Use headless", typeof(bool)));
        rootCommand.AddOption(new Option(new[] { "--useHeadlessDrawing" }, "Use headless drawing", typeof(bool)));
        rootCommand.AddOption(new Option(new[] { "--useHeadlessVnc" }, "Use headless vnc", typeof(bool)));
        rootCommand.AddOption(new Option(new[] { "--createHeadlessScreenshots" }, "Create headless screenshots", typeof(bool)));
        rootCommand.AddOption(new Option(new[] { "--screenshotExtension" }, "Screenshots file extension", typeof(string),  () => "png"));
        rootCommand.AddOption(new Option(new[] { "--screenshotWidth" }, "Screenshots width", typeof(double), () => 1366));
        rootCommand.AddOption(new Option(new[] { "--screenshotHeight" }, "Screenshots height", typeof(double), () => 690));
        rootCommand.AddOption(new Option(new[] { "--vncHost" }, "Vnc host", typeof(string)));
        rootCommand.AddOption(new Option(new[] { "--vncPort" }, "Vnc port", typeof (int), () => 590));
        rootCommand.AddOption(new Option(new[] { "--httpServer" }, "Run as http server"));

        return rootCommand;
    }

    [STAThread]
    internal static void Main(string[] args)
    {
        var rootCommand = CreateRootCommand();
        var rootSettings = default(Settings?);

        rootCommand.Handler = CommandHandler.Create((Settings settings) =>
        {
            rootSettings = settings;
        });

        rootCommand.Invoke(args);

        if (rootSettings is { })
        {
            StartAvaloniaApp(rootSettings, args);
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}
