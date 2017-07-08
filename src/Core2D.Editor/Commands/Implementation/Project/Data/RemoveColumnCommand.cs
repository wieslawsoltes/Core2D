// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data;
using Core2D.Editor.Input;

namespace Core2D.Editor.Commands
{
    /// <inheritdoc/>
    public class RemoveColumnCommand : Command<Column>, IRemoveColumnCommand
    {
        /// <inheritdoc/>
        public override bool CanRun(Column column)
            => ServiceProvider.GetService<ProjectEditor>().IsEditMode();

        /// <inheritdoc/>
        public override void Run(Column column)
            => ServiceProvider.GetService<ProjectEditor>().OnRemoveColumn(column);
    }
}
