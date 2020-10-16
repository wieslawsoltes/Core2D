using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Path
{
    public class PathGeometryControl : UserControl
    {
        public PathGeometryControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
