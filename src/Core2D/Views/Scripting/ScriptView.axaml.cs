using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaEdit;
using AvaloniaEdit.Indentation.CSharp;

namespace Core2D.Views.Scripting
{
    public class ScriptView : UserControl
    {
        private readonly TextEditor _scriptTextEditor;

        public ScriptView()
        {
            InitializeComponent();
            _scriptTextEditor = this.FindControl<TextEditor>("ScriptTextEditor");
            _scriptTextEditor.ShowLineNumbers = true;
            _scriptTextEditor.TextArea.IndentationStrategy = new CSharpIndentationStrategy();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
