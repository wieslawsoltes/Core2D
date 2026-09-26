using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using Core2D.Controls.Studio;
using Core2D.ViewModels.Scripting;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioCodeEditorTests
{
    [AvaloniaFact]
    public void NativeEditingUpdatesModelAndExternalReplacementClearsUndo()
    {
        var model = new ScriptViewModel(null) { Code = "// Source\n" };
        var control = new StudioCodeEditor { DataContext = model };
        control.Bind(StudioCodeEditor.TextProperty, new Binding(nameof(model.Code)) { Mode = BindingMode.TwoWay });
        var window = new Window { Width = 560, Height = 320, Content = control };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            var editor = control.GetVisualDescendants().OfType<TextEditor>().Single();
            Assert.Equal(model.Code, editor.Text);
            editor.TextArea.Focus();
            editor.CaretOffset = editor.Document.TextLength;
            window.KeyTextInput("int count = 4;");
            Assert.Contains("int count = 4;", model.Code);
            Assert.True(editor.Document.UndoStack.CanUndo);
            editor.Document.UndoStack.Undo();
            Assert.Equal("// Source\n", model.Code);
            model.Code = "// Different source";
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(model.Code, editor.Text);
            Assert.False(editor.Document.UndoStack.CanUndo);
            control.IsReadOnly = true;
            window.KeyTextInput("not inserted");
            Assert.Equal("// Different source", model.Code);
            control.WordWrap = true;
            Assert.True(editor.WordWrap);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void GrammarAndTextSynchronizeAfterRepeatedReattachmentAndThemeChanges()
    {
        var control = new StudioCodeEditor { Text = "var color = 42;", LanguageExtension = ".cs" };
        var window = new Window { Width = 560, Height = 320, Content = control, RequestedThemeVariant = ThemeVariant.Light };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            var editor = control.GetVisualDescendants().OfType<TextEditor>().Single();
            for (int i = 0; i < 3; i++)
            {
                Assert.Single(editor.TextArea.TextView.LineTransformers.OfType<TextMateColoringTransformer>());
                window.Content = null;
                Assert.Empty(editor.TextArea.TextView.LineTransformers.OfType<TextMateColoringTransformer>());
                window.Content = control;
                Dispatcher.UIThread.RunJobs();
                window.RequestedThemeVariant = i % 2 == 0 ? ThemeVariant.Dark : ThemeVariant.Light;
                control.Text = $"// Iteration {i}";
                Dispatcher.UIThread.RunJobs();
                Assert.Equal(control.Text, editor.Text);
                editor.Document.Insert(editor.Document.TextLength, "\n// Active");
                Assert.EndsWith("// Active", control.Text);
                Assert.Single(editor.TextArea.TextView.LineTransformers.OfType<TextMateColoringTransformer>());
            }
            control.LanguageExtension = ".txt";
            Dispatcher.UIThread.RunJobs();
            Assert.Single(editor.TextArea.TextView.LineTransformers.OfType<TextMateColoringTransformer>());
        }
        finally { window.Close(); }
    }
}
