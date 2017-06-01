// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor;
using Core2D.Editor.Commands;
using Core2D.Editor.Input;

namespace Core2D.Avalonia.Commands
{
    /// <inheritdoc/>
    public class ResetWindowLayoutCommand : Command, IResetWindowLayoutCommand
    {
        /// <inheritdoc/>
        public override bool CanRun() => true;

        /// <inheritdoc/>
        public override void Run()
            => ServiceProvider.GetService<ProjectEditor>().ResetLayout?.Invoke();
    }
}
