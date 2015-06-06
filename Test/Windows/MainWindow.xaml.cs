// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Practices.Prism.Commands;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using TestEDITOR;

namespace Test.Windows
{
    /// <summary>
    /// 
    /// </summary>
    internal class TextClipboard : ITextClipboard
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text)
        {
            Clipboard.SetText(text, TextDataFormat.UnicodeText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            return Clipboard.GetText(TextDataFormat.UnicodeText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ContainsText()
        {
            return Clipboard.ContainsText(TextDataFormat.UnicodeText);
        }
    }
 
    /// <summary>
    /// 
    /// </summary>
    public partial class MainWindow : Window, IView
    {
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
        /// 
        /// </summary>
        private void InitializeContext()
        {
            var context = new EditorContext()
            {
                View = this,
                Renderers = new IRenderer[] { new WpfRenderer(), new WpfRenderer() },
                TextClipboard = new TextClipboard(),
                Serializer = new NewtonsoftSerializer(),
                Compressor = new LZ4CodecCompressor(),
                ScriptEngine = new RoslynScriptEngine(),
                PdfWriter = new PdfWriter(),
                DxfWriter = new DxfWriter(),
                CsvReader = new VisualBasicReader(),
                CsvWriter = new CsvHelperWriter(),
                Execute = (action) => Dispatcher.Invoke(action)
            };
            context.InitializeEditor();
            context.InitializeSctipts();
            context.InitializeSimulation();
            context.Editor.Renderers[0].State.DrawShapeState = ShapeState.Visible;
            context.Editor.Renderers[1].State.DrawShapeState = ShapeState.Visible;
            context.Editor.GetImagePath = () => Image();

            context.Commands.OpenCommand = 
                new DelegateCommand(
                    () => Open(),
                    () => context.IsEditMode());

            context.Commands.SaveAsCommand = 
                new DelegateCommand(
                    () => SaveAs(),
                    () => context.IsEditMode());

            context.Commands.ExportCommand = 
                new DelegateCommand<object>(
                    (item) => Export(item),
                    (item) => context.IsEditMode());

            context.Commands.ImportDataCommand = 
                new DelegateCommand<object>(
                    (item) => ImportData(),
                    (item) => context.IsEditMode());

            context.Commands.ExportDataCommand = 
                new DelegateCommand<object>(
                    (item) => ExportData(),
                    (item) => context.IsEditMode());

            context.Commands.UpdateDataCommand = 
                new DelegateCommand<object>(
                    (item) => UpdateData(),
                    (item) => context.IsEditMode());

            context.Commands.ImportStyleCommand = 
                new DelegateCommand<object>(
                    (item) => ImportObject(item, ImportType.Style),
                    (item) => context.IsEditMode());

            context.Commands.ImportStylesCommand = 
                new DelegateCommand<object>(
                    (item) => ImportObject(item, ImportType.Styles),
                    (item) => context.IsEditMode());

            context.Commands.ImportStyleLibraryCommand = 
                new DelegateCommand<object>(
                    (item) => ImportObject(item, ImportType.StyleLibrary),
                    (item) => context.IsEditMode());

            context.Commands.ImportStyleLibrariesCommand = 
                new DelegateCommand<object>(
                    (item) => ImportObject(item, ImportType.StyleLibraries),
                    (item) => context.IsEditMode());

            context.Commands.ImportGroupCommand = 
                new DelegateCommand<object>(
                    (item) => ImportObject(item, ImportType.Group),
                    (item) => context.IsEditMode());

            context.Commands.ImportGroupsCommand = 
                new DelegateCommand<object>(
                    (item) => ImportObject(item, ImportType.Groups),
                    (item) => context.IsEditMode());

            context.Commands.ImportGroupLibraryCommand = 
                new DelegateCommand<object>(
                    (item) => ImportObject(item, ImportType.GroupLibrary),
                    (item) => context.IsEditMode());

            context.Commands.ImportGroupLibrariesCommand = 
                new DelegateCommand<object>(
                    (item) => ImportObject(item, ImportType.GroupLibraries),
                    (item) => context.IsEditMode());

            context.Commands.ImportTemplateCommand = 
                new DelegateCommand<object>(
                    (item) => ImportObject(item, ImportType.Template),
                    (item) => context.IsEditMode());

            context.Commands.ImportTemplatesCommand = 
                new DelegateCommand<object>(
                    (item) => ImportObject(item, ImportType.Templates),
                    (item) => context.IsEditMode());

            context.Commands.ExportStyleCommand = 
                new DelegateCommand<object>(
                    (item) => ExportObject(item, ExportType.Style),
                    (item) => context.IsEditMode());

            context.Commands.ExportStylesCommand = 
                new DelegateCommand<object>(
                    (item) => ExportObject(item, ExportType.Styles),
                    (item) => context.IsEditMode());

            context.Commands.ExportStyleLibraryCommand = 
                new DelegateCommand<object>(
                    (item) => ExportObject(item, ExportType.StyleLibrary),
                    (item) => context.IsEditMode());

            context.Commands.ExportStyleLibrariesCommand = 
                new DelegateCommand<object>(
                    (item) => ExportObject(item, ExportType.StyleLibraries),
                    (item) => context.IsEditMode());

            context.Commands.ExportGroupCommand = 
                new DelegateCommand<object>(
                    (item) => ExportObject(item, ExportType.Group),
                    (item) => context.IsEditMode());

            context.Commands.ExportGroupsCommand = 
                new DelegateCommand<object>(
                    (item) => ExportObject(item, ExportType.Groups),
                    (item) => context.IsEditMode());

            context.Commands.ExportGroupLibraryCommand = 
                new DelegateCommand<object>(
                    (item) => ExportObject(item, ExportType.GroupLibrary),
                    (item) => context.IsEditMode());

            context.Commands.ExportGroupLibrariesCommand = 
                new DelegateCommand<object>(
                    (item) => ExportObject(item, ExportType.GroupLibraries),
                    (item) => context.IsEditMode());

            context.Commands.ExportTemplateCommand = 
                new DelegateCommand<object>(
                    (item) => ExportObject(item, ExportType.Template),
                    (item) => context.IsEditMode());

            context.Commands.ExportTemplatesCommand = 
                new DelegateCommand<object>(
                    (item) => ExportObject(item, ExportType.Templates),
                    (item) => context.IsEditMode());

            context.Commands.CopyAsEmfCommand = 
                new DelegateCommand(
                    () => (new EmfWriter()).SetClipboard(context.Editor.Project.CurrentContainer),
                    () => context.IsEditMode());

            context.Commands.EvalCommand = 
                new DelegateCommand(
                    () => Eval(),
                    () => context.IsEditMode());

            context.Commands.ZoomResetCommand = 
                new DelegateCommand(
                    () => grid.ResetZoomAndPan(),
                    () => true);

            context.Commands.ZoomExtentCommand = 
                new DelegateCommand(
                    () => grid.AutoFit(),
                    () => true);

            context.Commands.DatabasesWindowCommand = 
                new DelegateCommand(
                    () => (new DatabasesWindow() { Owner = this, DataContext = context }).Show(),
                    () => true);

            context.Commands.LayersWindowCommand = 
                new DelegateCommand(
                    () => (new LayersWindow() { Owner = this, DataContext = context }).Show(),
                    () => true);

            context.Commands.StyleWindowCommand = 
                new DelegateCommand(
                    () =>(new StyleWindow() { Owner = this, DataContext = context }).Show(),
                    () => true);

            context.Commands.StylesWindowCommand = 
                new DelegateCommand(
                    () => (new StylesWindow() { Owner = this, DataContext = context }).Show(),
                    () => true);

            context.Commands.ShapesWindowCommand = 
                new DelegateCommand(
                    () => (new ShapesWindow() { Owner = this, DataContext = context }).Show(),
                    () => true);

            context.Commands.DocumentWindowCommand = 
                new DelegateCommand(
                    () => (new DocumentWindow() { Owner = this, DataContext = context }).Show(),
                    () => true);

            context.Commands.ScriptWindowCommand =
                new DelegateCommand(
                    () => (new ScriptWindow() { Owner = this, DataContext = context.ScriptDirectories }).Show(),
                    () => true);

            var pw = default(PropertiesWindow);

            context.Commands.PropertiesWindowCommand = 
                new DelegateCommand(
                    () =>
                    {
                        if (pw == null)
                        {
                            pw = new PropertiesWindow() { Owner = this, DataContext = context };
                            pw.Unloaded += (_s, _e) => pw = default(PropertiesWindow);
                        }
                        pw.Show();
                    },
                    () => true);

            context.Editor.PropertyChanged +=
                (s, e) =>
                {
                    if (e.PropertyName == "IsContextMenu")
                    {
                        if (context.Editor.IsContextMenu)
                        {
                            context.Editor.IsContextMenu = false;
                            
                            if (pw == null)
                            {
                                pw = new PropertiesWindow() { Owner = this, DataContext = context };
                                pw.Unloaded += (_s, _e) => pw = default(PropertiesWindow);
                            }
                            pw.Show();
                        }
                    }
                };

            grid.EnableAutoFit = context.Renderers[0].State.EnableAutofit;

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
                        grid.AutoFit();
                    }
                
                    if (e.ChangedButton == MouseButton.Middle && e.ClickCount == 3)
                    {
                        grid.ResetZoomAndPan();
                    }
                };

            Loaded += 
                (s, e) =>
                {
                    if (_isLoaded)
                        return;
                    else
                        _isLoaded = true;
                };

            Unloaded += (s, e) =>
            {
                if (!_isLoaded)
                    return;
                else
                    _isLoaded = false;

                DeInitializeContext();
            };

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
                            System.Diagnostics.Debug.Print(ex.Message);
                            System.Diagnostics.Debug.Print(ex.StackTrace);
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
                                context.Drop(group, p.X, p.Y);
                                e.Handled = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.Print(ex.Message);
                            System.Diagnostics.Debug.Print(ex.StackTrace);
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
                            System.Diagnostics.Debug.Print(ex.Message);
                            System.Diagnostics.Debug.Print(ex.StackTrace);
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
                            System.Diagnostics.Debug.Print(ex.Message);
                            System.Diagnostics.Debug.Print(ex.StackTrace);
                        }
                    }
                };

            DataContext = context;
        }

        /// <summary>
        /// 
        /// </summary>
        private void DeInitializeContext()
        {
            (DataContext as EditorContext).Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ImportData()
        {
            var dlg = new OpenFileDialog()
            {
                Filter = "Csv (*.csv)|*.csv|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = ""
            };

            if (dlg.ShowDialog() == true)
            {
                (DataContext as EditorContext).ImportData(dlg.FileName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ExportData()
        {
            var context = DataContext as EditorContext;
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
        public void UpdateData()
        {
            var context = DataContext as EditorContext;
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
                (DataContext as EditorContext).UpdateData(dlg.FileName, database);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Eval()
        {
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
                    (DataContext as EditorContext).Eval(path);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Open()
        {
            var dlg = new OpenFileDialog()
            {
                Filter = "Project (*.project)|*.project|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = ""
            };

            if (dlg.ShowDialog() == true)
            {
                (DataContext as EditorContext).Open(dlg.FileName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SaveAs()
        {
            var dlg = new SaveFileDialog()
            {
                Filter = "Project (*.project)|*.project|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = (DataContext as EditorContext).Editor.Project.Name
            };

            if (dlg.ShowDialog() == true)
            {
                (DataContext as EditorContext).Save(dlg.FileName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="type"></param>
        public void ImportObject(object item, ImportType type)
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
                    filter = "StyleLibrary (*.StyleLibrary)|*.StyleLibrary|All (*.*)|*.*";
                    break;
                case ImportType.StyleLibraries:
                    filter = "StyleLibraries (*.StyleLibraries)|*.StyleLibraries|All (*.*)|*.*";
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
                var context = DataContext as EditorContext;
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
        public void ExportObject(object item, ExportType type)
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
                    name = (item as StyleLibrary).Name;
                    break;
                case ExportType.StyleLibrary:
                    filter = "StyleLibrary (*.StyleLibrary)|*.StyleLibrary|All (*.*)|*.*";
                    name = (item as StyleLibrary).Name;
                    break;
                case ExportType.StyleLibraries:
                    filter = "StyleLibraries (*.StyleLibraries)|*.StyleLibraries|All (*.*)|*.*";
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
                var context = DataContext as EditorContext;
                context.ExportEx(path, item, type);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        public void ExportEmf(string path)
        {
            try
            {
                var context = (DataContext as EditorContext);
                var container = context.Editor.Project.CurrentContainer;
                (new EmfWriter()).Save(path, container);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Export(object item)
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
            else if (item is EditorContext)
            {
                var editor = (item as EditorContext).Editor;
                name = editor.Project.Name;
                item = editor.Project;
            }
            else if (item == null)
            {
                var editor = (DataContext as EditorContext).Editor;
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
                        (DataContext as EditorContext).ExportAsPdf(dlg.FileName, item);
                        System.Diagnostics.Process.Start(dlg.FileName);
                        break;
                    case 2:
                        ExportEmf(dlg.FileName);
                        System.Diagnostics.Process.Start(dlg.FileName);
                        break;
                    case 3:
                        (DataContext as EditorContext).ExportAsDxf(dlg.FileName, Dxf.DxfAcadVer.AC1015);
                        System.Diagnostics.Process.Start(dlg.FileName);
                        break;
                    case 4:
                        (DataContext as EditorContext).ExportAsDxf(dlg.FileName, Dxf.DxfAcadVer.AC1006);
                        System.Diagnostics.Process.Start(dlg.FileName);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Image()
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
    }
}
