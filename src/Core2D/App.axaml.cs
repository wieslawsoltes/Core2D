#nullable enable
using System;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Autofac;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
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

    static App()
    {
        DefaultTheme = "FluentDark";

        ChangeTheme = new RelayCommand<string>(SetTheme);

        InitializeDesigner();
    }

    public static void InitializeDesigner()
    {
        if (Design.IsDesignMode)
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule<AppModule>();

            var container = builder.Build();

            DesignerContext.InitializeContext(container.Resolve<IServiceProvider>());
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
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
