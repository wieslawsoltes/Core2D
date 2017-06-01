// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Avalonia.Controls;
using Core2D.Avalonia.Windows;
using Core2D.Editor;
using Core2D.Editor.Commands;
using Core2D.Editor.Input;
using Core2D.Interfaces;

namespace Core2D.Avalonia.Commands
{
    /// <inheritdoc/>
    public class ImportObjectCommand : Command<string>, IImportObjectCommand
    {
        /// <inheritdoc/>
        public override bool CanRun(string path)
        {
            return ServiceProvider.GetService<ProjectEditor>().IsEditMode();
        }

        /// <inheritdoc/>
        public override async void Run(string path)
        {
            if (path == null)
            {
                var dlg = new OpenFileDialog();
                dlg.AllowMultiple = true;
                dlg.Filters.Add(new FileDialogFilter() { Name = "Json", Extensions = { "json" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "Xaml", Extensions = { "xaml" } });
                var results = await dlg.ShowAsync(ServiceProvider.GetService<MainWindow>());
                if (results != null)
                {
                    foreach (var result in results)
                    {
                        string resultExtension = System.IO.Path.GetExtension(result);
                        if (string.Compare(resultExtension, ".json", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            ServiceProvider.GetService<ProjectEditor>().OnImportJson(result);
                        }
                        else if (string.Compare(resultExtension, ".xaml", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            ServiceProvider.GetService<ProjectEditor>().OnImportJson(result);
                        }
                    }
                }
            }
            else
            {
                if (ServiceProvider.GetService<IFileSystem>().Exists(path))
                {
                    string resultExtension = System.IO.Path.GetExtension(path);
                    if (string.Compare(resultExtension, ".json", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        ServiceProvider.GetService<ProjectEditor>().OnImportJson(path);
                    }
                    else if (string.Compare(resultExtension, ".xaml", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        ServiceProvider.GetService<ProjectEditor>().OnImportJson(path);
                    }
                }
            }
        }
    }
}
