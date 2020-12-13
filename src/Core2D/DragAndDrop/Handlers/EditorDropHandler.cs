using System.Linq;
using Avalonia.Input;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.DragAndDrop.Handlers
{
    public class EditorDropHandler : DefaultDropHandler
    {
        private bool Validate(ProjectEditorViewModel editorViewModel, object sender, DragEventArgs e, bool bExecute)
        {
            var point = GetPosition(sender, e);

            if (e.Data.Contains(DataFormats.Text))
            {
                var text = e.Data.GetText();

                if (bExecute)
                {
                    editorViewModel?.OnTryPaste(text);
                }

                return true;
            }

            foreach (var format in e.Data.GetDataFormats())
            {
                var data = e.Data.Get(format);

                switch (data)
                {
                    case BaseShapeViewModel shape:
                        return editorViewModel?.OnDropShape(shape, point.X, point.Y, bExecute) == true;
                    case RecordViewModel record:
                        return editorViewModel?.OnDropRecord(record, point.X, point.Y, bExecute) == true;
                    case ShapeStyleViewModel style:
                        return editorViewModel?.OnDropStyle(style, point.X, point.Y, bExecute) == true;
                    case TemplateContainerViewModel template:
                        return editorViewModel?.OnDropTemplate(template, point.X, point.Y, bExecute) == true;
                    default:
                        break;
                }
            }

            if (e.Data.Contains(DataFormats.FileNames))
            {
                var files = e.Data.GetFileNames().ToArray();
                if (bExecute)
                {
                    editorViewModel?.OnDropFiles(files, point.X, point.Y);
                }
                return true;
            }

            return false;
        }

        public override bool Validate(object sender, DragEventArgs e, object sourceContext, object targetContext, object state)
        {
            if (targetContext is ProjectEditorViewModel editor)
            {
                return Validate(editor, sender, e, false);
            }
            return false;
        }

        public override bool Execute(object sender, DragEventArgs e, object sourceContext, object targetContext, object state)
        {
            if (targetContext is ProjectEditorViewModel editor)
            {
                return Validate(editor, sender, e, true);
            }
            return false;
        }
    }
}
