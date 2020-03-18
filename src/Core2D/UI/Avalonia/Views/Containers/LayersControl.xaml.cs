using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Avalonia.Views.Containers
{
    /// <summary>
    /// Interaction logic for <see cref="LayersControl"/> xaml.
    /// </summary>
    public class LayersControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayersControl"/> class.
        /// </summary>
        public LayersControl()
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
