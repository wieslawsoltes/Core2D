using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Avalonia.Controls;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Editor;
using Core2D.FileWriter.Emf;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.SvgExporter.Svg;
using Core2D.Views;
using Core2D.XamlExporter.Avalonia;
using Microsoft.CodeAnalysis;

namespace Core2D.Editor
{
    public class AvaloniaProjectEditorPlatform : IProjectEditorPlatform
    {
        private readonly IServiceProvider _serviceProvider;

        public AvaloniaProjectEditorPlatform(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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
                if (result != null)
                {
                    var item = result.FirstOrDefault();
                    if (item != null)
                    {
                        var editor = _serviceProvider.GetService<ProjectEditor>();
                        editor.OnOpenProject(item);
                        editor.CanvasPlatform?.InvalidateControl?.Invoke();
                    }
                }
            }
            else
            {
                if (_serviceProvider.GetService<IFileSystem>().Exists(path))
                {
                    _serviceProvider.GetService<ProjectEditor>().OnOpenProject(path);
                }
            }
        }

        public void OnSave()
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
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
            var editor = _serviceProvider.GetService<ProjectEditor>();
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
                            _serviceProvider.GetService<ProjectEditor>().OnImportJson(item);
                        }
                    }
                }
            }
            else
            {
                if (_serviceProvider.GetService<IFileSystem>().Exists(path))
                {
                    _serviceProvider.GetService<ProjectEditor>().OnImportJson(path);
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
                            _serviceProvider.GetService<ProjectEditor>().OnImportSvg(item);
                        }
                    }
                }
            }
            else
            {
                if (_serviceProvider.GetService<IFileSystem>().Exists(path))
                {
                    _serviceProvider.GetService<ProjectEditor>().OnImportJson(path);
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
                                _serviceProvider.GetService<ProjectEditor>().OnImportJson(item);
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
                        _serviceProvider.GetService<ProjectEditor>().OnImportJson(path);
                    }
                }
            }
        }

        public async void OnExportJson(object item)
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
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
            var editor = _serviceProvider.GetService<ProjectEditor>();
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
            var editor = _serviceProvider.GetService<ProjectEditor>();

            string name = string.Empty;

            if (item == null || item is ProjectEditor)
            {
                if (editor.Project == null)
                {
                    return;
                }

                name = editor.Project.Name;
                item = editor.Project;
            }
            else if (item is ProjectContainer project)
            {
                name = project.Name;
            }
            else if (item is DocumentContainer document)
            {
                name = document.Name;
            }
            else if (item is PageContainer page)
            {
                name = page.Name;
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
                        await _serviceProvider.GetService<ProjectEditor>().OnExecuteScriptFile(result);
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
                    var editor = _serviceProvider.GetService<ProjectEditor>();
                    var exporter = new SvgSvgExporter(_serviceProvider);
                    var container = editor.Project.CurrentContainer;

                    var sources = editor.PageState?.SelectedShapes;
                    if (sources != null)
                    {
                        var xaml = exporter.Create(sources, container.Width, container.Height);
                        if (!string.IsNullOrEmpty(xaml))
                        {
                            editor.TextClipboard?.SetText(xaml);
                        }
                        return;
                    }

                    var shapes = container.Layers.SelectMany(x => x.Shapes);
                    if (shapes != null)
                    {
                        var xaml = exporter.Create(shapes, container.Width, container.Height);
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
                var editor = _serviceProvider.GetService<ProjectEditor>();
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
                    var editor = _serviceProvider.GetService<ProjectEditor>();
                    var exporter = new DrawingGroupXamlExporter(_serviceProvider);
                    var container = editor.Project.CurrentContainer;

                    var sources = editor.PageState?.SelectedShapes;
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
                var editor = _serviceProvider.GetService<ProjectEditor>();
                var imageChache = editor.Project as IImageCache;
                var page = editor.Project.CurrentContainer;
                var shapes = editor.PageState.SelectedShapes;
                var writer = editor.FileWriters.FirstOrDefault(x => x.GetType() == typeof(EmfWriter)) as EmfWriter;

                var db = (object)page.Properties;
                var record = (object)page.Record;
                editor.DataFlow.Bind(page.Template, db, record);
                editor.DataFlow.Bind(page, db, record);

                using var bitmap = new System.Drawing.Bitmap((int)page.Width, (int)page.Height);

                if (shapes != null && shapes.Count > 0)
                {
                    using var ms = writer.MakeMetafileStream(bitmap, shapes, imageChache);
                    ms.Position = 0;
                    SetClipboardMetafile(ms);
                }
                else
                {
                    using var ms = writer.MakeMetafileStream(bitmap, page, imageChache);
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
                    var editor = _serviceProvider.GetService<ProjectEditor>();
                    var converter = editor.PathConverter;
                    var container = editor.Project.CurrentContainer;

                    var shapes = editor.PageState?.SelectedShapes ?? container?.Layers.SelectMany(x => x.Shapes);
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
                var editor = _serviceProvider.GetService<ProjectEditor>();
                var converter = editor.PathConverter;

                var svgPath = await editor.TextClipboard?.GetText();
                if (!string.IsNullOrEmpty(svgPath))
                {
                    var pathShape = converter.FromSvgPathData(svgPath, isStroked: true, isFilled: false);
                    if (pathShape != null)
                    {
                        editor.OnPasteShapes(Enumerable.Repeat<BaseShape>(pathShape, 1));
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
                var editor = _serviceProvider.GetService<ProjectEditor>();
                var converter = editor.PathConverter;

                var svgPath = await editor.TextClipboard?.GetText();
                if (!string.IsNullOrEmpty(svgPath))
                {
                    var pathShape = converter.FromSvgPathData(svgPath, isStroked: false, isFilled: true);
                    if (pathShape != null)
                    {
                        editor.OnPasteShapes(Enumerable.Repeat<BaseShape>(pathShape, 1));
                    }
                }
            }
            catch (Exception ex)
            {
                _serviceProvider.GetService<ILog>()?.LogException(ex);
            }
        }

        public async void OnImportData(ProjectContainer project)
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
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

        public async void OnExportData(Database db)
        {
            if (db != null)
            {
                var editor = _serviceProvider.GetService<ProjectEditor>();
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

        public async void OnUpdateData(Database db)
        {
            if (db != null)
            {
                var editor = _serviceProvider.GetService<ProjectEditor>();
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
            new AboutWindow()
            {
                DataContext = _serviceProvider.GetService<ProjectEditor>()
            }
            .ShowDialog(GetWindow());
        }

        public void OnZoomReset()
        {
            _serviceProvider.GetService<ProjectEditor>().CanvasPlatform?.ResetZoom?.Invoke();
        }

        public void OnZoomFill()
        {
            _serviceProvider.GetService<ProjectEditor>().CanvasPlatform?.FillZoom?.Invoke();
        }

        public void OnZoomUniform()
        {
            _serviceProvider.GetService<ProjectEditor>().CanvasPlatform?.UniformZoom?.Invoke();
        }

        public void OnZoomUniformToFill()
        {
            _serviceProvider.GetService<ProjectEditor>().CanvasPlatform?.UniformToFillZoom?.Invoke();
        }

        public void OnZoomAutoFit()
        {
            _serviceProvider.GetService<ProjectEditor>().CanvasPlatform?.AutoFitZoom?.Invoke();
        }

        public void OnZoomIn()
        {
            _serviceProvider.GetService<ProjectEditor>().CanvasPlatform?.InZoom?.Invoke();
        }

        public void OnZoomOut()
        {
            _serviceProvider.GetService<ProjectEditor>().CanvasPlatform?.OutZoom?.Invoke();
        }
    }
}
