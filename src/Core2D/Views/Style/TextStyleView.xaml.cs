using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Style
{
    public class TextStyleView : UserControl
    {
        public TextStyleView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
