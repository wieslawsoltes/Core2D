// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core2D.Editor;
using Core2D.Editor.Interfaces;
using Core2D.Interfaces;
using Core2D.Project;
using Core2D.Renderer;
using Core2D.Wpf.Windows;
using FileWriter.Emf;
using Microsoft.Win32;

namespace Core2D.Wpf
{
    /// <summary>
    /// Editor application implementation for WPF.
    /// </summary>
    public partial class App : IEditorApplication
    {
        /// <inheritdoc/>
        async Task<string> IEditorApplication.OnGetImageKeyAsync()
        {
            var log = ServiceLocator.Instance.Resolve<ILog>();
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            var window = ServiceLocator.Instance.Resolve<MainWindow>();

            var dlg = new OpenFileDialog()
            {
                Filter = "All (*.*)|*.*",
                FilterIndex = 0,
                FileName = ""
            };

            if (dlg.ShowDialog(window) == true)
            {
                try
                {
                    var path = dlg.FileName;
                    var bytes = System.IO.File.ReadAllBytes(path);
                    var key = editor?.Project.AddImageFromFile(path, bytes);
                    return await Task.Run(() => key);
                }
                catch (Exception ex)
                {
                    log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }
            return null;
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnOpenAsync(string path)
        {
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            var window = ServiceLocator.Instance.Resolve<MainWindow>();

            if (path == null)
            {
                var dlg = new OpenFileDialog()
                {
                    Filter = "Project (*.project)|*.project|All (*.*)|*.*",
                    FilterIndex = 0,
                    FileName = ""
                };

                if (dlg.ShowDialog(window) == true)
                {
                    editor?.OnOpen(dlg.FileName);
                }
            }
            else
            {
                if (System.IO.File.Exists(path))
                {
                    editor?.OnOpen(path);
                }
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnSaveAsync()
        {
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();

            if (!string.IsNullOrEmpty(editor?.ProjectPath))
            {
                editor?.OnSave(editor?.ProjectPath);
            }
            else
            {
                await (this as IEditorApplication).OnSaveAsAsync();
            }
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnSaveAsAsync()
        {
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            var window = ServiceLocator.Instance.Resolve<MainWindow>();

            var dlg = new SaveFileDialog()
            {
                Filter = "Project (*.project)|*.project|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = editor?.Project?.Name
            };

            if (dlg.ShowDialog(window) == true)
            {
                editor?.OnSave(dlg.FileName);
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnImportXamlAsync(string path)
        {
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            var window = ServiceLocator.Instance.Resolve<MainWindow>();

            if (path == null)
            {
                var dlg = new OpenFileDialog()
                {
                    Filter = "Xaml (*.xaml)|*.xaml|All (*.*)|*.*",
                    FilterIndex = 0,
                    Multiselect = true,
                    FileName = ""
                };

                if (dlg.ShowDialog(window) == true)
                {
                    var results = dlg.FileNames;

                    foreach (var result in results)
                    {
                        editor?.OnImportXaml(result);
                    }
                }
            }
            else
            {
                if (System.IO.File.Exists(path))
                {
                    editor?.OnImportXaml(path);
                }
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnExportXamlAsync(object item)
        {
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            var window = ServiceLocator.Instance.Resolve<MainWindow>();

            var dlg = new SaveFileDialog()
            {
                Filter = "Xaml (*.xaml)|*.xaml|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = editor?.GetName(item)
            };

            if (dlg.ShowDialog(window) == true)
            {
                editor?.OnExportXaml(dlg.FileName, item);
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnImportJsonAsync(string path)
        {
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            var window = ServiceLocator.Instance.Resolve<MainWindow>();

            if (path == null)
            {
                var dlg = new OpenFileDialog()
                {
                    Filter = "Json (*.json)|*.json|All (*.*)|*.*",
                    FilterIndex = 0,
                    Multiselect = true,
                    FileName = ""
                };

                if (dlg.ShowDialog(window) == true)
                {
                    var results = dlg.FileNames;

                    foreach (var result in results)
                    {
                        editor?.OnImportJson(result);
                    }
                }
            }
            else
            {
                if (System.IO.File.Exists(path))
                {
                    editor?.OnImportJson(path);
                }
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnExportJsonAsync(object item)
        {
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            var window = ServiceLocator.Instance.Resolve<MainWindow>();

            var dlg = new SaveFileDialog()
            {
                Filter = "Xaml (*.xaml)|*.xaml|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = editor?.GetName(item)
            };

            if (dlg.ShowDialog(window) == true)
            {
                editor?.OnExportJson(dlg.FileName, item);
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnExportAsync(object item)
        {
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            var window = ServiceLocator.Instance.Resolve<MainWindow>();

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
                if (editorItem?.Project == null)
                    return;

                name = editorItem?.Project?.Name;
                item = editorItem?.Project;
            }
            else if (item == null)
            {
                if (editor.Project == null)
                    return;

                name = editor?.Project?.Name;
                item = editor?.Project;
            }

            var sb = new StringBuilder();
            foreach (var writer in editor?.FileWriters)
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

            if (dlg.ShowDialog(window) == true)
            {
                string result = dlg.FileName;
                IFileWriter writer = editor?.FileWriters[dlg.FilterIndex - 1];
                if (writer != null)
                {
                    editor?.OnExport(result, item, writer);
                }
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnImportDataAsync()
        {
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            var window = ServiceLocator.Instance.Resolve<MainWindow>();

            var dlg = new OpenFileDialog()
            {
                Filter = "Csv (*.csv)|*.csv|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = ""
            };

            if (dlg.ShowDialog(window) == true)
            {
                editor?.OnImportData(dlg.FileName);
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnExportDataAsync()
        {
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            var window = ServiceLocator.Instance.Resolve<MainWindow>();

            var database = editor?.Project?.CurrentDatabase;
            if (database != null)
            {
                var dlg = new SaveFileDialog()
                {
                    Filter = "Csv (*.csv)|*.csv|All (*.*)|*.*",
                    FilterIndex = 0,
                    FileName = database.Name
                };

                if (dlg.ShowDialog(window) == true)
                {
                    editor?.OnExportData(dlg.FileName, database);
                }
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnUpdateDataAsync()
        {
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            var window = ServiceLocator.Instance.Resolve<MainWindow>();

            var database = editor?.Project?.CurrentDatabase;
            if (database != null)
            {
                var dlg = new OpenFileDialog()
                {
                    Filter = "Csv (*.csv)|*.csv|All (*.*)|*.*",
                    FilterIndex = 0,
                    FileName = ""
                };

                if (dlg.ShowDialog(window) == true)
                {
                    editor?.OnUpdateData(dlg.FileName, database);
                }
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnImportObjectAsync(string path)
        {
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            var window = ServiceLocator.Instance.Resolve<MainWindow>();

            if (path == null)
            {
                var dlg = new OpenFileDialog()
                {
                    Filter = "Json (*.json)|*.json|Xaml (*.xaml)|*.xaml",
                    Multiselect = true,
                    FilterIndex = 0
                };

                if (dlg.ShowDialog(window) == true)
                {
                    var results = dlg.FileNames;
                    var index = dlg.FilterIndex;

                    foreach (var result in results)
                    {
                        switch (index)
                        {
                            case 1:
                                editor?.OnImportJson(result);
                                break;
                            case 2:
                                editor?.OnImportXaml(result);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            else
            {
                if (System.IO.File.Exists(path))
                {
                    string resultExtension = System.IO.Path.GetExtension(path);
                    if (string.Compare(resultExtension, ".json", true) == 0)
                    {
                        editor?.OnImportJson(path);
                    }
                    else if (string.Compare(resultExtension, ".xaml", true) == 0)
                    {
                        editor?.OnImportJson(path);
                    }
                }
            }

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnExportObjectAsync(object item)
        {
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            var window = ServiceLocator.Instance.Resolve<MainWindow>();

            if (item != null)
            {
                var dlg = new SaveFileDialog()
                {
                    Filter = "Json (*.json)|*.json|Xaml (*.xaml)|*.xaml",
                    FilterIndex = 0,
                    FileName = editor?.GetName(item)
                };

                if (dlg.ShowDialog(window) == true)
                {
                    switch (dlg.FilterIndex)
                    {
                        case 1:
                            editor?.OnExportJson(dlg.FileName, item);
                            break;
                        case 2:
                            editor?.OnExportXaml(dlg.FileName, item);
                            break;
                        default:
                            break;
                    }
                }
            }
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnCopyAsEmfAsync()
        {
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            var window = ServiceLocator.Instance.Resolve<MainWindow>();

            var page = editor?.Project?.CurrentContainer;
            if (page != null)
            {
                if (editor?.Renderers[0]?.State?.SelectedShape != null)
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
                else if (editor?.Renderers?[0]?.State?.SelectedShapes != null)
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

            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnExportAsEmfAsync(string path)
        {
            var log = ServiceLocator.Instance.Resolve<ILog>();
            var editor = ServiceLocator.Instance.Resolve<ProjectEditor>();
            var window = ServiceLocator.Instance.Resolve<MainWindow>();

            try
            {
                var page = editor?.Project?.CurrentContainer;
                if (page != null)
                {
                    EmfWriter.Save(path, page, editor.Project as IImageCache);
                }
            }
            catch (Exception ex)
            {
                log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }

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
            await Task.Delay(0);
        }

        /// <inheritdoc/>
        async Task IEditorApplication.OnShowDocumentViewerAsync()
        {
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
