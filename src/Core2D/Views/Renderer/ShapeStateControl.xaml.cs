using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Renderer
{
    public class ShapeStateControl : UserControl
    {
        public ShapeStateControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
