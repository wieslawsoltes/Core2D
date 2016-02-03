// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//#define SKIA_WIN
//#define SKIA_GTK
using System;
using System.Collections.Generic;
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
                if (_editor?.Log != null)
                {
                    _editor.Log.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
                else
                {
                    Trace.WriteLine($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
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
        public void InitializeEditor()
        {
            _editor = new Editor()
            {
                CurrentTool = Tool.Selection,
                CurrentPathTool = PathTool.Line,
                Renderers = new Renderer[] { new PerspexRenderer() },
                ProjectFactory = new ProjectFactory(),
                TextClipboard = new TextClipboard(),
                ProtoBufSerializer = new ProtoBufStreamSerializer(),
                JsonSerializer = new NewtonsoftTextSerializer(),
                XamlSerializer = new PortableXamlSerializer(),
                PdfWriter = new PdfWriter(),
                DxfWriter = new DxfWriter(),
                CsvReader = new CsvHelperReader(),
                CsvWriter = new CsvHelperWriter()
            };

            _editor.Log = new TraceLog();
            _editor.Log.Initialize(System.IO.Path.Combine(GetAssemblyPath(), _logFileName));

            _editor.FileIO = new FileSystem();

            _editor.Renderers[0].State.EnableAutofit = true;
            _editor.Renderers[0].State.DrawShapeState.Flags = ShapeStateFlags.Visible;

            _editor.GetImageKey = async () => await OnGetImageKey();

            _editor.DefaultTools();

            _editor.InitializeCommands();
            InitializeCommands(_editor);
            Commands.Register();
        }

        /// <summary>
        /// Initialize platform specific commands used by <see cref="Editor"/>.
        /// </summary>
        /// <param name="editor">The editor instance.</param>
        private void InitializeCommands(Editor editor)
        {
            Commands.OpenCommand =
                Command<string>.Create(
                    async (path) => await OnOpen(path),
                    (path) => editor.IsEditMode());

            Commands.SaveCommand =
                Command.Create(
                    async () => await OnSave(),
                    () => editor.IsEditMode());

            Commands.SaveAsCommand =
                Command.Create(
                    async () => await OnSaveAs(),
                    () => editor.IsEditMode());

            Commands.ImportXamlCommand =
                Command<string>.Create(
                    async (path) => await OnImportXaml(path),
                    (path) => editor.IsEditMode());

            Commands.ExportCommand =
                Command<object>.Create(
                    async (item) => await OnExport(item),
                    (item) => editor.IsEditMode());

            Commands.ImportDataCommand =
                Command<Project>.Create(
                    async (project) => await OnImportData(),
                    (project) => editor.IsEditMode());

            Commands.ExportDataCommand =
                Command<Database>.Create(
                    async (db) => await OnExportData(),
                    (db) => editor.IsEditMode());

            Commands.UpdateDataCommand =
                Command<Database>.Create(
                    async (db) => await OnUpdateData(),
                    (db) => editor.IsEditMode());

            Commands.ImportStyleCommand =
                Command<Library<ShapeStyle>>.Create(
                    async (item) => await OnImportObject(item, CoreType.Style),
                    (item) => editor.IsEditMode());

            Commands.ImportStylesCommand =
                Command<Library<ShapeStyle>>.Create(
                    async (item) => await OnImportObject(item, CoreType.Styles),
                    (item) => editor.IsEditMode());

            Commands.ImportStyleLibraryCommand =
                Command<Project>.Create(
                    async (item) => await OnImportObject(item, CoreType.StyleLibrary),
                    (item) => editor.IsEditMode());

            Commands.ImportStyleLibrariesCommand =
                Command<Project>.Create(
                    async (item) => await OnImportObject(item, CoreType.StyleLibraries),
                    (item) => editor.IsEditMode());

            Commands.ImportGroupCommand =
                Command<Library<XGroup>>.Create(
                    async (item) => await OnImportObject(item, CoreType.Group),
                    (item) => editor.IsEditMode());

            Commands.ImportGroupsCommand =
                Command<Library<XGroup>>.Create(
                    async (item) => await OnImportObject(item, CoreType.Groups),
                    (item) => editor.IsEditMode());

            Commands.ImportGroupLibraryCommand =
                Command<Project>.Create(
                    async (item) => await OnImportObject(item, CoreType.GroupLibrary),
                    (item) => editor.IsEditMode());

            Commands.ImportGroupLibrariesCommand =
                Command<Project>.Create(
                    async (item) => await OnImportObject(item, CoreType.GroupLibraries),
                    (item) => editor.IsEditMode());

            Commands.ImportTemplateCommand =
                Command<Project>.Create(
                    async (item) => await OnImportObject(item, CoreType.Template),
                    (item) => editor.IsEditMode());

            Commands.ImportTemplatesCommand =
                Command<Project>.Create(
                    async (item) => await OnImportObject(item, CoreType.Templates),
                    (item) => editor.IsEditMode());

            Commands.ExportStyleCommand =
                Command<ShapeStyle>.Create(
                    async (item) => await OnExportObject(item, CoreType.Style),
                    (item) => editor.IsEditMode());

            Commands.ExportStylesCommand =
                Command<Library<ShapeStyle>>.Create(
                    async (item) => await OnExportObject(item, CoreType.Styles),
                    (item) => editor.IsEditMode());

            Commands.ExportStyleLibraryCommand =
                Command<Library<ShapeStyle>>.Create(
                    async (item) => await OnExportObject(item, CoreType.StyleLibrary),
                    (item) => editor.IsEditMode());

            Commands.ExportStyleLibrariesCommand =
                Command<IEnumerable<Library<ShapeStyle>>>.Create(
                    async (item) => await OnExportObject(item, CoreType.StyleLibraries),
                    (item) => editor.IsEditMode());

            Commands.ExportGroupCommand =
                Command<XGroup>.Create(
                    async (item) => await OnExportObject(item, CoreType.Group),
                    (item) => editor.IsEditMode());

            Commands.ExportGroupsCommand =
                Command<Library<XGroup>>.Create(
                    async (item) => await OnExportObject(item, CoreType.Groups),
                    (item) => editor.IsEditMode());

            Commands.ExportGroupLibraryCommand =
                Command<Library<XGroup>>.Create(
                    async (item) => await OnExportObject(item, CoreType.GroupLibrary),
                    (item) => editor.IsEditMode());

            Commands.ExportGroupLibrariesCommand =
                Command<IEnumerable<Library<XGroup>>>.Create(
                    async (item) => await OnExportObject(item, CoreType.GroupLibraries),
                    (item) => editor.IsEditMode());

            Commands.ExportTemplateCommand =
                Command<Template>.Create(
                    async (item) => await OnExportObject(item, CoreType.Template),
                    (item) => editor.IsEditMode());

            Commands.ExportTemplatesCommand =
                Command<IEnumerable<Template>>.Create(
                    async (item) => await OnExportObject(item, CoreType.Templates),
                    (item) => editor.IsEditMode());

            Commands.ZoomResetCommand =
                Command.Create(
                    () => editor.ResetZoom(),
                    () => true);

            Commands.ZoomExtentCommand =
                Command.Create(
                    () => editor.ExtentZoom(),
                    () => true);

            Commands.LoadWindowLayoutCommand =
                Command.Create(
                    () => { },
                    () => true);

            Commands.SaveWindowLayoutCommand =
                Command.Create(
                    () => { },
                    () => true);

            Commands.ResetWindowLayoutCommand =
                Command.Create(
                    () => { },
                    () => true);
        }

        /// <summary>
        /// De-initialize <see cref="Editor"/> object.
        /// </summary>
        public void DeInitializeEditor()
        {
            _editor?.Dispose();
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
                        _editor?.Open(path);
                        _editor?.Invalidate?.Invoke();
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
                _editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
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
                if (!string.IsNullOrEmpty(_editor?.ProjectPath))
                {
                    _editor?.Save(_editor?.ProjectPath);
                }
                else
                {
                    await OnSaveAs();
                }
            }
            catch (Exception ex)
            {
                _editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
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

        /// <summary>
        /// Import Xaml from file.
        /// </summary>
        /// <param name="parameter">The Xaml file path.</param>
        /// <returns>The await <see cref="Task"/>.</returns>
        private async Task OnImportXaml(string parameter)
        {
            try
            {
                if (parameter == null)
                {
                    var dlg = new OpenFileDialog();
                    dlg.AllowMultiple = true;
                    dlg.Filters.Add(new FileDialogFilter() { Name = "Xaml", Extensions = { "xaml" } });
                    dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });

                    var results = await dlg.ShowAsync(_mainWindow);
                    if (results != null)
                    {
                        foreach (var path in results)
                        {
                            _editor?.OnImportXaml(path);
                        }
                    }
                }
                else
                {
                    string path = parameter;
                    if (path != null && System.IO.File.Exists(path))
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

        /// <summary>
        /// Import records into new database.
        /// </summary>
        /// <returns>The await <see cref="Task"/>.</returns>
        private async Task OnImportData()
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

        /// <summary>
        /// Export records to external file.
        /// </summary>
        /// <returns>The await <see cref="Task"/>.</returns>
        private async Task OnExportData()
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

        /// <summary>
        /// Update records in current database.
        /// </summary>
        /// <returns>The await <see cref="Task"/>.</returns>
        private async Task OnUpdateData()
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

        /// <summary>
        /// Import item object from external file.
        /// </summary>
        /// <param name="item">The item object to import.</param>
        /// <param name="type">The type of item object.</param>
        /// <returns>The await <see cref="Task"/>.</returns>
        private async Task OnImportObject(object item, CoreType type)
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

        /// <summary>
        /// Export item object to external file.
        /// </summary>
        /// <param name="item">The item object to export.</param>
        /// <param name="type">The type of item object.</param>
        /// <returns>The await <see cref="Task"/>.</returns>
        private async Task OnExportObject(object item, CoreType type)
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
                            initial = (item as Library<ShapeStyle>).Name;
                            break;
                        case CoreType.StyleLibrary:
                            name = "StyleLibrary";
                            extension = "stylelibrary";
                            initial = (item as Library<ShapeStyle>).Name;
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
                            initial = (item as Library<XGroup>).Name;
                            break;
                        case CoreType.GroupLibrary:
                            name = "GroupLibrary";
                            extension = "grouplibrary";
                            initial = (item as Library<XGroup>).Name;
                            break;
                        case CoreType.GroupLibraries:
                            name = "GroupLibraries";
                            extension = "grouplibraries";
                            initial = "GroupLibraries";
                            break;
                        case CoreType.Template:
                            name = "Template";
                            extension = "template";
                            initial = (item as Template).Name;
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
    }
}
