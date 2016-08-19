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
    public class OpenCommand : Command<string>, IOpenCommand
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
                    Filter = "Project (*.project)|*.project|All (*.*)|*.*",
                    FilterIndex = 0,
                    FileName = ""
                };

                if (dlg.ShowDialog(ServiceProvider.GetService<MainWindow>()) == true)
                {
                    ServiceProvider.GetService<ProjectEditor>().OnOpen(dlg.FileName);
                }
            }
            else
            {
                if (File.Exists(path))
                {
                    ServiceProvider.GetService<ProjectEditor>().OnOpen(path);
                }
            }
        }
    }
}
