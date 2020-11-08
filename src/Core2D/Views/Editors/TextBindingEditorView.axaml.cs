using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Editors
{
    public class TextBindingEditorView : UserControl
    {
        public TextBindingEditorView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
