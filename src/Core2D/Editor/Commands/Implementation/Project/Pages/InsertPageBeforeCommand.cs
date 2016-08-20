// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor.Input;

namespace Core2D.Editor.Commands
{
    /// <inheritdoc/>
    public class InsertPageBeforeCommand : Command<object>, IInsertPageBeforeCommand
    {
        /// <inheritdoc/>
        public override bool CanRun(object item)
            => ServiceProvider.GetService<ProjectEditor>().IsEditMode();

        /// <inheritdoc/>
        public override void Run(object item)
            => ServiceProvider.GetService<ProjectEditor>().OnInsertPageBefore(item);
    }
}
