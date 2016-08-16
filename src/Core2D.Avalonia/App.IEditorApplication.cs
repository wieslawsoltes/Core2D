// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Core2D.Avalonia.Windows;
using Core2D.Editor;
using Core2D.Editor.Interfaces;
using Core2D.Interfaces;
using Core2D.Project;

namespace Core2D.Avalonia
{
    /// <summary>
    /// Editor application implementation for Avalonia.
    /// </summary>
    public partial class App : IEditorApplication
    {
        /// <inheritdoc/>
        async Task<string> IEditorApplication.OnGetImageKeyAsync()
        {
            var log = ServiceLocator.Instance.Resolve<ILog>();
            var fileIO = ServiceLocator.Instance.Resolve<IFileSystem>();
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            var window = ServiceLocator.Instance.Resolve<MainWindow>();

            try
            {
                var dlg = new OpenFileDialog();
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                var result = await dlg.ShowAsync(window);
                if (result != null)
                {
                    var path = result.FirstOrDefault();
                    using (var stream = fileIO.Open(path))
                    {
                        var bytes = fileIO.ReadBinary(stream);
                        var key = editor?.Project?.AddImageFromFile(path, bytes);
                        return key;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                return null;
            }
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnOpenAsync(string path)
        {
            var log = ServiceLocator.Instance.Resolve<ILog>();
            var fileIO = ServiceLocator.Instance.Resolve<IFileSystem>();
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            var window = ServiceLocator.Instance.Resolve<MainWindow>();

            try
            {
                if (path == null)
                {
                    var dlg = new OpenFileDialog();
                    dlg.Filters.Add(new FileDialogFilter() { Name = "Project", Extensions = { "project" } });
                    dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                    var result = await dlg.ShowAsync(window);
                    if (result != null)
                    {
                        editor?.OnOpen(result.FirstOrDefault());
                        editor?.Invalidate?.Invoke();
                    }
                }
                else
                {
                    if (fileIO.Exists(path))
                    {
                        editor.OnOpen(path);
                    }
                }
            }
            catch (Exception ex)
            {
                log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnSaveAsync()
        {
            var log = ServiceLocator.Instance.Resolve<ILog>();
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();

            try
            {
                if (!string.IsNullOrEmpty(editor?.ProjectPath))
                {
                    editor?.OnSave(editor?.ProjectPath);
                }
                else
                {
                    await (this as IEditorApplication).OnSaveAsAsync();
                }
            }
            catch (Exception ex)
            {
                log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnSaveAsAsync()
        {
            var log = ServiceLocator.Instance.Resolve<ILog>();
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            var window = ServiceLocator.Instance.Resolve<MainWindow>();

            try
            {
                if (editor?.Project != null)
                {
                    var dlg = new SaveFileDialog();
                    dlg.Filters.Add(new FileDialogFilter() { Name = "Project", Extensions = { "project" } });
                    dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                    dlg.InitialFileName = editor.Project.Name;
                    dlg.DefaultExtension = "project";
                    var result = await dlg.ShowAsync(window);
                    if (result != null)
                    {
                        editor?.OnSave(result);
                    }
                }
            }
            catch (Exception ex)
            {
                log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnImportXamlAsync(string path)
        {
            var log = ServiceLocator.Instance.Resolve<ILog>();
            var fileIO = ServiceLocator.Instance.Resolve<IFileSystem>();
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            var window = ServiceLocator.Instance.Resolve<MainWindow>();

            try
            {
                if (path == null)
                {
                    var dlg = new OpenFileDialog();
                    dlg.AllowMultiple = true;
                    dlg.Filters.Add(new FileDialogFilter() { Name = "Xaml", Extensions = { "xaml" } });
                    dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });

                    var results = await dlg.ShowAsync(window);
                    if (results != null)
                    {
                        foreach (var result in results)
                        {
                            editor?.OnImportXaml(result);
                        }
                    }
                }
                else
                {
                    if (fileIO.Exists(path))
                    {
                        editor?.OnImportXaml(path);
                    }
                }
            }
            catch (Exception ex)
            {
                log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnExportXamlAsync(object item)
        {
            var log = ServiceLocator.Instance.Resolve<ILog>();
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            var window = ServiceLocator.Instance.Resolve<MainWindow>();

            try
            {
                var dlg = new SaveFileDialog();
                dlg.Filters.Add(new FileDialogFilter() { Name = "Xaml", Extensions = { "xaml" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                dlg.InitialFileName = editor?.GetName(item);
                dlg.DefaultExtension = "xaml";
                var result = await dlg.ShowAsync(window);
                if (result != null)
                {
                    editor?.OnExportXaml(result, item);
                }
            }
            catch (Exception ex)
            {
                log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnImportJsonAsync(string path)
        {
            var log = ServiceLocator.Instance.Resolve<ILog>();
            var fileIO = ServiceLocator.Instance.Resolve<IFileSystem>();
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            var window = ServiceLocator.Instance.Resolve<MainWindow>();

            try
            {
                if (path == null)
                {
                    var dlg = new OpenFileDialog();
                    dlg.AllowMultiple = true;
                    dlg.Filters.Add(new FileDialogFilter() { Name = "Json", Extensions = { "json" } });
                    dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });

                    var results = await dlg.ShowAsync(window);
                    if (results != null)
                    {
                        foreach (var result in results)
                        {
                            editor?.OnImportJson(result);
                        }
                    }
                }
                else
                {
                    if (fileIO.Exists(path))
                    {
                        editor?.OnImportJson(path);
                    }
                }
            }
            catch (Exception ex)
            {
                log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnExportJsonAsync(object item)
        {
            var log = ServiceLocator.Instance.Resolve<ILog>();
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            var window = ServiceLocator.Instance.Resolve<MainWindow>();

            try
            {
                var dlg = new SaveFileDialog();
                dlg.Filters.Add(new FileDialogFilter() { Name = "Json", Extensions = { "json" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                dlg.InitialFileName = editor?.GetName(item);
                dlg.DefaultExtension = "json";
                var result = await dlg.ShowAsync(window);
                if (result != null)
                {
                    editor?.OnExportJson(result, item);
                }
            }
            catch (Exception ex)
            {
                log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnExportAsync(object item)
        {
            var log = ServiceLocator.Instance.Resolve<ILog>();
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            var window = ServiceLocator.Instance.Resolve<MainWindow>();

            try
            {
                string name = string.Empty;

                if (item is XContainer)
                {
                    name = (item as XContainer).Name;
                }
                else if (item is XDocument)
                {
                    name = (item as XDocument).Name;
                }
                else if (item is XProject)
                {
                    name = (item as XProject).Name;
                }
                else if (item is ProjectEditor)
                {
                    var editorItem = (item as ProjectEditor);
                    if (editor?.Project == null)
                        return;

                    name = editorItem?.Project?.Name;
                    item = editorItem?.Project;
                }
                else if (item == null)
                {
                    if (editor?.Project == null)
                        return;

                    name = editor?.Project?.Name;
                    item = editor?.Project;
                }

                var dlg = new SaveFileDialog();
                foreach (var writer in editor?.FileWriters)
                {
                    dlg.Filters.Add(new FileDialogFilter() { Name = writer.Name, Extensions = { writer.Extension } });
                }
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                dlg.InitialFileName = name;
                dlg.DefaultExtension = editor?.FileWriters.FirstOrDefault()?.Extension;
                var result = await dlg.ShowAsync(window);
                if (result != null)
                {
                    string ext = System.IO.Path.GetExtension(result).ToLower().TrimStart('.');
                    IFileWriter writer = editor?.FileWriters.Where(w => string.Compare(w.Extension, ext, StringComparison.OrdinalIgnoreCase) == 0).FirstOrDefault();
                    if (writer != null)
                    {
                        editor?.OnExport(result, item, writer);
                    }
                }
            }
            catch (Exception ex)
            {
                log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnImportDataAsync()
        {
            var log = ServiceLocator.Instance.Resolve<ILog>();
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            var window = ServiceLocator.Instance.Resolve<MainWindow>();

            try
            {
                if (editor?.Project != null)
                {
                    var dlg = new OpenFileDialog();
                    dlg.Filters.Add(new FileDialogFilter() { Name = "Csv", Extensions = { "csv" } });
                    dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                    var result = await dlg.ShowAsync(window);
                    if (result != null)
                    {
                        var path = result.FirstOrDefault();
                        editor?.OnImportData(path);
                    }
                }
            }
            catch (Exception ex)
            {
                log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnExportDataAsync()
        {
            var log = ServiceLocator.Instance.Resolve<ILog>();
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            var window = ServiceLocator.Instance.Resolve<MainWindow>();

            try
            {
                var database = editor?.Project?.CurrentDatabase;
                if (database != null)
                {
                    var dlg = new SaveFileDialog();
                    dlg.Filters.Add(new FileDialogFilter() { Name = "Csv", Extensions = { "csv" } });
                    dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                    dlg.InitialFileName = database.Name;
                    dlg.DefaultExtension = "csv";
                    var result = await dlg.ShowAsync(window);
                    if (result != null)
                    {
                        editor?.OnExportData(result, database);
                    }
                }
            }
            catch (Exception ex)
            {
                log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnUpdateDataAsync()
        {
            var log = ServiceLocator.Instance.Resolve<ILog>();
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            var window = ServiceLocator.Instance.Resolve<MainWindow>();

            try
            {
                var database = editor?.Project?.CurrentDatabase;
                if (database != null)
                {
                    var dlg = new OpenFileDialog();
                    dlg.Filters.Add(new FileDialogFilter() { Name = "Csv", Extensions = { "csv" } });
                    dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                    var result = await dlg.ShowAsync(window);
                    if (result != null)
                    {
                        var path = result.FirstOrDefault();
                        editor?.OnUpdateData(path, database);
                    }
                }
            }
            catch (Exception ex)
            {
                log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnImportObjectAsync(string path)
        {
            var log = ServiceLocator.Instance.Resolve<ILog>();
            var fileIO = ServiceLocator.Instance.Resolve<IFileSystem>();
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            var window = ServiceLocator.Instance.Resolve<MainWindow>();

            try
            {
                if (path == null)
                {
                    var dlg = new OpenFileDialog();
                    dlg.AllowMultiple = true;
                    dlg.Filters.Add(new FileDialogFilter() { Name = "Json", Extensions = { "json" } });
                    dlg.Filters.Add(new FileDialogFilter() { Name = "Xaml", Extensions = { "xaml" } });
                    var results = await dlg.ShowAsync(window);
                    if (results != null)
                    {
                        foreach (var result in results)
                        {
                            string resultExtension = System.IO.Path.GetExtension(result);
                            if (string.Compare(resultExtension, ".json", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                editor?.OnImportJson(result);
                            }
                            else if (string.Compare(resultExtension, ".xaml", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                editor?.OnImportJson(result);
                            }
                        }
                    }
                }
                else
                {
                    if (fileIO.Exists(path))
                    {
                        string resultExtension = System.IO.Path.GetExtension(path);
                        if (string.Compare(resultExtension, ".json", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            editor?.OnImportJson(path);
                        }
                        else if (string.Compare(resultExtension, ".xaml", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            editor?.OnImportJson(path);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnExportObjectAsync(object item)
        {
            var log = ServiceLocator.Instance.Resolve<ILog>();
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            var window = ServiceLocator.Instance.Resolve<MainWindow>();

            try
            {
                if (item != null)
                {
                    var dlg = new SaveFileDialog();
                    dlg.Filters.Add(new FileDialogFilter() { Name = "Json", Extensions = { "json" } });
                    dlg.Filters.Add(new FileDialogFilter() { Name = "Xaml", Extensions = { "xaml" } });
                    dlg.InitialFileName = editor?.GetName(item);
                    dlg.DefaultExtension = "json";
                    var path = await dlg.ShowAsync(window);
                    if (path != null)
                    {
                        string resultExtension = System.IO.Path.GetExtension(path);
                        if (string.Compare(resultExtension, ".json", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            editor?.OnExportJson(path, item);
                        }
                        else if (string.Compare(resultExtension, ".xaml", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            editor?.OnExportXaml(path, item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnCopyAsEmfAsync()
        {
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnExportAsEmfAsync(string path)
        {
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnZoomResetAsync()
        {
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            editor.ResetZoom?.Invoke();
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnZoomAutoFitAsync()
        {
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            editor.AutoFitZoom?.Invoke();
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnLoadWindowLayout()
        {
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            editor.LoadLayout?.Invoke();
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnSaveWindowLayoutAsync()
        {
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            editor.SaveLayout?.Invoke();
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnResetWindowLayoutAsync()
        {
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            editor.ResetLayout?.Invoke();
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnShowObjectBrowserAsync()
        {
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();

            var browser = new Windows.BrowserWindow();
            browser.DataContext = editor;
            browser.Show();

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnShowDocumentViewerAsync()
        {
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();

            var document = new Windows.DocumentWindow();
            document.DataContext = editor;
            document.Show();

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        void IEditorApplication.OnCloseView()
        {
            var window = ServiceLocator.Instance.Resolve<MainWindow>();

            window?.Close();
        }
    }
}
