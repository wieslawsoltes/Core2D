// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor;
using Core2D.Editor.Commands;
using Core2D.Editor.Input;
using Core2D.Wpf.Windows;
using Microsoft.Win32;

namespace Core2D.Wpf.Commands
{
    /// <inheritdoc/>
    public class ExportObjectCommand : Command<object>, IExportObjectCommand
    {
        /// <inheritdoc/>
        public override bool CanRun(object item)
        {
            return ServiceProvider.GetService<ProjectEditor>().IsEditMode();
        }

        /// <inheritdoc/>
        public override void Run(object item)
        {
            var editor = ServiceProvider.GetService<ProjectEditor>();
            if (item != null)
            {
                var dlg = new SaveFileDialog()
                {
                    Filter = "Json (*.json)|*.json|Xaml (*.xaml)|*.xaml",
                    FilterIndex = 0,
                    FileName = editor.GetName(item)
                };

                if (dlg.ShowDialog(ServiceProvider.GetService<MainWindow>()) == true)
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
    }
}
