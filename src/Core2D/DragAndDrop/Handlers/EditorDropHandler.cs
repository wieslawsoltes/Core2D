using System.Linq;
using Avalonia.Input;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Editor;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.DragAndDrop.Handlers
{
    public class EditorDropHandler : DefaultDropHandler
    {
        private bool Validate(ProjectEditor editor, object sender, DragEventArgs e, bool bExecute)
        {
            var point = GetPosition(sender, e);

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
                    case BaseShape shape:
                        return editor?.OnDropShape(shape, point.X, point.Y, bExecute) == true;
                    case Record record:
                        return editor?.OnDropRecord(record, point.X, point.Y, bExecute) == true;
                    case ShapeStyle style:
                        return editor?.OnDropStyle(style, point.X, point.Y, bExecute) == true;
                    case PageContainer page:
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
                    editor?.OnDropFiles(files, point.X, point.Y);
                }
                return true;
            }

            return false;
        }

        public override bool Validate(object sender, DragEventArgs e, object sourceContext, object targetContext, object state)
        {
            if (targetContext is ProjectEditor editor)
            {
                return Validate(editor, sender, e, false);
            }
            return false;
        }

        public override bool Execute(object sender, DragEventArgs e, object sourceContext, object targetContext, object state)
        {
            if (targetContext is ProjectEditor editor)
            {
                return Validate(editor, sender, e, true);
            }
            return false;
        }
    }
}
