using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Path.Segments
{
    public class QuadraticBezierSegmentControl : UserControl
    {
        public QuadraticBezierSegmentControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
