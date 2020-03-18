using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Avalonia.Views
{
    /// <summary>
    /// Interaction logic for <see cref="AboutControl"/> xaml.
    /// </summary>
    public class AboutControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AboutControl"/> class.
        /// </summary>
        public AboutControl()
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
