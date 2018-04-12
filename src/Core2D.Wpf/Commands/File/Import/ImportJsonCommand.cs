// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.IO;
using Core2D.Editor;
using Core2D.Editor.Commands;
using Core2D.Editor.Input;
using Core2D.Wpf.Windows;
using Microsoft.Win32;

namespace Core2D.Wpf.Commands
{
    /// <inheritdoc/>
    public class ImportJsonCommand : Command<string>, IImportJsonCommand
    {
        /// <inheritdoc/>
        public override bool CanRun(string path)
        {
            return ServiceProvider.GetService<ProjectEditor>().IsEditMode();
        }

        /// <inheritdoc/>
        public override void Run(string path)
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

                if (dlg.ShowDialog(ServiceProvider.GetService<MainWindow>()) == true)
                {
                    var results = dlg.FileNames;

                    foreach (var result in results)
                    {
                        ServiceProvider.GetService<ProjectEditor>().OnImportJson(result);
                    }
                }
            }
            else
            {
                if (File.Exists(path))
                {
                    ServiceProvider.GetService<ProjectEditor>().OnImportJson(path);
                }
            }
        }
    }
}
