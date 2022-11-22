using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaEdit;
using AvaloniaEdit.Indentation.CSharp;

namespace Core2D.Views.Scripting;

public class ScriptView : UserControl
{
    public ScriptView()
    {
        InitializeComponent();

        var scriptTextEditor = this.FindControl<TextEditor>("ScriptTextEditor");
        if (scriptTextEditor is { })
        {
            scriptTextEditor.ShowLineNumbers = true;
            scriptTextEditor.TextArea.IndentationStrategy = new CSharpIndentationStrategy();
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
