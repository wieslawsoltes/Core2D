// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia.Controls;
using Core2D.Avalonia.Windows;
using Core2D.Data.Database;
using Core2D.Editor;
using Core2D.Editor.Commands;
using Core2D.Editor.Input;

namespace Core2D.Avalonia.Commands
{
    /// <inheritdoc/>
    public class ExportDataCommand : Command<Database>, IExportDataCommand
    {
        /// <inheritdoc/>
        public override bool CanRun(Database db)
        {
            return ServiceProvider.GetService<ProjectEditor>().IsEditMode();
        }

        /// <inheritdoc/>
        public override async void Run(Database db)
        {
            if (db != null)
            {
                var dlg = new SaveFileDialog();
                dlg.Filters.Add(new FileDialogFilter() { Name = "Csv", Extensions = { "csv" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                dlg.InitialFileName = db.Name;
                dlg.DefaultExtension = "csv";
                var result = await dlg.ShowAsync(ServiceProvider.GetService<MainWindow>());
                if (result != null)
                {
                    ServiceProvider.GetService<ProjectEditor>().OnExportData(result, db);
                }
            }
        }
    }
}
