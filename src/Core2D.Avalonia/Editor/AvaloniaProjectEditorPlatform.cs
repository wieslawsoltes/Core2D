// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Core2D.Avalonia.Windows;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Editor;
using Core2D.Editor.Input;
using Core2D.Editor.Views.Interfaces;
using Core2D.Interfaces;

namespace Core2D.Avalonia.Editor
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
                var dlg = new OpenFileDialog();
                dlg.Filters.Add(new FileDialogFilter() { Name = "Project", Extensions = { "project" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                var result = await dlg.ShowAsync(_serviceProvider.GetService<MainWindow>());
                if (result != null)
                {
                    var editor = _serviceProvider.GetService<ProjectEditor>();
                    editor.OnOpenProject(result.FirstOrDefault());
                    editor.Canvas?.Invalidate?.Invoke();
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public async void OnSaveAs()
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var dlg = new SaveFileDialog();
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
                var dlg = new OpenFileDialog();
                dlg.AllowMultiple = true;
                dlg.Filters.Add(new FileDialogFilter() { Name = "Json", Extensions = { "json" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });

                var results = await dlg.ShowAsync(_serviceProvider.GetService<MainWindow>());
                if (results != null)
                {
                    foreach (var result in results)
                    {
                        _serviceProvider.GetService<ProjectEditor>().OnImportJson(result);
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

        /// <inheritdoc/>
        public async void OnImportObject(string path)
        {
            if (path == null)
            {
                var dlg = new OpenFileDialog();
                dlg.AllowMultiple = true;
                dlg.Filters.Add(new FileDialogFilter() { Name = "Json", Extensions = { "json" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "Xaml", Extensions = { "xaml" } });
                var results = await dlg.ShowAsync(_serviceProvider.GetService<MainWindow>());
                if (results != null)
                {
                    foreach (var result in results)
                    {
                        string resultExtension = System.IO.Path.GetExtension(result);
                        if (string.Compare(resultExtension, ".json", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            _serviceProvider.GetService<ProjectEditor>().OnImportJson(result);
                        }
                        else if (string.Compare(resultExtension, ".xaml", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            _serviceProvider.GetService<ProjectEditor>().OnImportJson(result);
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
                    else if (string.Compare(resultExtension, ".xaml", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        _serviceProvider.GetService<ProjectEditor>().OnImportJson(path);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public async void OnImportXaml(string path)
        {
            if (path == null)
            {
                var dlg = new OpenFileDialog();
                dlg.AllowMultiple = true;
                dlg.Filters.Add(new FileDialogFilter() { Name = "Xaml", Extensions = { "xaml" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });

                var results = await dlg.ShowAsync(_serviceProvider.GetService<MainWindow>());
                if (results != null)
                {
                    foreach (var result in results)
                    {
                        _serviceProvider.GetService<ProjectEditor>().OnImportXaml(result);
                    }
                }
            }
            else
            {
                if (_serviceProvider.GetService<IFileSystem>().Exists(path))
                {
                    _serviceProvider.GetService<ProjectEditor>().OnImportXaml(path);
                }
            }
        }

        /// <inheritdoc/>
        public async void OnExportJson(object item)
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var dlg = new SaveFileDialog();
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
            var editor = _serviceProvider.GetService<ProjectEditor>();
            if (item != null)
            {
                var dlg = new SaveFileDialog();
                dlg.Filters.Add(new FileDialogFilter() { Name = "Json", Extensions = { "json" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "Xaml", Extensions = { "xaml" } });
                dlg.InitialFileName = editor?.GetName(item);
                dlg.DefaultExtension = "json";
                var path = await dlg.ShowAsync(_serviceProvider.GetService<MainWindow>());
                if (path != null)
                {
                    string resultExtension = System.IO.Path.GetExtension(path);
                    if (string.Compare(resultExtension, ".json", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        editor.OnExportJson(path, item);
                    }
                    else if (string.Compare(resultExtension, ".xaml", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        editor.OnExportXaml(path, item);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public async void OnExportXaml(object item)
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var dlg = new SaveFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Xaml", Extensions = { "xaml" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
            dlg.InitialFileName = editor?.GetName(item);
            dlg.DefaultExtension = "xaml";
            var result = await dlg.ShowAsync(_serviceProvider.GetService<MainWindow>());
            if (result != null)
            {
                editor.OnExportXaml(result, item);
            }
        }

        /// <inheritdoc/>
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
            else if (item is ProjectContainer)
            {
                name = (item as ProjectContainer).Name;
            }
            else if (item is DocumentContainer)
            {
                name = (item as DocumentContainer).Name;
            }
            else if (item is PageContainer)
            {
                name = (item as PageContainer).Name;
            }

            var dlg = new SaveFileDialog();
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
                IFileWriter writer = editor.FileWriters.Where(w => string.Compare(w.Extension, ext, StringComparison.OrdinalIgnoreCase) == 0).FirstOrDefault();
                if (writer != null)
                {
                    editor.OnExport(result, item, writer);
                }
            }
        }

        /// <inheritdoc/>
        public async void OnExecuteScript(string path)
        {
            if (path == null)
            {
                var dlg = new OpenFileDialog();
                dlg.Filters.Add(new FileDialogFilter() { Name = "Script", Extensions = { "cs" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                dlg.AllowMultiple = true;
                var result = await dlg.ShowAsync(_serviceProvider.GetService<MainWindow>());
                if (result != null)
                {
                    Load(result);
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

        private void Load(string[] paths)
        {
            foreach (var path in paths)
            {
                Load(path);
            }
        }

        private void Load(string path)
        {
            var fileIO = _serviceProvider.GetService<IFileSystem>();
            var csharp = fileIO.ReadUtf8Text(path);
            if (!string.IsNullOrEmpty(csharp))
            {
                _serviceProvider.GetService<IScriptRunner>().Execute(csharp);
            }
        }

        /// <inheritdoc/>
        public void OnExit()
        {
            _serviceProvider.GetService<MainWindow>().Close();
        }

        /// <inheritdoc/>
        public void OnCopyAsEmf(object item)
        {
        }

        /// <inheritdoc/>
        public async void OnImportData(ProjectContainer project)
        {
            var dlg = new OpenFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Csv", Extensions = { "csv" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
            var result = await dlg.ShowAsync(_serviceProvider.GetService<MainWindow>());
            if (result != null)
            {
                _serviceProvider.GetService<ProjectEditor>().OnImportData(project, result.FirstOrDefault());
            }
        }

        /// <inheritdoc/>
        public async void OnExportData(Database db)
        {
            if (db != null)
            {
                var dlg = new SaveFileDialog();
                dlg.Filters.Add(new FileDialogFilter() { Name = "Csv", Extensions = { "csv" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                dlg.InitialFileName = db.Name;
                dlg.DefaultExtension = "csv";
                var result = await dlg.ShowAsync(_serviceProvider.GetService<MainWindow>());
                if (result != null)
                {
                    _serviceProvider.GetService<ProjectEditor>().OnExportData(result, db);
                }
            }
        }

        /// <inheritdoc/>
        public async void OnUpdateData(Database db)
        {
            if (db != null)
            {
                var dlg = new OpenFileDialog();
                dlg.Filters.Add(new FileDialogFilter() { Name = "Csv", Extensions = { "csv" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                var result = await dlg.ShowAsync(_serviceProvider.GetService<MainWindow>());
                if (result != null)
                {
                    _serviceProvider.GetService<ProjectEditor>().OnUpdateData(result.FirstOrDefault(), db);
                }
            }
        }

        /// <inheritdoc/>
        public void OnDocumentViewer()
        {
            new DocumentWindow()
            {
                DataContext = _serviceProvider.GetService<ProjectEditor>()
            }
            .Show();
        }

        /// <inheritdoc/>
        public void OnObjectBrowser()
        {
            new BrowserWindow()
            {
                DataContext = _serviceProvider.GetService<ProjectEditor>()
            }
            .Show();
        }

        /// <inheritdoc/>
        public void OnAboutDialog()
        {
            new AboutWindow()
            {
                DataContext = _serviceProvider.GetService<ProjectEditor>()
            }
            .ShowDialog();
        }

        /// <inheritdoc/>
        public void OnZoomAutoFit()
        {
            _serviceProvider.GetService<ProjectEditor>().Canvas?.AutoFitZoom?.Invoke();
        }

        /// <inheritdoc/>
        public void OnZoomReset()
        {
            _serviceProvider.GetService<ProjectEditor>().Canvas?.ResetZoom?.Invoke();
        }
    }
}
