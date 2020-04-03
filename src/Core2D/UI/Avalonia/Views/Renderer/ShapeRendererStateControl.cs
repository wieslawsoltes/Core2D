using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Avalonia.Views.Renderer
{
    /// <summary>
    /// Interaction logic for <see cref="ShapeRendererStateControl"/> xaml.
    /// </summary>
    public class ShapeRendererStateControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShapeRendererStateControl"/> class.
        /// </summary>
        public ShapeRendererStateControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialize the Xaml components.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
