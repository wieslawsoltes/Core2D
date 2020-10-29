using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaEdit;
using AvaloniaEdit.Indentation.CSharp;

namespace Core2D.Views.Scripting
{
    public class ScriptView : UserControl
    {
        private readonly TextEditor _textEditor;

        public ScriptView()
        {
            InitializeComponent();
            _textEditor = this.FindControl<TextEditor>("textCode");
            _textEditor.ShowLineNumbers = true;
            _textEditor.TextArea.IndentationStrategy = new CSharpIndentationStrategy();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
