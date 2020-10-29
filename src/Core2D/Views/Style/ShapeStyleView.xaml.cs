using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Style
{
    public class ShapeStyleView : UserControl
    {
        public ShapeStyleView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
