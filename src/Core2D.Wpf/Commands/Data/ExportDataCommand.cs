// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data.Database;
using Core2D.Editor;
using Core2D.Editor.Commands;
using Core2D.Editor.Input;
using Core2D.Wpf.Windows;
using Microsoft.Win32;

namespace Core2D.Wpf.Commands
{
    /// <inheritdoc/>
    public class ExportDataCommand : Command<XDatabase>, IExportDataCommand
    {
        /// <inheritdoc/>
        public override bool CanRun(XDatabase db)
        {
            return ServiceProvider.GetService<ProjectEditor>().IsEditMode();
        }

        /// <inheritdoc/>
        public override void Run(XDatabase db)
        {
            if (db != null)
            {
                var dlg = new SaveFileDialog()
                {
                    Filter = "Csv (*.csv)|*.csv|All (*.*)|*.*",
                    FilterIndex = 0,
                    FileName = db.Name
                };

                if (dlg.ShowDialog(ServiceProvider.GetService<MainWindow>()) == true)
                {
                    ServiceProvider.GetService<ProjectEditor>().OnExportData(dlg.FileName, db);
                }
            }
        }
    }
}
