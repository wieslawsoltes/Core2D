// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Editor;
using Core2D.FileWriter.Emf;
using Core2D.Interfaces;
using Core2D.Wpf.Windows;
using Microsoft.Win32;

namespace Core2D.Wpf.Editor
{
    /// <summary>
    /// Project editor WPF platform.
    /// </summary>
    public class WpfProjectEditorPlatform : IProjectEditorPlatform
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initialize new instance of <see cref="WpfProjectEditorPlatform"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public WpfProjectEditorPlatform(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public void OnOpen(string path)
        {
            if (path == null)
            {
                var dlg = new OpenFileDialog()
                {
                    Filter = "Project (*.project)|*.project|All (*.*)|*.*",
                    FilterIndex = 0,
                    FileName = ""
                };

                if (dlg.ShowDialog(_serviceProvider.GetService<MainWindow>()) == true)
                {
                    _serviceProvider.GetService<ProjectEditor>().OnOpenProject(dlg.FileName);
                }
            }
            else
            {
                if (File.Exists(path))
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
        public void OnSaveAs()
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var dlg = new SaveFileDialog()
            {
                Filter = "Project (*.project)|*.project|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = editor.Project?.Name
            };

            if (dlg.ShowDialog(_serviceProvider.GetService<MainWindow>()) == true)
            {
                editor.OnSaveProject(dlg.FileName);
            }
        }

        /// <inheritdoc/>
        public void OnImportJson(string path)
        {
            if (path == null)
            {
                var dlg = new OpenFileDialog()
                {
                    Filter = "Json (*.json)|*.json|All (*.*)|*.*",
                    FilterIndex = 0,
                    Multiselect = true,
                    FileName = ""
                };

                if (dlg.ShowDialog(_serviceProvider.GetService<MainWindow>()) == true)
                {
                    var results = dlg.FileNames;

                    foreach (var result in results)
                    {
                        _serviceProvider.GetService<ProjectEditor>().OnImportJson(result);
                    }
                }
            }
            else
            {
                if (File.Exists(path))
                {
                    _serviceProvider.GetService<ProjectEditor>().OnImportJson(path);
                }
            }
        }

        /// <inheritdoc/>
        public void OnImportObject(string path)
        {
            if (path == null)
            {
                var dlg = new OpenFileDialog()
                {
                    Filter = "Json (*.json)|*.json|Xaml (*.xaml)|*.xaml",
                    Multiselect = true,
                    FilterIndex = 0
                };

                if (dlg.ShowDialog(_serviceProvider.GetService<MainWindow>()) == true)
                {
                    var results = dlg.FileNames;
                    var index = dlg.FilterIndex;

                    foreach (var result in results)
                    {
                        switch (index)
                        {
                            case 1:
                                _serviceProvider.GetService<ProjectEditor>().OnImportJson(result);
                                break;
                            case 2:
                                _serviceProvider.GetService<ProjectEditor>().OnImportXaml(result);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            else
            {
                if (File.Exists(path))
                {
                    string resultExtension = System.IO.Path.GetExtension(path);
                    if (string.Compare(resultExtension, ".json", true) == 0)
                    {
                        _serviceProvider.GetService<ProjectEditor>().OnImportJson(path);
                    }
                    else if (string.Compare(resultExtension, ".xaml", true) == 0)
                    {
                        _serviceProvider.GetService<ProjectEditor>().OnImportJson(path);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnImportXaml(string path)
        {
            if (path == null)
            {
                var dlg = new OpenFileDialog()
                {
                    Filter = "Xaml (*.xaml)|*.xaml|All (*.*)|*.*",
                    FilterIndex = 0,
                    Multiselect = true,
                    FileName = ""
                };

                if (dlg.ShowDialog(_serviceProvider.GetService<MainWindow>()) == true)
                {
                    var results = dlg.FileNames;

                    foreach (var result in results)
                    {
                        _serviceProvider.GetService<ProjectEditor>().OnImportXaml(result);
                    }
                }
            }
            else
            {
                if (File.Exists(path))
                {
                    _serviceProvider.GetService<ProjectEditor>().OnImportXaml(path);
                }
            }
        }

        /// <inheritdoc/>
        public void OnExportJson(object item)
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var dlg = new SaveFileDialog()
            {
                Filter = "Xaml (*.xaml)|*.xaml|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = editor.GetName(item)
            };

            if (dlg.ShowDialog(_serviceProvider.GetService<MainWindow>()) == true)
            {
                editor.OnExportJson(dlg.FileName, item);
            }
        }

        /// <inheritdoc/>
        public void OnExportObject(object item)
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
            if (item != null)
            {
                var dlg = new SaveFileDialog()
                {
                    Filter = "Json (*.json)|*.json|Xaml (*.xaml)|*.xaml",
                    FilterIndex = 0,
                    FileName = editor.GetName(item)
                };

                if (dlg.ShowDialog(_serviceProvider.GetService<MainWindow>()) == true)
                {
                    switch (dlg.FilterIndex)
                    {
                        case 1:
                            editor.OnExportJson(dlg.FileName, item);
                            break;
                        case 2:
                            editor.OnExportXaml(dlg.FileName, item);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnExportXaml(object item)
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var dlg = new SaveFileDialog()
            {
                Filter = "Xaml (*.xaml)|*.xaml|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = editor.GetName(item)
            };

            if (dlg.ShowDialog(_serviceProvider.GetService<MainWindow>()) == true)
            {
                editor.OnExportXaml(dlg.FileName, item);
            }
        }

        /// <inheritdoc/>
        public void OnExport(object item)
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

            var sb = new StringBuilder();
            foreach (var writer in editor.FileWriters)
            {
                sb.Append($"{writer.Name} (*.{writer.Extension})|*.{writer.Extension}|");
            }
            sb.Append("All (*.*)|*.*");

            var dlg = new SaveFileDialog()
            {
                Filter = sb.ToString(),
                FilterIndex = 0,
                FileName = name
            };

            if (dlg.ShowDialog(_serviceProvider.GetService<MainWindow>()) == true)
            {
                string result = dlg.FileName;
                IFileWriter writer = editor.FileWriters[dlg.FilterIndex - 1];
                if (writer != null)
                {
                    editor.OnExport(result, item, writer);
                }
            }
        }

        /// <inheritdoc/>
        public void OnExecuteScript(string path)
        {
            if (path == null)
            {
                var dlg = new OpenFileDialog()
                {
                    Filter = "Script (*.cs)|*.cs|All (*.*)|*.*",
                    FilterIndex = 0,
                    FileName = "",
                    Multiselect = true
                };

                if (dlg.ShowDialog(_serviceProvider.GetService<MainWindow>()) == true)
                {
                    Load(dlg.FileNames);
                }
            }
            else
            {
                if (File.Exists(path))
                {
                    Load(path);
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
            var window = _serviceProvider.GetService<MainWindow>();
            var editor = _serviceProvider.GetService<ProjectEditor>();
            if (editor.IsProjectDirty)
            {
                var result = MessageBox.Show(
                    "Save changes to the project?",
                    "Unsaved Changes",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question,
                    MessageBoxResult.Yes);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        {
                            OnSave();
                            window.Close();
                        }
                        break;
                    case MessageBoxResult.No:
                        {
                            window.Close();
                        }
                        break;
                    case MessageBoxResult.Cancel:
                        break;
                }
            }
            else
            {
                window.Close();
            }
        }

        /// <inheritdoc/>
        public void OnCopyAsEmf(object item)
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var page = editor.Project?.CurrentContainer;
            if (page != null)
            {
                if (editor.Renderers[0]?.State?.SelectedShape != null)
                {
                    var shapes = Enumerable.Repeat(editor.Renderers[0].State.SelectedShape, 1).ToList();
                    EmfWriter.SetClipboard(
                        shapes,
                        page.Template.Width,
                        page.Template.Height,
                        page.Data.Properties,
                        page.Data.Record,
                        editor.Project);
                }
                else if (editor.Renderers?[0]?.State?.SelectedShapes != null)
                {
                    var shapes = editor.Renderers[0].State.SelectedShapes.ToList();
                    EmfWriter.SetClipboard(
                        shapes,
                        page.Template.Width,
                        page.Template.Height,
                        page.Data.Properties,
                        page.Data.Record,
                        editor.Project);
                }
                else
                {
                    EmfWriter.SetClipboard(page, editor.Project);
                }
            }
        }

        /// <inheritdoc/>
        public void OnImportData(ProjectContainer project)
        {
            var dlg = new OpenFileDialog()
            {
                Filter = "Csv (*.csv)|*.csv|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = ""
            };

            if (dlg.ShowDialog(_serviceProvider.GetService<MainWindow>()) == true)
            {
                _serviceProvider.GetService<ProjectEditor>().OnImportData(project, dlg.FileName);
            }
        }

        /// <inheritdoc/>
        public void OnExportData(Database db)
        {
            if (db != null)
            {
                var dlg = new SaveFileDialog()
                {
                    Filter = "Csv (*.csv)|*.csv|All (*.*)|*.*",
                    FilterIndex = 0,
                    FileName = db.Name
                };

                if (dlg.ShowDialog(_serviceProvider.GetService<MainWindow>()) == true)
                {
                    _serviceProvider.GetService<ProjectEditor>().OnExportData(dlg.FileName, db);
                }
            }
        }

        /// <inheritdoc/>
        public void OnUpdateData(Database db)
        {
            if (db != null)
            {
                var dlg = new OpenFileDialog()
                {
                    Filter = "Csv (*.csv)|*.csv|All (*.*)|*.*",
                    FilterIndex = 0,
                    FileName = ""
                };

                if (dlg.ShowDialog(_serviceProvider.GetService<MainWindow>()) == true)
                {
                    _serviceProvider.GetService<ProjectEditor>().OnUpdateData(dlg.FileName, db);
                }
            }
        }

        /// <inheritdoc/>
        public void OnDocumentViewer()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void OnObjectBrowser()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void OnScriptEditor()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void OnAboutDialog()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void OnZoomAutoFit()
        {
            _serviceProvider.GetService<ProjectEditor>().CanvasPlatform?.AutoFitZoom?.Invoke();
        }

        /// <inheritdoc/>
        public void OnZoomReset()
        {
            _serviceProvider.GetService<ProjectEditor>().CanvasPlatform?.ResetZoom?.Invoke();
        }

        /// <inheritdoc/>
        public void OnLoadLayout()
        {
            _serviceProvider.GetService<ProjectEditor>().LayoutPlatform?.LoadLayout?.Invoke();
        }

        /// <inheritdoc/>
        public void OnSaveLayout()
        {
            _serviceProvider.GetService<ProjectEditor>().LayoutPlatform?.SaveLayout?.Invoke();
        }

        /// <inheritdoc/>
        public void OnResetLayout()
        {
            _serviceProvider.GetService<ProjectEditor>().LayoutPlatform?.ResetLayout?.Invoke();
        }
    }
}
