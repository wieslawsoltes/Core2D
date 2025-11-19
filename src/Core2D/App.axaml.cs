// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using Core2D.Configuration.Windows;
using Core2D.ViewModels;
using Core2D.ViewModels.Designer;
using Core2D.ViewModels.Editor;
using Core2D.Views;

namespace Core2D;

public class App : Application
{
    public static string DefaultTheme { get; set; }

    public static ICommand? ChangeTheme { get; }

    private static bool _globalExceptionHandlersRegistered;

    static App()
    {
        DefaultTheme = "FluentDark";

        ChangeTheme = new RelayCommand<string>(SetTheme);

        InitializeDesigner();
        RegisterGlobalExceptionHandlers();
    }

    public static void InitializeDesigner()
    {
        if (Design.IsDesignMode)
        {
            using var serviceProvider = AppModule.CreateServiceProvider();
            DesignerContext.InitializeContext(serviceProvider);
        }
    }

    public static void InitializationClassicDesktopStyle(IClassicDesktopStyleApplicationLifetime desktopLifetime, out ProjectEditorViewModel? editor)
    {
        var appState = new AppState();

        var mainWindow = appState.ServiceProvider.GetService<Window>();
        if (mainWindow is null)
        {
            editor = null;
            return;
        }

        if (appState.WindowConfiguration is { })
        {
            WindowConfigurationFactory.Load(mainWindow, appState.WindowConfiguration);
        }

        mainWindow.DataContext = appState.Editor;

        mainWindow.Closing += (_, _) =>
        {
            appState.WindowConfiguration = WindowConfigurationFactory.Save(mainWindow);
            appState.Save();
        };

        desktopLifetime.MainWindow = mainWindow;
        desktopLifetime.Exit += (_, _) => appState.Dispose();

        editor = appState.Editor;
    }

    public static void InitializeSingleView(ISingleViewApplicationLifetime singleViewLifetime, out ProjectEditorViewModel? editor)
    {
        var appState = new AppState();

        var mainView = new MainView
        {
            DataContext = appState.Editor
        };

        singleViewLifetime.MainView = mainView;
        singleViewLifetime.MainView.Unloaded += (_, _) => appState.Dispose();

        editor = appState.Editor;
    }

    public static void SetTheme(string? themeName)
    {
        if (Current is null || themeName is null)
        {
            return;
        }

        switch (themeName)
        {
            case "FluentLight":
            {
                Current.RequestedThemeVariant = ThemeVariant.Light;
                break;
            }
            case "FluentDark":
            {
                Current.RequestedThemeVariant = ThemeVariant.Dark;
                break;
            }
        }
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            InitializationClassicDesktopStyle(desktopLifetime, out var editor);
            
            DataContext = editor;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewLifetime)
        {
            InitializeSingleView(singleViewLifetime, out var editor);

            DataContext = editor;
        }

        base.OnFrameworkInitializationCompleted();
#if DEBUG
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime)
        {
            this.AttachDevTools();
        }
#endif
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private static void RegisterGlobalExceptionHandlers()
    {
        if (_globalExceptionHandlersRegistered)
        {
            return;
        }

        _globalExceptionHandlersRegistered = true;

        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            if (args.ExceptionObject is Exception ex)
            {
                LogUnhandledException("AppDomain.CurrentDomain.UnhandledException", ex);
            }
        };

        System.Threading.Tasks.TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            LogUnhandledException("TaskScheduler.UnobservedTaskException", args.Exception);
            args.SetObserved();
        };

        Dispatcher.UIThread.UnhandledException += (_, args) =>
        {
            LogUnhandledException("Dispatcher.UIThread.UnhandledException", args.Exception);
            args.Handled = true;
        };
    }

    private static void LogUnhandledException(string source, Exception exception)
    {
        try
        {
            Console.Error.WriteLine($"[Unhandled:{source}] {exception}");
        }
        catch
        {
            // Swallow logging failures to avoid recursive crashes.
        }
    }
}
