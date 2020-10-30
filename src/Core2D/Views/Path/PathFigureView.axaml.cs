using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Path
{
    public class PathFigureView : UserControl
    {
        public PathFigureView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
