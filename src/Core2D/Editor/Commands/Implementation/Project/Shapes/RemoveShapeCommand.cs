// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor.Input;
using Core2D.Shape;

namespace Core2D.Editor.Commands
{
    /// <inheritdoc/>
    public class RemoveShapeCommand : Command<BaseShape>, IRemoveShapeCommand
    {
        /// <inheritdoc/>
        public override bool CanRun(BaseShape shape)
            => ServiceProvider.GetService<ProjectEditor>().IsEditMode();

        /// <inheritdoc/>
        public override void Run(BaseShape shape)
            => ServiceProvider.GetService<ProjectEditor>().OnRemoveShape(shape);
    }
}
