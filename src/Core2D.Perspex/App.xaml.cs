// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//#define SKIA_WIN
//#define SKIA_GTK
using Core2D.Editor;
using Core2D.Editor.Designer;
using Core2D.Editor.Factories;
using Core2D.Editor.Interfaces;
using Core2D.Interfaces;
using Core2D.Project;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;
using FileWriter.Dxf;
using FileWriter.Pdf_core;
using Log.Trace;
using Perspex;
using Perspex.Controls;
using Perspex.Diagnostics;
using Perspex.Markup.Xaml;
#if SKIA_WIN
using Perspex.Win32;
using Perspex.Skia;
#endif
#if SKIA_GTK
using Perspex.Gtk;
using Perspex.Skia;
#endif
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
#if SKIA_WIN
            Win32Platform.Initialize();
            SkiaPlatform.Initialize();
#elif SKIA_GTK
            GtkPlatform.Initialize();
            SkiaPlatform.Initialize();
#else
            InitializeSubsystems((int)Environment.OSVersion.Platform);
#endif
            InitializeComponent();
        }

        /// <summary>
        /// Initialize the Xaml components.
        /// </summary>
        private void InitializeComponent()
        {
            PerspexXamlLoader.Load(this);
        }

        /// <summary>
        /// Attach development tool in debug mode.
        /// </summary>
        /// <param name="window"></param>
        public static void AttachDevTools(Window window)
        {
#if DEBUG
            DevTools.Attach(window);
#endif
        }

        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args">The program arguments.</param>
        private static void Main(string[] args)
        {
            new App().Start();
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
                Renderers = new ShapeRenderer[] { new PerspexRenderer() },
                ProjectFactory = new ProjectFactory(),
                TextClipboard = new PerspexTextClipboard(),
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
        public async Task OnOpenAsync(string path)
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
        public async Task OnSaveAsync()
        {
            try
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
            catch (Exception ex)
            {
                _editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <inheritdoc/>
        public async Task OnSaveAsAsync()
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
        public async Task OnImportXamlAsync(string path)
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
        public async Task OnExportAsync(object item)
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
        public async Task OnImportDataAsync()
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
        public async Task OnExportDataAsync()
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
        public async Task OnUpdateDataAsync()
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
        public async Task OnImportObjectAsync(object item, CoreType type)
        {
            try
            {
                if (item != null)
                {
                    string name = string.Empty;
                    string ext = string.Empty;

                    switch (type)
                    {
                        case CoreType.Style:
                            name = "Style";
                            ext = "style";
                            break;
                        case CoreType.Styles:
                            name = "Styles";
                            ext = "styles";
                            break;
                        case CoreType.StyleLibrary:
                            name = "StyleLibrary";
                            ext = "stylelibrary";
                            break;
                        case CoreType.StyleLibraries:
                            name = "StyleLibraries";
                            ext = "stylelibraries";
                            break;
                        case CoreType.Group:
                            name = "Group";
                            ext = "group";
                            break;
                        case CoreType.Groups:
                            name = "Groups";
                            ext = "groups";
                            break;
                        case CoreType.GroupLibrary:
                            name = "GroupLibrary";
                            ext = "grouplibrary";
                            break;
                        case CoreType.GroupLibraries:
                            name = "GroupLibraries";
                            ext = "grouplibraries";
                            break;
                        case CoreType.Template:
                            name = "Template";
                            ext = "template";
                            break;
                        case CoreType.Templates:
                            name = "Templates";
                            ext = "templates";
                            break;
                    }

                    var dlg = new OpenFileDialog();
                    dlg.AllowMultiple = true;
                    dlg.Filters.Add(new FileDialogFilter() { Name = name, Extensions = { ext } });
                    dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                    var results = await dlg.ShowAsync(_mainWindow);
                    if (results != null)
                    {
                        foreach (var path in results)
                        {
                            _editor?.OnImportObject(path, item, type);
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
        public async Task OnExportObjectAsync(object item, CoreType type)
        {
            try
            {
                if (item != null)
                {
                    string initial = string.Empty;
                    string name = string.Empty;
                    string extension = string.Empty;

                    switch (type)
                    {
                        case CoreType.Style:
                            name = "Style";
                            extension = "style";
                            initial = (item as ShapeStyle).Name;
                            break;
                        case CoreType.Styles:
                            name = "Styles";
                            extension = "styles";
                            initial = (item as XLibrary<ShapeStyle>).Name;
                            break;
                        case CoreType.StyleLibrary:
                            name = "StyleLibrary";
                            extension = "stylelibrary";
                            initial = (item as XLibrary<ShapeStyle>).Name;
                            break;
                        case CoreType.StyleLibraries:
                            name = "StyleLibraries";
                            extension = "stylelibraries";
                            initial = "StyleLibraries";
                            break;
                        case CoreType.Group:
                            name = "Group";
                            extension = "group";
                            initial = (item as XGroup).Name;
                            break;
                        case CoreType.Groups:
                            name = "Groups";
                            extension = "groups";
                            initial = (item as XLibrary<XGroup>).Name;
                            break;
                        case CoreType.GroupLibrary:
                            name = "GroupLibrary";
                            extension = "grouplibrary";
                            initial = (item as XLibrary<XGroup>).Name;
                            break;
                        case CoreType.GroupLibraries:
                            name = "GroupLibraries";
                            extension = "grouplibraries";
                            initial = "GroupLibraries";
                            break;
                        case CoreType.Template:
                            name = "Template";
                            extension = "template";
                            initial = (item as XTemplate).Name;
                            break;
                        case CoreType.Templates:
                            name = "Templates";
                            extension = "templates";
                            initial = "Templates";
                            break;
                    }

                    var dlg = new SaveFileDialog();
                    dlg.Filters.Add(new FileDialogFilter() { Name = name, Extensions = { extension } });
                    dlg.Filters.Add(new FileDialogFilter() { Name = name, Extensions = { "xaml" } });
                    dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                    dlg.InitialFileName = initial;
                    var result = await dlg.ShowAsync(_mainWindow);
                    if (result != null)
                    {
                        string resultExtension = System.IO.Path.GetExtension(result);
                        if (string.Compare(resultExtension, ".xaml", true) == 0)
                        {
                            _editor?.OnExportXaml(result, item);
                        }
                        else
                        {
                            _editor?.OnExportObject(result, item, type);
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
        public async Task OnCopyAsEmfAsync()
        {
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        public async Task OnExportAsEmfAsync(string path)
        {
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        public async Task OnZoomResetAsync()
        {
            _editor.ResetZoom();
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        public async Task OnZoomAutoFitAsync()
        {
            _editor.AutoFitZoom();
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        public async Task OnLoadWindowLayout()
        {
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        public async Task OnSaveWindowLayoutAsync()
        {
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        public async Task OnResetWindowLayoutAsync()
        {
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        public void OnCloseView()
        {
            _mainWindow?.Close();
        }
    }
}
