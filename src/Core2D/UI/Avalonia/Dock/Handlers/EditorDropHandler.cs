// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Linq;
using Avalonia.Input;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Editor;
using Core2D.Shapes;
using Core2D.Style;
using Dock.Avalonia;

namespace Core2D.UI.Avalonia.Dock.Handlers
{
    /// <summary>
    /// Project editor drop handler.
    /// </summary>
    public class EditorDropHandler : DefaultDropHandler
    {
        public static IDropHandler Instance = new EditorDropHandler();

        private bool Validate(IProjectEditor editor, object sender, DragEventArgs e, bool bExecute)
        {
            var point = DropHelper.GetPosition(sender, e);

            if (e.Data.Contains(DataFormats.Text))
            {
                var text = e.Data.GetText();

                if (bExecute)
                {
                    editor?.OnTryPaste(text);
                }

                return true;
            }

            foreach (var format in e.Data.GetDataFormats())
            {
                var data = e.Data.Get(format);

                switch (data)
                {
                    case IBaseShape shape:
                        return editor?.OnDropShape(shape, point.X, point.Y, bExecute) == true;
                    case IRecord record:
                        return editor?.OnDropRecord(record, point.X, point.Y, bExecute) == true;
                    case IShapeStyle style:
                        return editor?.OnDropStyle(style, point.X, point.Y, bExecute) == true;
                    case IPageContainer page:
                        return editor?.OnDropTemplate(page, point.X, point.Y, bExecute) == true;
                    default:
                        break;
                }
            }

            if (e.Data.Contains(DataFormats.FileNames))
            {
                var files = e.Data.GetFileNames().ToArray();
                if (bExecute)
                {
                    editor?.OnDropFiles(files);
                }
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public override bool Validate(object sender, DragEventArgs e, object sourceContext, object targetContext, object state)
        {
            if (targetContext is IProjectEditor editor)
            {
                return Validate(editor, sender, e, false);
            }
            return false;
        }

        /// <inheritdoc/>
        public override bool Execute(object sender, DragEventArgs e, object sourceContext, object targetContext, object state)
        {
            if (targetContext is IProjectEditor editor)
            {
                return Validate(editor, sender, e, true);
            }
            return false;
        }
    }
}
