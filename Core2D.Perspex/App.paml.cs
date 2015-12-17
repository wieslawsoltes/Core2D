// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//#define SKIA_WIN
//#define SKIA_GTK
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Perspex;
using Perspex.Controls;
using Perspex.Diagnostics;
using Perspex.Markup.Xaml;
using Perspex.Themes.Default;
#if SKIA_WIN
using Perspex.Win32;
using Perspex.Skia;
#endif
#if SKIA_GTK
using Perspex.Gtk;
using Perspex.Skia;
#endif
using Dependencies;

namespace Core2D.Perspex
{
    /// <summary>
    /// Encapsulates a Core2D Prespex application.
    /// </summary>
    public class App : Application
    {
        private Editor _editor;
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
                new TextClipboard(),
                new NewtonsoftSerializer());
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
            Styles = new DefaultTheme();
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
            try
            {
                InitializeEditor();

                LoadRecent();

                _mainWindow = new Windows.MainWindow();

                _mainWindow.Closed +=
                    (sender, e) =>
                    {
                        SaveRecent();
                        DeInitializeEditor();
                    };

                _editor.View = _mainWindow;

                _mainWindow.DataContext = _editor;
                _mainWindow.Show();

                Run(_mainWindow);
            }
            catch (Exception ex)
            {
                if (_editor != null && _editor != null && _editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
                else
                {
                    Trace.WriteLine(string.Format("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace));
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
                    if (_editor.Log != null)
                    {
                        _editor.Log.LogError("{0}{1}{2}",
                            ex.Message,
                            Environment.NewLine,
                            ex.StackTrace);
                    }
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
                    if (_editor.Log != null)
                    {
                        _editor.Log.LogError("{0}{1}{2}",
                            ex.Message,
                            Environment.NewLine,
                            ex.StackTrace);
                    }
                }
            }
        }

        /// <summary>
        /// Initialize <see cref="Editor"/> object.
        /// </summary>
        public void InitializeEditor()
        {
            _editor = new Editor()
            {
                CurrentTool = Tool.Selection,
                CurrentPathTool = PathTool.Line,
                Renderers = new Renderer[] { new PerspexRenderer() },
                ProjectFactory = new ProjectFactory(),
                TextClipboard = new TextClipboard(),
                Serializer = new NewtonsoftSerializer(),
                PdfWriter = new PdfWriter(),
                DxfWriter = new DxfWriter(),
                CsvReader = new CsvHelperReader(),
                CsvWriter = new CsvHelperWriter()
            };

            _editor.Log = new TraceLog();
            _editor.Log.Initialize(System.IO.Path.Combine(GetAssemblyPath(), _logFileName));

            _editor.Renderers[0].State.EnableAutofit = true;
            _editor.Renderers[0].State.DrawShapeState.Flags = ShapeStateFlags.Visible;

            _editor.GetImageKey = async () => await OnGetImageKey();

            _editor.DefaultTools();

            Commands.InitializeCommonCommands(_editor);
            InitializePlatformCommands(_editor);
        }

        /// <summary>
        /// Initialize platform commands used by <see cref="Editor"/>.
        /// </summary>
        /// <param name="editor">The editor instance.</param>
        private void InitializePlatformCommands(Editor editor)
        {
            Commands.OpenCommand =
                Command<object>.Create(
                    async (parameter) => await OnOpen(parameter),
                    (parameter) => editor.IsEditMode());

            Commands.SaveCommand =
                Command.Create(
                    async () => await OnSave(),
                    () => editor.IsEditMode());

            Commands.SaveAsCommand =
                Command.Create(
                    async () => await OnSaveAs(),
                    () => editor.IsEditMode());

            Commands.ExportCommand =
                Command<object>.Create(
                    async (item) => await OnExport(item),
                    (item) => editor.IsEditMode());

            Commands.ImportDataCommand =
                Command<object>.Create(
                    async (item) => await OnImportData(),
                    (item) => editor.IsEditMode());

            Commands.ExportDataCommand =
                Command<object>.Create(
                    async (item) => await OnExportData(),
                    (item) => editor.IsEditMode());

            Commands.UpdateDataCommand =
                Command<object>.Create(
                    async (item) => await OnUpdateData(),
                    (item) => editor.IsEditMode());

            Commands.ImportStyleCommand =
                Command<object>.Create(
                    async (item) => await OnImportObject(item, ImportType.Style),
                    (item) => editor.IsEditMode());

            Commands.ImportStylesCommand =
                Command<object>.Create(
                    async (item) => await OnImportObject(item, ImportType.Styles),
                    (item) => editor.IsEditMode());

            Commands.ImportStyleLibraryCommand =
                Command<object>.Create(
                    async (item) => await OnImportObject(item, ImportType.StyleLibrary),
                    (item) => editor.IsEditMode());

            Commands.ImportStyleLibrariesCommand =
                Command<object>.Create(
                    async (item) => await OnImportObject(item, ImportType.StyleLibraries),
                    (item) => editor.IsEditMode());

            Commands.ImportGroupCommand =
                Command<object>.Create(
                    async (item) => await OnImportObject(item, ImportType.Group),
                    (item) => editor.IsEditMode());

            Commands.ImportGroupsCommand =
                Command<object>.Create(
                    async (item) => await OnImportObject(item, ImportType.Groups),
                    (item) => editor.IsEditMode());

            Commands.ImportGroupLibraryCommand =
                Command<object>.Create(
                    async (item) => await OnImportObject(item, ImportType.GroupLibrary),
                    (item) => editor.IsEditMode());

            Commands.ImportGroupLibrariesCommand =
                Command<object>.Create(
                    async (item) => await OnImportObject(item, ImportType.GroupLibraries),
                    (item) => editor.IsEditMode());

            Commands.ImportTemplateCommand =
                Command<object>.Create(
                    async (item) => await OnImportObject(item, ImportType.Template),
                    (item) => editor.IsEditMode());

            Commands.ImportTemplatesCommand =
                Command<object>.Create(
                    async (item) => await OnImportObject(item, ImportType.Templates),
                    (item) => editor.IsEditMode());

            Commands.ExportStyleCommand =
                Command<object>.Create(
                    async (item) => await OnExportObject(item, ExportType.Style),
                    (item) => editor.IsEditMode());

            Commands.ExportStylesCommand =
                Command<object>.Create(
                    async (item) => await OnExportObject(item, ExportType.Styles),
                    (item) => editor.IsEditMode());

            Commands.ExportStyleLibraryCommand =
                Command<object>.Create(
                    async (item) => await OnExportObject(item, ExportType.StyleLibrary),
                    (item) => editor.IsEditMode());

            Commands.ExportStyleLibrariesCommand =
                Command<object>.Create(
                    async (item) => await OnExportObject(item, ExportType.StyleLibraries),
                    (item) => editor.IsEditMode());

            Commands.ExportGroupCommand =
                Command<object>.Create(
                    async (item) => await OnExportObject(item, ExportType.Group),
                    (item) => editor.IsEditMode());

            Commands.ExportGroupsCommand =
                Command<object>.Create(
                    async (item) => await OnExportObject(item, ExportType.Groups),
                    (item) => editor.IsEditMode());

            Commands.ExportGroupLibraryCommand =
                Command<object>.Create(
                    async (item) => await OnExportObject(item, ExportType.GroupLibrary),
                    (item) => editor.IsEditMode());

            Commands.ExportGroupLibrariesCommand =
                Command<object>.Create(
                    async (item) => await OnExportObject(item, ExportType.GroupLibraries),
                    (item) => editor.IsEditMode());

            Commands.ExportTemplateCommand =
                Command<object>.Create(
                    async (item) => await OnExportObject(item, ExportType.Template),
                    (item) => editor.IsEditMode());

            Commands.ExportTemplatesCommand =
                Command<object>.Create(
                    async (item) => await OnExportObject(item, ExportType.Templates),
                    (item) => editor.IsEditMode());

            Commands.ZoomResetCommand =
                Command.Create(
                    () => editor.ResetZoom(),
                    () => true);

            Commands.ZoomExtentCommand =
                Command.Create(
                    () => editor.ExtentZoom(),
                    () => true);
        }

        /// <summary>
        /// De-initialize <see cref="Editor"/> object.
        /// </summary>
        public void DeInitializeEditor()
        {
            _editor.Dispose();
        }

        /// <summary>
        /// Get the <see cref="XImage"/> key from file path.
        /// </summary>
        /// <returns>The image key.</returns>
        private async Task<string> OnGetImageKey()
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
                    var key = _editor.Project.AddImageFromFile(path, bytes);
                    return key;
                }
                return null;
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
                return null;
            }
        }

        /// <summary>
        /// Open <see cref="Project"/> from file.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>The await <see cref="Task"/>.</returns>
        private async Task OnOpen(object parameter)
        {
            try
            {
                if (parameter == null)
                {
                    var dlg = new OpenFileDialog();
                    dlg.Filters.Add(new FileDialogFilter() { Name = "Project", Extensions = { "project" } });
                    dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                    var result = await dlg.ShowAsync(_mainWindow);
                    if (result != null)
                    {
                        var path = result.FirstOrDefault();
                        _editor.Open(path);
                        _editor.Invalidate();
                    }
                }
                else
                {
                    string path = parameter as string;
                    if (path != null && System.IO.File.Exists(path))
                    {
                        _editor.Open(path);
                    }
                }
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Save current <see cref="Project"/> to external file.
        /// </summary>
        /// <returns>The await <see cref="Task"/>.</returns>
        private async Task OnSave()
        {
            try
            {
                if (!string.IsNullOrEmpty(_editor.ProjectPath))
                {
                    _editor.Save(_editor.ProjectPath);
                }
                else
                {
                    await OnSaveAs();
                }
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Save current <see cref="Project"/> to external file.
        /// </summary>
        /// <returns>The await <see cref="Task"/>.</returns>
        private async Task OnSaveAs()
        {
            try
            {
                if (_editor.Project != null)
                {
                    var dlg = new SaveFileDialog();
                    dlg.Filters.Add(new FileDialogFilter() { Name = "Project", Extensions = { "project" } });
                    dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                    dlg.InitialFileName = _editor.Project.Name;
                    var result = await dlg.ShowAsync(_mainWindow);
                    if (result != null)
                    {
                        _editor.Save(result);
                    }
                }
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Export item object to external file.
        /// </summary>
        /// <param name="item">The item object to export.</param>
        /// <returns>The await <see cref="Task"/>.</returns>
        private async Task OnExport(object item)
        {
            try
            {
                string name = string.Empty;

                if (item is Container)
                {
                    name = (item as Container).Name;
                }
                else if (item is Document)
                {
                    name = (item as Document).Name;
                }
                else if (item is Project)
                {
                    name = (item as Project).Name;
                }
                else if (item is Editor)
                {
                    var editor = (item as Editor);
                    if (editor.Project == null)
                        return;

                    name = editor.Project.Name;
                    item = editor.Project;
                }
                else if (item == null)
                {
                    if (_editor.Project == null)
                        return;

                    name = _editor.Project.Name;
                    item = _editor.Project;
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
                        _editor.ExportAsPdf(result, item);
                    }

                    if (ext == ".dxf")
                    {
                        _editor.ExportAsDxf(result);
                    }
                }
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Import records into new database.
        /// </summary>
        /// <returns>The await <see cref="Task"/>.</returns>
        private async Task OnImportData()
        {
            try
            {
                if (_editor.Project != null)
                {
                    var dlg = new OpenFileDialog();
                    dlg.Filters.Add(new FileDialogFilter() { Name = "Csv", Extensions = { "csv" } });
                    dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                    var result = await dlg.ShowAsync(_mainWindow);
                    if (result != null)
                    {
                        var path = result.FirstOrDefault();
                        _editor.ImportData(path);
                    }
                }
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Export records to external file.
        /// </summary>
        /// <returns>The await <see cref="Task"/>.</returns>
        private async Task OnExportData()
        {
            try
            {
                if (_editor.Project != null && _editor.Project.CurrentDatabase != null)
                {
                    var database = _editor.Project.CurrentDatabase;

                    var dlg = new SaveFileDialog();
                    dlg.Filters.Add(new FileDialogFilter() { Name = "Csv", Extensions = { "csv" } });
                    dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                    dlg.InitialFileName = database.Name;
                    var result = await dlg.ShowAsync(_mainWindow);
                    if (result != null)
                    {
                        _editor.ExportData(result, database);
                    }
                }
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Update records in current database.
        /// </summary>
        /// <returns>The await <see cref="Task"/>.</returns>
        private async Task OnUpdateData()
        {
            try
            {
                if (_editor.Project != null && _editor.Project.CurrentDatabase != null)
                {
                    var database = _editor.Project.CurrentDatabase;

                    var dlg = new OpenFileDialog();
                    dlg.Filters.Add(new FileDialogFilter() { Name = "Csv", Extensions = { "csv" } });
                    dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                    var result = await dlg.ShowAsync(_mainWindow);
                    if (result != null)
                    {
                        var path = result.FirstOrDefault();
                        _editor.UpdateData(path, database);
                    }
                }
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Import item object from external file.
        /// </summary>
        /// <param name="item">The item object to import.</param>
        /// <param name="type">The type of item object.</param>
        /// <returns>The await <see cref="Task"/>.</returns>
        private async Task OnImportObject(object item, ImportType type)
        {
            try
            {
                if (item != null)
                {
                    string name = string.Empty;
                    string ext = string.Empty;

                    switch (type)
                    {
                        case ImportType.Style:
                            name = "Style";
                            ext = "style";
                            break;
                        case ImportType.Styles:
                            name = "Styles";
                            ext = "styles";
                            break;
                        case ImportType.StyleLibrary:
                            name = "StyleLibrary";
                            ext = "stylelibrary";
                            break;
                        case ImportType.StyleLibraries:
                            name = "StyleLibraries";
                            ext = "stylelibraries";
                            break;
                        case ImportType.Group:
                            name = "Group";
                            ext = "group";
                            break;
                        case ImportType.Groups:
                            name = "Groups";
                            ext = "groups";
                            break;
                        case ImportType.GroupLibrary:
                            name = "GroupLibrary";
                            ext = "grouplibrary";
                            break;
                        case ImportType.GroupLibraries:
                            name = "GroupLibraries";
                            ext = "grouplibraries";
                            break;
                        case ImportType.Template:
                            name = "Template";
                            ext = "template";
                            break;
                        case ImportType.Templates:
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
                            _editor.ImportObject(path, item, type);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Export item object to external file.
        /// </summary>
        /// <param name="item">The item object to export.</param>
        /// <param name="type">The type of item object.</param>
        /// <returns>The await <see cref="Task"/>.</returns>
        private async Task OnExportObject(object item, ExportType type)
        {
            try
            {
                if (item != null)
                {
                    string initial = string.Empty;
                    string name = string.Empty;
                    string ext = string.Empty;

                    switch (type)
                    {
                        case ExportType.Style:
                            name = "Style";
                            ext = "style";
                            initial = (item as ShapeStyle).Name;
                            break;
                        case ExportType.Styles:
                            name = "Styles";
                            ext = "styles";
                            initial = (item as Library<ShapeStyle>).Name;
                            break;
                        case ExportType.StyleLibrary:
                            name = "StyleLibrary";
                            ext = "stylelibrary";
                            initial = (item as Library<ShapeStyle>).Name;
                            break;
                        case ExportType.StyleLibraries:
                            name = "StyleLibraries";
                            ext = "stylelibraries";
                            initial = (item as Project).Name;
                            break;
                        case ExportType.Group:
                            name = "Group";
                            ext = "group";
                            initial = (item as XGroup).Name;
                            break;
                        case ExportType.Groups:
                            name = "Groups";
                            ext = "groups";
                            initial = (item as Library<XGroup>).Name;
                            break;
                        case ExportType.GroupLibrary:
                            name = "GroupLibrary";
                            ext = "grouplibrary";
                            initial = (item as Library<XGroup>).Name;
                            break;
                        case ExportType.GroupLibraries:
                            name = "GroupLibraries";
                            ext = "grouplibraries";
                            initial = (item as Project).Name;
                            break;
                        case ExportType.Template:
                            name = "Template";
                            ext = "template";
                            initial = (item as Container).Name;
                            break;
                        case ExportType.Templates:
                            name = "Templates";
                            ext = "templates";
                            initial = (item as Project).Name;
                            break;
                    }

                    var dlg = new SaveFileDialog();
                    dlg.Filters.Add(new FileDialogFilter() { Name = name, Extensions = { ext } });
                    dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                    dlg.InitialFileName = initial;
                    var result = await dlg.ShowAsync(_mainWindow);
                    if (result != null)
                    {
                        _editor.ExportObject(result, item, type);
                    }
                }
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }
    }
}
