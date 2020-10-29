using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Shapes
{
    public class QuadraticBezierShapeView : UserControl
    {
        public QuadraticBezierShapeView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
