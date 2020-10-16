using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Path.Segments
{
    public class LineSegmentControl : UserControl
    {
        public LineSegmentControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
