// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data.Database;
using Core2D.Editor.Input;

namespace Core2D.Editor.Commands
{
    /// <inheritdoc/>
    public class AddColumnCommand : Command<Database>, IAddColumnCommand
    {
        /// <inheritdoc/>
        public override bool CanRun(Database db)
            => ServiceProvider.GetService<ProjectEditor>().IsEditMode();

        /// <inheritdoc/>
        public override void Run(Database db)
            => ServiceProvider.GetService<ProjectEditor>().OnAddColumn(db);
    }
}
