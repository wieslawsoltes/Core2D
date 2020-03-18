using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Avalonia.Views
{
    /// <summary>
    /// Interaction logic for <see cref="AboutMenuControl"/> xaml.
    /// </summary>
    public class AboutMenuControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AboutMenuControl"/> class.
        /// </summary>
        public AboutMenuControl()
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
