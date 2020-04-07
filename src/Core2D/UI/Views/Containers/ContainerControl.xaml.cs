using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Containers
{
    /// <summary>
    /// Interaction logic for <see cref="ContainerControl"/> xaml.
    /// </summary>
    public class ContainerControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerControl"/> class.
        /// </summary>
        public ContainerControl()
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
