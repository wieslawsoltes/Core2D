// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia.Controls;
using Core2D.Avalonia.Windows;
using Core2D.Editor;
using Core2D.Editor.Commands;
using Core2D.Editor.Input;
using Core2D.Interfaces;

namespace Core2D.Avalonia.Commands
{
    /// <inheritdoc/>
    public class ExecuteScriptCommand : Command<string>, IExecuteScriptCommand
    {
        /// <inheritdoc/>
        public override bool CanRun(string path)
        {
            return true;
        }

        /// <inheritdoc/>
        public override async void Run(string path)
        {
            if (path == null)
            {
                var dlg = new OpenFileDialog();
                dlg.Filters.Add(new FileDialogFilter() { Name = "Script", Extensions = { "cs" } });
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                dlg.AllowMultiple = true;
                var result = await dlg.ShowAsync(ServiceProvider.GetService<MainWindow>());
                if (result != null)
                {
                    Load(result);
                }
            }
            else
            {
                if (ServiceProvider.GetService<IFileSystem>().Exists(path))
                {
                    ServiceProvider.GetService<ProjectEditor>().OnOpenProject(path);
                }
            }
        }

        private void Load(string[] paths)
        {
            foreach (var path in paths)
            {
                Load(path);
            }
        }

        private void Load(string path)
        {
            var fileIO = ServiceProvider.GetService<IFileSystem>();
            var csharp = fileIO.ReadUtf8Text(path);
            if (!string.IsNullOrEmpty(csharp))
            {
                ServiceProvider.GetService<IScriptRunner>().Execute(csharp);
                Execute(csharp);
            }
        }
    }
}
