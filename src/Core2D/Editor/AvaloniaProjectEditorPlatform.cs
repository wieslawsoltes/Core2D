using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Avalonia.Controls;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Renderer;
using Core2D.Modules.FileWriter.Emf;
using Core2D.Modules.SvgExporter.Svg;
using Core2D.Modules.XamlExporter.Avalonia;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Shapes;
using Core2D.Views;

namespace Core2D.Editor
{
    public partial class AvaloniaProjectEditorPlatform : ViewModelBase, IProjectEditorPlatform
    {
        public AvaloniaProjectEditorPlatform(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        private MainWindow GetWindow()
        {
            return _serviceProvider.GetService<MainWindow>();
        }

        public async void OnOpen(string path)
        {
            if (path == null)
            {
                var dlg = new OpenFileDialog() { Title = "Open" };
                dlg.Filters.Add(new FileDialogFilter() { Name = "Project", Extensions = { "project" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                var result = await dlg.ShowAsync(GetWindow());
                var item = result?.FirstOrDefault();
                if (item != null)
                {
                    var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
                    editor.OnOpenProject(item);
                    editor.CanvasPlatform?.InvalidateControl?.Invoke();
                }
            }
            else
            {
                if (_serviceProvider.GetService<IFileSystem>().Exists(path))
                {
                    _serviceProvider.GetService<ProjectEditorViewModel>().OnOpenProject(path);
                }
            }
        }

        public void OnSave()
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            if (!string.IsNullOrEmpty(editor.ProjectPath))
            {
                editor.OnSaveProject(editor.ProjectPath);
            }
            else
            {
                OnSaveAs();
            }
        }

        public async void OnSaveAs()
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var dlg = new SaveFileDialog() { Title = "Save" };
            dlg.Filters.Add(new FileDialogFilter() { Name = "Project", Extensions = { "project" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
            dlg.InitialFileName = editor.Project?.Name;
            dlg.DefaultExtension = "project";
            var result = await dlg.ShowAsync(GetWindow());
            if (result != null)
            {
                editor.OnSaveProject(result);
            }
        }

        public async void OnImportJson(string path)
        {
            if (path == null)
            {
                var dlg = new OpenFileDialog() { Title = "Open" };
                dlg.AllowMultiple = true;
                dlg.Filters.Add(new FileDialogFilter() { Name = "Json", Extensions = { "json" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });

                var result = await dlg.ShowAsync(GetWindow());
                if (result != null)
                {
                    foreach (var item in result)
                    {
                        if (item != null)
                        {
                            _serviceProvider.GetService<ProjectEditorViewModel>().OnImportJson(item);
                        }
                    }
                }
            }
            else
            {
                if (_serviceProvider.GetService<IFileSystem>().Exists(path))
                {
                    _serviceProvider.GetService<ProjectEditorViewModel>().OnImportJson(path);
                }
            }
        }

        public async void OnImportSvg(string path)
        {
            if (path == null)
            {
                var dlg = new OpenFileDialog() { Title = "Open" };
                dlg.AllowMultiple = true;
                dlg.Filters.Add(new FileDialogFilter() { Name = "Svg", Extensions = { "svg" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });

                var result = await dlg.ShowAsync(GetWindow());
                if (result != null)
                {
                    foreach (var item in result)
                    {
                        if (item != null)
                        {
                            _serviceProvider.GetService<ProjectEditorViewModel>().OnImportSvg(item);
                        }
                    }
                }
            }
            else
            {
                if (_serviceProvider.GetService<IFileSystem>().Exists(path))
                {
                    _serviceProvider.GetService<ProjectEditorViewModel>().OnImportJson(path);
                }
            }
        }

        public async void OnImportObject(string path)
        {
            if (path == null)
            {
                var dlg = new OpenFileDialog() { Title = "Open" };
                dlg.AllowMultiple = true;
                dlg.Filters.Add(new FileDialogFilter() { Name = "Json", Extensions = { "json" } });
                var result = await dlg.ShowAsync(GetWindow());
                if (result != null)
                {
                    foreach (var item in result)
                    {
                        if (item != null)
                        {
                            string resultExtension = System.IO.Path.GetExtension(item);
                            if (string.Compare(resultExtension, ".json", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                _serviceProvider.GetService<ProjectEditorViewModel>().OnImportJson(item);
                            }
                        }
                    }
                }
            }
            else
            {
                if (_serviceProvider.GetService<IFileSystem>().Exists(path))
                {
                    string resultExtension = System.IO.Path.GetExtension(path);
                    if (string.Compare(resultExtension, ".json", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        _serviceProvider.GetService<ProjectEditorViewModel>().OnImportJson(path);
                    }
                }
            }
        }

        public async void OnExportJson(object item)
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var dlg = new SaveFileDialog() { Title = "Save" };
            dlg.Filters.Add(new FileDialogFilter() { Name = "Json", Extensions = { "json" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
            dlg.InitialFileName = editor?.GetName(item);
            dlg.DefaultExtension = "json";
            var result = await dlg.ShowAsync(GetWindow());
            if (result != null)
            {
                editor.OnExportJson(result, item);
            }
        }

        public async void OnExportObject(object item)
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            if (item != null)
            {
                var dlg = new SaveFileDialog() { Title = "Save" };
                dlg.Filters.Add(new FileDialogFilter() { Name = "Json", Extensions = { "json" } });
                dlg.InitialFileName = editor?.GetName(item);
                dlg.DefaultExtension = "json";
                var result = await dlg.ShowAsync(GetWindow());
                if (result != null)
                {
                    string resultExtension = System.IO.Path.GetExtension(result);
                    if (string.Compare(resultExtension, ".json", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        editor.OnExportJson(result, item);
                    }
                }
            }
        }

        public async void OnExport(object item)
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();

            string name = string.Empty;

            if (item == null || item is ProjectEditorViewModel)
            {
                if (editor.Project == null)
                {
                    return;
                }

                name = editor.Project.Name;
                item = editor.Project;
            }
            else if (item is ProjectContainerViewModel project)
            {
                name = project.Name;
            }
            else if (item is DocumentContainerViewModel document)
            {
                name = document.Name;
            }
            else if (item is BaseContainerViewModel container)
            {
                name = container.Name;
            }

            var dlg = new SaveFileDialog() { Title = "Save" };
            foreach (var writer in editor?.FileWriters)
            {
                dlg.Filters.Add(new FileDialogFilter() { Name = writer.Name, Extensions = { writer.Extension } });
            }
            dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
            dlg.InitialFileName = name;
            dlg.DefaultExtension = editor?.FileWriters.FirstOrDefault()?.Extension;

            var result = await dlg.ShowAsync(GetWindow());
            if (result != null)
            {
                string ext = System.IO.Path.GetExtension(result).ToLower().TrimStart('.');
                var writer = editor.FileWriters.Where(w => string.Compare(w.Extension, ext, StringComparison.OrdinalIgnoreCase) == 0).FirstOrDefault();
                if (writer != null)
                {
                    editor.OnExport(result, item, writer);
                }
            }
        }

        public async void OnExecuteScriptFile(string path)
        {
            if (path == null)
            {
                var dlg = new OpenFileDialog() { Title = "Open" };
                dlg.Filters.Add(new FileDialogFilter() { Name = "Script", Extensions = { "csx", "cs" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                dlg.AllowMultiple = true;
                var result = await dlg.ShowAsync(GetWindow());
                if (result != null)
                {
                    if (result.All(r => r != null))
                    {
                        await _serviceProvider.GetService<ProjectEditorViewModel>().OnExecuteScriptFile(result);
                    }
                }
            }
        }

        public void OnExit()
        {
            GetWindow().Close();
        }

        public void OnCopyAsSvg(object item)
        {
            try
            {
                if (item == null)
                {
                    var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
                    var exporter = new SvgSvgExporter(_serviceProvider);
                    var container = editor.Project.CurrentContainer;

                    var width = 0.0;
                    var height = 0.0;
                    switch (container)
                    {
                        case TemplateContainerViewModel template:
                            width = template.Width;
                            height = template.Height;
                            break;
                        case PageContainerViewModel page:
                            width = page.Template.Width;
                            height = page.Template.Height;
                            break;
                    }

                    var sources = editor.Project?.SelectedShapes;
                    if (sources != null)
                    {
                        var xaml = exporter.Create(sources, width, height);
                        if (!string.IsNullOrEmpty(xaml))
                        {
                            editor.TextClipboard?.SetText(xaml);
                        }
                        return;
                    }

                    var shapes = container.Layers.SelectMany(x => x.Shapes);
                    if (shapes != null)
                    {
                        var xaml = exporter.Create(shapes, width, height);
                        if (!string.IsNullOrEmpty(xaml))
                        {
                            editor.TextClipboard?.SetText(xaml);
                        }
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                _serviceProvider.GetService<ILog>()?.LogException(ex);
            }
        }

        public async void OnPasteSvg()
        {
            try
            {
                var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
                var converter = editor.SvgConverter;

                var svgText = await editor.TextClipboard?.GetText();
                if (!string.IsNullOrEmpty(svgText))
                {
                    var shapes = converter.FromString(svgText, out _, out _);
                    if (shapes != null)
                    {
                        editor.OnPasteShapes(shapes);
                    }
                }
            }
            catch (Exception ex)
            {
                _serviceProvider.GetService<ILog>()?.LogException(ex);
            }
        }

        public void OnCopyAsXaml(object item)
        {
            try
            {
                if (item == null)
                {
                    var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
                    var exporter = new DrawingGroupXamlExporter(_serviceProvider);
                    var container = editor.Project.CurrentContainer;

                    var sources = editor.Project?.SelectedShapes;
                    if (sources != null)
                    {
                        var xaml = exporter.Create(sources, null);
                        if (!string.IsNullOrEmpty(xaml))
                        {
                            editor.TextClipboard?.SetText(xaml);
                        }
                        return;
                    }

                    var shapes = container.Layers.SelectMany(x => x.Shapes);
                    if (shapes != null)
                    {
                        var key = container?.Name;
                        var xaml = exporter.Create(shapes, key);
                        if (!string.IsNullOrEmpty(xaml))
                        {
                            editor.TextClipboard?.SetText(xaml);
                        }
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                _serviceProvider.GetService<ILog>()?.LogException(ex);
            }
        }

        private static class Win32
        {
            public const uint CF_ENHMETAFILE = 14;

            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool OpenClipboard(IntPtr hWndNewOwner);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool EmptyClipboard();

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool CloseClipboard();

            [DllImport("user32.dll")]
            public static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

            [DllImport("gdi32.dll")]
            public static extern IntPtr CopyEnhMetaFile(IntPtr hemetafileSrc, IntPtr hNULL);

            [DllImport("gdi32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool DeleteEnhMetaFile(IntPtr hemetafile);
        }

        private void SetClipboardMetafile(MemoryStream ms)
        {
            using var metafile = new System.Drawing.Imaging.Metafile(ms);

            var emfHandle = metafile.GetHenhmetafile();
            if (emfHandle.Equals(IntPtr.Zero))
            {
                return;
            }

            var emfCloneHandle = Win32.CopyEnhMetaFile(emfHandle, IntPtr.Zero);
            if (emfCloneHandle.Equals(IntPtr.Zero))
            {
                return;
            }

            try
            {
                if (Win32.OpenClipboard(IntPtr.Zero) && Win32.EmptyClipboard())
                {
                    Win32.SetClipboardData(Win32.CF_ENHMETAFILE, emfCloneHandle);
                    Win32.CloseClipboard();
                }
            }
            finally
            {
                Win32.DeleteEnhMetaFile(emfHandle);
            }
        }

        public void OnCopyAsEmf(object item)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                return;
            }

            try
            {
                var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
                var imageChache = editor.Project as IImageCache;
                var container = editor.Project.CurrentContainer;
                var shapes = editor.Project.SelectedShapes;
                var writer = editor.FileWriters.FirstOrDefault(x => x.GetType() == typeof(EmfWriter)) as EmfWriter;

                var db = (object)container.Properties;
                var record = (object)container.Record;

                var width = 0.0;
                var height = .0;

                if (container is PageContainerViewModel page)
                {
                    editor.DataFlow.Bind(page.Template, db, record);
                    width = page.Template.Width;
                    height = page.Template.Height;
                }
                else if (container is TemplateContainerViewModel template)
                {
                    width = template.Width;
                    height = template.Height;
                }
 
                editor.DataFlow.Bind(container, db, record);

                using var bitmap = new System.Drawing.Bitmap((int)width, (int)height);

                if (shapes != null && shapes.Count > 0)
                {
                    using var ms = writer.MakeMetafileStream(bitmap, shapes, imageChache);
                    ms.Position = 0;
                    SetClipboardMetafile(ms);
                }
                else
                {
                    using var ms = writer.MakeMetafileStream(bitmap, container, imageChache);
                    ms.Position = 0;
                    SetClipboardMetafile(ms);
                }
            }
            catch (Exception ex)
            {
                _serviceProvider.GetService<ILog>()?.LogException(ex);
            }
        }

        public async void OnCopyAsPathData(object item)
        {
            try
            {
                if (item == null)
                {
                    var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
                    var converter = editor.PathConverter;
                    var container = editor.Project.CurrentContainer;

                    var shapes = editor.Project?.SelectedShapes ?? container?.Layers.SelectMany(x => x.Shapes);
                    if (shapes == null)
                    {
                        return;
                    }

                    var sb = new StringBuilder();

                    foreach (var shape in shapes)
                    {
                        var svgPath = converter.ToSvgPathData(shape);
                        if (!string.IsNullOrEmpty(svgPath))
                        {
                            sb.Append(svgPath);
                        }
                    }

                    var result = sb.ToString();
                    if (!string.IsNullOrEmpty(result))
                    {
                        await editor.TextClipboard?.SetText(result);
                    }
                }
            }
            catch (Exception ex)
            {
                _serviceProvider.GetService<ILog>()?.LogException(ex);
            }
        }

        public async void OnPastePathDataStroked()
        {
            try
            {
                var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
                var converter = editor.PathConverter;

                var svgPath = await editor.TextClipboard?.GetText();
                if (!string.IsNullOrEmpty(svgPath))
                {
                    var pathShape = converter.FromSvgPathData(svgPath, isStroked: true, isFilled: false);
                    if (pathShape != null)
                    {
                        editor.OnPasteShapes(Enumerable.Repeat<BaseShapeViewModel>(pathShape, 1));
                    }
                }
            }
            catch (Exception ex)
            {
                _serviceProvider.GetService<ILog>()?.LogException(ex);
            }
        }

        public async void OnPastePathDataFilled()
        {
            try
            {
                var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
                var converter = editor.PathConverter;

                var svgPath = await editor.TextClipboard?.GetText();
                if (!string.IsNullOrEmpty(svgPath))
                {
                    var pathShape = converter.FromSvgPathData(svgPath, isStroked: false, isFilled: true);
                    if (pathShape != null)
                    {
                        editor.OnPasteShapes(Enumerable.Repeat<BaseShapeViewModel>(pathShape, 1));
                    }
                }
            }
            catch (Exception ex)
            {
                _serviceProvider.GetService<ILog>()?.LogException(ex);
            }
        }

        public async void OnImportData(ProjectContainerViewModel project)
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var dlg = new OpenFileDialog() { Title = "Open" };
            foreach (var reader in editor?.TextFieldReaders)
            {
                dlg.Filters.Add(new FileDialogFilter() { Name = reader.Name, Extensions = { reader.Extension } });
            }
            dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
            var result = await dlg.ShowAsync(GetWindow());

            if (result != null)
            {
                var path = result.FirstOrDefault();
                if (path == null)
                {
                    return;
                }
                string ext = System.IO.Path.GetExtension(path).ToLower().TrimStart('.');
                var reader = editor.TextFieldReaders.Where(w => string.Compare(w.Extension, ext, StringComparison.OrdinalIgnoreCase) == 0).FirstOrDefault();
                if (reader != null)
                {
                    editor.OnImportData(project, path, reader);
                }
            }
        }

        public async void OnExportData(DatabaseViewModel db)
        {
            if (db != null)
            {
                var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
                var dlg = new SaveFileDialog() { Title = "Save" };
                foreach (var writer in editor?.TextFieldWriters)
                {
                    dlg.Filters.Add(new FileDialogFilter() { Name = writer.Name, Extensions = { writer.Extension } });
                }
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                dlg.InitialFileName = db.Name;
                dlg.DefaultExtension = editor?.TextFieldWriters.FirstOrDefault()?.Extension;
                var result = await dlg.ShowAsync(GetWindow());
                if (result != null)
                {
                    string ext = System.IO.Path.GetExtension(result).ToLower().TrimStart('.');
                    var writer = editor.TextFieldWriters.Where(w => string.Compare(w.Extension, ext, StringComparison.OrdinalIgnoreCase) == 0).FirstOrDefault();
                    if (writer != null)
                    {
                        editor.OnExportData(result, db, writer);
                    }
                }
            }
        }

        public async void OnUpdateData(DatabaseViewModel db)
        {
            if (db != null)
            {
                var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
                var dlg = new OpenFileDialog() { Title = "Open" };
                foreach (var reader in editor?.TextFieldReaders)
                {
                    dlg.Filters.Add(new FileDialogFilter() { Name = reader.Name, Extensions = { reader.Extension } });
                }
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                var result = await dlg.ShowAsync(GetWindow());
                if (result != null)
                {
                    var path = result.FirstOrDefault();
                    if (path == null)
                    {
                        return;
                    }
                    string ext = System.IO.Path.GetExtension(path).ToLower().TrimStart('.');
                    var reader = editor.TextFieldReaders.Where(w => string.Compare(w.Extension, ext, StringComparison.OrdinalIgnoreCase) == 0).FirstOrDefault();
                    if (reader != null)
                    {
                        editor.OnUpdateData(path, db, reader);
                    }
                }
            }
        }

        public void OnAboutDialog()
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            if (editor.AboutInfo is { })
            {
                var dialog = new DialogViewModel(_serviceProvider, editor)
                {
                    Title = $"About {editor.AboutInfo.Title}",
                    IsOverlayVisible = true,
                    IsTitleBarVisible = true,
                    IsCloseButtonVisible = true,
                    ViewModel = editor.AboutInfo
                };
                editor.ShowDialog(dialog);
            }
        }

        public void OnZoomReset()
        {
            _serviceProvider.GetService<ProjectEditorViewModel>().CanvasPlatform?.ResetZoom?.Invoke();
        }

        public void OnZoomFill()
        {
            _serviceProvider.GetService<ProjectEditorViewModel>().CanvasPlatform?.FillZoom?.Invoke();
        }

        public void OnZoomUniform()
        {
            _serviceProvider.GetService<ProjectEditorViewModel>().CanvasPlatform?.UniformZoom?.Invoke();
        }

        public void OnZoomUniformToFill()
        {
            _serviceProvider.GetService<ProjectEditorViewModel>().CanvasPlatform?.UniformToFillZoom?.Invoke();
        }

        public void OnZoomAutoFit()
        {
            _serviceProvider.GetService<ProjectEditorViewModel>().CanvasPlatform?.AutoFitZoom?.Invoke();
        }

        public void OnZoomIn()
        {
            _serviceProvider.GetService<ProjectEditorViewModel>().CanvasPlatform?.InZoom?.Invoke();
        }

        public void OnZoomOut()
        {
            _serviceProvider.GetService<ProjectEditorViewModel>().CanvasPlatform?.OutZoom?.Invoke();
        }
    }
}
