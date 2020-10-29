using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Style
{
    public class ArgbColorView : UserControl
    {
        public ArgbColorView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
