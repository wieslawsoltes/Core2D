// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor;
using Core2D.Editor.Factories;
using Core2D.Editor.Interfaces;
using Core2D.Interfaces;
using Core2D.Project;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;
using FileWriter.Dxf;
using FileWriter.Emf;
using FileWriter.Pdf_wpf;
using Log.Trace;
using Microsoft.Win32;
using Renderer.Wpf;
using Serializer.Newtonsoft;
using Serializer.ProtoBuf;
using Serializer.Xaml;
using System;
using System.Linq;
using System.Reflection;
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
        private ProjectEditor _editor;
        private Windows.MainWindow _mainWindow;
        private bool _isLoaded = false;
        private string _recentFileName = "Core2D.recent";
        private string _logFileName = "Core2D.log";
        private bool _enableRecent = true;
        private bool _restoreLayout = true;

        /// <summary>
        /// Raises the <see cref="Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Start();
        }

        /// <summary>
        /// Initialize application context and displays main window.
        /// </summary>
        public void Start()
        {
            using (var log = new TraceLog())
            {
                log.Initialize(System.IO.Path.Combine(GetAssemblyPath(), _logFileName));

                try
                {
                    InitializeEditor(log);
                    LoadRecent();

                    _mainWindow = new Windows.MainWindow();
                    _mainWindow.InitializeEditor(_editor);
                    _mainWindow.Loaded += (sender, e) => OnLoaded();
                    _mainWindow.Closed += (sender, e) => OnClosed();
                    _mainWindow.DataContext = _editor;
                    _mainWindow.ShowDialog();
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
        /// Initialize main window after loaded.
        /// </summary>
        private void OnLoaded()
        {
            if (_isLoaded)
                return;
            else
                _isLoaded = true;

            if (_restoreLayout)
            {
                _mainWindow.AutoLoadLayout(_editor);
            }
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

            if (_restoreLayout)
            {
                _mainWindow.AutoSaveLayout(_editor);
            }
        }

        /// <summary>
        /// Gets the location of the assembly as specified originally.
        /// </summary>
        /// <returns>The location of the assembly as specified originally.</returns>
        private string GetAssemblyPath()
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
                        _editor?.LoadRecent(path);
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
                    _editor?.SaveRecent(path);
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
        private void InitializeEditor(ILog log)
        {
            _editor = new ProjectEditor()
            {
                CurrentTool = Tool.Selection,
                CurrentPathTool = PathTool.Line,
                Application = this,
                Log = log,
                FileIO = new WpfFileSystem(),
                CommandManager = new WpfCommandManager(),
                Renderers = new ShapeRenderer[] { new WpfRenderer() },
                ProjectFactory = new ProjectFactory(),
                TextClipboard = new WpfTextClipboard(),
                ProtoBufSerializer = new ProtoBufStreamSerializer(),
                JsonSerializer = new NewtonsoftTextSerializer(),
                XamlSerializer = new PortableXamlSerializer(),
                PdfWriter = new PdfWriter(),
                DxfWriter = new DxfWriter(),
                CsvReader = new CsvHelperReader(),
                CsvWriter = new CsvHelperWriter(),
                GetImageKey = async () => await OnGetImageKeyAsync()
            };

            _editor.DefaultTools();
            _editor.InitializeCommands();
            _editor.CommandManager.RegisterCommands();
        }

        /// <inheritdoc/>
        public async Task<string> OnGetImageKeyAsync()
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
                    _editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }
            return null;
        }

        /// <inheritdoc/>
        public async Task OnOpenAsync(string path)
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
                    _editor?.Open(dlg.FileName);
                }
            }
            else
            {
                if (System.IO.File.Exists(path))
                {
                    _editor?.Open(path);
                }
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        public async Task OnSaveAsync()
        {
            if (!string.IsNullOrEmpty(_editor?.ProjectPath))
            {
                _editor?.Save(_editor?.ProjectPath);
            }
            else
            {
                await OnSaveAsAsync();
            }
        }

        /// <inheritdoc/>
        public async Task OnSaveAsAsync()
        {
            var dlg = new SaveFileDialog()
            {
                Filter = "Project (*.project)|*.project|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = _editor?.Project?.Name
            };

            if (dlg.ShowDialog(_mainWindow) == true)
            {
                _editor?.Save(dlg.FileName);
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        public async Task OnImportXamlAsync(string path)
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
        public async Task OnExportXamlAsync(object item)
        {
            var dlg = new SaveFileDialog()
            {
                Filter = "Xaml (*.xaml)|*.xaml|All (*.*)|*.*",
                FilterIndex = 0,
            };

            if (dlg.ShowDialog(_mainWindow) == true)
            {
                _editor?.OnExportXaml(dlg.FileName, item);
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        public async Task OnExportAsync(object item)
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

            var dlg = new SaveFileDialog()
            {
                Filter = "Pdf (*.pdf)|*.pdf|Emf (*.emf)|*.emf|Dxf (*.dxf)|*.dxf|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = name
            };

            if (dlg.ShowDialog(_mainWindow) == true)
            {
                switch (dlg.FilterIndex)
                {
                    case 1:
                        _editor?.ExportAsPdf(dlg.FileName, item);
                        break;
                    case 2:
                        await OnExportAsEmfAsync(dlg.FileName);
                        break;
                    case 3:
                        _editor?.ExportAsDxf(dlg.FileName, item);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <inheritdoc/>
        public async Task OnImportDataAsync()
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
        public async Task OnExportDataAsync()
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
        public async Task OnUpdateDataAsync()
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
        public async Task OnImportObjectAsync(object item, CoreType type)
        {
            if (item == null)
                return;

            string filter = string.Empty;

            switch (type)
            {
                case CoreType.Style:
                    filter = "Style (*.style)|*.style|All (*.*)|*.*";
                    break;
                case CoreType.Styles:
                    filter = "Styles (*.styles)|*.styles|All (*.*)|*.*";
                    break;
                case CoreType.StyleLibrary:
                    filter = "StyleLibrary (*.stylelibrary)|*.stylelibrary|All (*.*)|*.*";
                    break;
                case CoreType.StyleLibraries:
                    filter = "StyleLibraries (*.styleLibraries)|*.stylelibraries|All (*.*)|*.*";
                    break;
                case CoreType.Group:
                    filter = "Group (*.group)|*.group|All (*.*)|*.*";
                    break;
                case CoreType.Groups:
                    filter = "Groups (*.groups)|*.groups|All (*.*)|*.*";
                    break;
                case CoreType.GroupLibrary:
                    filter = "GroupLibrary (*.grouplibrary)|*.grouplibrary|All (*.*)|*.*";
                    break;
                case CoreType.GroupLibraries:
                    filter = "GroupLibraries (*.grouplibraries)|*.grouplibraries|All (*.*)|*.*";
                    break;
                case CoreType.Template:
                    filter = "Template (*.template)|*.template|All (*.*)|*.*";
                    break;
                case CoreType.Templates:
                    filter = "Templates (*.templates)|*.templates|All (*.*)|*.*";
                    break;
            }

            var dlg = new OpenFileDialog()
            {
                Filter = filter,
                Multiselect = true,
                FilterIndex = 0
            };

            if (dlg.ShowDialog(_mainWindow) == true)
            {
                var paths = dlg.FileNames;

                foreach (var path in paths)
                {
                    _editor?.OnImportObject(path, item, type);
                }
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        public async Task OnExportObjectAsync(object item, CoreType type)
        {
            if (item == null)
                return;

            string name = string.Empty;
            string filter = string.Empty;

            switch (type)
            {
                case CoreType.Style:
                    filter = "Style (*.style)|*.style|Style (*.xaml)|*.xaml|All (*.*)|*.*";
                    name = (item as ShapeStyle).Name;
                    break;
                case CoreType.Styles:
                    filter = "Styles (*.styles)|*.styles|Styles (*.xaml)|*.xaml|All (*.*)|*.*";
                    name = (item as XLibrary<ShapeStyle>).Name;
                    break;
                case CoreType.StyleLibrary:
                    filter = "StyleLibrary (*.stylelibrary)|*.stylelibrary|StyleLibrary (*.xaml)|*.xaml|All (*.*)|*.*";
                    name = (item as XLibrary<ShapeStyle>).Name;
                    break;
                case CoreType.StyleLibraries:
                    filter = "StyleLibraries (*.stylelibraries)|*.stylelibraries|StyleLibraries (*.xaml)|*.xaml|All (*.*)|*.*";
                    name = "StyleLibraries";
                    break;
                case CoreType.Group:
                    filter = "Group (*.group)|*.group|Group (*.xaml)|*.xaml|All (*.*)|*.*";
                    name = (item as XGroup).Name;
                    break;
                case CoreType.Groups:
                    filter = "Groups (*.groups)|*.groups|Groups (*.xaml)|*.xaml|All (*.*)|*.*";
                    name = (item as XLibrary<XGroup>).Name;
                    break;
                case CoreType.GroupLibrary:
                    filter = "GroupLibrary (*.grouplibrary)|*.grouplibrary|GroupLibrary (*.xaml)|*.xaml|All (*.*)|*.*";
                    name = (item as XLibrary<XGroup>).Name;
                    break;
                case CoreType.GroupLibraries:
                    filter = "GroupLibraries (*.grouplibraries)|*.grouplibraries|GroupLibraries (*.xaml)|*.xaml|All (*.*)|*.*";
                    name = "GroupLibraries";
                    break;
                case CoreType.Template:
                    filter = "Template (*.template)|*.template|Template (*.xaml)|*.xaml|All (*.*)|*.*";
                    name = (item as XTemplate).Name;
                    break;
                case CoreType.Templates:
                    filter = "Templates (*.templates)|*.templates|Templates (*.xaml)|*.xaml|All (*.*)|*.*";
                    name = "Templates";
                    break;
            }

            var dlg = new SaveFileDialog()
            {
                Filter = filter,
                FilterIndex = 0,
                FileName = name
            };

            if (dlg.ShowDialog(_mainWindow) == true)
            {
                switch (dlg.FilterIndex)
                {
                    case 1:
                        _editor?.OnExportObject(dlg.FileName, item, type);
                        break;
                    case 2:
                        _editor?.OnExportXaml(dlg.FileName, item);
                        break;
                    case 3:
                        throw new NotSupportedException();
                    default:
                        break;
                }
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        public async Task OnCopyAsEmfAsync()
        {
            var page = _editor?.Project?.CurrentContainer as XPage;
            if (page != null)
            {
                var writer = new EmfWriter();

                if (_editor?.Renderers[0]?.State?.SelectedShape != null)
                {
                    var shapes = Enumerable.Repeat(_editor.Renderers[0].State.SelectedShape, 1).ToList();
                    writer.SetClipboard(
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
                    writer.SetClipboard(
                        shapes,
                        page.Template.Width,
                        page.Template.Height,
                        page.Data.Properties,
                        page.Data.Record,
                        _editor.Project);
                }
                else
                {
                    writer.SetClipboard(page, _editor.Project);
                }
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        public async Task OnExportAsEmfAsync(string path)
        {
            try
            {
                if (_editor?.Project?.CurrentContainer != null)
                {
                    var writer = new EmfWriter();
                    writer.Save(
                        path,
                        _editor.Project.CurrentContainer,
                        _editor.Project);
                }
            }
            catch (Exception ex)
            {
                _editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        public async Task OnZoomResetAsync()
        {
            _mainWindow.OnZoomReset();
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        public async Task OnZoomAutoFitAsync()
        {
            _mainWindow.OnZoomAutoFit();
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        public async Task OnLoadWindowLayout()
        {
            _mainWindow.OnLoadLayout();
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        public async Task OnSaveWindowLayoutAsync()
        {
            _mainWindow.OnSaveLayout();
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        public async Task OnResetWindowLayoutAsync()
        {
            _mainWindow.OnResetLayout();
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        public async Task OnShowObjectBrowserAsync()
        {
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        public async Task OnShowDocumentViewerAsync()
        {
            await Task.Delay(0);
        }

        /// <summary>
        /// Close application view.
        /// </summary>
        public void OnCloseView()
        {
            _mainWindow?.Close();
        }
    }
}
