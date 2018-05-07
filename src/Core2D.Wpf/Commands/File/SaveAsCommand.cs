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
    public class SaveAsCommand : Command, ISaveAsCommand
    {
        /// <inheritdoc/>
        public override bool CanRun()
        {
            return ServiceProvider.GetService<ProjectEditor>().IsEditMode();
        }

        /// <inheritdoc/>
        public override void Run()
        {
            var editor = ServiceProvider.GetService<ProjectEditor>();
            var dlg = new SaveFileDialog()
            {
                Filter = "Project (*.project)|*.project|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = editor.Project?.Name
            };

            if (dlg.ShowDialog(ServiceProvider.GetService<MainWindow>()) == true)
            {
                editor.OnSaveProject(dlg.FileName);
            }
        }
    }
}
