using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Shapes
{
    public class RectangleShapeView : UserControl
    {
        public RectangleShapeView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
