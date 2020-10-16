using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Path.Segments
{
    public class ArcSegmentControl : UserControl
    {
        public ArcSegmentControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
