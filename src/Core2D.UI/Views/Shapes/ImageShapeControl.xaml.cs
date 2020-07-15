using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Shapes
{
    /// <summary>
    /// Interaction logic for <see cref="ImageShapeControl"/> xaml.
    /// </summary>
    public class ImageShapeControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageShapeControl"/> class.
        /// </summary>
        public ImageShapeControl()
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
