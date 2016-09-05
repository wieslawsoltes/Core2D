// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.IO;
using Core2D.Editor;
using Core2D.Editor.Commands;
using Core2D.Editor.Input;
using Core2D.Interfaces;
using Core2D.Wpf.Windows;
using Microsoft.Win32;

namespace Core2D.Wpf.Commands
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
        public override void Run(string path)
        {
            if (path == null)
            {
                var dlg = new OpenFileDialog()
                {
                    Filter = "Script (*.cs)|*.cs|All (*.*)|*.*",
                    FilterIndex = 0,
                    FileName = "",
                    Multiselect = true
                };

                if (dlg.ShowDialog(ServiceProvider.GetService<MainWindow>()) == true)
                {
                    Load(dlg.FileNames);
                }
            }
            else
            {
                if (File.Exists(path))
                {
                    Load(path);
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
