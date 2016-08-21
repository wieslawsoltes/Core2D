// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor.Input;

namespace Core2D.Editor.Commands
{
    /// <inheritdoc/>
    public class ZoomAutoFitCommand : Command, IZoomAutoFitCommand
    {
        /// <inheritdoc/>
        public override bool CanRun()
            => ServiceProvider.GetService<ProjectEditor>().IsEditMode();

        /// <inheritdoc/>
        public override void Run()
            => ServiceProvider.GetService<ProjectEditor>().AutoFitZoom?.Invoke();
    }
}
