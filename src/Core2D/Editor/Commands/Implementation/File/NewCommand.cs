// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor.Input;

namespace Core2D.Editor.Commands
{
    /// <inheritdoc/>
    public class NewCommand : Command<object>, INewCommand
    {
        /// <inheritdoc/>
        public override bool CanRun(object parameter) 
            => ServiceProvider.GetService<ProjectEditor>().IsEditMode();

        /// <inheritdoc/>
        public override void Run(object parameter) 
            => ServiceProvider.GetService<ProjectEditor>()?.OnNew(parameter);
    }
}
