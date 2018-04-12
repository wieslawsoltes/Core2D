// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Avalonia.Windows;
using Core2D.Editor;
using Core2D.Editor.Commands;
using Core2D.Editor.Input;

namespace Core2D.Avalonia.Commands
{
    /// <inheritdoc/>
    public class ObjectBrowserCommand : Command, IObjectBrowserCommand
    {
        /// <inheritdoc/>
        public override bool CanRun()
        {
            return ServiceProvider.GetService<ProjectEditor>().IsEditMode();
        }

        /// <inheritdoc/>
        public override void Run()
        {
            new BrowserWindow()
            {
                DataContext = ServiceProvider.GetService<ProjectEditor>()
            }
            .Show();
        }
    }
}
