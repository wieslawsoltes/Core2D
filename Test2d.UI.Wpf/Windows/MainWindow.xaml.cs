// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Practices.Prism.Commands;
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
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Test.Windows
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MainWindow : Window, IView
    {
        private string _resourceLayoutRoot = "Test2d.UI.Wpf.Layouts.";
        private string _resourceLayoutPath = "Test2d.UI.Wpf.layout";
        private string _defaultLayoutPath = "Test2d.UI.Wpf.layout";
        private bool _enableRestoreLayout = true;
        private bool _isLoaded = false;

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
        private void InitializeContext()
        {
            var context = new EditorContext()
            {
                View = this,
                Renderers = new IRenderer[] { new WpfRenderer(), new WpfRenderer() },
                ProjectFactory = new ProjectFactory(),
                SimulationTimer = new SimulationTimer(),
                TextClipboard = new TextClipboard(),
                Serializer = new NewtonsoftSerializer(),
                ScriptEngine = new RoslynScriptEngine(),
                CodeEngine = new RoslynCodeEngine(),
                PdfWriter = new PdfWriter(),
                DxfWriter = new DxfWriter(),
                CsvReader = new CsvHelperReader(),
                CsvWriter = new CsvHelperWriter()
            };

            context.InitializeEditor();
            context.InitializeScripts();
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

                    if (_enableRestoreLayout)
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

                if (_enableRestoreLayout)
                {
                    AutoSaveLayout(context);
                }
            };

            DataContext = context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        private void InitializeCommands(EditorContext context)
        {
            context.Commands.OpenCommand =
                new DelegateCommand(
                    () => OnOpen(),
                    () => context.IsEditMode());

            context.Commands.SaveAsCommand =
                new DelegateCommand(
                    () => OnSaveAs(),
                    () => context.IsEditMode());

            context.Commands.ExportCommand =
                new DelegateCommand<object>(
                    (item) => OnExport(item),
                    (item) => context.IsEditMode());

            context.Commands.ImportDataCommand =
                new DelegateCommand<object>(
                    (item) => OnImportData(),
                    (item) => context.IsEditMode());

            context.Commands.ExportDataCommand =
                new DelegateCommand<object>(
                    (item) => OnExportData(),
                    (item) => context.IsEditMode());

            context.Commands.UpdateDataCommand =
                new DelegateCommand<object>(
                    (item) => OnUpdateData(),
                    (item) => context.IsEditMode());

            context.Commands.ImportStyleCommand =
                new DelegateCommand<object>(
                    (item) => OnImportObject(item, ImportType.Style),
                    (item) => context.IsEditMode());

            context.Commands.ImportStylesCommand =
                new DelegateCommand<object>(
                    (item) => OnImportObject(item, ImportType.Styles),
                    (item) => context.IsEditMode());

            context.Commands.ImportStyleLibraryCommand =
                new DelegateCommand<object>(
                    (item) => OnImportObject(item, ImportType.StyleLibrary),
                    (item) => context.IsEditMode());

            context.Commands.ImportStyleLibrariesCommand =
                new DelegateCommand<object>(
                    (item) => OnImportObject(item, ImportType.StyleLibraries),
                    (item) => context.IsEditMode());

            context.Commands.ImportGroupCommand =
                new DelegateCommand<object>(
                    (item) => OnImportObject(item, ImportType.Group),
                    (item) => context.IsEditMode());

            context.Commands.ImportGroupsCommand =
                new DelegateCommand<object>(
                    (item) => OnImportObject(item, ImportType.Groups),
                    (item) => context.IsEditMode());

            context.Commands.ImportGroupLibraryCommand =
                new DelegateCommand<object>(
                    (item) => OnImportObject(item, ImportType.GroupLibrary),
                    (item) => context.IsEditMode());

            context.Commands.ImportGroupLibrariesCommand =
                new DelegateCommand<object>(
                    (item) => OnImportObject(item, ImportType.GroupLibraries),
                    (item) => context.IsEditMode());

            context.Commands.ImportTemplateCommand =
                new DelegateCommand<object>(
                    (item) => OnImportObject(item, ImportType.Template),
                    (item) => context.IsEditMode());

            context.Commands.ImportTemplatesCommand =
                new DelegateCommand<object>(
                    (item) => OnImportObject(item, ImportType.Templates),
                    (item) => context.IsEditMode());

            context.Commands.ExportStyleCommand =
                new DelegateCommand<object>(
                    (item) => OnExportObject(item, ExportType.Style),
                    (item) => context.IsEditMode());

            context.Commands.ExportStylesCommand =
                new DelegateCommand<object>(
                    (item) => OnExportObject(item, ExportType.Styles),
                    (item) => context.IsEditMode());

            context.Commands.ExportStyleLibraryCommand =
                new DelegateCommand<object>(
                    (item) => OnExportObject(item, ExportType.StyleLibrary),
                    (item) => context.IsEditMode());

            context.Commands.ExportStyleLibrariesCommand =
                new DelegateCommand<object>(
                    (item) => OnExportObject(item, ExportType.StyleLibraries),
                    (item) => context.IsEditMode());

            context.Commands.ExportGroupCommand =
                new DelegateCommand<object>(
                    (item) => OnExportObject(item, ExportType.Group),
                    (item) => context.IsEditMode());

            context.Commands.ExportGroupsCommand =
                new DelegateCommand<object>(
                    (item) => OnExportObject(item, ExportType.Groups),
                    (item) => context.IsEditMode());

            context.Commands.ExportGroupLibraryCommand =
                new DelegateCommand<object>(
                    (item) => OnExportObject(item, ExportType.GroupLibrary),
                    (item) => context.IsEditMode());

            context.Commands.ExportGroupLibrariesCommand =
                new DelegateCommand<object>(
                    (item) => OnExportObject(item, ExportType.GroupLibraries),
                    (item) => context.IsEditMode());

            context.Commands.ExportTemplateCommand =
                new DelegateCommand<object>(
                    (item) => OnExportObject(item, ExportType.Template),
                    (item) => context.IsEditMode());

            context.Commands.ExportTemplatesCommand =
                new DelegateCommand<object>(
                    (item) => OnExportObject(item, ExportType.Templates),
                    (item) => context.IsEditMode());

            context.Commands.CopyAsEmfCommand =
                new DelegateCommand(
                    () => OnCopyAsEmf(),
                    () => context.IsEditMode());

            context.Commands.EvalCommand =
                new DelegateCommand(
                    () => OnEval(),
                    () => context.IsEditMode());

            context.Commands.ImportShapeCodeCommand =
                new DelegateCommand(
                    () => OnImportShapeCode(),
                    () => context.IsEditMode());

            context.Commands.ExportShapeCodeCommand =
                new DelegateCommand(
                    () => OnExportShapeCode(),
                    () => context.IsEditMode());

            context.Commands.ZoomResetCommand =
                new DelegateCommand(
                    () => OnZoomReset(),
                    () => true);

            context.Commands.ZoomExtentCommand =
                new DelegateCommand(
                    () => OnZoomExtent(),
                    () => true);

            context.Commands.ProjectWindowCommand =
                new DelegateCommand(
                    () => projectWindow.Show(),
                    () => true);

            context.Commands.OptionsWindowCommand =
                new DelegateCommand(
                    () => optionsWindow.Show(),
                    () => true);

            context.Commands.TemplatesWindowCommand =
                new DelegateCommand(
                    () => templatesWindow.Show(),
                    () => true);

            context.Commands.GroupsWindowCommand =
                new DelegateCommand(
                    () => groupsWindow.Show(),
                    () => true);

            context.Commands.DatabasesWindowCommand =
                new DelegateCommand(
                    () => databasesWindow.Show(),
                    () => true);

            context.Commands.DatabaseWindowCommand =
                new DelegateCommand(
                    () => databaseWindow.Show(),
                    () => true);

            //context.Commands.ContainerWindowCommand = 
            //    new DelegateCommand(
            //        () => ,
            //        () => true);

            //context.Commands.ScriptWindowCommand = 
            //    new DelegateCommand(
            //        () => ,
            //        () => true);

            //context.Commands.DocumentWindowCommand = 
            //    new DelegateCommand(
            //        () => ,
            //        () => true);

            context.Commands.StylesWindowCommand =
                new DelegateCommand(
                    () => stylesWindow.Show(),
                    () => true);

            context.Commands.LayersWindowCommand =
                new DelegateCommand(
                    () => layersWindow.Show(),
                    () => true);

            context.Commands.ShapesWindowCommand =
                new DelegateCommand(
                    () => shapesWindow.Show(),
                    () => true);

            context.Commands.TemplateWindowCommand =
                new DelegateCommand(
                    () => templateWindow.Show(),
                    () => true);

            context.Commands.PropertiesWindowCommand =
                new DelegateCommand(
                    () => propertiesWindow.Show(),
                    () => true);

            context.Commands.StateWindowCommand =
                new DelegateCommand(
                    () => stateWindow.Show(),
                    () => true);

            context.Commands.CodeWindowCommand =
                new DelegateCommand(
                    () => codeWindow.Show(),
                    () => true);

            context.Commands.DataWindowCommand =
                new DelegateCommand(
                    () => dataWindow.Show(),
                    () => true);

            context.Commands.StyleWindowCommand =
                new DelegateCommand(
                    () => styleWindow.Show(),
                    () => true);

            context.Commands.LoadWindowLayoutCommand =
                new DelegateCommand(
                    () => OnLoadLayout(),
                    () => true);

            context.Commands.SaveWindowLayoutCommand =
                new DelegateCommand(
                    () => OnSaveLayout(),
                    () => true);

            context.Commands.ResetWindowLayoutCommand =
                new DelegateCommand(
                    () => OnResetLayout(),
                    () => true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        private void InitializeZoom(EditorContext context)
        {

            panAndZoomGrid.EnableAutoFit = context.Renderers[0].State.EnableAutofit;

            border.InvalidateChild =
                (z, x, y) =>
                {
                    bool invalidate = context.Editor.Renderers[0].State.Zoom != z;

                    context.Editor.Renderers[0].State.Zoom = z;
                    context.Editor.Renderers[0].State.PanX = x;
                    context.Editor.Renderers[0].State.PanY = y;

                    if (invalidate)
                    {
                        context.Invalidate();
                    }
                };

            border.AutoFitChild =
                (width, height) =>
                {
                    if (border != null
                        && context != null
                        && context.Editor.Project.CurrentContainer != null)
                    {
                        if (!context.Renderers[0].State.EnableAutofit)
                            return;

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

                                // NOTE: Drop XGroup as reference (hold Shift key).
                                if (Keyboard.Modifiers == ModifierKeys.Shift)
                                {
                                    context.DropAsReference(group, p.X, p.Y);
                                }
                                // NOTE: Drop XGroup as clone (without Shift key).
                                else
                                {
                                    context.DropAsClone(group, p.X, p.Y);
                                }

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

        /// <summary>
        /// 
        /// </summary>
        private void OnOpen()
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;

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

        /// <summary>
        /// 
        /// </summary>
        private void OnSaveAs()
        {
            var context = DataContext as EditorContext;
            if (context == null)
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
                name = editor.Project.Name;
                item = editor.Project;
            }
            else if (item == null)
            {
                var editor = context.Editor;
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
            if (context == null)
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
            if (context == null)
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
                if (context.CsvWriter != null)
                {
                    context.CsvWriter.Write(dlg.FileName, database);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnUpdateData()
        {
            var context = DataContext as EditorContext;
            if (context == null)
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
                    context.ImportEx(path, item, type);
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
                context.ExportEx(path, item, type);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnCopyAsEmf()
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;

            (new EmfWriter()).SetClipboard(context.Editor.Project.CurrentContainer);
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnEval()
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;

            var dlg = new OpenFileDialog()
            {
                Filter = "C# (*.cs)|*.cs|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = "",
                Multiselect = true
            };

            if (dlg.ShowDialog() == true)
            {
                foreach (var path in dlg.FileNames)
                {
                    context.Eval(path);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnImportShapeCode()
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;

            var shape = context.Editor.Renderers[0].State.SelectedShape;
            if (shape == null)
                return;

            try
            {
                var dlg = new OpenFileDialog()
                {
                    Filter = "Code (*.code)|*.code|All (*.*)|*.*",
                    FilterIndex = 0,
                    FileName = ""
                };

                if (dlg.ShowDialog() == true)
                {
                    context.ImportShapeCode(dlg.FileName, shape);
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

        /// <summary>
        /// 
        /// </summary>
        private void OnExportShapeCode()
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;

            var shape = context.Editor.Renderers[0].State.SelectedShape;
            if (shape == null)
                return;

            try
            {
                string name = "shape";

                var dlg = new SaveFileDialog()
                {
                    Filter = "Code (*.code)|*.code|All (*.*)|*.*",
                    FilterIndex = 0,
                    FileName = name
                };

                if (dlg.ShowDialog() == true)
                {
                    context.ExportShapeCode(dlg.FileName, shape);
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
        private string GetImagePath()
        {
            var dlg = new OpenFileDialog()
            {
                Filter = "All (*.*)|*.*",
                FilterIndex = 0,
                FileName = ""
            };

            if (dlg.ShowDialog() == true)
            {
                return dlg.FileName;
            }
            return null;
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
    }
}
