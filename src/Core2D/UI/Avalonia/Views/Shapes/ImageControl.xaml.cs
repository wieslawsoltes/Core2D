using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Avalonia.Views.Shapes
{
    /// <summary>
    /// Interaction logic for <see cref="ImageControl"/> xaml.
    /// </summary>
    public class ImageControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageControl"/> class.
        /// </summary>
        public ImageControl()
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
