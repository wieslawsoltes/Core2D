// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Linq;
using Avalonia.Controls;
using Core2D.Avalonia.Windows;
using Core2D.Data.Database;
using Core2D.Editor;
using Core2D.Editor.Commands;
using Core2D.Editor.Input;

namespace Core2D.Avalonia.Commands
{
    /// <inheritdoc/>
    public class UpdateDataCommand : Command<XDatabase>, IUpdateDataCommand
    {
        /// <inheritdoc/>
        public override bool CanRun(XDatabase db)
        {
            return ServiceProvider.GetService<ProjectEditor>().IsEditMode();
        }

        /// <inheritdoc/>
        public override async void Run(XDatabase db)
        {
            if (db != null)
            {
                var dlg = new OpenFileDialog();
                dlg.Filters.Add(new FileDialogFilter() { Name = "Csv", Extensions = { "csv" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                var result = await dlg.ShowAsync(ServiceProvider.GetService<MainWindow>());
                if (result != null)
                {
                    ServiceProvider.GetService<ProjectEditor>().OnUpdateData(result.FirstOrDefault(), db);
                }
            }
        }
    }
}
