using System;
using System.Linq;
using Avalonia.Controls;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Editor;
using Core2D.Interfaces;
using Core2D.UI.Avalonia.Views;
using Core2D.XamlExporter.Avalonia;
using DM = Dock.Model;

namespace Core2D.UI.Avalonia.Editor
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

        /// <inheritdoc/>
        public async void OnOpen(string path)
        {
            if (path == null)
            {
                var dlg = new OpenFileDialog() { Title = "Open" };
                dlg.Filters.Add(new FileDialogFilter() { Name = "Project", Extensions = { "project" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                var window = _serviceProvider.GetService<MainWindow>();
                var result = await dlg.ShowAsync(window);
                if (result != null)
                {
                    var item = result.FirstOrDefault();
                    if (item != null)
                    {
                        var editor = _serviceProvider.GetService<IProjectEditor>();
                        editor.OnOpenProject(item);
                        editor.CanvasPlatform?.Invalidate?.Invoke();
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
            var result = await dlg.ShowAsync(_serviceProvider.GetService<MainWindow>());
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

                var result = await dlg.ShowAsync(_serviceProvider.GetService<MainWindow>());
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
        public async void OnImportObject(string path)
        {
            if (path == null)
            {
                var dlg = new OpenFileDialog() { Title = "Open" };
                dlg.AllowMultiple = true;
                dlg.Filters.Add(new FileDialogFilter() { Name = "Json", Extensions = { "json" } });
                var result = await dlg.ShowAsync(_serviceProvider.GetService<MainWindow>());
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
            var result = await dlg.ShowAsync(_serviceProvider.GetService<MainWindow>());
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
                var result = await dlg.ShowAsync(_serviceProvider.GetService<MainWindow>());
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

            var result = await dlg.ShowAsync(_serviceProvider.GetService<MainWindow>());
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
                var result = await dlg.ShowAsync(_serviceProvider.GetService<MainWindow>());
                if (result != null)
                {
                    if (result.All(r => r != null))
                    {
                        _serviceProvider.GetService<IProjectEditor>().OnExecuteScriptFile(result);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnExit()
        {
            _serviceProvider.GetService<MainWindow>().Close();
        }

        /// <inheritdoc/>
        public void OnCopyAsDrawing(object item)
        {
            try
            {
                if (item == null)
                {
                    var editor = _serviceProvider.GetService<IProjectEditor>();
                    var exporter = new DrawingGroupXamlExporter(_serviceProvider);
                    var container = editor.Project.CurrentContainer;

                    var source = editor.PageState?.SelectedShape;
                    if (source != null)
                    {
                        var key = source.Name;
                        var xaml = exporter.Create(source, key);
                        if (!string.IsNullOrEmpty(xaml))
                        {
                            editor.TextClipboard?.SetText(xaml);
                        }
                        return;
                    }

                    var sources = editor.PageState?.SelectedShapes;
                    if (sources != null)
                    {
                        var key = container?.Name;
                        var xaml = exporter.Create(sources, key);
                        if (!string.IsNullOrEmpty(xaml))
                        {
                            editor.TextClipboard?.SetText(xaml);
                        }
                        return;
                    }

                    var shapes = container.Layers.Select(x => x.Shapes);
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
        public async void OnImportData(IProjectContainer project)
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();
            var dlg = new OpenFileDialog() { Title = "Open" };
            foreach (var reader in editor?.TextFieldReaders)
            {
                dlg.Filters.Add(new FileDialogFilter() { Name = reader.Name, Extensions = { reader.Extension } });
            }
            dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
            var result = await dlg.ShowAsync(_serviceProvider.GetService<MainWindow>());

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
                var result = await dlg.ShowAsync(_serviceProvider.GetService<MainWindow>());
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
                var result = await dlg.ShowAsync(_serviceProvider.GetService<MainWindow>());
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
            .ShowDialog(_serviceProvider.GetService<MainWindow>());
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

        /// <inheritdoc/>
        public async void OnLoadLayout()
        {
            var dlg = new OpenFileDialog() { Title = "Open" };
            dlg.Filters.Add(new FileDialogFilter() { Name = "Layout", Extensions = { "layout" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
            var window = _serviceProvider.GetService<MainWindow>();
            var result = await dlg.ShowAsync(window);
            if (result != null)
            {
                var path = result.FirstOrDefault();
                if (path != null)
                {
                    var editor = _serviceProvider.GetService<IProjectEditor>();
                    editor.OnLoadLayout(path);

                    var dockFactory = _serviceProvider.GetService<DM.IFactory>();
                    dockFactory.InitLayout(editor.Layout);
                }
            }
        }

        /// <inheritdoc/>
        public async void OnSaveLayout()
        {
            var dlg = new SaveFileDialog() { Title = "Save" };
            dlg.Filters.Add(new FileDialogFilter() { Name = "Layout", Extensions = { "layout" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
            dlg.InitialFileName = "Core2D";
            dlg.DefaultExtension = "layout";
            var result = await dlg.ShowAsync(_serviceProvider.GetService<MainWindow>());
            if (result != null)
            {
                var editor = _serviceProvider.GetService<IProjectEditor>();
                editor.OnSaveLayout(result);
            }
        }

        /// <inheritdoc/>
        public void OnResetLayout()
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();
            var dockFactory = _serviceProvider.GetService<DM.IFactory>();

            var activeDockableId = editor.Layout.ActiveDockable.Id;
            editor.Layout = dockFactory.CreateLayout();
            dockFactory.InitLayout(editor.Layout);

            var dockable = dockFactory.FindDockable(editor.Layout, (v) => v.Id == activeDockableId);
            if (dockable != null)
            {
                editor.Layout.Navigate(dockable);
            }
        }
    }
}
