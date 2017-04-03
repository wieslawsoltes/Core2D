// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using Autofac;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Core2D.Avalonia.Converters;
using Core2D.Avalonia.Modules;
using Core2D.Avalonia.Views;
using Core2D.Editor;
using Core2D.Editor.Designer;
using Core2D.Interfaces;

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
        /// Initialize application context and displays main window.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public void Start(IServiceProvider serviceProvider)
        {
            InitializeConverters(serviceProvider);

            var log = serviceProvider.GetService<ILog>();
            var fileIO = serviceProvider.GetService<IFileSystem>();

            log?.Initialize(System.IO.Path.Combine(fileIO?.GetAssemblyPath(null), "Core2D.log"));

            try
            {
                var editor = serviceProvider.GetService<ProjectEditor>();

                var path = System.IO.Path.Combine(fileIO.GetAssemblyPath(null), "Core2D.recent");
                if (fileIO.Exists(path))
                {
                    editor.OnLoadRecent(path);
                }

                editor.CurrentView = editor.Views.FirstOrDefault(v => v.Name == "Dashboard");
                editor.CurrentTool = editor.Tools.FirstOrDefault(t => t.Name == "Selection");
                editor.CurrentPathTool = editor.PathTools.FirstOrDefault(t => t.Name == "Line");

                var window = serviceProvider.GetService<Windows.MainWindow>();

                window.Closed +=
                    (sender, e) =>
                    {
                        editor.OnSaveRecent(path);
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

            log?.Initialize(System.IO.Path.Combine(fileIO?.GetAssemblyPath(null), "Core2D.log"));

            var editor = serviceProvider.GetService<ProjectEditor>();

            editor.CurrentView = editor.Views.FirstOrDefault(v => v.Name == "Dashboard");
            editor.CurrentTool = editor.Tools.FirstOrDefault(t => t.Name == "Selection");
            editor.CurrentPathTool = editor.PathTools.FirstOrDefault(t => t.Name == "Line");

            var view = new MainControl();
            view.DataContext = editor;

            return view;
        }
    }
}
