using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaEdit;
using AvaloniaEdit.CodeCompletion;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Indentation.CSharp;
using AvaloniaEdit.Rendering;

namespace Core2D.Views.Scripting
{
    /// <summary>
    /// Interaction logic for <see cref="ScriptControl"/> xaml.
    /// </summary>
    public class ScriptControl : UserControl
    {
        private readonly TextEditor _textEditor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptControl"/> class.
        /// </summary>
        public ScriptControl()
        {
            InitializeComponent();
            _textEditor = this.FindControl<TextEditor>("textCode");
            _textEditor.ShowLineNumbers = true;
            _textEditor.TextArea.IndentationStrategy = new CSharpIndentationStrategy();
        }

        /// <summary>
        /// Initialize the Xaml components.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
