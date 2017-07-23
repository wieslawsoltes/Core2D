// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor.Input;
using Core2D.Containers;
using Core2D.Shapes;

namespace Core2D.Editor.Commands
{
    /// <inheritdoc/>
    public class RemoveGroupLibraryCommand : Command<Library<GroupShape>>, IRemoveGroupLibraryCommand
    {
        /// <inheritdoc/>
        public override bool CanRun(Library<GroupShape> library)
            => ServiceProvider.GetService<ProjectEditor>().IsEditMode();

        /// <inheritdoc/>
        public override void Run(Library<GroupShape> library)
            => ServiceProvider.GetService<ProjectEditor>().OnRemoveGroupLibrary(library);
    }
}
