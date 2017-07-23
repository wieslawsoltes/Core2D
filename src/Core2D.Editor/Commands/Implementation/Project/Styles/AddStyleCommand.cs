// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor.Input;
using Core2D.Containers;
using Core2D.Style;

namespace Core2D.Editor.Commands
{
    /// <inheritdoc/>
    public class AddStyleCommand : Command<Library<ShapeStyle>>, IAddStyleCommand
    {
        /// <inheritdoc/>
        public override bool CanRun(Library<ShapeStyle> library)
            => ServiceProvider.GetService<ProjectEditor>().IsEditMode();

        /// <inheritdoc/>
        public override void Run(Library<ShapeStyle> library)
            => ServiceProvider.GetService<ProjectEditor>().OnAddStyle(library);
    }
}
