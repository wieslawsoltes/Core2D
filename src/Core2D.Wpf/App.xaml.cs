// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using System.Windows;
using Core2D.Data.Database;
using Core2D.Editor;
using Core2D.Editor.Factories;
using Core2D.Editor.Input;
using Core2D.Editor.Interfaces;
using Core2D.Interfaces;
using Core2D.Renderer;
using FileSystem.DotNetFx;
using FileWriter.Dxf;
using FileWriter.Emf;
using FileWriter.Pdf_wpf;
using FileWriter.Vdx;
using Log.Trace;
using Renderer.Wpf;
using Serializer.Newtonsoft;
using Serializer.Xaml;
using TextFieldReader.CsvHelper;
using TextFieldWriter.CsvHelper;
using Utilities.Wpf;

namespace Core2D.Wpf
{
    /// <summary>
    /// Encapsulates an WPF application.
    /// </summary>
    public partial class App : Application
    {
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

            RegisterServices();

            using (var log = ServiceLocator.Instance.Resolve<ILog>())
            {
                Start();
            }
        }

        /// <summary>
        /// Register application services.
        /// </summary>
        private void RegisterServices()
        {
            ServiceLocator.Instance.RegisterSingleton<ProjectEditor>(() => new ProjectEditor());
            ServiceLocator.Instance.RegisterSingleton<IEditorApplication>(() => this);
            ServiceLocator.Instance.RegisterSingleton<ILog>(() => new TraceLog());
            ServiceLocator.Instance.RegisterSingleton<CommandManager>(() => new WpfCommandManager());
            ServiceLocator.Instance.RegisterSingleton<ShapeRenderer[]>(() => new[] { new WpfRenderer() });
            ServiceLocator.Instance.RegisterSingleton<IFileSystem>(() => new DotNetFxFileSystem());
            ServiceLocator.Instance.RegisterSingleton<IProjectFactory>(() => new ProjectFactory());
            ServiceLocator.Instance.RegisterSingleton<ITextClipboard>(() => new WpfTextClipboard());
            ServiceLocator.Instance.RegisterSingleton<IJsonSerializer>(() => new NewtonsoftJsonSerializer());
            ServiceLocator.Instance.RegisterSingleton<IXamlSerializer>(() => new PortableXamlSerializer());
            ServiceLocator.Instance.RegisterSingleton<ImmutableArray<IFileWriter>>(
                () =>
                {
                    return new IFileWriter[]
                    {
                        new PdfWriter(),
                        new DxfWriter(),
                        new EmfWriter(),
                        new VdxWriter()
                    }.ToImmutableArray();
                });
            ServiceLocator.Instance.RegisterSingleton<ITextFieldReader<XDatabase>>(() => new CsvHelperReader());
            ServiceLocator.Instance.RegisterSingleton<ITextFieldWriter<XDatabase>>(() => new CsvHelperWriter());
            ServiceLocator.Instance.RegisterSingleton<Windows.MainWindow>(() => new Windows.MainWindow());
        }

        /// <summary>
        /// Initialize application context and displays main window.
        /// </summary>
        public void Start()
        {
            var log = ServiceLocator.Instance.Resolve<ILog>();
            var fileIO = ServiceLocator.Instance.Resolve<IFileSystem>();
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();

            log?.Initialize(System.IO.Path.Combine(fileIO.GetAssemblyPath(null), LogFileName));

            try
            {
                LoadRecent();

                var window = ServiceLocator.Instance.Resolve<Windows.MainWindow>();
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
            var log = ServiceLocator.Instance.Resolve<ILog>();
            var fileIO = ServiceLocator.Instance.Resolve<IFileSystem>();
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();

            try
            {
                var path = System.IO.Path.Combine(fileIO.GetAssemblyPath(null), RecentFileName);
                if (System.IO.File.Exists(path))
                {
                    editor?.OnLoadRecent(path);
                }
            }
            catch (Exception ex)
            {
                log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Save recent project files list.
        /// </summary>
        private void SaveRecent()
        {
            var log = ServiceLocator.Instance.Resolve<ILog>();
            var fileIO = ServiceLocator.Instance.Resolve<IFileSystem>();
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();

            try
            {
                var path = System.IO.Path.Combine(fileIO.GetAssemblyPath(null), RecentFileName);
                editor?.OnSaveRecent(path);
            }
            catch (Exception ex)
            {
                log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }
    }
}
