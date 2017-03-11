// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor;
using Core2D.Editor.Commands;
using Core2D.Editor.Input;
using Core2D.Interfaces;

namespace Core2D.Universal.Commands
{
    /// <inheritdoc/>
    public class OpenCommand : Command<string>, IOpenCommand
    {
        /// <inheritdoc/>
        public override bool CanRun(string path)
        {
            return ServiceProvider.GetService<ProjectEditor>().IsEditMode();
        }

        /// <inheritdoc/>
        public override async void Run(string path)
        {
            if (path == null)
            {
                await ServiceProvider.GetService<MainPage>().OpenProject();
            }
            else
            {
                if (ServiceProvider.GetService<IFileSystem>().Exists(path))
                {
                    ServiceProvider.GetService<ProjectEditor>().OnOpen(path);
                }
            }
        }
    }
}
