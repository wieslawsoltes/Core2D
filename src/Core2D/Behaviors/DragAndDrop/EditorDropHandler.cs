#nullable enable
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.Behaviors.DragAndDrop;

public class EditorDropHandler : DefaultDropHandler
{
    public static readonly StyledProperty<IControl?> RelativeToProperty = 
        AvaloniaProperty.Register<EditorDropHandler, IControl?>(nameof(RelativeTo));

    public IControl? RelativeTo
    {
        get => GetValue(RelativeToProperty);
        set => SetValue(RelativeToProperty, value);
    }

    private bool Validate(ProjectEditorViewModel editor, object? sender, DragEventArgs e, bool bExecute)
    {
        var relativeTo = RelativeTo ?? sender as IControl;
        if (relativeTo is null)
        {
            return false;
        }
        var point = GetPosition(relativeTo, e);

        if (e.Data.Contains(DataFormats.Text))
        {
            var text = e.Data.GetText();

            if (bExecute)
            {
                if (text is { })
                {
                    editor.ClipboardService?.OnTryPaste(text);
                }
            }

            return true;
        }

        foreach (var format in e.Data.GetDataFormats())
        {
            var data = e.Data.Get(format);

            switch (data)
            {
                case BaseShapeViewModel shape:
                    return editor.OnDropShape(shape, point.X, point.Y, bExecute);
                case RecordViewModel record:
                    return editor.OnDropRecord(record, point.X, point.Y, bExecute);
                case ShapeStyleViewModel style:
                    return editor.OnDropStyle(style, point.X, point.Y, bExecute);
                case TemplateContainerViewModel template:
                    return editor.OnDropTemplate(template, point.X, point.Y, bExecute);
            }
        }

        if (e.Data.Contains(DataFormats.FileNames))
        {
            var files = e.Data.GetFileNames()?.ToArray();
            if (bExecute)
            {
                if (files is { })
                {
                    return editor.OnDropFiles(files, point.X, point.Y).Result;
                }
            }
            return true;
        }

        return false;
    }

    public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        if (targetContext is ProjectEditorViewModel editor)
        {
            return Validate(editor, sender, e, false);
        }
        return false;
    }

    public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        if (targetContext is ProjectEditorViewModel editor)
        {
            return Validate(editor, sender, e, true);
        }
        return false;
    }
}
