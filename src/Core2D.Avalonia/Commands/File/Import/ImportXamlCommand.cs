// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia.Controls;
using Core2D.Avalonia.Windows;
using Core2D.Editor;
using Core2D.Editor.Commands;
using Core2D.Editor.Input;
using Core2D.Interfaces;

namespace Core2D.Avalonia.Commands
{
    /// <inheritdoc/>
    public class ImportXamlCommand : Command<string>, IImportXamlCommand
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
                dlg.Filters.Add(new FileDialogFilter() { Name = "Xaml", Extensions = { "xaml" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });

                var results = await dlg.ShowAsync(ServiceProvider.GetService<MainWindow>());
                if (results != null)
                {
                    foreach (var result in results)
                    {
                        ServiceProvider.GetService<ProjectEditor>().OnImportXaml(result);
                    }
                }
            }
            else
            {
                if (ServiceProvider.GetService<IFileSystem>().Exists(path))
                {
                    ServiceProvider.GetService<ProjectEditor>().OnImportXaml(path);
                }
            }
        }
    }
}
