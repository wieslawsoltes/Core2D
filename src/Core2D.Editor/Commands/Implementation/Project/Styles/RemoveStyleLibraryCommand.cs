// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor.Input;
using Core2D.Project;
using Core2D.Style;

namespace Core2D.Editor.Commands
{
    /// <inheritdoc/>
    public class RemoveStyleLibraryCommand : Command<Library<ShapeStyle>>, IRemoveStyleLibraryCommand
    {
        /// <inheritdoc/>
        public override bool CanRun(Library<ShapeStyle> library)
            => ServiceProvider.GetService<ProjectEditor>().IsEditMode();

        /// <inheritdoc/>
        public override void Run(Library<ShapeStyle> library)
            => ServiceProvider.GetService<ProjectEditor>().OnRemoveStyleLibrary(library);
    }
}
