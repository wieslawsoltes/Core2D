using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using Autofac;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Platform;
using Avalonia.Styling;
using Core2D;
using Core2D.Editor;
using Core2D.Configuration.Layouts;
using Core2D.Configuration.Themes;
using Core2D.Configuration.Windows;
using Core2D.Designer;
using Core2D.Modules;
using Core2D.Views;

namespace Core2D
{
    public class App : Application
    {
        public Styles DefaultDark { get; set; }

        public Styles DefaultLight { get; set; }

        public Styles FluentDark { get; set; }

        public Styles FluentLight { get; set; }

        public static ThemeName DefaultTheme { get; set; }

        public static ICommand ChangeTheme { get; set; }

        private class ChangeThemeCommand : ICommand
        {
            private App _app;

#pragma warning disable CS0067
            public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067

            public ChangeThemeCommand(App app)
            {
                _app = app;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                if (parameter is string value)
                {
                    _app.SetTheme(value);
                }
            }
        }

        static App()
        {
            DefaultTheme = ThemeName.FluentDark;

            InitializeDesigner();
        }

        public static void InitializeDesigner()
        {
            if (Design.IsDesignMode)
            {
                var builder = new ContainerBuilder();

                builder.RegisterModule<AvaloniaModule>();

                var container = builder.Build();

                DesignerContext.InitializeContext(container.Resolve<IServiceProvider>());
            }
        }

        public AboutInfo CreateAboutInfo(RuntimePlatformInfo runtimeInfo, string windowingSubsystem, string renderingSubsystem)
        {
            return new AboutInfo()
            {
                Title = "Core2D",
                Version = $"{Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}",
                Description = "A multi-platform data driven 2D diagram editor.",
                Copyright = "Copyright (c) Wiesław Šoltés. All rights reserved.",
                License = "Licensed under the MIT license. See LICENSE file in the project root for full license information.",
                OperatingSystem = $"{runtimeInfo.OperatingSystem}",
                IsDesktop = runtimeInfo.IsDesktop,
                IsMobile = runtimeInfo.IsMobile,
                IsCoreClr = runtimeInfo.IsCoreClr,
                IsMono = runtimeInfo.IsMono,
                IsDotNetFramework = runtimeInfo.IsDotNetFramework,
                IsUnix = runtimeInfo.IsUnix,
                WindowingSubsystemName = windowingSubsystem,
                RenderingSubsystemName = renderingSubsystem
            };
        }

        private void InitializationClassicDesktopStyle(IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule<AvaloniaModule>();

            var container = builder.Build();

            var serviceProvider = container.Resolve<IServiceProvider>();

            var log = serviceProvider.GetService<ILog>();
            var fileIO = serviceProvider.GetService<IFileSystem>();

            log?.Initialize(System.IO.Path.Combine(fileIO?.GetBaseDirectory(), "Core2D.log"));

            var windowSettings = default(WindowConfiguration);
            var windowSettingsPath = System.IO.Path.Combine(fileIO?.GetBaseDirectory(), "Core2D.window");
            if (fileIO.Exists(windowSettingsPath))
            {
                var jsonWindowSettings = fileIO?.ReadUtf8Text(windowSettingsPath);
                if (!string.IsNullOrEmpty(jsonWindowSettings))
                {
                    windowSettings = JsonSerializer.Deserialize<WindowConfiguration>(jsonWindowSettings);
                }
            }

            var windowLayout = default(LayoutConfiguration);
            var windowLayoutPath = System.IO.Path.Combine(fileIO?.GetBaseDirectory(), "Core2D.layout");
            if (fileIO.Exists(windowLayoutPath))
            {
                var jsonWindowLayout = fileIO?.ReadUtf8Text(windowLayoutPath);
                if (!string.IsNullOrEmpty(jsonWindowLayout))
                {
                    windowLayout = JsonSerializer.Deserialize<LayoutConfiguration>(jsonWindowLayout);
                }
            }

            var editor = serviceProvider.GetService<ProjectEditor>();

            var recentPath = System.IO.Path.Combine(fileIO.GetBaseDirectory(), "Core2D.recent");
            if (fileIO.Exists(recentPath))
            {
                editor.OnLoadRecent(recentPath);
            }

            editor.CurrentTool = editor.Tools.FirstOrDefault(t => t.Title == "Selection");
            editor.CurrentPathTool = editor.PathTools.FirstOrDefault(t => t.Title == "Line");
            editor.IsToolIdle = true;

            var runtimeInfo = AvaloniaLocator.Current.GetService<IRuntimePlatform>().GetRuntimeInfo();
            var windowingPlatform = AvaloniaLocator.Current.GetService<IWindowingPlatform>();
            var platformRenderInterface = AvaloniaLocator.Current.GetService<IPlatformRenderInterface>();
            var windowingSubsystemName = windowingPlatform.GetType().Assembly.GetName().Name;
            var renderingSubsystemName = platformRenderInterface.GetType().Assembly.GetName().Name;
            var aboutInfo = CreateAboutInfo(runtimeInfo, windowingSubsystemName, renderingSubsystemName);
            editor.AboutInfo = aboutInfo;

            var mainWindow = serviceProvider.GetService<MainWindow>();
            var mainControl = mainWindow.FindControl<MainControl>("MainControl");

            if (windowSettings != null)
            {
                WindowConfigurationFactory.Load(mainWindow, windowSettings);
            }

            if (mainControl != null && windowLayout != null)
            {
                LayoutConfigurationFactory.Load(mainControl, windowLayout);
            }

            mainWindow.DataContext = editor;

            mainWindow.Closing += (sender, e) =>
            {
                editor.OnSaveRecent(recentPath);

                windowSettings = WindowConfigurationFactory.Save(mainWindow);
                var jsonWindowSettings = JsonSerializer.Serialize(windowSettings, new JsonSerializerOptions() { WriteIndented = true });
                if (!string.IsNullOrEmpty(jsonWindowSettings))
                {
                    fileIO?.WriteUtf8Text(windowSettingsPath, jsonWindowSettings);
                }

                windowLayout = LayoutConfigurationFactory.Save(mainControl);
                var jsonWindowLayout = JsonSerializer.Serialize(windowLayout, new JsonSerializerOptions() { WriteIndented = true });
                if (!string.IsNullOrEmpty(jsonWindowLayout))
                {
                    fileIO?.WriteUtf8Text(windowLayoutPath, jsonWindowLayout);
                }
            };

            desktopLifetime.MainWindow = mainWindow;

            desktopLifetime.Exit += (sennder, e) =>
            {
                log.Dispose();
                container.Dispose();
            };
        }

        private void InitializeSingleView(ISingleViewApplicationLifetime singleViewLifetime)
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule<AvaloniaModule>();

            var container = builder.Build(); // TODO: Dispose()
            var serviceProvider = container.Resolve<IServiceProvider>();

            var log = serviceProvider.GetService<ILog>(); // TODO: Dispose()
            var fileIO = serviceProvider.GetService<IFileSystem>();

            log?.Initialize(System.IO.Path.Combine(fileIO?.GetBaseDirectory(), "Core2D.log"));

            var editor = serviceProvider.GetService<ProjectEditor>();

            editor.CurrentTool = editor.Tools.FirstOrDefault(t => t.Title == "Selection");
            editor.CurrentPathTool = editor.PathTools.FirstOrDefault(t => t.Title == "Line");
            editor.IsToolIdle = true;

            var mainView = new MainControl()
            {
                DataContext = editor
            };

            singleViewLifetime.MainView = mainView;
        }

        public void InitTheme(ThemeName themeName)
        {
            switch (themeName)
            {
                case ThemeName.DefaultDark:
                    Styles.Insert(0, DefaultDark);
                    break;
                case ThemeName.DefaultLight:
                    Styles.Insert(0, DefaultLight);
                    break;
                case ThemeName.FluentDark:
                    Styles.Insert(0, FluentDark);
                    break;
                default:
                case ThemeName.FluentLight:
                    Styles.Insert(0, FluentLight);
                    break;
            }
        }

        public void SetTheme(ThemeName themeName)
        {
            switch (themeName)
            {
                case ThemeName.DefaultDark:
                    Styles[0] = DefaultDark;
                    break;
                case ThemeName.DefaultLight:
                    Styles[0] = DefaultLight;
                    break;
                case ThemeName.FluentDark:
                    Styles[0] = FluentDark;
                    break;
                default:
                case ThemeName.FluentLight:
                    Styles[0] = FluentLight;
                    break;
            }
        }

        public void SetTheme(string themeName)
        {
            if (Enum.TryParse<ThemeName>(themeName, out var result))
            {
                SetTheme(result);
            }
        }

        public override void OnFrameworkInitializationCompleted()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                InitializationClassicDesktopStyle(desktopLifetime);
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewLifetime)
            {
                InitializeSingleView(singleViewLifetime);
            }

            base.OnFrameworkInitializationCompleted();
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            DefaultDark = new Styles
            {
                new StyleInclude(new Uri("avares://Core2D/Styles"))
                {
                    Source = new Uri("avares://Core2D/Themes/DefaultDark.xaml")
                }
            };

            DefaultLight = new Styles
            {
                new StyleInclude(new Uri("avares://Core2D/Styles"))
                {
                    Source = new Uri("avares://Core2D/Themes/DefaultLight.xaml")
                }
            };

            FluentDark = new Styles
            {
                new StyleInclude(new Uri("avares://Core2D/Styles"))
                {
                    Source = new Uri("avares://Core2D/Themes/FluentDark.xaml")
                }
            };

            FluentLight = new Styles
            {
                new StyleInclude(new Uri("avares://Core2D/Styles"))
                {
                    Source = new Uri("avares://Core2D/Themes/FluentLight.xaml")
                }
            };

            ChangeTheme = new ChangeThemeCommand(this);

            InitTheme(DefaultTheme);
        }
    }
}
