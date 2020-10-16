using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Renderer
{
    public class ShapeRendererStateControl : UserControl
    {
        public ShapeRendererStateControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
