// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Test2d;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Test.Windows
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MainWindow : Window, IView
    {
        private bool _isLoaded = false;
        private string _recentProjectsPath = "Test2d.UI.Wpf.recent";
        private string _resourceLayoutRoot = "Test2d.UI.Wpf.Layouts.";
        private string _resourceLayoutPath = "Test2d.UI.Wpf.layout";
        private string _defaultLayoutPath = "Test2d.UI.Wpf.layout";
        private bool _autoRestoreLayout = true;
        private IDictionary<string, LayoutContent> _layouts;
        private bool _autoLoadRecent = true;

        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            InitializeContext();
        }

        /// <summary>
        /// Load docking manager layout.
        /// </summary>
        private void OnLoadLayout()
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;

            var dlg = new OpenFileDialog()
            {
                Filter = "Layout (*.layout)|*.layout|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = ""
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    LoadLayout(dlg.FileName, context);
                }
                catch(Exception ex)
                {
                    if (context.Editor.Log != null)
                    {
                        context.Editor.Log.LogError("{0}{1}{2}",
                            ex.Message,
                            Environment.NewLine,
                            ex.StackTrace);
                    }
                }
            }
        }

        /// <summary>
        /// Save docking manager layout.
        /// </summary>
        private void OnSaveLayout()
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;

            var dlg = new SaveFileDialog()
            {
                Filter = "Layout (*.layout)|*.layout|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = _defaultLayoutPath
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    SaveLayout(dlg.FileName);
                }
                catch (Exception ex)
                {
                    if (context.Editor.Log != null)
                    {
                        context.Editor.Log.LogError("{0}{1}{2}",
                            ex.Message,
                            Environment.NewLine,
                            ex.StackTrace);
                    }
                }
            }
        }

        /// <summary>
        /// Reset docking manager layout.
        /// </summary>
        private void OnResetLayout()
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;
            
            try
            {
                LoadLayoutFromResource(_resourceLayoutRoot + _resourceLayoutPath, context);
            }
            catch (Exception ex)
            {
                if (context.Editor.Log != null)
                {
                    context.Editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnOpen(object parameter)
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;

            if (parameter == null)
            {
                var dlg = new OpenFileDialog()
                {
                    Filter = "Project (*.project)|*.project|All (*.*)|*.*",
                    FilterIndex = 0,
                    FileName = ""
                };

                if (dlg.ShowDialog() == true)
                {
                    context.Open(dlg.FileName);
                }
            }
            else
            {
                string path = parameter as string;
                if (path != null && System.IO.File.Exists(path))
                {
                    context.Open(path);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnSave()
        {
            var context = DataContext as EditorContext;
            if (context == null || context.Editor.Project == null)
                return;

            if (!string.IsNullOrEmpty(context.Editor.ProjectPath))
            {
                context.Save(context.Editor.ProjectPath);
            }
            else
            {
                OnSaveAs();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnSaveAs()
        {
            var context = DataContext as EditorContext;
            if (context == null || context.Editor.Project == null)
                return;

            var dlg = new SaveFileDialog()
            {
                Filter = "Project (*.project)|*.project|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = context.Editor.Project.Name
            };

            if (dlg.ShowDialog() == true)
            {
                context.Save(dlg.FileName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void OnExport(object item)
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;

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
            else if (item is EditorContext)
            {
                var editor = (item as EditorContext).Editor;
                if (editor.Project == null)
                    return;

                name = editor.Project.Name;
                item = editor.Project;
            }
            else if (item == null)
            {
                var editor = context.Editor;
                if (editor.Project == null)
                    return;
                
                name = editor.Project.Name;
                item = editor.Project;
            }

            var dlg = new SaveFileDialog()
            {
                Filter = "Pdf (*.pdf)|*.pdf|Emf (*.emf)|*.emf|Dxf AutoCAD 2000 (*.dxf)|*.dxf|Dxf R10 (*.dxf)|*.dxf|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = name
            };

            if (dlg.ShowDialog() == true)
            {
                switch (dlg.FilterIndex)
                {
                    case 1:
                        context.ExportAsPdf(dlg.FileName, item);
                        Process.Start(dlg.FileName);
                        break;
                    case 2:
                        ExportAsEmf(dlg.FileName);
                        Process.Start(dlg.FileName);
                        break;
                    case 3:
                        context.ExportAsDxf(dlg.FileName, Dxf.DxfAcadVer.AC1015);
                        Process.Start(dlg.FileName);
                        break;
                    case 4:
                        context.ExportAsDxf(dlg.FileName, Dxf.DxfAcadVer.AC1006);
                        Process.Start(dlg.FileName);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnImportData()
        {
            var context = DataContext as EditorContext;
            if (context == null || context.Editor.Project == null)
                return;

            var dlg = new OpenFileDialog()
            {
                Filter = "Csv (*.csv)|*.csv|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = ""
            };

            if (dlg.ShowDialog() == true)
            {
                context.ImportData(dlg.FileName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnExportData()
        {
            var context = DataContext as EditorContext;
            if (context == null || context.Editor.Project == null)
                return;

            var database = context.Editor.Project.CurrentDatabase;
            if (database == null)
                return;

            var dlg = new SaveFileDialog()
            {
                Filter = "Csv (*.csv)|*.csv|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = database.Name
            };

            if (dlg.ShowDialog() == true)
            {
                context.ExportData(dlg.FileName, database);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnUpdateData()
        {
            var context = DataContext as EditorContext;
            if (context == null || context.Editor.Project == null)
                return;

            var database = context.Editor.Project.CurrentDatabase;
            if (database == null)
                return;

            var dlg = new OpenFileDialog()
            {
                Filter = "Csv (*.csv)|*.csv|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = ""
            };

            if (dlg.ShowDialog() == true)
            {
                context.UpdateData(dlg.FileName, database);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="type"></param>
        private void OnImportObject(object item, ImportType type)
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;

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

            if (dlg.ShowDialog() == true)
            {
                var paths = dlg.FileNames;

                foreach (var path in paths)
                {
                    context.ImportObject(path, item, type);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="type"></param>
        private void OnExportObject(object item, ExportType type)
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;

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
                    name = (item as StyleLibrary).Name;
                    break;
                case ExportType.StyleLibrary:
                    filter = "StyleLibrary (*.stylelibrary)|*.stylelibrary|All (*.*)|*.*";
                    name = (item as StyleLibrary).Name;
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
                    name = (item as GroupLibrary).Name;
                    break;
                case ExportType.GroupLibrary:
                    filter = "GroupLibrary (*.grouplibrary)|*.grouplibrary|All (*.*)|*.*";
                    name = (item as GroupLibrary).Name;
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

            if (dlg.ShowDialog() == true)
            {
                var path = dlg.FileName;
                context.ExportObject(path, item, type);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnCopyAsEmf()
        {
            var context = DataContext as EditorContext;
            if (context == null 
                || context.Editor == null 
                || context.Editor.Project == null
                || context.Editor.Project.CurrentContainer == null)
                return;

            if (context.Editor.Renderers[0].State.SelectedShape != null)
            {
                var container = context.Editor.Project.CurrentContainer;
                var shapes = Enumerable.Repeat(context.Editor.Renderers[0].State.SelectedShape, 1).ToList();
                (new EmfWriter()).SetClipboard(shapes, container.Width, container.Height, container.Properties);
            }
            else if (context.Editor.Renderers[0].State.SelectedShapes != null)
            {
                var container = context.Editor.Project.CurrentContainer;
                var shapes = context.Editor.Renderers[0].State.SelectedShapes.ToList();
                (new EmfWriter()).SetClipboard(shapes, container.Width, container.Height, container.Properties);
            }
            else
            {
                var container = context.Editor.Project.CurrentContainer;
                (new EmfWriter()).SetClipboard(container);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnZoomReset()
        {
            panAndZoomGrid.ResetZoomAndPan();
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnZoomExtent()
        {
            panAndZoomGrid.AutoFit();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        private void ExportAsEmf(string path)
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;

            try
            {
                var container = context.Editor.Project.CurrentContainer;
                (new EmfWriter()).Save(path, container);
            }
            catch (Exception ex)
            {
                if (context.Editor.Log != null)
                {
                    context.Editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Uri GetImagePath()
        {
            var dlg = new OpenFileDialog()
            {
                Filter = "All (*.*)|*.*",
                FilterIndex = 0,
                FileName = ""
            };

            if (dlg.ShowDialog() == true)
            {
                return new Uri(dlg.FileName);
            }
            return null;
        }

        /// <summary>
        /// Load recent project files.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="context"></param>
        private void LoadRecent(string path, object context)
        {
            if (context == null)
                return;

            if (!System.IO.File.Exists(path))
                return;

            (context as EditorContext).LoadRecent(path);
        }

        /// <summary>
        /// Save recent project files.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="context"></param>
        private void SaveRecent(string path, object context)
        {
            if (context == null)
                return;

            (context as EditorContext).SaveRecent(path);
        }

        /// <summary>
        /// Auto load recent project files.
        /// </summary>
        /// <param name="context"></param>
        private void AutoLoadRecent(EditorContext context)
        {
            try
            {
                LoadRecent(_recentProjectsPath, context);
            }
            catch (Exception ex)
            {
                if (context.Editor.Log != null)
                {
                    context.Editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Auto save recent project files.
        /// </summary>
        /// <param name="context"></param>
        private void AutoSaveRecent(EditorContext context)
        {
            try
            {
                SaveRecent(_recentProjectsPath, context);
            }
            catch (Exception ex)
            {
                if (context.Editor.Log != null)
                {
                    context.Editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Load docking manager layout from resource.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="context"></param>
        private void LoadLayoutFromResource(string path, object context)
        {
            var serializer = new XmlLayoutSerializer(dockingManager);

            serializer.LayoutSerializationCallback +=
                (s, e) =>
                {
                    _layouts[e.Model.ContentId] = e.Model;

                    var element = e.Content as FrameworkElement;
                    if (element != null)
                    {
                        element.DataContext = context;
                    }
                };

            var assembly = this.GetType().Assembly;
            using (var stream = assembly.GetManifestResourceStream(path))
            {
                using (var reader = new System.IO.StreamReader(stream))
                {
                    serializer.Deserialize(reader);
                }
            }
        }

        /// <summary>
        /// Load docking manager layout.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="context"></param>
        private void LoadLayout(string path, object context)
        {
            if (!System.IO.File.Exists(path))
                return;

            var serializer = new XmlLayoutSerializer(dockingManager);

            serializer.LayoutSerializationCallback +=
                (s, e) =>
                {
                    _layouts[e.Model.ContentId] = e.Model;

                    var element = e.Content as FrameworkElement;
                    if (element != null)
                    {
                        element.DataContext = context;
                    }
                };

            using (var reader = new System.IO.StreamReader(path))
            {
                serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// Save docking manager layout.
        /// </summary>
        /// <param name="path"></param>
        private void SaveLayout(string path)
        {
            var serializer = new XmlLayoutSerializer(dockingManager);
            using (var writer = new System.IO.StreamWriter(path))
            {
                serializer.Serialize(writer);
            }
        }

        /// <summary>
        /// Auto load docking manager layout.
        /// </summary>
        /// <param name="context"></param>
        private void AutoLoadLayout(EditorContext context)
        {
            try
            {
                LoadLayout(_defaultLayoutPath, context);
            }
            catch (Exception ex)
            {
                if (context.Editor.Log != null)
                {
                    context.Editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Auto save docking manager layout.
        /// </summary>
        /// <param name="context"></param>
        private void AutoSaveLayout(EditorContext context)
        {
            try
            {
                SaveLayout(_defaultLayoutPath);
            }
            catch (Exception ex)
            {
                if (context.Editor.Log != null)
                {
                    context.Editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeContext()
        {
            var context = new EditorContext()
            {
                View = this,
                Renderers = new IRenderer[] { new WpfRenderer(), new WpfRenderer() },
                ProjectFactory = new ProjectFactory(),
                TextClipboard = new TextClipboard(),
                Serializer = new NewtonsoftSerializer(),
                PdfWriter = new PdfWriter(),
                DxfWriter = new DxfWriter(),
                CsvReader = new CsvHelperReader(),
                CsvWriter = new CsvHelperWriter()
            };

            context.InitializeEditor(new TraceLog());
            context.Editor.Renderers[0].State.DrawShapeState = ShapeState.Visible;
            context.Editor.Renderers[1].State.DrawShapeState = ShapeState.Visible;
            context.Editor.GetImagePath = () => GetImagePath();

            InitializeCommands(context);
            InitializeZoom(context);
            InitializeDrop(context);

            Loaded +=
                (s, e) =>
                {
                    if (_isLoaded)
                        return;
                    else
                        _isLoaded = true;

                    InitializeLayouts();

                    if (_autoLoadRecent)
                    {
                        AutoLoadRecent(context);
                    }

                    if (_autoRestoreLayout)
                    {
                        AutoLoadLayout(context);
                    }
                };

            Unloaded += (s, e) =>
            {
                if (!_isLoaded)
                    return;
                else
                    _isLoaded = false;

                DeInitializeContext();

                if (_autoLoadRecent)
                {
                    AutoSaveRecent(context);
                }

                if (_autoRestoreLayout)
                {
                    AutoSaveLayout(context);
                }
            };

            DataContext = context;
        }

        /// <summary>
        /// Initialize docking manager layouts dictionary.
        /// </summary>
        private void InitializeLayouts()
        {
            _layouts = new Dictionary<string, LayoutContent>();
            _layouts.Add("project", projectWindow);
            _layouts.Add("templates", templatesWindow);
            _layouts.Add("databases", databasesWindow);
            _layouts.Add("options", optionsWindow);
            _layouts.Add("template", templateWindow);
            _layouts.Add("groups", groupsWindow);
            _layouts.Add("database", databaseWindow);
            _layouts.Add("container", containerWindow);
            _layouts.Add("styles", stylesWindow);
            _layouts.Add("layers", layersWindow);
            _layouts.Add("shapes", shapesWindow);
            _layouts.Add("properties", propertiesWindow);
            _layouts.Add("style", shapesWindow);
            _layouts.Add("state", shapesWindow);
            _layouts.Add("data", shapesWindow);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        private void InitializeCommands(EditorContext context)
        {
            context.Commands.OpenCommand =
                Command<object>.Create(
                    (parameter) => OnOpen(parameter),
                    (parameter) => context.IsEditMode());
            
            context.Commands.SaveCommand =
                Command.Create(
                    () => OnSave(),
                    () => context.IsEditMode());

            context.Commands.SaveAsCommand =
                Command.Create(
                    () => OnSaveAs(),
                    () => context.IsEditMode());

            context.Commands.ExportCommand =
                Command<object>.Create(
                    (item) => OnExport(item),
                    (item) => context.IsEditMode());

            context.Commands.ImportDataCommand =
                Command<object>.Create(
                    (item) => OnImportData(),
                    (item) => context.IsEditMode());

            context.Commands.ExportDataCommand =
                Command<object>.Create(
                    (item) => OnExportData(),
                    (item) => context.IsEditMode());

            context.Commands.UpdateDataCommand =
                Command<object>.Create(
                    (item) => OnUpdateData(),
                    (item) => context.IsEditMode());

            context.Commands.ImportStyleCommand =
                Command<object>.Create(
                    (item) => OnImportObject(item, ImportType.Style),
                    (item) => context.IsEditMode());

            context.Commands.ImportStylesCommand =
                Command<object>.Create(
                    (item) => OnImportObject(item, ImportType.Styles),
                    (item) => context.IsEditMode());

            context.Commands.ImportStyleLibraryCommand =
                Command<object>.Create(
                    (item) => OnImportObject(item, ImportType.StyleLibrary),
                    (item) => context.IsEditMode());

            context.Commands.ImportStyleLibrariesCommand =
                Command<object>.Create(
                    (item) => OnImportObject(item, ImportType.StyleLibraries),
                    (item) => context.IsEditMode());

            context.Commands.ImportGroupCommand =
                Command<object>.Create(
                    (item) => OnImportObject(item, ImportType.Group),
                    (item) => context.IsEditMode());

            context.Commands.ImportGroupsCommand =
                Command<object>.Create(
                    (item) => OnImportObject(item, ImportType.Groups),
                    (item) => context.IsEditMode());

            context.Commands.ImportGroupLibraryCommand =
                Command<object>.Create(
                    (item) => OnImportObject(item, ImportType.GroupLibrary),
                    (item) => context.IsEditMode());

            context.Commands.ImportGroupLibrariesCommand =
                Command<object>.Create(
                    (item) => OnImportObject(item, ImportType.GroupLibraries),
                    (item) => context.IsEditMode());

            context.Commands.ImportTemplateCommand =
                Command<object>.Create(
                    (item) => OnImportObject(item, ImportType.Template),
                    (item) => context.IsEditMode());

            context.Commands.ImportTemplatesCommand =
                Command<object>.Create(
                    (item) => OnImportObject(item, ImportType.Templates),
                    (item) => context.IsEditMode());

            context.Commands.ExportStyleCommand =
                Command<object>.Create(
                    (item) => OnExportObject(item, ExportType.Style),
                    (item) => context.IsEditMode());

            context.Commands.ExportStylesCommand =
                Command<object>.Create(
                    (item) => OnExportObject(item, ExportType.Styles),
                    (item) => context.IsEditMode());

            context.Commands.ExportStyleLibraryCommand =
                Command<object>.Create(
                    (item) => OnExportObject(item, ExportType.StyleLibrary),
                    (item) => context.IsEditMode());

            context.Commands.ExportStyleLibrariesCommand =
                Command<object>.Create(
                    (item) => OnExportObject(item, ExportType.StyleLibraries),
                    (item) => context.IsEditMode());

            context.Commands.ExportGroupCommand =
                Command<object>.Create(
                    (item) => OnExportObject(item, ExportType.Group),
                    (item) => context.IsEditMode());

            context.Commands.ExportGroupsCommand =
                Command<object>.Create(
                    (item) => OnExportObject(item, ExportType.Groups),
                    (item) => context.IsEditMode());

            context.Commands.ExportGroupLibraryCommand =
                Command<object>.Create(
                    (item) => OnExportObject(item, ExportType.GroupLibrary),
                    (item) => context.IsEditMode());

            context.Commands.ExportGroupLibrariesCommand =
                Command<object>.Create(
                    (item) => OnExportObject(item, ExportType.GroupLibraries),
                    (item) => context.IsEditMode());

            context.Commands.ExportTemplateCommand =
                Command<object>.Create(
                    (item) => OnExportObject(item, ExportType.Template),
                    (item) => context.IsEditMode());

            context.Commands.ExportTemplatesCommand =
                Command<object>.Create(
                    (item) => OnExportObject(item, ExportType.Templates),
                    (item) => context.IsEditMode());

            context.Commands.CopyAsEmfCommand =
                Command.Create(
                    () => OnCopyAsEmf(),
                    () => context.IsEditMode());
            
            context.Commands.ZoomResetCommand =
                Command.Create(
                    () => OnZoomReset(),
                    () => true);

            context.Commands.ZoomExtentCommand =
                Command.Create(
                    () => OnZoomExtent(),
                    () => true);

            context.Commands.ProjectWindowCommand =
                Command.Create(
                    () => (_layouts["project"] as LayoutAnchorable).Show(),
                    () => true);

            context.Commands.OptionsWindowCommand =
                Command.Create(
                    () => (_layouts["options"] as LayoutAnchorable).Show(),
                    () => true);

            context.Commands.TemplatesWindowCommand =
                Command.Create(
                    () => (_layouts["templates"] as LayoutAnchorable).Show(),
                    () => true);

            context.Commands.GroupsWindowCommand =
                Command.Create(
                    () => (_layouts["groups"] as LayoutAnchorable).Show(),
                    () => true);

            context.Commands.DatabasesWindowCommand =
                Command.Create(
                    () => (_layouts["databases"] as LayoutAnchorable).Show(),
                    () => true);

            context.Commands.DatabaseWindowCommand =
                Command.Create(
                    () => (_layouts["database"] as LayoutAnchorable).Show(),
                    () => true);

            //context.Commands.ContainerWindowCommand = 
            //    Command.Create(
            //        () => ,
            //        () => true);

            //context.Commands.DocumentWindowCommand = 
            //    Command.Create(
            //        () => ,
            //        () => true);

            context.Commands.StylesWindowCommand =
                Command.Create(
                    () => (_layouts["styles"] as LayoutAnchorable).Show(),
                    () => true);

            context.Commands.LayersWindowCommand =
                Command.Create(
                    () => (_layouts["layers"] as LayoutAnchorable).Show(),
                    () => true);

            context.Commands.ShapesWindowCommand =
                Command.Create(
                    () => (_layouts["shapes"] as LayoutAnchorable).Show(),
                    () => true);

            context.Commands.TemplateWindowCommand =
                Command.Create(
                    () => (_layouts["template"] as LayoutAnchorable).Show(),
                    () => true);

            context.Commands.PropertiesWindowCommand =
                Command.Create(
                    () => (_layouts["properties"] as LayoutAnchorable).Show(),
                    () => true);

            context.Commands.StateWindowCommand =
                Command.Create(
                    () => (_layouts["state"] as LayoutAnchorable).Show(),
                    () => true);

            context.Commands.DataWindowCommand =
                Command.Create(
                    () => (_layouts["data"] as LayoutAnchorable).Show(),
                    () => true);

            context.Commands.StyleWindowCommand =
                Command.Create(
                    () => (_layouts["style"] as LayoutAnchorable).Show(),
                    () => true);

            context.Commands.LoadWindowLayoutCommand =
                Command.Create(
                    () => OnLoadLayout(),
                    () => true);

            context.Commands.SaveWindowLayoutCommand =
                Command.Create(
                    () => OnSaveLayout(),
                    () => true);

            context.Commands.ResetWindowLayoutCommand =
                Command.Create(
                    () => OnResetLayout(),
                    () => true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        private void InitializeZoom(EditorContext context)
        {
            border.InvalidateChild =
                (z, x, y) =>
                {
                    bool invalidate = context.Editor.Renderers[0].State.Zoom != z;
                    context.Editor.Renderers[0].State.Zoom = z;
                    context.Editor.Renderers[0].State.PanX = x;
                    context.Editor.Renderers[0].State.PanY = y;
                    if (invalidate)
                    {
                        context.Invalidate(isZooming: true);
                    }
                };

            border.AutoFitChild =
                (width, height) =>
                {
                    if (border != null
                        && context != null
                        && context.Editor.Project != null
                        && context.Editor.Project.CurrentContainer != null)
                    {
                        border.AutoFit(
                            width,
                            height,
                            context.Editor.Project.CurrentContainer.Width,
                            context.Editor.Project.CurrentContainer.Height);
                    }
                };

            border.MouseDown +=
                (s, e) =>
                {
                    if (e.ChangedButton == MouseButton.Middle && e.ClickCount == 2)
                    {
                        panAndZoomGrid.AutoFit();
                    }

                    if (e.ChangedButton == MouseButton.Middle && e.ClickCount == 3)
                    {
                        panAndZoomGrid.ResetZoomAndPan();
                    }
                };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        private void InitializeDrop(EditorContext context)
        {
            containerControl.AllowDrop = true;

            containerControl.DragEnter +=
                (s, e) =>
                {
                    if (!e.Data.GetDataPresent(DataFormats.FileDrop)
                        && !e.Data.GetDataPresent(typeof(XGroup))
                        && !e.Data.GetDataPresent(typeof(Record))
                        && !e.Data.GetDataPresent(typeof(ShapeStyle)))
                    {
                        e.Effects = DragDropEffects.None;
                        e.Handled = true;
                    }
                };

            containerControl.Drop +=
                (s, e) =>
                {
                    if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    {
                        try
                        {
                            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                            if (context.Drop(files))
                            {
                                e.Handled = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (context.Editor.Log != null)
                            {
                                context.Editor.Log.LogError("{0}{1}{2}",
                                    ex.Message,
                                    Environment.NewLine,
                                    ex.StackTrace);
                            }
                        }
                    }

                    if (e.Data.GetDataPresent(typeof(XGroup)))
                    {
                        try
                        {
                            var group = e.Data.GetData(typeof(XGroup)) as XGroup;
                            if (group != null)
                            {
                                var p = e.GetPosition(containerControl);
                                context.DropAsClone(group, p.X, p.Y);
                                e.Handled = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (context.Editor.Log != null)
                            {
                                context.Editor.Log.LogError("{0}{1}{2}",
                                    ex.Message,
                                    Environment.NewLine,
                                    ex.StackTrace);
                            }
                        }
                    }

                    if (e.Data.GetDataPresent(typeof(Record)))
                    {
                        try
                        {
                            var record = e.Data.GetData(typeof(Record)) as Record;
                            if (record != null)
                            {
                                var p = e.GetPosition(containerControl);
                                context.Drop(record, p.X, p.Y);
                                e.Handled = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (context.Editor.Log != null)
                            {
                                context.Editor.Log.LogError("{0}{1}{2}",
                                    ex.Message,
                                    Environment.NewLine,
                                    ex.StackTrace);
                            }
                        }
                    }

                    if (e.Data.GetDataPresent(typeof(ShapeStyle)))
                    {
                        try
                        {
                            var style = e.Data.GetData(typeof(ShapeStyle)) as ShapeStyle;
                            if (style != null)
                            {
                                var p = e.GetPosition(containerControl);
                                context.Drop(style, p.X, p.Y);
                                e.Handled = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (context.Editor.Log != null)
                            {
                                context.Editor.Log.LogError("{0}{1}{2}",
                                    ex.Message,
                                    Environment.NewLine,
                                    ex.StackTrace);
                            }
                        }
                    }
                };
        }

        /// <summary>
        /// 
        /// </summary>
        private void DeInitializeContext()
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;

            context.Dispose();
        }
    }
}
