// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Linq;
using Avalonia.Controls;
using Core2D.Avalonia.Windows;
using Core2D.Editor;
using Core2D.Editor.Commands;
using Core2D.Editor.Input;
using Core2D.Interfaces;

namespace Core2D.Avalonia.Commands
{
    /// <inheritdoc/>
    public class OpenCommand : Command<string>, IOpenCommand
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
                dlg.Filters.Add(new FileDialogFilter() { Name = "Project", Extensions = { "project" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                var result = await dlg.ShowAsync(ServiceProvider.GetService<MainWindow>());
                if (result != null)
                {
                    var editor = ServiceProvider.GetService<ProjectEditor>();
                    editor.OnOpen(result.FirstOrDefault());
                    editor.Invalidate?.Invoke();
                }
            }
            else
            {
                if (ServiceProvider.GetService<IFileSystem>().Exists(path))
                {
                    ServiceProvider.GetService<ProjectEditor>().OnOpen(path);
                }
            }
        }
    }
}
