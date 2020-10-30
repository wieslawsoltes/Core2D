using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Shapes
{
    public class PointShapeView : UserControl
    {
        public PointShapeView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
