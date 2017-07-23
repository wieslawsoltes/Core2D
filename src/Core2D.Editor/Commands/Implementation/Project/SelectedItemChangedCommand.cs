// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor.Input;
using Core2D.Containers;

namespace Core2D.Editor.Commands
{
    /// <inheritdoc/>
    public class SelectedItemChangedCommand : Command<SelectableObject>, ISelectedItemChangedCommand
    {
        /// <inheritdoc/>
        public override bool CanRun(SelectableObject item)
            => ServiceProvider.GetService<ProjectEditor>().IsEditMode();

        /// <inheritdoc/>
        public override void Run(SelectableObject item)
            => ServiceProvider.GetService<ProjectEditor>().OnSelectedItemChanged(item);
    }
}
