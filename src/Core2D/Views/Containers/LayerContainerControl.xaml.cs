using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Containers
{
    /// <summary>
    /// Interaction logic for <see cref="LayerContainerControl"/> xaml.
    /// </summary>
    public class LayerContainerControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayerContainerControl"/> class.
        /// </summary>
        public LayerContainerControl()
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
