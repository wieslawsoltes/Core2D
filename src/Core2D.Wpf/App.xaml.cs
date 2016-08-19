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
        private IServiceProvider _serviceProvider;

        private static string RecentFileName = "Core2D.recent";

        private static string LogFileName = "Core2D.log";

        private bool _isLoaded = false;

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
                    _serviceProvider = container.Resolve<IServiceProvider>();
                    Start();
                }
            }
        }

        /// <summary>
        /// Initialize application context and displays main window.
        /// </summary>
        private void Start()
        {
            var log = _serviceProvider.GetService<ILog>();
            var fileIO = _serviceProvider.GetService<IFileSystem>();
            var editor = _serviceProvider.GetService<ProjectEditor>();

            log?.Initialize(System.IO.Path.Combine(fileIO.GetAssemblyPath(null), LogFileName));

            try
            {
                LoadRecent();

                editor.CurrentView = editor.Views.FirstOrDefault(view => view.Name == "Dashboard");
                editor.CurrentTool = editor.Tools.FirstOrDefault(tool => tool.Name == "Selection");
                editor.CurrentPathTool = editor.PathTools.FirstOrDefault(tool => tool.Name == "Line");

                var window = _serviceProvider.GetService<Windows.MainWindow>();
                window.Loaded += (sender, e) => OnLoaded();
                window.Closed += (sender, e) => OnClosed();
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

        /// <summary>
        /// Initialize main window after loaded.
        /// </summary>
        private void OnLoaded()
        {
            if (_isLoaded)
                return;
            else
                _isLoaded = true;
        }

        /// <summary>
        /// De-initialize main window after closed.
        /// </summary>
        private void OnClosed()
        {
            if (!_isLoaded)
                return;
            else
                _isLoaded = false;

            SaveRecent();
        }

        /// <summary>
        /// Load recent project files list.
        /// </summary>
        private void LoadRecent()
        {
            try
            {
                var fileIO = _serviceProvider.GetService<IFileSystem>();
                var editor = _serviceProvider.GetService<ProjectEditor>();
                var path = System.IO.Path.Combine(fileIO.GetAssemblyPath(null), RecentFileName);
                if (System.IO.File.Exists(path))
                {
                    editor?.OnLoadRecent(path);
                }
            }
            catch (Exception ex)
            {
                _serviceProvider.GetService<ILog>()?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Save recent project files list.
        /// </summary>
        private void SaveRecent()
        {
            try
            {
                var fileIO = _serviceProvider.GetService<IFileSystem>();
                var editor = _serviceProvider.GetService<ProjectEditor>();
                var path = System.IO.Path.Combine(fileIO.GetAssemblyPath(null), RecentFileName);
                editor?.OnSaveRecent(path);
            }
            catch (Exception ex)
            {
                _serviceProvider.GetService<ILog>()?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }
    }
}
