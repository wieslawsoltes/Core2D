// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data;
using Core2D.Editor.Input;

namespace Core2D.Editor.Commands
{
    /// <inheritdoc/>
    public class RemovePropertyCommand : Command<XProperty>, IRemovePropertyCommand
    {
        /// <inheritdoc/>
        public override bool CanRun(XProperty property)
            => ServiceProvider.GetService<ProjectEditor>().IsEditMode();

        /// <inheritdoc/>
        public override void Run(XProperty property)
            => ServiceProvider.GetService<ProjectEditor>().OnRemoveProperty(property);
    }
}
