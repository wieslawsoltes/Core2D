// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor.Input;
using Core2D.Shapes;

namespace Core2D.Editor.Commands
{
    /// <inheritdoc/>
    public class RemoveGroupCommand : Command<XGroup>, IRemoveGroupCommand
    {
        /// <inheritdoc/>
        public override bool CanRun(XGroup group)
            => ServiceProvider.GetService<ProjectEditor>().IsEditMode();

        /// <inheritdoc/>
        public override void Run(XGroup group)
            => ServiceProvider.GetService<ProjectEditor>().OnRemoveGroup(group);
    }
}
