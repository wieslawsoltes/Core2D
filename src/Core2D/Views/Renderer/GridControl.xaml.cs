using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Renderer
{
    public class GridControl : UserControl
    {
        public GridControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
