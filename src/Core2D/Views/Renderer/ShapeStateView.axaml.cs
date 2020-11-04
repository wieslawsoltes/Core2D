using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Renderer
{
    public class ShapeStateView : UserControl
    {
        public ShapeStateView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
