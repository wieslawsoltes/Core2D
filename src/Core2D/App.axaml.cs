#nullable disable
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
using Core2D.Configuration.Themes;
using Core2D.Configuration.Windows;
using Core2D.ViewModels.Designer;
using Core2D.Model;
using Core2D.ViewModels;
using Core2D.ViewModels.Editor;
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
            private readonly App _app;

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

                builder.RegisterModule<AppModule>();

                var container = builder.Build();

                DesignerContext.InitializeContext(container.Resolve<IServiceProvider>());
            }
        }

        public AboutInfoViewModel CreateAboutInfo(IServiceProvider serviceProvider, RuntimePlatformInfo runtimeInfo, string windowingSubsystem, string renderingSubsystem)
        {
            return new AboutInfoViewModel(serviceProvider)
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

            builder.RegisterModule<AppModule>();

            var container = builder.Build();

            var serviceProvider = container.Resolve<IServiceProvider>();

            var log = serviceProvider.GetService<ILog>();
            var fileSystem = serviceProvider.GetService<IFileSystem>();

            log?.Initialize(System.IO.Path.Combine(fileSystem?.GetBaseDirectory(), "Core2D.log"));

            var windowSettings = default(WindowConfiguration);
            var windowSettingsPath = System.IO.Path.Combine(fileSystem?.GetBaseDirectory(), "Core2D.window");
            if (fileSystem.Exists(windowSettingsPath))
            {
                var jsonWindowSettings = fileSystem?.ReadUtf8Text(windowSettingsPath);
                if (!string.IsNullOrEmpty(jsonWindowSettings))
                {
                    windowSettings = JsonSerializer.Deserialize<WindowConfiguration>(jsonWindowSettings);
                }
            }

            var editor = serviceProvider.GetService<ProjectEditorViewModel>();

            var recentPath = System.IO.Path.Combine(fileSystem.GetBaseDirectory(), "Core2D.recent");
            if (fileSystem.Exists(recentPath))
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
            var aboutInfo = CreateAboutInfo(serviceProvider, runtimeInfo, windowingSubsystemName, renderingSubsystemName);
            editor.AboutInfo = aboutInfo;

            var mainWindow = serviceProvider.GetService<MainWindow>();

            if (windowSettings is { })
            {
                WindowConfigurationFactory.Load(mainWindow, windowSettings);
            }

            mainWindow.DataContext = editor;

            mainWindow.Closing += (sender, e) =>
            {
                editor.OnSaveRecent(recentPath);

                windowSettings = WindowConfigurationFactory.Save(mainWindow);
                var jsonWindowSettings = JsonSerializer.Serialize(windowSettings, new JsonSerializerOptions() { WriteIndented = true });
                if (!string.IsNullOrEmpty(jsonWindowSettings))
                {
                    fileSystem?.WriteUtf8Text(windowSettingsPath, jsonWindowSettings);
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

            builder.RegisterModule<AppModule>();

            var container = builder.Build(); // TODO: Dispose()
            var serviceProvider = container.Resolve<IServiceProvider>();

            var log = serviceProvider.GetService<ILog>(); // TODO: Dispose()
            var fileSystem = serviceProvider.GetService<IFileSystem>();

            log?.Initialize(System.IO.Path.Combine(fileSystem?.GetBaseDirectory(), "Core2D.log"));

            var editor = serviceProvider.GetService<ProjectEditorViewModel>();

            editor.CurrentTool = editor.Tools.FirstOrDefault(t => t.Title == "Selection");
            editor.CurrentPathTool = editor.PathTools.FirstOrDefault(t => t.Title == "Line");
            editor.IsToolIdle = true;

            var mainView = new MainView()
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
                    Source = new Uri("avares://Core2D/Themes/DefaultDark.axaml")
                }
            };

            DefaultLight = new Styles
            {
                new StyleInclude(new Uri("avares://Core2D/Styles"))
                {
                    Source = new Uri("avares://Core2D/Themes/DefaultLight.axaml")
                }
            };

            FluentDark = new Styles
            {
                new StyleInclude(new Uri("avares://Core2D/Styles"))
                {
                    Source = new Uri("avares://Core2D/Themes/FluentDark.axaml")
                }
            };

            FluentLight = new Styles
            {
                new StyleInclude(new Uri("avares://Core2D/Styles"))
                {
                    Source = new Uri("avares://Core2D/Themes/FluentLight.axaml")
                }
            };

            ChangeTheme = new ChangeThemeCommand(this);

            InitTheme(DefaultTheme);
        }
    }
}
