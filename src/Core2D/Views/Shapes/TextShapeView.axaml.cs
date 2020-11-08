using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Core2D.Editor;
using Core2D.Editors;
using Core2D.Shapes;
using Core2D.Views.Editors;

namespace Core2D.Views.Shapes
{
    public class TextShapeView : UserControl
    {
        public TextShapeView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void OnEditTextBinding(object shape)
        {
            if (this.VisualRoot is TopLevel topLevel 
                && topLevel.DataContext is ProjectEditor editor
                && shape is TextShape text)
            {
                var textBindingEditor = new TextBindingEditor()
                {
                    Editor = editor,
                    Text = text
                };
                editor.Dialog = textBindingEditor;
            }
        }
    }
}
