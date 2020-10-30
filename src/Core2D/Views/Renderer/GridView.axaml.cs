using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Renderer
{
    public class GridView : UserControl
    {
        public GridView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
