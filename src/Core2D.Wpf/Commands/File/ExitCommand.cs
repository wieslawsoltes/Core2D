// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows;
using Core2D.Editor;
using Core2D.Editor.Commands;
using Core2D.Editor.Input;
using Core2D.Wpf.Windows;

namespace Core2D.Wpf.Commands
{
    /// <inheritdoc/>
    public class ExitCommand : Command, IExitCommand
    {
        /// <inheritdoc/>
        public override bool CanRun()
        {
            return true;
        }

        /// <inheritdoc/>
        public override void Run()
        {
            var window = ServiceProvider.GetService<MainWindow>();
            var editor = ServiceProvider.GetService<ProjectEditor>();
            if (editor.IsProjectDirty)
            {
                var result = MessageBox.Show(
                    "Save changes to the project?",
                    "Unsaved Changes",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question,
                    MessageBoxResult.Yes);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        {
                            var save = new SaveCommand();
                            if (save.CanRun())
                            {
                                save.Run();
                            }
                            window.Close();
                        }
                        break;
                    case MessageBoxResult.No:
                        {
                            window.Close();
                        }
                        break;
                    case MessageBoxResult.Cancel:
                        break;
                }
            }
            else
            {
                window.Close();
            }
        }
    }
}
