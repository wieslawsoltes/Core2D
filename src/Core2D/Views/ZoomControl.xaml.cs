using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views
{
    /// <summary>
    /// Interaction logic for <see cref="ZoomControl"/> xaml.
    /// </summary>
    public class ZoomControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ZoomControl"/> class.
        /// </summary>
        public ZoomControl()
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
