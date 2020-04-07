using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views
{
    /// <summary>
    /// Interaction logic for <see cref="MainControl"/> xaml.
    /// </summary>
    public class MainControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainControl"/> class.
        /// </summary>
        public MainControl()
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
