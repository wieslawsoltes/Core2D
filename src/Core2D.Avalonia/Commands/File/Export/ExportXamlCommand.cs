// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia.Controls;
using Core2D.Avalonia.Windows;
using Core2D.Editor;
using Core2D.Editor.Commands;
using Core2D.Editor.Input;

namespace Core2D.Avalonia.Commands
{
    /// <inheritdoc/>
    public class ExportXamlCommand : Command<object>, IExportXamlCommand
    {
        /// <inheritdoc/>
        public override bool CanRun(object item)
        {
            return ServiceProvider.GetService<ProjectEditor>().IsEditMode();
        }

        /// <inheritdoc/>
        public override async void Run(object item)
        {
            var editor = ServiceProvider.GetService<ProjectEditor>();
            var dlg = new SaveFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Xaml", Extensions = { "xaml" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
            dlg.InitialFileName = editor?.GetName(item);
            dlg.DefaultExtension = "xaml";
            var result = await dlg.ShowAsync(ServiceProvider.GetService<MainWindow>());
            if (result != null)
            {
                editor.OnExportXaml(result, item);
            }
        }
    }
}
