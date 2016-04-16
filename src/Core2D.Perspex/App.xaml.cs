// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor;
using Core2D.Editor.Designer;
using Core2D.Editor.Factories;
using Core2D.Editor.Interfaces;
using Core2D.Interfaces;
using Core2D.Project;
using Core2D.Renderer;
using FileWriter.Dxf;
using FileWriter.Pdf_core;
using Log.Trace;
using Perspex;
using Perspex.Controls;
using Renderer.Perspex;
using Serializer.Newtonsoft;
using Serializer.ProtoBuf;
using Serializer.Xaml;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TextFieldReader.CsvHelper;
using TextFieldWriter.CsvHelper;

namespace Core2D.Perspex
{
    /// <summary>
    /// Encapsulates a Core2D Prespex application.
    /// </summary>
    public class App : Application, IEditorApplication
    {
        private ProjectEditor _editor;
        private Windows.MainWindow _mainWindow;
        private string _recentFileName = "Core2D.recent";
        private string _logFileName = "Core2D.log";
        private bool _enableRecent = true;

        /// <summary>
        /// Initializes static data.
        /// </summary>
        static App()
        {
            DesignerContext.InitializeContext(
                new PerspexRenderer(),
                new PerspexTextClipboard(),
                new ProtoBufStreamSerializer(),
                new NewtonsoftTextSerializer(),
                new PortableXamlSerializer());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            RegisterServices();
        }

        /// <summary>
        /// Initialize application context and displays main window.
        /// </summary>
        public void Start()
        {
            using (ILog log = new TraceLog())
            {
                log.Initialize(System.IO.Path.Combine(GetAssemblyPath(), _logFileName));

                try
                {
                    InitializeEditor(log);
                    LoadRecent();
                    _mainWindow = new Windows.MainWindow();
                    _mainWindow.Closed += (sender, e) => SaveRecent();
                    _mainWindow.DataContext = _editor;
                    _mainWindow.Show();
                    Run(_mainWindow);
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

        /// <summary>
        /// Gets the location of the assembly as specified originally.
        /// </summary>
        /// <returns>The location of the assembly as specified originally.</returns>
        public static string GetAssemblyPath()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return System.IO.Path.GetDirectoryName(path);
        }

        /// <summary>
        /// Load recent project files list.
        /// </summary>
        private void LoadRecent()
        {
            if (_enableRecent)
            {
                try
                {
                    var path = System.IO.Path.Combine(GetAssemblyPath(), _recentFileName);
                    if (System.IO.File.Exists(path))
                    {
                        _editor.LoadRecent(path);
                    }
                }
                catch (Exception ex)
                {
                    _editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }
        }

        /// <summary>
        /// Save recent project files list.
        /// </summary>
        private void SaveRecent()
        {
            if (_enableRecent)
            {
                try
                {
                    var path = System.IO.Path.Combine(GetAssemblyPath(), _recentFileName);
                    _editor.SaveRecent(path);
                }
                catch (Exception ex)
                {
                    _editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }
        }

        /// <summary>
        /// Initialize <see cref="Editor"/> object.
        /// </summary>
        /// <param name="log">The log instance.</param>
        public void InitializeEditor(ILog log)
        {
            _editor = new ProjectEditor()
            {
                CurrentTool = Tool.Selection,
                CurrentPathTool = PathTool.Line,
                Application = this,
                Log = log,
                FileIO = new PerspexFileSystem(),
                CommandManager = new PerspexCommandManager(),
                Renderers = new ShapeRenderer[] { new PerspexRenderer(), new PerspexRenderer() },
                ProjectFactory = new ProjectFactory(),
                TextClipboard = new PerspexTextClipboard(),
                ProtoBufSerializer = new ProtoBufStreamSerializer(),
                JsonSerializer = new NewtonsoftTextSerializer(),
                XamlSerializer = new PortableXamlSerializer(),
                PdfWriter = new PdfWriter(),
                DxfWriter = new DxfWriter(),
                CsvReader = new CsvHelperReader(),
                CsvWriter = new CsvHelperWriter(),
                GetImageKey = async () => await (this as IEditorApplication).OnGetImageKeyAsync()
            };

            _editor.InitializeCommands();
            _editor.CommandManager.RegisterCommands();
        }

        /// <inheritdoc/>
        async Task<string> IEditorApplication.OnGetImageKeyAsync()
        {
            try
            {
                var dlg = new OpenFileDialog();
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                var result = await dlg.ShowAsync(_mainWindow);
                if (result != null)
                {
                    var path = result.FirstOrDefault();
                    var bytes = System.IO.File.ReadAllBytes(path);
                    var key = _editor?.Project?.AddImageFromFile(path, bytes);
                    return key;
                }
                return null;
            }
            catch (Exception ex)
            {
                _editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                return null;
            }
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnOpenAsync(string path)
        {
            try
            {
                if (path == null)
                {
                    var dlg = new OpenFileDialog();
                    dlg.Filters.Add(new FileDialogFilter() { Name = "Project", Extensions = { "project" } });
                    dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                    var result = await dlg.ShowAsync(_mainWindow);
                    if (result != null)
                    {
                        _editor?.Open(result.FirstOrDefault());
                        _editor?.Invalidate?.Invoke();
                    }
                }
                else
                {
                    if (System.IO.File.Exists(path))
                    {
                        _editor.Open(path);
                    }
                }
            }
            catch (Exception ex)
            {
                _editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnSaveAsync()
        {
            try
            {
                if (!string.IsNullOrEmpty(_editor?.ProjectPath))
                {
                    _editor?.Save(_editor?.ProjectPath);
                }
                else
                {
                    await (this as IEditorApplication).OnSaveAsAsync();
                }
            }
            catch (Exception ex)
            {
                _editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnSaveAsAsync()
        {
            try
            {
                if (_editor?.Project != null)
                {
                    var dlg = new SaveFileDialog();
                    dlg.Filters.Add(new FileDialogFilter() { Name = "Project", Extensions = { "project" } });
                    dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                    dlg.InitialFileName = _editor.Project.Name;
                    var result = await dlg.ShowAsync(_mainWindow);
                    if (result != null)
                    {
                        _editor?.Save(result);
                    }
                }
            }
            catch (Exception ex)
            {
                _editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnImportXamlAsync(string path)
        {
            try
            {
                if (path == null)
                {
                    var dlg = new OpenFileDialog();
                    dlg.AllowMultiple = true;
                    dlg.Filters.Add(new FileDialogFilter() { Name = "Xaml", Extensions = { "xaml" } });
                    dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });

                    var results = await dlg.ShowAsync(_mainWindow);
                    if (results != null)
                    {
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
            }
            catch (Exception ex)
            {
                _editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnExportXamlAsync(object item)
        {
            try
            {
                var dlg = new SaveFileDialog();
                dlg.Filters.Add(new FileDialogFilter() { Name = "Xaml", Extensions = { "xaml" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                dlg.InitialFileName = _editor?.GetName(item);

                var result = await dlg.ShowAsync(_mainWindow);
                if (result != null)
                {
                    _editor?.OnExportXaml(result, item);
                }
            }
            catch (Exception ex)
            {
                _editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnImportJsonAsync(string path)
        {
            try
            {
                if (path == null)
                {
                    var dlg = new OpenFileDialog();
                    dlg.AllowMultiple = true;
                    dlg.Filters.Add(new FileDialogFilter() { Name = "Json", Extensions = { "json" } });
                    dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });

                    var results = await dlg.ShowAsync(_mainWindow);
                    if (results != null)
                    {
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
            }
            catch (Exception ex)
            {
                _editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnExportJsonAsync(object item)
        {
            try
            {
                var dlg = new SaveFileDialog();
                dlg.Filters.Add(new FileDialogFilter() { Name = "Json", Extensions = { "json" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                dlg.InitialFileName = _editor?.GetName(item);

                var result = await dlg.ShowAsync(_mainWindow);
                if (result != null)
                {
                    _editor?.OnExportJson(result, item);
                }
            }
            catch (Exception ex)
            {
                _editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnExportAsync(object item)
        {
            try
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
                    if (_editor?.Project == null)
                        return;

                    name = _editor?.Project?.Name;
                    item = _editor?.Project;
                }

                var dlg = new SaveFileDialog();
                dlg.Filters.Add(new FileDialogFilter() { Name = "Pdf", Extensions = { "pdf" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "Dxf", Extensions = { "dxf" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                dlg.InitialFileName = name;
                var result = await dlg.ShowAsync(_mainWindow);
                if (result != null)
                {
                    var ext = System.IO.Path.GetExtension(result).ToLower();

                    if (ext == ".pdf")
                    {
                        _editor?.ExportAsPdf(result, item);
                    }

                    if (ext == ".dxf")
                    {
                        _editor?.ExportAsDxf(result, item);
                    }
                }
            }
            catch (Exception ex)
            {
                _editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnImportDataAsync()
        {
            try
            {
                if (_editor?.Project != null)
                {
                    var dlg = new OpenFileDialog();
                    dlg.Filters.Add(new FileDialogFilter() { Name = "Csv", Extensions = { "csv" } });
                    dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                    var result = await dlg.ShowAsync(_mainWindow);
                    if (result != null)
                    {
                        var path = result.FirstOrDefault();
                        _editor?.OnImportData(path);
                    }
                }
            }
            catch (Exception ex)
            {
                _editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnExportDataAsync()
        {
            try
            {
                var database = _editor?.Project?.CurrentDatabase;
                if (database != null)
                {
                    var dlg = new SaveFileDialog();
                    dlg.Filters.Add(new FileDialogFilter() { Name = "Csv", Extensions = { "csv" } });
                    dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                    dlg.InitialFileName = database.Name;
                    var result = await dlg.ShowAsync(_mainWindow);
                    if (result != null)
                    {
                        _editor?.OnExportData(result, database);
                    }
                }
            }
            catch (Exception ex)
            {
                _editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnUpdateDataAsync()
        {
            try
            {
                var database = _editor?.Project?.CurrentDatabase;
                if (database != null)
                {
                    var dlg = new OpenFileDialog();
                    dlg.Filters.Add(new FileDialogFilter() { Name = "Csv", Extensions = { "csv" } });
                    dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                    var result = await dlg.ShowAsync(_mainWindow);
                    if (result != null)
                    {
                        var path = result.FirstOrDefault();
                        _editor?.OnUpdateData(path, database);
                    }
                }
            }
            catch (Exception ex)
            {
                _editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnImportObjectAsync(string path)
        {
            try
            {
                if (path == null)
                {
                    var dlg = new OpenFileDialog();
                    dlg.AllowMultiple = true;
                    dlg.Filters.Add(new FileDialogFilter() { Name = "Json", Extensions = { "json" } });
                    dlg.Filters.Add(new FileDialogFilter() { Name = "Xaml", Extensions = { "xaml" } });
                    var results = await dlg.ShowAsync(_mainWindow);
                    if (results != null)
                    {
                        foreach (var result in results)
                        {
                            string resultExtension = System.IO.Path.GetExtension(result);
                            if (string.Compare(resultExtension, ".json", true) == 0)
                            {
                                _editor?.OnImportJson(result);
                            }
                            else if (string.Compare(resultExtension, ".xaml", true) == 0)
                            {
                                _editor?.OnImportJson(result);
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
            }
            catch (Exception ex)
            {
                _editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnExportObjectAsync(object item)
        {
            try
            {
                if (item != null)
                {
                    var dlg = new SaveFileDialog();
                    dlg.Filters.Add(new FileDialogFilter() { Name = "Json", Extensions = { "json" } });
                    dlg.Filters.Add(new FileDialogFilter() { Name = "Xaml", Extensions = { "xaml" } });
                    dlg.InitialFileName = _editor?.GetName(item);

                    var path = await dlg.ShowAsync(_mainWindow);
                    if (path != null)
                    {
                        string resultExtension = System.IO.Path.GetExtension(path);
                        if (string.Compare(resultExtension, ".json", true) == 0)
                        {
                            _editor?.OnExportJson(path, item);
                        }
                        else if (string.Compare(resultExtension, ".xaml", true) == 0)
                        {
                            _editor?.OnExportXaml(path, item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnCopyAsEmfAsync()
        {
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnExportAsEmfAsync(string path)
        {
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
            var browser = new Windows.BrowserWindow();
            browser.DataContext = _editor;
            browser.Show();

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnShowDocumentViewerAsync()
        {
            var document = new Windows.DocumentWindow();
            document.DataContext = _editor;
            document.Show();

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        void IEditorApplication.OnCloseView()
        {
            _mainWindow?.Close();
        }
    }
}
