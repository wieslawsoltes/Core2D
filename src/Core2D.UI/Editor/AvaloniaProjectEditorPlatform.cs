using System;
using System.Linq;
using System.Text;
using Avalonia.Controls;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Editor;
using Core2D.Shapes;
using Core2D.SvgExporter.Svg;
using Core2D.UI.Views;
using Core2D.XamlExporter.Avalonia;
using Microsoft.CodeAnalysis;

namespace Core2D.UI.Editor
{
    /// <summary>
    /// Project editor Avalonia platform.
    /// </summary>
    public class AvaloniaProjectEditorPlatform : IProjectEditorPlatform
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initialize new instance of <see cref="AvaloniaProjectEditorPlatform"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public AvaloniaProjectEditorPlatform(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private MainWindow GetWindow()
        {
            return _serviceProvider.GetService<MainWindow>();
        }

        /// <inheritdoc/>
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
                        var editor = _serviceProvider.GetService<IProjectEditor>();
                        editor.OnOpenProject(item);
                        editor.CanvasPlatform?.InvalidateControl?.Invoke();
                    }
                }
            }
            else
            {
                if (_serviceProvider.GetService<IFileSystem>().Exists(path))
                {
                    _serviceProvider.GetService<IProjectEditor>().OnOpenProject(path);
                }
            }
        }

        /// <inheritdoc/>
        public void OnSave()
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();
            if (!string.IsNullOrEmpty(editor.ProjectPath))
            {
                editor.OnSaveProject(editor.ProjectPath);
            }
            else
            {
                OnSaveAs();
            }
        }

        /// <inheritdoc/>
        public async void OnSaveAs()
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();
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

        /// <inheritdoc/>
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
                            _serviceProvider.GetService<IProjectEditor>().OnImportJson(item);
                        }
                    }
                }
            }
            else
            {
                if (_serviceProvider.GetService<IFileSystem>().Exists(path))
                {
                    _serviceProvider.GetService<IProjectEditor>().OnImportJson(path);
                }
            }
        }

        /// <inheritdoc/>
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
                            _serviceProvider.GetService<IProjectEditor>().OnImportSvg(item);
                        }
                    }
                }
            }
            else
            {
                if (_serviceProvider.GetService<IFileSystem>().Exists(path))
                {
                    _serviceProvider.GetService<IProjectEditor>().OnImportJson(path);
                }
            }
        }

        /// <inheritdoc/>
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
                                _serviceProvider.GetService<IProjectEditor>().OnImportJson(item);
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
                        _serviceProvider.GetService<IProjectEditor>().OnImportJson(path);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public async void OnExportJson(object item)
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();
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

        /// <inheritdoc/>
        public async void OnExportObject(object item)
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();
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

        /// <inheritdoc/>
        public async void OnExport(object item)
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();

            string name = string.Empty;

            if (item == null || item is IProjectEditor)
            {
                if (editor.Project == null)
                {
                    return;
                }

                name = editor.Project.Name;
                item = editor.Project;
            }
            else if (item is IProjectContainer project)
            {
                name = project.Name;
            }
            else if (item is IDocumentContainer document)
            {
                name = document.Name;
            }
            else if (item is IPageContainer page)
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

        /// <inheritdoc/>
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
                        await _serviceProvider.GetService<IProjectEditor>().OnExecuteScriptFile(result);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnExit()
        {
            GetWindow().Close();
        }

        /// <inheritdoc/>
        public void OnCopyAsSvg(object item)
        {
            try
            {
                if (item == null)
                {
                    var editor = _serviceProvider.GetService<IProjectEditor>();
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

        /// <inheritdoc/>
        public async void OnPasteSvg()
        {
            try
            {
                var editor = _serviceProvider.GetService<IProjectEditor>();
                var converter = editor.SvgConverter;

                var svgText = await editor.TextClipboard?.GetText();
                if (!string.IsNullOrEmpty(svgText))
                {
                    var shapes = converter.FromString(svgText);
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

        /// <inheritdoc/>
        public void OnCopyAsXaml(object item)
        {
            try
            {
                if (item == null)
                {
                    var editor = _serviceProvider.GetService<IProjectEditor>();
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

        /// <inheritdoc/>
        public void OnCopyAsEmf(object item)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                _serviceProvider.GetService<ILog>()?.LogException(ex);
            }
        }

        /// <inheritdoc/>
        public async void OnCopyAsPathData(object item)
        {
            try
            {
                if (item == null)
                {
                    var editor = _serviceProvider.GetService<IProjectEditor>();
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

        /// <inheritdoc/>
        public async void OnPastePathDataStroked()
        {
            try
            {
                var editor = _serviceProvider.GetService<IProjectEditor>();
                var converter = editor.PathConverter;

                var svgPath = await editor.TextClipboard?.GetText();
                if (!string.IsNullOrEmpty(svgPath))
                {
                    var pathShape = converter.FromSvgPathData(svgPath, isStroked: true, isFilled: false);
                    if (pathShape != null)
                    {
                        editor.OnPasteShapes(Enumerable.Repeat<IBaseShape>(pathShape, 1));
                    }
                }
            }
            catch (Exception ex)
            {
                _serviceProvider.GetService<ILog>()?.LogException(ex);
            }
        }

        /// <inheritdoc/>
        public async void OnPastePathDataFilled()
        {
            try
            {
                var editor = _serviceProvider.GetService<IProjectEditor>();
                var converter = editor.PathConverter;

                var svgPath = await editor.TextClipboard?.GetText();
                if (!string.IsNullOrEmpty(svgPath))
                {
                    var pathShape = converter.FromSvgPathData(svgPath, isStroked: false, isFilled: true);
                    if (pathShape != null)
                    {
                        editor.OnPasteShapes(Enumerable.Repeat<IBaseShape>(pathShape, 1));
                    }
                }
            }
            catch (Exception ex)
            {
                _serviceProvider.GetService<ILog>()?.LogException(ex);
            }
        }

        /// <inheritdoc/>
        public async void OnImportData(IProjectContainer project)
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();
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

        /// <inheritdoc/>
        public async void OnExportData(IDatabase db)
        {
            if (db != null)
            {
                var editor = _serviceProvider.GetService<IProjectEditor>();
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

        /// <inheritdoc/>
        public async void OnUpdateData(IDatabase db)
        {
            if (db != null)
            {
                var editor = _serviceProvider.GetService<IProjectEditor>();
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

        /// <inheritdoc/>
        public void OnAboutDialog()
        {
            new AboutWindow()
            {
                DataContext = _serviceProvider.GetService<IProjectEditor>()
            }
            .ShowDialog(GetWindow());
        }

        /// <inheritdoc/>
        public void OnZoomAutoFit()
        {
            _serviceProvider.GetService<IProjectEditor>().CanvasPlatform?.AutoFitZoom?.Invoke();
        }

        /// <inheritdoc/>
        public void OnZoomReset()
        {
            _serviceProvider.GetService<IProjectEditor>().CanvasPlatform?.ResetZoom?.Invoke();
        }
    }
}
