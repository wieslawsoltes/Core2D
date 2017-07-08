// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Text;
using Core2D.Editor;
using Core2D.Editor.Commands;
using Core2D.Editor.Input;
using Core2D.Interfaces;
using Core2D.Project;
using Core2D.Wpf.Windows;
using Microsoft.Win32;

namespace Core2D.Wpf.Commands
{
    /// <inheritdoc/>
    public class ExportCommand : Command<object>, IExportCommand
    {
        /// <inheritdoc/>
        public override bool CanRun(object item)
        {
            return ServiceProvider.GetService<ProjectEditor>().IsEditMode();
        }

        /// <inheritdoc/>
        public override void Run(object item)
        {
            var editor = ServiceProvider.GetService<ProjectEditor>();

            string name = string.Empty;

            if (item == null || item is ProjectEditor)
            {
                if (editor.Project == null)
                {
                    return;
                }

                name = editor.Project.Name;
                item = editor.Project;
            }
            else if (item is ProjectContainer)
            {
                name = (item as ProjectContainer).Name;
            }
            else if (item is DocumentContainer)
            {
                name = (item as DocumentContainer).Name;
            }
            else if (item is PageContainer)
            {
                name = (item as PageContainer).Name;
            }

            var sb = new StringBuilder();
            foreach (var writer in editor.FileWriters)
            {
                sb.Append($"{writer.Name} (*.{writer.Extension})|*.{writer.Extension}|");
            }
            sb.Append("All (*.*)|*.*");

            var dlg = new SaveFileDialog()
            {
                Filter = sb.ToString(),
                FilterIndex = 0,
                FileName = name
            };

            if (dlg.ShowDialog(ServiceProvider.GetService<MainWindow>()) == true)
            {
                string result = dlg.FileName;
                IFileWriter writer = editor.FileWriters[dlg.FilterIndex - 1];
                if (writer != null)
                {
                    editor.OnExport(result, item, writer);
                }
            }
        }
    }
}
