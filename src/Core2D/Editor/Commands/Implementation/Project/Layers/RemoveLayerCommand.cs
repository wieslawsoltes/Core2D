// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor.Input;
using Core2D.Project;

namespace Core2D.Editor.Commands
{
    /// <inheritdoc/>
    public class RemoveLayerCommand : Command<XLayer>, IRemoveLayerCommand
    {
        /// <inheritdoc/>
        public override bool CanRun(XLayer layer)
            => ServiceProvider.GetService<ProjectEditor>().IsEditMode();

        /// <inheritdoc/>
        public override void Run(XLayer layer)
            => ServiceProvider.GetService<ProjectEditor>().OnRemoveLayer(layer);
    }
}
