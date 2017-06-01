// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Threading.Tasks;
using Core2D.Editor.Input;

namespace Core2D.Editor.Commands
{
    /// <inheritdoc/>
    public class AddImageKeyCommand : Command, IAddImageKeyCommand
    {
        /// <inheritdoc/>
        public override bool CanRun()
            => ServiceProvider.GetService<ProjectEditor>().IsEditMode();

        /// <inheritdoc/>
        public override async void Run()
            => await (ServiceProvider.GetService<ProjectEditor>().OnAddImageKey(null) ?? Task.FromResult(string.Empty));
    }
}
