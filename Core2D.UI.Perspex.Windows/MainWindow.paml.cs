// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Perspex.Controls;
using Perspex.Markup.Xaml;
using Core2D;

namespace TestPerspex
{
    /// <summary>
    /// 
    /// </summary>
    public class MainWindow : Window, IView
    {
        private EditorContext _context;
        private string _recentFileName = "Core2D.recent";
        private string _logFileName = "Core2D.log";
        private bool _enableRecent = true;
        
        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            App.AttachDevTools(this);
            
            this.InitializeContext();
            this.Closed += (sender, e) => this.DeInitializeContext();
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeComponent()
        {
            PerspexXamlLoader.Load(this);
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
        /// 
        /// </summary>
        private void InitializeContext()
        {
            _context = new EditorContext()
            {
                View = this,
                Renderers = new IRenderer[] { new PerspexRenderer() },
                ProjectFactory = new ProjectFactory(),
                TextClipboard = new TextClipboard(),
                Serializer = new NewtonsoftSerializer(),
                PdfWriter = new PdfWriter(),
                DxfWriter = new DxfWriter(),
                CsvReader = new CsvHelperReader(),
                CsvWriter = new CsvHelperWriter()
            };

            _context.InitializeEditor(new TraceLog(), System.IO.Path.Combine(GetAssemblyPath(), _logFileName));

            _context.Editor.Renderers[0].State.DrawShapeState.Flags = ShapeStateFlags.Visible;
            _context.Editor.GetImageKey = async () => await OnGetImageKey();

            _context.Commands.OpenCommand =
                Command<object>.Create(
                    async (parameter) => await OnOpen(parameter),
                    (parameter) => _context.IsEditMode());

            _context.Commands.SaveCommand =
                Command.Create(
                    async () => await OnSave(),
                    () => _context.IsEditMode());

            _context.Commands.SaveAsCommand =
                Command.Create(
                    async () => await OnSaveAs(),
                    () => _context.IsEditMode());

            _context.Commands.ExportCommand =
                Command<object>.Create(
                    async (item) => await OnExport(),
                    (item) => _context.IsEditMode());

            _context.Commands.ImportDataCommand =
                Command<object>.Create(
                    async (item) => await OnImportData(),
                    (item) => _context.IsEditMode());

            _context.Commands.ExportDataCommand =
                Command<object>.Create(
                    async (item) => await OnExportData(),
                    (item) => _context.IsEditMode());

            _context.Commands.UpdateDataCommand =
                Command<object>.Create(
                    async (item) => await OnUpdateData(),
                    (item) => _context.IsEditMode());
            
            _context.Commands.ImportStyleCommand =
                Command<object>.Create(
                    async (item) => await OnImportObject(item, ImportType.Style),
                    (item) => _context.IsEditMode());

            _context.Commands.ImportStylesCommand =
                Command<object>.Create(
                    async (item) => await OnImportObject(item, ImportType.Styles),
                    (item) => _context.IsEditMode());

            _context.Commands.ImportStyleLibraryCommand =
                Command<object>.Create(
                    async (item) => await OnImportObject(item, ImportType.StyleLibrary),
                    (item) => _context.IsEditMode());

            _context.Commands.ImportStyleLibrariesCommand =
                Command<object>.Create(
                    async (item) => await OnImportObject(item, ImportType.StyleLibraries),
                    (item) => _context.IsEditMode());

            _context.Commands.ImportGroupCommand =
                Command<object>.Create(
                    async (item) => await OnImportObject(item, ImportType.Group),
                    (item) => _context.IsEditMode());

            _context.Commands.ImportGroupsCommand =
                Command<object>.Create(
                    async (item) => await OnImportObject(item, ImportType.Groups),
                    (item) => _context.IsEditMode());

            _context.Commands.ImportGroupLibraryCommand =
                Command<object>.Create(
                    async (item) => await OnImportObject(item, ImportType.GroupLibrary),
                    (item) => _context.IsEditMode());

            _context.Commands.ImportGroupLibrariesCommand =
                Command<object>.Create(
                    async (item) => await OnImportObject(item, ImportType.GroupLibraries),
                    (item) => _context.IsEditMode());

            _context.Commands.ImportTemplateCommand =
                Command<object>.Create(
                    async (item) => await OnImportObject(item, ImportType.Template),
                    (item) => _context.IsEditMode());

            _context.Commands.ImportTemplatesCommand =
                Command<object>.Create(
                    async (item) => await OnImportObject(item, ImportType.Templates),
                    (item) => _context.IsEditMode());

            _context.Commands.ExportStyleCommand =
                Command<object>.Create(
                    async (item) => await OnExportObject(item, ExportType.Style),
                    (item) => _context.IsEditMode());

            _context.Commands.ExportStylesCommand =
                Command<object>.Create(
                    async (item) => await OnExportObject(item, ExportType.Styles),
                    (item) => _context.IsEditMode());

            _context.Commands.ExportStyleLibraryCommand =
                Command<object>.Create(
                    async (item) => await OnExportObject(item, ExportType.StyleLibrary),
                    (item) => _context.IsEditMode());

            _context.Commands.ExportStyleLibrariesCommand =
                Command<object>.Create(
                    async (item) => await OnExportObject(item, ExportType.StyleLibraries),
                    (item) => _context.IsEditMode());

            _context.Commands.ExportGroupCommand =
                Command<object>.Create(
                    async (item) => await OnExportObject(item, ExportType.Group),
                    (item) => _context.IsEditMode());

            _context.Commands.ExportGroupsCommand =
                Command<object>.Create(
                    async (item) => await OnExportObject(item, ExportType.Groups),
                    (item) => _context.IsEditMode());

            _context.Commands.ExportGroupLibraryCommand =
                Command<object>.Create(
                    async (item) => await OnExportObject(item, ExportType.GroupLibrary),
                    (item) => _context.IsEditMode());

            _context.Commands.ExportGroupLibrariesCommand =
                Command<object>.Create(
                    async (item) => await OnExportObject(item, ExportType.GroupLibraries),
                    (item) => _context.IsEditMode());

            _context.Commands.ExportTemplateCommand =
                Command<object>.Create(
                    async (item) => await OnExportObject(item, ExportType.Template),
                    (item) => _context.IsEditMode());

            _context.Commands.ExportTemplatesCommand =
                Command<object>.Create(
                    async (item) => await OnExportObject(item, ExportType.Templates),
                    (item) => _context.IsEditMode());

            // TODO: Initialize other commands.

            if (_enableRecent)
            {
                try
                {
                    var path = System.IO.Path.Combine(GetAssemblyPath(), _recentFileName);
                    if (System.IO.File.Exists(path))
                    {
                        _context.LoadRecent(path);
                    }
                }
                catch (Exception ex)
                {
                    if (_context.Editor.Log != null)
                    {
                        _context.Editor.Log.LogError("{0}{1}{2}",
                            ex.Message,
                            Environment.NewLine,
                            ex.StackTrace);
                    }
                }
            }
            
            DataContext = _context;
        }

        /// <summary>
        /// 
        /// </summary>
        private void DeInitializeContext()
        {
            if (_enableRecent)
            {
                try
                {
                    var path = System.IO.Path.Combine(GetAssemblyPath(), _recentFileName);
                    _context.SaveRecent(path);
                }
                catch (Exception ex)
                {
                    if (_context.Editor.Log != null)
                    {
                        _context.Editor.Log.LogError("{0}{1}{2}",
                            ex.Message,
                            Environment.NewLine,
                            ex.StackTrace);
                    }
                }
            }
            
            _context.Dispose();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<string> OnGetImageKey()
        {
            var dlg = new OpenFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
            var result = await dlg.ShowAsync(this);
            if (result != null)
            {
                var path = result.FirstOrDefault();
                var bytes = System.IO.File.ReadAllBytes(path);
                var key = _context.Editor.Project.AddImageFromFile(path, bytes);
                return key;
            }
            return null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        private async Task OnOpen(object parameter)
        {
            if (parameter == null)
            {
                var dlg = new OpenFileDialog();
                dlg.Filters.Add(new FileDialogFilter() { Name = "Project", Extensions = { "project" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                var result = await dlg.ShowAsync(this);
                if (result != null)
                {
                    var path = result.FirstOrDefault();
                    _context.Open(path);
                    if (_context.Invalidate != null)
                    {
                        _context.Invalidate();
                    }
                }
            }
            else
            {
                string path = parameter as string;
                if (path != null && System.IO.File.Exists(path))
                {
                    _context.Open(path);
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task OnSave()
        {
            if (!string.IsNullOrEmpty(_context.Editor.ProjectPath))
            {
                _context.Save(_context.Editor.ProjectPath);
            }
            else
            {
                await OnSaveAs();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task OnSaveAs()
        {
            if (_context.Editor.Project != null)
            {
                var dlg = new SaveFileDialog();
                dlg.Filters.Add(new FileDialogFilter() { Name = "Project", Extensions = { "project" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                dlg.InitialFileName = _context.Editor.Project.Name;
                var result = await dlg.ShowAsync(this);
                if (result != null)
                {
                    _context.Save(result);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task OnExport()
        {
            if (_context.Editor.Project != null)
            {
                var dlg = new SaveFileDialog();
                dlg.Filters.Add(new FileDialogFilter() { Name = "Pdf", Extensions = { "pdf" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "Dxf", Extensions = { "dxf" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                dlg.InitialFileName = _context.Editor.Project.Name;
                var result = await dlg.ShowAsync(this);
                if (result != null)
                {
                    var ext = System.IO.Path.GetExtension(result).ToLower();
    
                    if (ext == ".pdf")
                    {
                        _context.ExportAsPdf(result, _context.Editor.Project);
                        Process.Start(result);
                    }
    
                    if (ext == ".dxf")
                    {
                        _context.ExportAsDxf(result);
                        Process.Start(result);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task OnImportData()
        {
            if (_context.Editor.Project != null)
            {
                var dlg = new OpenFileDialog();
                dlg.Filters.Add(new FileDialogFilter() { Name = "Csv", Extensions = { "csv" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                var result = await dlg.ShowAsync(this);
                if (result != null)
                {
                    var path = result.FirstOrDefault();
                    _context.ImportData(path);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task OnExportData()
        {
            if (_context.Editor.Project != null && _context.Editor.Project.CurrentDatabase != null)
            {
                var database = _context.Editor.Project.CurrentDatabase;

                var dlg = new SaveFileDialog();
                dlg.Filters.Add(new FileDialogFilter() { Name = "Csv", Extensions = { "csv" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                dlg.InitialFileName = database.Name;
                var result = await dlg.ShowAsync(this);
                if (result != null)
                {
                    _context.ExportData(result, database);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task OnUpdateData()
        {
            if (_context.Editor.Project != null && _context.Editor.Project.CurrentDatabase != null)
            {
                var database = _context.Editor.Project.CurrentDatabase;
                
                var dlg = new OpenFileDialog();
                dlg.Filters.Add(new FileDialogFilter() { Name = "Csv", Extensions = { "csv" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                var result = await dlg.ShowAsync(this);
                if (result != null)
                {
                    var path = result.FirstOrDefault();
                    _context.UpdateData(path, database);
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="type"></param>
        private async Task OnImportObject(object item, ImportType type)
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
                var results = await dlg.ShowAsync(this);
                if (results != null)
                {
                    foreach (var path in results)
                    {
                        _context.ImportObject(path, item, type);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="type"></param>
        private async Task OnExportObject(object item, ExportType type)
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
                        initial = (item as StyleLibrary).Name;
                        break;
                    case ExportType.StyleLibrary:
                        name = "StyleLibrary";
                        ext = "stylelibrary";
                        initial = (item as StyleLibrary).Name;
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
                        initial = (item as GroupLibrary).Name;
                        break;
                    case ExportType.GroupLibrary:
                        name = "GroupLibrary";
                        ext = "grouplibrary";
                        initial = (item as GroupLibrary).Name;
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
                var result = await dlg.ShowAsync(this);
                if (result != null)
                {
                    _context.ExportObject(result, item, type);
                }
            }
        }
    }
}
