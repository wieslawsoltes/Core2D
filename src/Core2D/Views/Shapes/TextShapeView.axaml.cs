using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Editors;
using Core2D.ViewModels.Shapes;

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
                && topLevel.DataContext is ProjectEditorViewModel editor
                && shape is TextShapeViewModel text)
            {
                var textBindingEditor = new TextBindingEditorViewModel()
                {
                    Editor = editor,
                    Text = text
                };
                var dialog = new DialogViewModel(editor)
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
