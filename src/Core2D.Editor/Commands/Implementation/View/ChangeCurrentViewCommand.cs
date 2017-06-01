// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor.Input;
using Core2D.Editor.Views.Interfaces;

namespace Core2D.Editor.Commands
{
    /// <inheritdoc/>
    public class ChangeCurrentViewCommand : Command<IView>, IChangeCurrentViewCommand
    {
        /// <inheritdoc/>
        public override bool CanRun(IView view)
            => ServiceProvider.GetService<ProjectEditor>().IsEditMode();

        /// <inheritdoc/>
        public override void Run(IView view)
            => ServiceProvider.GetService<ProjectEditor>().OnChangeCurrentView(view);
    }
}
