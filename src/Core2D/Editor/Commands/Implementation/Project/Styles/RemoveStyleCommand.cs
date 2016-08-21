// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor.Input;
using Core2D.Style;

namespace Core2D.Editor.Commands
{
    /// <inheritdoc/>
    public class RemoveStyleCommand : Command<ShapeStyle>, IRemoveStyleCommand
    {
        /// <inheritdoc/>
        public override bool CanRun(ShapeStyle style)
            => ServiceProvider.GetService<ProjectEditor>().IsEditMode();

        /// <inheritdoc/>
        public override void Run(ShapeStyle style)
            => ServiceProvider.GetService<ProjectEditor>().OnRemoveStyle(style);
    }
}
