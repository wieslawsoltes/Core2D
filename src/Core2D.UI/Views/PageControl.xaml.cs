using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views
{
    /// <summary>
    /// Interaction logic for <see cref="PageControl"/> xaml.
    /// </summary>
    public class PageControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageControl"/> class.
        /// </summary>
        public PageControl()
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
