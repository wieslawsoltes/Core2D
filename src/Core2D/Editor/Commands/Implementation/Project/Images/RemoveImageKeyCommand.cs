// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor.Input;

namespace Core2D.Editor.Commands
{
    /// <inheritdoc/>
    public class RemoveImageKeyCommand : Command<string>, IRemoveImageKeyCommand
    {
        /// <inheritdoc/>
        public override bool CanRun(string key)
            => ServiceProvider.GetService<ProjectEditor>().IsEditMode();

        /// <inheritdoc/>
        public override void Run(string key)
            => ServiceProvider.GetService<ProjectEditor>().OnRemoveImageKey(key);
    }
}
