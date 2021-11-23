using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Controls.Editor
{
    public class MenuView : UserControl
    {
        public MenuView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
