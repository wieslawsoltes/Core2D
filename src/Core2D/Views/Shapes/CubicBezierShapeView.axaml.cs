using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Shapes
{
    public class CubicBezierShapeView : UserControl
    {
        public CubicBezierShapeView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
