using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Core2D.Editor;
using Core2D.Editors;
using Core2D.Shapes;

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
                var dialog = new Dialog(editor)
                {
                    Title = "Text Binding",
                    IsOverlayVisible = false,
                    IsTitleBarVisible = true,
                    IsCloseButtonVisible = true,
                    ViewModel = textBindingEditor
                };
                editor.ShowDialog(dialog);
            }
        }
    }
}
