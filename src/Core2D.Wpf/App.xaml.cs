// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using System.Windows;
using Autofac;
using Core2D.Editor;
using Core2D.Interfaces;
using Core2D.Wpf.Modules;

namespace Core2D.Wpf
{
    /// <summary>
    /// Encapsulates an WPF application.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Raises the <see cref="Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var builder = new ContainerBuilder();

            builder.RegisterModule<LocatorModule>();
            builder.RegisterModule<CoreModule>();
            builder.RegisterModule<DependenciesModule>();
            builder.RegisterModule<AppModule>();

            using (IContainer container = builder.Build())
            {
                using (var log = container.Resolve<ILog>())
                {
                    Start(container.Resolve<IServiceProvider>());
                }
            }
        }

        /// <summary>
        /// Initialize application context and displays main window.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        private void Start(IServiceProvider serviceProvider)
        {
            var log = serviceProvider.GetService<ILog>();
            var fileIO = serviceProvider.GetService<IFileSystem>();

            log?.Initialize(System.IO.Path.Combine(fileIO.GetAssemblyPath(null), "Core2D.log"));

            try
            {
                var editor = serviceProvider.GetService<ProjectEditor>();

                var path = System.IO.Path.Combine(fileIO.GetAssemblyPath(null), "Core2D.recent");
                if (System.IO.File.Exists(path))
                {
                    editor.OnLoadRecent(path);
                }

                editor.CurrentView = editor.Views.FirstOrDefault(v => v.Name == "Dashboard");
                editor.CurrentTool = editor.Tools.FirstOrDefault(t => t.Name == "Selection");
                editor.CurrentPathTool = editor.PathTools.FirstOrDefault(t => t.Name == "Line");

                var window = serviceProvider.GetService<Windows.MainWindow>();
                bool isLoaded = false;

                window.Loaded +=
                    (sender, e) =>
                    {
                        if (isLoaded)
                            return;
                        else
                            isLoaded = true;
                    };

                window.Closed +=
                    (sender, e) =>
                    {
                        if (!isLoaded)
                            return;
                        else
                            isLoaded = false;

                        editor.OnSaveRecent(path);
                    };

                window.DataContext = editor;
                window.ShowDialog();
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
    }
}
