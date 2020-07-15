using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Shapes
{
    /// <summary>
    /// Interaction logic for <see cref="RectangleShapeControl"/> xaml.
    /// </summary>
    public class RectangleShapeControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleShapeControl"/> class.
        /// </summary>
        public RectangleShapeControl()
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
