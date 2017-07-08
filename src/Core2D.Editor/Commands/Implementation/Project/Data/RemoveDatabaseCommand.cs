// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data.Database;
using Core2D.Editor.Input;

namespace Core2D.Editor.Commands
{
    /// <inheritdoc/>
    public class RemoveDatabaseCommand : Command<Database>, IRemoveDatabaseCommand
    {
        /// <inheritdoc/>
        public override bool CanRun(Database db)
            => ServiceProvider.GetService<ProjectEditor>().IsEditMode();

        /// <inheritdoc/>
        public override void Run(Database db)
            => ServiceProvider.GetService<ProjectEditor>().OnRemoveDatabase(db);
    }
}
