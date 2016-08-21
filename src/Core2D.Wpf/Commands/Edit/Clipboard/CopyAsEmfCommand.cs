// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Linq;
using Core2D.Editor;
using Core2D.Editor.Commands;
using Core2D.Editor.Input;
using FileWriter.Emf;

namespace Core2D.Wpf.Commands
{
    /// <inheritdoc/>
    public class CopyAsEmfCommand : Command<object>, ICopyAsEmfCommand
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
            var page = editor.Project?.CurrentContainer;
            if (page != null)
            {
                if (editor.Renderers[0]?.State?.SelectedShape != null)
                {
                    var shapes = Enumerable.Repeat(editor.Renderers[0].State.SelectedShape, 1).ToList();
                    EmfWriter.SetClipboard(
                        shapes,
                        page.Template.Width,
                        page.Template.Height,
                        page.Data.Properties,
                        page.Data.Record,
                        editor.Project);
                }
                else if (editor.Renderers?[0]?.State?.SelectedShapes != null)
                {
                    var shapes = editor.Renderers[0].State.SelectedShapes.ToList();
                    EmfWriter.SetClipboard(
                        shapes,
                        page.Template.Width,
                        page.Template.Height,
                        page.Data.Properties,
                        page.Data.Record,
                        editor.Project);
                }
                else
                {
                    EmfWriter.SetClipboard(page, editor.Project);
                }
            }
        }
    }
}
