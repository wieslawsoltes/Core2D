using System;
using System.Linq;
using System.Reflection;
using System.Text;
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
using Core2D.UI.Designer;
using Core2D.UI.Modules;
using Core2D.UI.Views;

namespace Core2D.UI
{
    public enum ThemeName
    {
        DefaultDark,
        DefaultLight,
        FluentDark,
        FluentLight
    }

    /// <summary>
    /// Window settings.
    /// </summary>
    public class WindowSettings
    {
        /// <summary>
        /// Gets or sets X-axis window position.
        /// </summary>
        public double X { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets Y-axis window position.
        /// </summary>
        public double Y { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets window width.
        /// </summary>
        public double Width { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets window width.
        /// </summary>
        public double Height { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets window state.
        /// </summary>
        public WindowState WindowState { get; set; } = WindowState.Normal;

        /// <summary>
        /// Gets window settings.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns>The window settings.</returns>
        public static WindowSettings GetWindowSettings(Window window)
        {
            return new WindowSettings()
           {
                Width = window.Width,
                Height = window.Height,
                X = window.Position.X,
                Y = window.Position.Y,
                WindowState = window.WindowState
            };
        }

        /// <summary>
        /// Sets window settings.
        /// </summary>
        /// <param name="window">The window.</param>
        public static void SetWindowSettings(Window window, WindowSettings settings)
        {
            if (!double.IsNaN(settings.Width))
            {
                window.Width = settings.Width;
            }

            if (!double.IsNaN(settings.Height))
            {
                window.Height = settings.Height;
            }

            if (!double.IsNaN(settings.X) && !double.IsNaN(settings.Y))
            {
                window.Position = new PixelPoint((int)settings.X, (int)settings.Y);
                window.WindowStartupLocation = WindowStartupLocation.Manual;
            }
            else
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            window.WindowState = settings.WindowState;
        }
    }

    /// <summary>
    /// Encapsulates an Avalonia application.
    /// </summary>
    public class App : Application
    {
        // <summary>
        /// Default dark theme.
        /// </summary>
        public Styles DefaultDark = new Styles
        {
            new StyleInclude(new Uri("avares://Core2D.UI/Styles"))
            {
                Source = new Uri("avares://Core2D.UI/Themes/DefaultDark.xaml")
            }
        };

        // <summary>
        /// Default light theme.
        /// </summary>
        public Styles DefaultLight = new Styles
        {
            new StyleInclude(new Uri("avares://Core2D.UI/Styles"))
            {
                Source = new Uri("avares://Core2D.UI/Themes/DefaultLight.xaml")
            }
        };

        // <summary>
        /// Default fluent dark theme.
        /// </summary>
        public Styles FluentDark = new Styles
        {
            new StyleInclude(new Uri("avares://Core2D.UI/Styles"))
            {
                Source = new Uri("avares://Core2D.UI/Themes/FluentDark.xaml")
            }
        };

        // <summary>
        /// Default fluent light theme.
        /// </summary>
        public Styles FluentLight = new Styles
        {
            new StyleInclude(new Uri("avares://Core2D.UI/Styles"))
            {
                Source = new Uri("avares://Core2D.UI/Themes/FluentLight.xaml")
            }
        };

        /// <summary>
        /// Gets or sets default theme.
        /// </summary>
        public static ThemeName DefaultTheme { get; set; } = ThemeName.FluentLight;

        /// <summary>
        /// Initializes static data.
        /// </summary>
        static App()
        {
            InitializeDesigner();
        }

        /// <summary>
        /// Initializes designer.
        /// </summary>
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

        /// <summary>
        /// Initialize application about information.
        /// </summary>
        /// <param name="runtimeInfo">The runtime info.</param>
        /// <param name="windowingSubsystem">The windowing subsystem.</param>
        /// <param name="renderingSubsystem">The rendering subsystem.</param>
        /// <returns>The about information.</returns>
        public AboutInfo CreateAboutInfo(RuntimePlatformInfo runtimeInfo, string windowingSubsystem, string renderingSubsystem)
        {
            return new AboutInfo()
            {
                Title = "Core2D",
                Version = $"{Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute> ().InformationalVersion}",
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
            var jsonSerializer = serviceProvider.GetService<IJsonSerializer>();

            log?.Initialize(System.IO.Path.Combine(fileIO?.GetBaseDirectory(), "Core2D.log"));

            var windowSettings = default(WindowSettings);
            var windowSettingsPath = System.IO.Path.Combine(fileIO?.GetBaseDirectory(), "Core2D.window");
            if (fileIO.Exists(windowSettingsPath))
            {
                var jsonWindowSettings = fileIO?.ReadUtf8Text(windowSettingsPath);
                if (!string.IsNullOrEmpty(jsonWindowSettings))
                {
                    windowSettings = jsonSerializer.Deserialize<WindowSettings>(jsonWindowSettings);
                }
            }

            var editor = serviceProvider.GetService<IProjectEditor>();

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

            //editor.OnNew(null);

            var mainWindow = serviceProvider.GetService<MainWindow>();

            if (windowSettings != null)
            {
                WindowSettings.SetWindowSettings(mainWindow, windowSettings);
            }

            mainWindow.DataContext = editor;

            mainWindow.Closing += (sender, e) =>
            {
                editor.OnSaveRecent(recentPath);

                windowSettings = WindowSettings.GetWindowSettings(mainWindow);
                var jsonWindowSettings = jsonSerializer?.Serialize(windowSettings);
                if (!string.IsNullOrEmpty(jsonWindowSettings))
                {
                    fileIO?.WriteUtf8Text(windowSettingsPath, jsonWindowSettings);
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

            var editor = serviceProvider.GetService<IProjectEditor>();

            editor.CurrentTool = editor.Tools.FirstOrDefault(t => t.Title == "Selection");
            editor.CurrentPathTool = editor.PathTools.FirstOrDefault(t => t.Title == "Line");
            editor.IsToolIdle = true;

            var mainView = new MainControl()
            {
                DataContext = editor
            };

            singleViewLifetime.MainView = mainView;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            switch (DefaultTheme)
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
    }
}
