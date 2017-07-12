// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor.Input;
using Core2D.Data;

namespace Core2D.Editor.Commands
{
    /// <inheritdoc/>
    public class ResetRecordCommand : Command<Context>, IResetRecordCommand
    {
        /// <inheritdoc/>
        public override bool CanRun(Context data)
            => ServiceProvider.GetService<ProjectEditor>().IsEditMode();

        /// <inheritdoc/>
        public override void Run(Context data)
            => ServiceProvider.GetService<ProjectEditor>().OnResetRecord(data);
    }
}
