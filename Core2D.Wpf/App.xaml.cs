// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
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

            Commands.InitializeCommonCommands(_editor);
            InitializePlatformCommands(_editor);

            _mainWindow = new Windows.MainWindow();

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

                DeInitializeContext();
            };

            _editor.View = _mainWindow;

            _mainWindow.DataContext = _editor;

            try
            {
                _mainWindow.ShowDialog();
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
        private void InitializeEditor()
        {
            _editor = new Editor()
            {
                Renderers = new Renderer[] { new WpfRenderer(), new WpfRenderer() },
                ProjectFactory = new ProjectFactory(),
                TextClipboard = new TextClipboard(),
                Serializer = new NewtonsoftSerializer(),
                PdfWriter = new PdfWriter(),
                DxfWriter = new DxfWriter(),
                CsvReader = new CsvHelperReader(),
                CsvWriter = new CsvHelperWriter()
            };

            _editor.Initialize();

            _editor.Renderers[0].State.EnableAutofit = true;
            _editor.Log = new TraceLog();
            _editor.Log.Initialize(System.IO.Path.Combine(GetAssemblyPath(), _logFileName));
            _editor.Renderers[0].State.DrawShapeState.Flags = ShapeStateFlags.Visible;
            _editor.Renderers[1].State.DrawShapeState.Flags = ShapeStateFlags.Visible;
            _editor.GetImageKey = async () => await GetImageKey();
        }

        /// <summary>
        /// Initialize platform commands used by <see cref="Editor"/>.
        /// </summary>
        /// <param name="editor">The editor instance.</param>
        private void InitializePlatformCommands(Editor editor)
        {
            Commands.OpenCommand =
                Command<object>.Create(
                    (parameter) => OnOpen(parameter),
                    (parameter) => editor.IsEditMode());

            Commands.SaveCommand =
                Command.Create(
                    () => OnSave(),
                    () => editor.IsEditMode());

            Commands.SaveAsCommand =
                Command.Create(
                    () => OnSaveAs(),
                    () => editor.IsEditMode());

            Commands.ExportCommand =
                Command<object>.Create(
                    (item) => OnExport(item),
                    (item) => editor.IsEditMode());

            Commands.ImportDataCommand =
                Command<object>.Create(
                    (item) => OnImportData(),
                    (item) => editor.IsEditMode());

            Commands.ExportDataCommand =
                Command<object>.Create(
                    (item) => OnExportData(),
                    (item) => editor.IsEditMode());

            Commands.UpdateDataCommand =
                Command<object>.Create(
                    (item) => OnUpdateData(),
                    (item) => editor.IsEditMode());

            Commands.ImportStyleCommand =
                Command<object>.Create(
                    (item) => OnImportObject(item, ImportType.Style),
                    (item) => editor.IsEditMode());

            Commands.ImportStylesCommand =
                Command<object>.Create(
                    (item) => OnImportObject(item, ImportType.Styles),
                    (item) => editor.IsEditMode());

            Commands.ImportStyleLibraryCommand =
                Command<object>.Create(
                    (item) => OnImportObject(item, ImportType.StyleLibrary),
                    (item) => editor.IsEditMode());

            Commands.ImportStyleLibrariesCommand =
                Command<object>.Create(
                    (item) => OnImportObject(item, ImportType.StyleLibraries),
                    (item) => editor.IsEditMode());

            Commands.ImportGroupCommand =
                Command<object>.Create(
                    (item) => OnImportObject(item, ImportType.Group),
                    (item) => editor.IsEditMode());

            Commands.ImportGroupsCommand =
                Command<object>.Create(
                    (item) => OnImportObject(item, ImportType.Groups),
                    (item) => editor.IsEditMode());

            Commands.ImportGroupLibraryCommand =
                Command<object>.Create(
                    (item) => OnImportObject(item, ImportType.GroupLibrary),
                    (item) => editor.IsEditMode());

            Commands.ImportGroupLibrariesCommand =
                Command<object>.Create(
                    (item) => OnImportObject(item, ImportType.GroupLibraries),
                    (item) => editor.IsEditMode());

            Commands.ImportTemplateCommand =
                Command<object>.Create(
                    (item) => OnImportObject(item, ImportType.Template),
                    (item) => editor.IsEditMode());

            Commands.ImportTemplatesCommand =
                Command<object>.Create(
                    (item) => OnImportObject(item, ImportType.Templates),
                    (item) => editor.IsEditMode());

            Commands.ExportStyleCommand =
                Command<object>.Create(
                    (item) => OnExportObject(item, ExportType.Style),
                    (item) => editor.IsEditMode());

            Commands.ExportStylesCommand =
                Command<object>.Create(
                    (item) => OnExportObject(item, ExportType.Styles),
                    (item) => editor.IsEditMode());

            Commands.ExportStyleLibraryCommand =
                Command<object>.Create(
                    (item) => OnExportObject(item, ExportType.StyleLibrary),
                    (item) => editor.IsEditMode());

            Commands.ExportStyleLibrariesCommand =
                Command<object>.Create(
                    (item) => OnExportObject(item, ExportType.StyleLibraries),
                    (item) => editor.IsEditMode());

            Commands.ExportGroupCommand =
                Command<object>.Create(
                    (item) => OnExportObject(item, ExportType.Group),
                    (item) => editor.IsEditMode());

            Commands.ExportGroupsCommand =
                Command<object>.Create(
                    (item) => OnExportObject(item, ExportType.Groups),
                    (item) => editor.IsEditMode());

            Commands.ExportGroupLibraryCommand =
                Command<object>.Create(
                    (item) => OnExportObject(item, ExportType.GroupLibrary),
                    (item) => editor.IsEditMode());

            Commands.ExportGroupLibrariesCommand =
                Command<object>.Create(
                    (item) => OnExportObject(item, ExportType.GroupLibraries),
                    (item) => editor.IsEditMode());

            Commands.ExportTemplateCommand =
                Command<object>.Create(
                    (item) => OnExportObject(item, ExportType.Template),
                    (item) => editor.IsEditMode());

            Commands.ExportTemplatesCommand =
                Command<object>.Create(
                    (item) => OnExportObject(item, ExportType.Templates),
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
        private void DeInitializeContext()
        {
            _editor.Dispose();
        }

        /// <summary>
        /// Get the <see cref="XImage"/> key from file path.
        /// </summary>
        /// <returns>The image key.</returns>
        private async Task<string> GetImageKey()
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
                    var key = _editor.Project.AddImageFromFile(path, bytes);
                    return await Task.Run(() => key);
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
            return null;
        }

        /// <summary>
        /// Open <see cref="Project"/> from file.
        /// </summary>
        /// <param name="parameter"></param>
        private void OnOpen(object parameter)
        {
            if (parameter == null)
            {
                var dlg = new OpenFileDialog()
                {
                    Filter = "Project (*.project)|*.project|All (*.*)|*.*",
                    FilterIndex = 0,
                    FileName = ""
                };

                if (dlg.ShowDialog(_mainWindow) == true)
                {
                    _editor.Open(dlg.FileName);
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

        /// <summary>
        /// Save current <see cref="Project"/> to external file.
        /// </summary>
        private void OnSave()
        {
            if (!string.IsNullOrEmpty(_editor.ProjectPath))
            {
                _editor.Save(_editor.ProjectPath);
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
                FileName = _editor.Project.Name
            };

            if (dlg.ShowDialog(_mainWindow) == true)
            {
                _editor.Save(dlg.FileName);
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
                        _editor.ExportAsPdf(dlg.FileName, item);
                        break;
                    case 2:
                        ExportAsEmf(dlg.FileName);
                        break;
                    case 3:
                        _editor.ExportAsDxf(dlg.FileName);
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
                _editor.ImportData(dlg.FileName);
            }
        }

        /// <summary>
        ///  Export records to external file.
        /// </summary>
        private void OnExportData()
        {
            if (_editor.Project == null || _editor.Project.CurrentDatabase == null)
                return;

            var database = _editor.Project.CurrentDatabase;

            var dlg = new SaveFileDialog()
            {
                Filter = "Csv (*.csv)|*.csv|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = database.Name
            };

            if (dlg.ShowDialog(_mainWindow) == true)
            {
                _editor.ExportData(dlg.FileName, database);
            }
        }

        /// <summary>
        /// Update records in current database.
        /// </summary>
        private void OnUpdateData()
        {
            if (_editor.Project == null || _editor.Project.CurrentDatabase == null)
                return;

            var database = _editor.Project.CurrentDatabase;

            var dlg = new OpenFileDialog()
            {
                Filter = "Csv (*.csv)|*.csv|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = ""
            };

            if (dlg.ShowDialog(_mainWindow) == true)
            {
                _editor.UpdateData(dlg.FileName, database);
            }
        }

        /// <summary>
        /// Import item object from external file.
        /// </summary>
        /// <param name="item">The item object to import.</param>
        /// <param name="type">The type of item object.</param>
        private void OnImportObject(object item, ImportType type)
        {
            if (item == null)
                return;

            string filter = string.Empty;

            switch (type)
            {
                case ImportType.Style:
                    filter = "Style (*.style)|*.style|All (*.*)|*.*";
                    break;
                case ImportType.Styles:
                    filter = "Styles (*.styles)|*.styles|All (*.*)|*.*";
                    break;
                case ImportType.StyleLibrary:
                    filter = "StyleLibrary (*.stylelibrary)|*.stylelibrary|All (*.*)|*.*";
                    break;
                case ImportType.StyleLibraries:
                    filter = "StyleLibraries (*.styleLibraries)|*.stylelibraries|All (*.*)|*.*";
                    break;
                case ImportType.Group:
                    filter = "Group (*.group)|*.group|All (*.*)|*.*";
                    break;
                case ImportType.Groups:
                    filter = "Groups (*.groups)|*.groups|All (*.*)|*.*";
                    break;
                case ImportType.GroupLibrary:
                    filter = "GroupLibrary (*.grouplibrary)|*.grouplibrary|All (*.*)|*.*";
                    break;
                case ImportType.GroupLibraries:
                    filter = "GroupLibraries (*.grouplibraries)|*.grouplibraries|All (*.*)|*.*";
                    break;
                case ImportType.Template:
                    filter = "Template (*.template)|*.template|All (*.*)|*.*";
                    break;
                case ImportType.Templates:
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
                    _editor.ImportObject(path, item, type);
                }
            }
        }

        /// <summary>
        /// Export item object to external file.
        /// </summary>
        /// <param name="item">The item object to export.</param>
        /// <param name="type">The type of item object.</param>
        private void OnExportObject(object item, ExportType type)
        {
            if (item == null)
                return;

            string name = string.Empty;
            string filter = string.Empty;

            switch (type)
            {
                case ExportType.Style:
                    filter = "Style (*.style)|*.style|All (*.*)|*.*";
                    name = (item as ShapeStyle).Name;
                    break;
                case ExportType.Styles:
                    filter = "Styles (*.styles)|*.styles|All (*.*)|*.*";
                    name = (item as Library<ShapeStyle>).Name;
                    break;
                case ExportType.StyleLibrary:
                    filter = "StyleLibrary (*.stylelibrary)|*.stylelibrary|All (*.*)|*.*";
                    name = (item as Library<ShapeStyle>).Name;
                    break;
                case ExportType.StyleLibraries:
                    filter = "StyleLibraries (*.stylelibraries)|*.stylelibraries|All (*.*)|*.*";
                    name = (item as Project).Name;
                    break;
                case ExportType.Group:
                    filter = "Group (*.group)|*.group|All (*.*)|*.*";
                    name = (item as XGroup).Name;
                    break;
                case ExportType.Groups:
                    filter = "Groups (*.groups)|*.groups|All (*.*)|*.*";
                    name = (item as Library<XGroup>).Name;
                    break;
                case ExportType.GroupLibrary:
                    filter = "GroupLibrary (*.grouplibrary)|*.grouplibrary|All (*.*)|*.*";
                    name = (item as Library<XGroup>).Name;
                    break;
                case ExportType.GroupLibraries:
                    filter = "GroupLibraries (*.grouplibraries)|*.grouplibraries|All (*.*)|*.*";
                    name = (item as Project).Name;
                    break;
                case ExportType.Template:
                    filter = "Template (*.template)|*.template|All (*.*)|*.*";
                    name = (item as Container).Name;
                    break;
                case ExportType.Templates:
                    filter = "Templates (*.templates)|*.templates|All (*.*)|*.*";
                    name = (item as Project).Name;
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
                _editor.ExportObject(dlg.FileName, item, type);
            }
        }

        /// <summary>
        /// Copy currently selected shapes to clipboard as enhanced metafile.
        /// </summary>
        private void OnCopyAsEmf()
        {
            if (_editor.Project == null || _editor.Project.CurrentContainer == null)
                return;

            var project = _editor.Project;
            var container = _editor.Project.CurrentContainer;
            var writer = new EmfWriter();

            if (_editor.Renderers[0].State.SelectedShape != null)
            {
                var shapes = Enumerable.Repeat(_editor.Renderers[0].State.SelectedShape, 1).ToList();
                writer.SetClipboard(
                    shapes,
                    container.Template.Width,
                    container.Template.Height,
                    container.Data.Properties,
                    project);
            }
            else if (_editor.Renderers[0].State.SelectedShapes != null)
            {
                var shapes = _editor.Renderers[0].State.SelectedShapes.ToList();
                writer.SetClipboard(
                    shapes,
                    container.Template.Width,
                    container.Template.Height,
                    container.Data.Properties,
                    project);
            }
            else
            {
                writer.SetClipboard(container, project);
            }
        }

        /// <summary>
        /// Save currently selected shapes as enhanced metafile.
        /// </summary>
        /// <param name="path">The file path.</param>
        private void ExportAsEmf(string path)
        {
            try
            {
                var writer = new EmfWriter();
                writer.Save(
                    path,
                    _editor.Project.CurrentContainer,
                    _editor.Project);
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
