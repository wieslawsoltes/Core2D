using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views
{
    /// <summary>
    /// Interaction logic for <see cref="BrowserControl"/> xaml.
    /// </summary>
    public class BrowserControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserControl"/> class.
        /// </summary>
        public BrowserControl()
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
