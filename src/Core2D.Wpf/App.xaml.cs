// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor;
using Core2D.Editor.Designer;
using Core2D.Editor.Factories;
using Core2D.Editor.Interfaces;
using Core2D.Interfaces;
using Core2D.Project;
using Core2D.Renderer;
using FileSystem.DotNetFx;
using FileWriter.Dxf;
using FileWriter.Emf;
using FileWriter.Pdf_wpf;
using Log.Trace;
using Microsoft.Win32;
using Renderer.Wpf;
using Serializer.Newtonsoft;
using Serializer.Xaml;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TextFieldReader.CsvHelper;
using TextFieldWriter.CsvHelper;

namespace Core2D.Wpf
{
    /// <summary>
    /// Encapsulates a Core2D WPF application.
    /// </summary>
    public partial class App : Application, IEditorApplication
    {
        private IFileSystem _fileIO;
        private ILog _log;
        private ImmutableArray<IFileWriter> _writers;
        private ProjectEditor _editor;
        private Windows.MainWindow _mainWindow;
        private bool _isLoaded = false;
        private string _recentFileName = "Core2D.recent";
        private string _logFileName = "Core2D.log";

        /// <summary>
        /// Initializes static data.
        /// </summary>
        static App()
        {
            DesignerContext.InitializeContext(
                new WpfRenderer(),
                new WpfTextClipboard(),
                new NewtonsoftTextSerializer(),
                new PortableXamlSerializer());
        }

        /// <summary>
        /// Raises the <see cref="Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using (ILog log = new TraceLog())
            {
                IFileSystem fileIO = new DotNetFxFileSystem();
                ImmutableArray<IFileWriter> writers = 
                    new IFileWriter[] 
                    {
                        new PdfWriter(),
                        new DxfWriter(),
                        new EmfWriter()
                    }.ToImmutableArray();

                Start(fileIO, log, writers);
            }
        }

        /// <summary>
        /// Initialize application context and displays main window.
        /// </summary>
        /// <param name="fileIO">The file system instance.</param>
        /// <param name="log">The log instance.</param>
        /// <param name="writers">The file writers.</param>
        public void Start(IFileSystem fileIO, ILog log, ImmutableArray<IFileWriter> writers)
        {
            _fileIO = fileIO;
            _log = log;
            _writers = writers;

            _log.Initialize(System.IO.Path.Combine(_fileIO.AssemblyPath, _logFileName));

            try
            {
                InitializeEditor(_fileIO, _log, _writers);
                LoadRecent();
                _mainWindow = new Windows.MainWindow();
                _mainWindow.Loaded += (sender, e) => OnLoaded();
                _mainWindow.Closed += (sender, e) => OnClosed();
                _mainWindow.DataContext = _editor;
                _mainWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    _log?.LogError($"{ex.InnerException.Message}{Environment.NewLine}{ex.InnerException.StackTrace}");
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
                var path = System.IO.Path.Combine(_fileIO.AssemblyPath, _recentFileName);
                if (System.IO.File.Exists(path))
                {
                    _editor?.OnLoadRecent(path);
                }
            }
            catch (Exception ex)
            {
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Save recent project files list.
        /// </summary>
        private void SaveRecent()
        {
            try
            {
                var path = System.IO.Path.Combine(_fileIO.AssemblyPath, _recentFileName);
                _editor?.OnSaveRecent(path);
            }
            catch (Exception ex)
            {
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Initialize <see cref="ProjectEditor"/> object.
        /// </summary>
        /// <param name="fileIO">The file system instance.</param>
        /// <param name="log">The log instance.</param>
        /// <param name="writers">The file writers.</param>
        private void InitializeEditor(IFileSystem fileIO, ILog log, ImmutableArray<IFileWriter> writers)
        {
            _editor = new ProjectEditor()
            {
                CurrentTool = Tool.Selection,
                CurrentPathTool = PathTool.Line,
                Application = this,
                Log = log,
                FileIO = fileIO,
                CommandManager = new WpfCommandManager(),
                Renderers = new ShapeRenderer[] { new WpfRenderer() },
                ProjectFactory = new ProjectFactory(),
                TextClipboard = new WpfTextClipboard(),
                JsonSerializer = new NewtonsoftTextSerializer(),
                XamlSerializer = new PortableXamlSerializer(),
                FileWriters = writers,
                CsvReader = new CsvHelperReader(),
                CsvWriter = new CsvHelperWriter(),
                GetImageKey = async () => await (this as IEditorApplication).OnGetImageKeyAsync()
            }.Defaults();

            _editor.InitializeCommands();
            _editor.CommandManager.RegisterCommands();
        }

        /// <inheritdoc/>
        async Task<string> IEditorApplication.OnGetImageKeyAsync()
        {
            var dlg = new OpenFileDialog()
            {
                Filter = "All (*.*)|*.*",
                FilterIndex = 0,
                FileName = ""
            };

            if (dlg.ShowDialog(_mainWindow) == true)
            {
                try
                {
                    var path = dlg.FileName;
                    var bytes = System.IO.File.ReadAllBytes(path);
                    var key = _editor?.Project.AddImageFromFile(path, bytes);
                    return await Task.Run(() => key);
                }
                catch (Exception ex)
                {
                    _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }
            return null;
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnOpenAsync(string path)
        {
            if (path == null)
            {
                var dlg = new OpenFileDialog()
                {
                    Filter = "Project (*.project)|*.project|All (*.*)|*.*",
                    FilterIndex = 0,
                    FileName = ""
                };

                if (dlg.ShowDialog(_mainWindow) == true)
                {
                    _editor?.OnOpen(dlg.FileName);
                }
            }
            else
            {
                if (System.IO.File.Exists(path))
                {
                    _editor?.OnOpen(path);
                }
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnSaveAsync()
        {
            if (!string.IsNullOrEmpty(_editor?.ProjectPath))
            {
                _editor?.OnSave(_editor?.ProjectPath);
            }
            else
            {
                await (this as IEditorApplication).OnSaveAsAsync();
            }
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnSaveAsAsync()
        {
            var dlg = new SaveFileDialog()
            {
                Filter = "Project (*.project)|*.project|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = _editor?.Project?.Name
            };

            if (dlg.ShowDialog(_mainWindow) == true)
            {
                _editor?.OnSave(dlg.FileName);
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnImportXamlAsync(string path)
        {
            if (path == null)
            {
                var dlg = new OpenFileDialog()
                {
                    Filter = "Xaml (*.xaml)|*.xaml|All (*.*)|*.*",
                    FilterIndex = 0,
                    Multiselect = true,
                    FileName = ""
                };

                if (dlg.ShowDialog(_mainWindow) == true)
                {
                    var results = dlg.FileNames;

                    foreach (var result in results)
                    {
                        _editor?.OnImportXaml(result);
                    }
                }
            }
            else
            {
                if (System.IO.File.Exists(path))
                {
                    _editor?.OnImportXaml(path);
                }
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnExportXamlAsync(object item)
        {
            var dlg = new SaveFileDialog()
            {
                Filter = "Xaml (*.xaml)|*.xaml|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = _editor?.GetName(item)
            };

            if (dlg.ShowDialog(_mainWindow) == true)
            {
                _editor?.OnExportXaml(dlg.FileName, item);
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnImportJsonAsync(string path)
        {
            if (path == null)
            {
                var dlg = new OpenFileDialog()
                {
                    Filter = "Json (*.json)|*.json|All (*.*)|*.*",
                    FilterIndex = 0,
                    Multiselect = true,
                    FileName = ""
                };

                if (dlg.ShowDialog(_mainWindow) == true)
                {
                    var results = dlg.FileNames;

                    foreach (var result in results)
                    {
                        _editor?.OnImportJson(result);
                    }
                }
            }
            else
            {
                if (System.IO.File.Exists(path))
                {
                    _editor?.OnImportJson(path);
                }
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnExportJsonAsync(object item)
        {
            var dlg = new SaveFileDialog()
            {
                Filter = "Xaml (*.xaml)|*.xaml|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = _editor?.GetName(item)
            };

            if (dlg.ShowDialog(_mainWindow) == true)
            {
                _editor?.OnExportJson(dlg.FileName, item);
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnExportAsync(object item)
        {
            string name = string.Empty;

            if (item is XContainer)
            {
                name = (item as XContainer).Name;
            }
            else if (item is XDocument)
            {
                name = (item as XDocument).Name;
            }
            else if (item is XProject)
            {
                name = (item as XProject).Name;
            }
            else if (item is ProjectEditor)
            {
                var editor = (item as ProjectEditor);
                if (editor?.Project == null)
                    return;

                name = editor?.Project?.Name;
                item = editor?.Project;
            }
            else if (item == null)
            {
                if (_editor.Project == null)
                    return;

                name = _editor?.Project?.Name;
                item = _editor?.Project;
            }

            var sb = new StringBuilder();
            foreach (var writer in _editor?.FileWriters)
            {
                sb.Append($"{writer.Name} (*.{writer.Extension})|*.{writer.Extension}|");
            }
            sb.Append("All (*.*)|*.*");

            var dlg = new SaveFileDialog()
            {
                Filter = sb.ToString(),
                FilterIndex = 0,
                FileName = name
            };

            if (dlg.ShowDialog(_mainWindow) == true)
            {
                string result = dlg.FileName;
                string ext = System.IO.Path.GetExtension(result).ToLower().TrimStart('.');
                IFileWriter writer = _editor?.FileWriters.Where(w => string.Compare(w.Extension, ext, StringComparison.OrdinalIgnoreCase) == 0).FirstOrDefault();
                if (writer != null)
                {
                    _editor?.OnExport(result, item, writer);
                }
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnImportDataAsync()
        {
            var dlg = new OpenFileDialog()
            {
                Filter = "Csv (*.csv)|*.csv|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = ""
            };

            if (dlg.ShowDialog(_mainWindow) == true)
            {
                _editor?.OnImportData(dlg.FileName);
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnExportDataAsync()
        {
            var database = _editor?.Project?.CurrentDatabase;
            if (database != null)
            {
                var dlg = new SaveFileDialog()
                {
                    Filter = "Csv (*.csv)|*.csv|All (*.*)|*.*",
                    FilterIndex = 0,
                    FileName = database.Name
                };

                if (dlg.ShowDialog(_mainWindow) == true)
                {
                    _editor?.OnExportData(dlg.FileName, database);
                }
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnUpdateDataAsync()
        {
            var database = _editor?.Project?.CurrentDatabase;
            if (database != null)
            {
                var dlg = new OpenFileDialog()
                {
                    Filter = "Csv (*.csv)|*.csv|All (*.*)|*.*",
                    FilterIndex = 0,
                    FileName = ""
                };

                if (dlg.ShowDialog(_mainWindow) == true)
                {
                    _editor?.OnUpdateData(dlg.FileName, database);
                }
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnImportObjectAsync(string path)
        {
            if (path == null)
            {
                var dlg = new OpenFileDialog()
                {
                    Filter = "Json (*.json)|*.json|Xaml (*.xaml)|*.xaml",
                    Multiselect = true,
                    FilterIndex = 0
                };

                if (dlg.ShowDialog(_mainWindow) == true)
                {
                    var results = dlg.FileNames;
                    var index = dlg.FilterIndex;

                    foreach (var result in results)
                    {
                        switch (index)
                        {
                            case 1:
                                _editor?.OnImportJson(result);
                                break;
                            case 2:
                                _editor?.OnImportXaml(result);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            else
            {
                if (System.IO.File.Exists(path))
                {
                    string resultExtension = System.IO.Path.GetExtension(path);
                    if (string.Compare(resultExtension, ".json", true) == 0)
                    {
                        _editor?.OnImportJson(path);
                    }
                    else if (string.Compare(resultExtension, ".xaml", true) == 0)
                    {
                        _editor?.OnImportJson(path);
                    }
                }
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnExportObjectAsync(object item)
        {
            if (item != null)
            {
                var dlg = new SaveFileDialog()
                {
                    Filter = "Json (*.json)|*.json|Xaml (*.xaml)|*.xaml",
                    FilterIndex = 0,
                    FileName = _editor?.GetName(item)
                };

                if (dlg.ShowDialog(_mainWindow) == true)
                {
                    switch (dlg.FilterIndex)
                    {
                        case 1:
                            _editor?.OnExportJson(dlg.FileName, item);
                            break;
                        case 2:
                            _editor?.OnExportXaml(dlg.FileName, item);
                            break;
                        default:
                            break;
                    }
                }
            }
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnCopyAsEmfAsync()
        {
            var page = _editor?.Project?.CurrentContainer;
            if (page != null)
            {
                if (_editor?.Renderers[0]?.State?.SelectedShape != null)
                {
                    var shapes = Enumerable.Repeat(_editor.Renderers[0].State.SelectedShape, 1).ToList();
                    EmfWriter.SetClipboard(
                        shapes,
                        page.Template.Width,
                        page.Template.Height,
                        page.Data.Properties,
                        page.Data.Record,
                        _editor.Project);
                }
                else if (_editor?.Renderers?[0]?.State?.SelectedShapes != null)
                {
                    var shapes = _editor.Renderers[0].State.SelectedShapes.ToList();
                    EmfWriter.SetClipboard(
                        shapes,
                        page.Template.Width,
                        page.Template.Height,
                        page.Data.Properties,
                        page.Data.Record,
                        _editor.Project);
                }
                else
                {
                    EmfWriter.SetClipboard(page, _editor.Project);
                }
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnExportAsEmfAsync(string path)
        {
            try
            {
                var page = _editor?.Project?.CurrentContainer;
                if (page != null)
                {
                    EmfWriter.Save(path, page, _editor.Project as IImageCache);
                }
            }
            catch (Exception ex)
            {
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnZoomResetAsync()
        {
            _editor.ResetZoom?.Invoke();
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnZoomAutoFitAsync()
        {
            _editor.AutoFitZoom?.Invoke();
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnLoadWindowLayout()
        {
            _editor.LoadLayout?.Invoke();
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnSaveWindowLayoutAsync()
        {
            _editor.SaveLayout?.Invoke();
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnResetWindowLayoutAsync()
        {
            _editor.ResetLayout?.Invoke();
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnShowObjectBrowserAsync()
        {
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnShowDocumentViewerAsync()
        {
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        void IEditorApplication.OnCloseView()
        {
            _mainWindow?.Close();
        }
    }
}
