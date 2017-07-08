// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor.Input;
using Core2D.Project;
using Core2D.Shapes;

namespace Core2D.Editor.Commands
{
    /// <inheritdoc/>
    public class AddGroupCommand : Command<XLibrary<GroupShape>>, IAddGroupCommand
    {
        /// <inheritdoc/>
        public override bool CanRun(XLibrary<GroupShape> library)
            => ServiceProvider.GetService<ProjectEditor>().IsEditMode();

        /// <inheritdoc/>
        public override void Run(XLibrary<GroupShape> library)
            => ServiceProvider.GetService<ProjectEditor>().OnAddGroup(library);
    }
}
