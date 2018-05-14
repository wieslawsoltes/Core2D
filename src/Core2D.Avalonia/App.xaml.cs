// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Autofac;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Core2D.Avalonia.Converters;
using Core2D.Avalonia.Dock;
using Core2D.Avalonia.Modules;
using Core2D.Avalonia.Views;
using Core2D.Editor;
using Core2D.Editor.Designer;
using Core2D.Interfaces;
using Dock.Model;

namespace Core2D.Avalonia
{
    /// <summary>
    /// Encapsulates an Avalonia application.
    /// </summary>
    public partial class App : Application
    {
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

                builder.RegisterModule<LocatorModule>();
                builder.RegisterModule<CoreModule>();
                builder.RegisterModule<DesignerModule>();
                builder.RegisterModule<AppModule>();
                builder.RegisterModule<ViewModule>();

                var container = builder.Build();

                DesignerContext.InitializeContext(container.Resolve<IServiceProvider>());
            }
        }

        /// <summary>
        /// Initializes converters.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public static void InitializeConverters(IServiceProvider serviceProvider)
        {
            ObjectToXamlStringConverter.XamlSerializer = serviceProvider.GetServiceLazily<IXamlSerializer>();
            ObjectToJsonStringConverter.JsonSerializer = serviceProvider.GetServiceLazily<IJsonSerializer>();
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
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
                Version = $"{this.GetType().GetTypeInfo().Assembly.GetName().Version}",
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

        private void UpdateWindows(IList<IViewsWindow> windows, IList<IView> views, ProjectEditor editor)
        {
            foreach (var window in windows)
            {
                var dock = new DockWindow()
                {
                    DataContext = editor
                };

                window.Window = dock;

                window.Context = editor;

                var layout = window.Layout;

                UpdateLayout(layout, views, editor);

                var control = dock.FindControl<ViewsControl>("control");
                if (control != null)
                {
                    control.DataContext = layout.Panels[0];
                }
            }
        }

        private void UpdateViews(IList<IView> source, IList<IView> views, ProjectEditor editor)
        {
            for (int i = 0; i < source.Count; i++)
            {
                var original = source[i];
                source[i] = views.FirstOrDefault(v => v.Title == original.Title);
                source[i].Windows = original.Windows;

                if (original.Windows != null)
                {
                    UpdateWindows(original.Windows, views, editor);
                }
            }
        }

        private void UpdatePanel(IViewsPanel panel, IList<IView> views, ProjectEditor editor)
        {
            UpdateViews(panel.Views, views, editor);

            panel.CurrentView = views.FirstOrDefault(v => v.Title == panel.CurrentView.Title);
        }

        private void UpdateLayout(IViewsLayout layout, IList<IView> views, ProjectEditor editor)
        {
            UpdateViews(layout.Views, views, editor);

            foreach (var panel in layout.Panels)
            {
                UpdatePanel(panel, views, editor);
            }

            layout.CurrentView = views.FirstOrDefault(v => v.Title == layout.CurrentView.Title);
        }

        private void CreateOrUpdateLayout(ProjectEditor editor)
        {
            var views = editor.Views;

            if (editor.Layout != null)
            {
                var layout = editor.Layout;

                UpdateLayout(layout, views, editor);

                layout.CurrentView = views.FirstOrDefault(v => v.Title == "Dashboard");
            }
            else
            {
                var layout = new ViewsLayout
                {
                    Views = new ObservableCollection<IView>(views),
                    Panels = new ObservableCollection<IViewsPanel>
                    {
                        new ViewsPanel
                        {
                            Row = 0,
                            Column = 0,
                            Views = new ObservableCollection<IView>
                            {
                                views.FirstOrDefault(v => v.Title == "Project"),
                                views.FirstOrDefault(v => v.Title == "Options"),
                                views.FirstOrDefault(v => v.Title == "Images")
                            },
                            CurrentView = views.FirstOrDefault(v => v.Title == "Project")
                        },
                        new ViewsPanel
                        {
                            Row = 2,
                            Column = 0,
                            Views = new ObservableCollection<IView>
                            {
                                views.FirstOrDefault(v => v.Title == "Groups"),
                                views.FirstOrDefault(v => v.Title == "Databases")
                            },
                            CurrentView = views.FirstOrDefault(v => v.Title == "Groups")
                        },
                        new ViewsPanel
                        {
                            Row = 0,
                            Column = 0,
                            Views = new ObservableCollection<IView>
                            {
                                views.FirstOrDefault(v => v.Title == "Styles"),
                                views.FirstOrDefault(v => v.Title == "Templates"),
                                views.FirstOrDefault(v => v.Title == "Container"),
                                views.FirstOrDefault(v => v.Title == "Zoom")
                            },
                            CurrentView = views.FirstOrDefault(v => v.Title == "Styles")
                        },
                        new ViewsPanel
                        {
                            Row = 2,
                            Column = 0,
                            Views = new ObservableCollection<IView>
                            {
                                views.FirstOrDefault(v => v.Title == "Tools"),
                                views.FirstOrDefault(v => v.Title == "Shape"),
                                views.FirstOrDefault(v => v.Title == "Data"),
                                views.FirstOrDefault(v => v.Title == "Style"),
                                views.FirstOrDefault(v => v.Title == "Template")
                            },
                            CurrentView = views.FirstOrDefault(v => v.Title == "Tools")
                        },
                    },
                    CurrentView = views.FirstOrDefault(v => v.Title == "Dashboard")
                };

                editor.Layout = layout;
            }
        }

        /// <summary>
        /// Initialize application context and displays main window.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="aboutInfo">The about information.</param>
        public void Start(IServiceProvider serviceProvider, AboutInfo aboutInfo)
        {
            InitializeConverters(serviceProvider);

            var log = serviceProvider.GetService<ILog>();
            var fileIO = serviceProvider.GetService<IFileSystem>();

            log?.Initialize(System.IO.Path.Combine(fileIO?.GetBaseDirectory(), "Core2D.log"));

            try
            {
                var editor = serviceProvider.GetService<ProjectEditor>();

                var layoutPath = System.IO.Path.Combine(fileIO.GetBaseDirectory(), "Core2D.layout");
                if (fileIO.Exists(layoutPath))
                {
                    editor.OnLoadLayout(layoutPath);
                }

                CreateOrUpdateLayout(editor);

                var recentPath = System.IO.Path.Combine(fileIO.GetBaseDirectory(), "Core2D.recent");
                if (fileIO.Exists(recentPath))
                {
                    editor.OnLoadRecent(recentPath);
                }

                editor.CurrentTool = editor.Tools.FirstOrDefault(t => t.Title == "Selection");
                editor.CurrentPathTool = editor.PathTools.FirstOrDefault(t => t.Title == "Line");
                editor.IsToolIdle = true;
                editor.AboutInfo = aboutInfo;

                var window = serviceProvider.GetService<Windows.MainWindow>();
                window.Closed += (sender, e) =>
                {
                    editor.Layout.CurrentView.CloseWindows();
                    editor.OnSaveLayout(layoutPath);

                    editor.OnSaveRecent(recentPath);
                };
                window.DataContext = editor;
                window.Show();
                Run(window);
            }
            catch (Exception ex)
            {
                log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    log?.LogError($"{ex.InnerException.Message}{Environment.NewLine}{ex.InnerException.StackTrace}");
                }
            }
        }

        /// <summary>
        /// Initialize application context and returns main view.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>The main view.</returns>
        public UserControl CreateView(IServiceProvider serviceProvider)
        {
            InitializeConverters(serviceProvider);

            var log = serviceProvider.GetService<ILog>();
            var fileIO = serviceProvider.GetService<IFileSystem>();

            log?.Initialize(System.IO.Path.Combine(fileIO?.GetBaseDirectory(), "Core2D.log"));

            var editor = serviceProvider.GetService<ProjectEditor>();

            CreateOrUpdateLayout(editor);

            editor.CurrentTool = editor.Tools.FirstOrDefault(t => t.Title == "Selection");
            editor.CurrentPathTool = editor.PathTools.FirstOrDefault(t => t.Title == "Line");
            editor.IsToolIdle = true;

            return new MainControl()
            {
                DataContext = editor
            };
        }
    }
}
