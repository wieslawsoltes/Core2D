using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Path
{
    public class PathGeometryView : UserControl
    {
        public PathGeometryView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
