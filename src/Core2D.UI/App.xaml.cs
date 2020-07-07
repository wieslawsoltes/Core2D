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
    /// Encapsulates an Avalonia application.
    /// </summary>
    public class App : Application
    {
        public Styles DefaultDark = new Styles
        {
            new StyleInclude(new Uri("avares://Core2D.UI/Styles"))
            {
                Source = new Uri("avares://Core2D.UI/Themes/DefaultDark.xaml")
            }
        };

        public Styles DefaultLight = new Styles
        {
            new StyleInclude(new Uri("avares://Core2D.UI/Styles"))
            {
                Source = new Uri("avares://Core2D.UI/Themes/DefaultLight.xaml")
            }
        };

        public Styles FluentDark = new Styles
        {
            new StyleInclude(new Uri("avares://Core2D.UI/Styles"))
            {
                Source = new Uri("avares://Core2D.UI/Themes/FluentDark.xaml")
            }
        };

        public Styles FluentLight = new Styles
        {
            new StyleInclude(new Uri("avares://Core2D.UI/Styles"))
            {
                Source = new Uri("avares://Core2D.UI/Themes/FluentLight.xaml")
            }
        };

        public static ThemeName DefaultTheme = ThemeName.FluentLight;

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
                Version = $"{GetType().GetTypeInfo().Assembly.GetName().Version}",
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

            mainWindow.DataContext = editor;

            mainWindow.Closing += (sender, e) =>
            {
                editor.OnSaveRecent(recentPath);
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
