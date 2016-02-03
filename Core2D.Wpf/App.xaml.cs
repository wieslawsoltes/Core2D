// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using Dependencies;

namespace Core2D.Wpf
{
    /// <summary>
    /// Encapsulates a WPF application.
    /// </summary>
    public partial class App : Application
    {
        private Editor _editor;
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
            InitializeEditor();
            LoadRecent();

            _mainWindow = new Windows.MainWindow();

            _mainWindow.InitializeMouse(_editor);
            _mainWindow.InitializeZoom(_editor);
            _mainWindow.InitializeDrop(_editor);

            _mainWindow.Loaded +=
                (sender, e) =>
                {
                    if (_isLoaded)
                        return;
                    else
                        _isLoaded = true;

                    if (_restoreLayout)
                    {
                        _mainWindow.AutoLoadLayout(_editor);
                    }
                };

            _mainWindow.Unloaded += (sender, e) => { };

            _mainWindow.Closed += (sender, e) =>
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

                DeInitializeEditor();
            };

            _editor.View = _mainWindow;

            _mainWindow.DataContext = _editor;

            try
            {
                _mainWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                _editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
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
        private void InitializeEditor()
        {
            _editor = new Editor()
            {
                CurrentTool = Tool.Selection,
                CurrentPathTool = PathTool.Line,
                Renderers = new Renderer[] { new WpfRenderer() },
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
                    (path) => OnOpen(path),
                    (path) => editor.IsEditMode());

            Commands.SaveCommand =
                Command.Create(
                    () => OnSave(),
                    () => editor.IsEditMode());

            Commands.SaveAsCommand =
                Command.Create(
                    () => OnSaveAs(),
                    () => editor.IsEditMode());

            Commands.ImportXamlCommand =
                Command<string>.Create(
                    (path) => OnImportXaml(path),
                    (path) => editor.IsEditMode());

            Commands.ExportCommand =
                Command<object>.Create(
                    (item) => OnExport(item),
                    (item) => editor.IsEditMode());

            Commands.ImportDataCommand =
                Command<Project>.Create(
                    (project) => OnImportData(),
                    (project) => editor.IsEditMode());

            Commands.ExportDataCommand =
                Command<Database>.Create(
                    (db) => OnExportData(),
                    (db) => editor.IsEditMode());

            Commands.UpdateDataCommand =
                Command<Database>.Create(
                    (db) => OnUpdateData(),
                    (db) => editor.IsEditMode());

            Commands.ImportStyleCommand =
                Command<Library<ShapeStyle>>.Create(
                    (item) => OnImportObject(item, CoreType.Style),
                    (item) => editor.IsEditMode());

            Commands.ImportStylesCommand =
                Command<Library<ShapeStyle>>.Create(
                    (item) => OnImportObject(item, CoreType.Styles),
                    (item) => editor.IsEditMode());

            Commands.ImportStyleLibraryCommand =
                Command<Project>.Create(
                    (item) => OnImportObject(item, CoreType.StyleLibrary),
                    (item) => editor.IsEditMode());

            Commands.ImportStyleLibrariesCommand =
                Command<Project>.Create(
                    (item) => OnImportObject(item, CoreType.StyleLibraries),
                    (item) => editor.IsEditMode());

            Commands.ImportGroupCommand =
                Command<Library<XGroup>>.Create(
                    (item) => OnImportObject(item, CoreType.Group),
                    (item) => editor.IsEditMode());

            Commands.ImportGroupsCommand =
                Command<Library<XGroup>>.Create(
                    (item) => OnImportObject(item, CoreType.Groups),
                    (item) => editor.IsEditMode());

            Commands.ImportGroupLibraryCommand =
                Command<Project>.Create(
                    (item) => OnImportObject(item, CoreType.GroupLibrary),
                    (item) => editor.IsEditMode());

            Commands.ImportGroupLibrariesCommand =
                Command<Project>.Create(
                    (item) => OnImportObject(item, CoreType.GroupLibraries),
                    (item) => editor.IsEditMode());

            Commands.ImportTemplateCommand =
                Command<Project>.Create(
                    (item) => OnImportObject(item, CoreType.Template),
                    (item) => editor.IsEditMode());

            Commands.ImportTemplatesCommand =
                Command<Project>.Create(
                    (item) => OnImportObject(item, CoreType.Templates),
                    (item) => editor.IsEditMode());

            Commands.ExportStyleCommand =
                Command<ShapeStyle>.Create(
                    (item) => OnExportObject(item, CoreType.Style),
                    (item) => editor.IsEditMode());

            Commands.ExportStylesCommand =
                Command<Library<ShapeStyle>>.Create(
                    (item) => OnExportObject(item, CoreType.Styles),
                    (item) => editor.IsEditMode());

            Commands.ExportStyleLibraryCommand =
                Command<Library<ShapeStyle>>.Create(
                    (item) => OnExportObject(item, CoreType.StyleLibrary),
                    (item) => editor.IsEditMode());

            Commands.ExportStyleLibrariesCommand =
                Command<IEnumerable<Library<ShapeStyle>>>.Create(
                    (item) => OnExportObject(item, CoreType.StyleLibraries),
                    (item) => editor.IsEditMode());

            Commands.ExportGroupCommand =
                Command<XGroup>.Create(
                    (item) => OnExportObject(item, CoreType.Group),
                    (item) => editor.IsEditMode());

            Commands.ExportGroupsCommand =
                Command<Library<XGroup>>.Create(
                    (item) => OnExportObject(item, CoreType.Groups),
                    (item) => editor.IsEditMode());

            Commands.ExportGroupLibraryCommand =
                Command<Library<XGroup>>.Create(
                    (item) => OnExportObject(item, CoreType.GroupLibrary),
                    (item) => editor.IsEditMode());

            Commands.ExportGroupLibrariesCommand =
                Command<IEnumerable<Library<XGroup>>>.Create(
                    (item) => OnExportObject(item, CoreType.GroupLibraries),
                    (item) => editor.IsEditMode());

            Commands.ExportTemplateCommand =
                Command<Template>.Create(
                    (item) => OnExportObject(item, CoreType.Template),
                    (item) => editor.IsEditMode());

            Commands.ExportTemplatesCommand =
                Command<IEnumerable<Template>>.Create(
                    (item) => OnExportObject(item, CoreType.Templates),
                    (item) => editor.IsEditMode());

            Commands.CopyAsEmfCommand =
                Command.Create(
                    () => OnCopyAsEmf(),
                    () => editor.IsEditMode());

            Commands.ZoomResetCommand =
                Command.Create(
                    () => _mainWindow.OnZoomReset(),
                    () => true);

            Commands.ZoomExtentCommand =
                Command.Create(
                    () => _mainWindow.OnZoomExtent(),
                    () => true);

            Commands.LoadWindowLayoutCommand =
                Command.Create(
                    () => _mainWindow.OnLoadLayout(),
                    () => true);

            Commands.SaveWindowLayoutCommand =
                Command.Create(
                    () => _mainWindow.OnSaveLayout(),
                    () => true);

            Commands.ResetWindowLayoutCommand =
                Command.Create(
                    () => _mainWindow.OnResetLayout(),
                    () => true);
        }

        /// <summary>
        /// De-initialize <see cref="Editor"/> object.
        /// </summary>
        private void DeInitializeEditor()
        {
            _editor?.Dispose();
        }

        /// <summary>
        /// Get the <see cref="XImage"/> key from file path.
        /// </summary>
        /// <returns>The image key.</returns>
        private async Task<string> OnGetImageKey()
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

        /// <summary>
        /// Open <see cref="Project"/> from file.
        /// </summary>
        /// <param name="path"></param>
        private void OnOpen(string path)
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
                if (path != null && System.IO.File.Exists(path))
                {
                    _editor?.Open(path);
                }
            }
        }

        /// <summary>
        /// Save current <see cref="Project"/> to external file.
        /// </summary>
        private void OnSave()
        {
            if (!string.IsNullOrEmpty(_editor?.ProjectPath))
            {
                _editor?.Save(_editor?.ProjectPath);
            }
            else
            {
                OnSaveAs();
            }
        }

        /// <summary>
        /// Save current <see cref="Project"/> to external file.
        /// </summary>
        private void OnSaveAs()
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
        }

        /// <summary>
        /// Import Xaml from file.
        /// </summary>
        /// <param name="parameter">The Xaml file path.</param>
        private void OnImportXaml(string parameter)
        {
            if (parameter == null)
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
                    var paths = dlg.FileNames;

                    foreach (var path in paths)
                    {
                        _editor?.OnImportXaml(path);
                    }
                }
            }
            else
            {
                if (parameter != null && System.IO.File.Exists(parameter))
                {
                    _editor?.OnImportXaml(parameter);
                }
            }
        }

        /// <summary>
        /// Export item object to external file.
        /// </summary>
        /// <param name="item">The item object to export.</param>
        private void OnExport(object item)
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
                        OnExportAsEmf(dlg.FileName);
                        break;
                    case 3:
                        _editor?.ExportAsDxf(dlg.FileName, item);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Import records into new database.
        /// </summary>
        private void OnImportData()
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
        }

        /// <summary>
        ///  Export records to external file.
        /// </summary>
        private void OnExportData()
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
        }

        /// <summary>
        /// Update records in current database.
        /// </summary>
        private void OnUpdateData()
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
        }

        /// <summary>
        /// Import item object from external file.
        /// </summary>
        /// <param name="item">The item object to import.</param>
        /// <param name="type">The type of item object.</param>
        private void OnImportObject(object item, CoreType type)
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
        }

        /// <summary>
        /// Export item object to external file.
        /// </summary>
        /// <param name="item">The item object to export.</param>
        /// <param name="type">The type of item object.</param>
        private void OnExportObject(object item, CoreType type)
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
                    name = (item as Library<ShapeStyle>).Name;
                    break;
                case CoreType.StyleLibrary:
                    filter = "StyleLibrary (*.stylelibrary)|*.stylelibrary|StyleLibrary (*.xaml)|*.xaml|All (*.*)|*.*";
                    name = (item as Library<ShapeStyle>).Name;
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
                    name = (item as Library<XGroup>).Name;
                    break;
                case CoreType.GroupLibrary:
                    filter = "GroupLibrary (*.grouplibrary)|*.grouplibrary|GroupLibrary (*.xaml)|*.xaml|All (*.*)|*.*";
                    name = (item as Library<XGroup>).Name;
                    break;
                case CoreType.GroupLibraries:
                    filter = "GroupLibraries (*.grouplibraries)|*.grouplibraries|GroupLibraries (*.xaml)|*.xaml|All (*.*)|*.*";
                    name = "GroupLibraries";
                    break;
                case CoreType.Template:
                    filter = "Template (*.template)|*.template|Template (*.xaml)|*.xaml|All (*.*)|*.*";
                    name = (item as Template).Name;
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
        }

        /// <summary>
        /// Copy currently selected shapes to clipboard as enhanced metafile.
        /// </summary>
        private void OnCopyAsEmf()
        {
            var page = _editor?.Project?.CurrentContainer as Page;
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
        }

        /// <summary>
        /// Save currently selected shapes as enhanced metafile.
        /// </summary>
        /// <param name="path">The file path.</param>
        private void OnExportAsEmf(string path)
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
        }
    }
}
