using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Containers
{
    /// <summary>
    /// Interaction logic for <see cref="PageContainerControl"/> xaml.
    /// </summary>
    public class PageContainerControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageContainerControl"/> class.
        /// </summary>
        public PageContainerControl()
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
