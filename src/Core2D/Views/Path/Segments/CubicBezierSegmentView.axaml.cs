using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Path.Segments
{
    public class CubicBezierSegmentView : UserControl
    {
        public CubicBezierSegmentView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
