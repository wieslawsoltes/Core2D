// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor.Input;
using Core2D.Project;
using Core2D.Shapes;

namespace Core2D.Editor.Commands
{
    /// <inheritdoc/>
    public class RemoveGroupLibraryCommand : Command<XLibrary<XGroup>>, IRemoveGroupLibraryCommand
    {
        /// <inheritdoc/>
        public override bool CanRun(XLibrary<XGroup> library)
            => ServiceProvider.GetService<ProjectEditor>().IsEditMode();

        /// <inheritdoc/>
        public override void Run(XLibrary<XGroup> library)
            => ServiceProvider.GetService<ProjectEditor>().OnRemoveGroupLibrary(library);
    }
}
