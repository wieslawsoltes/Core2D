using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views
{
    public class EditorMenuView : UserControl
    {
        public EditorMenuView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
