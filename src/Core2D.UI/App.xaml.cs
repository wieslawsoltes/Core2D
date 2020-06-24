using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.ThemeManager;
using Core2D;
using Core2D.Editor;
using Core2D.UI.Designer;
using Core2D.UI.Modules;
using Core2D.UI.Views;
using DM = Dock.Model;
using DMC = Dock.Model.Controls;

namespace Core2D.UI
{
    /// <summary>
    /// Encapsulates an Avalonia application.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Gets the theme selector instance.
        /// </summary>
        public static IThemeSelector Selector { get; set; }

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

            Selector = ThemeSelector.Create("Themes");
            Selector.LoadSelectedTheme("Core2D.theme");

            var log = serviceProvider.GetService<ILog>();
            var fileIO = serviceProvider.GetService<IFileSystem>();

            log?.Initialize(System.IO.Path.Combine(fileIO?.GetBaseDirectory(), "Core2D.log"));

            var editor = serviceProvider.GetService<IProjectEditor>();

            var layoutPath = System.IO.Path.Combine(fileIO.GetBaseDirectory(), "Core2D.layout");
            if (fileIO.Exists(layoutPath) && editor.LayoutPlatform != null)
            {
                var layout = editor.LayoutPlatform.DeserializeLayout(layoutPath);
                if (layout != null)
                {
                    editor.LayoutPlatform.Layout = layout;
                }
            }

            if (editor.LayoutPlatform != null)
            {
                var dockFactory = serviceProvider.GetService<DM.IFactory>();

                if (editor.LayoutPlatform.Layout == null)
                {
                    editor.LayoutPlatform.Layout = dockFactory.CreateLayout();
                }

                if (editor.LayoutPlatform.Layout is DMC.IRootDock layout)
                {
                    dockFactory.InitLayout(layout);  
                }
            }

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
                if (editor.LayoutPlatform?.Layout is DMC.IRootDock layout)
                {
                    layout.Close();
                }
                editor.LayoutPlatform?.SerializeLayout(layoutPath, editor.LayoutPlatform?.Layout);
                editor.OnSaveRecent(recentPath);
                Selector.SaveSelectedTheme("Core2D.theme");
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

            if (editor.LayoutPlatform != null)
            {
                var dockFactory = serviceProvider.GetService<DM.IFactory>();

                if (editor.LayoutPlatform.Layout == null)
                {
                    editor.LayoutPlatform.Layout = dockFactory.CreateLayout();
                }

                if (editor.LayoutPlatform.Layout is DMC.IRootDock layout)
                {
                    dockFactory.InitLayout(layout);
                }
            }

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
        }
    }
}
