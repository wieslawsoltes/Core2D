using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Containers
{
    /// <summary>
    /// Interaction logic for <see cref="ShapesControl"/> xaml.
    /// </summary>
    public class ShapesControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShapesControl"/> class.
        /// </summary>
        public ShapesControl()
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
