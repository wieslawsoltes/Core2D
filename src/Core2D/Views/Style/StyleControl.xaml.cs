using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Style
{
    /// <summary>
    /// Interaction logic for <see cref="StyleControl"/> xaml.
    /// </summary>
    public class StyleControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StyleControl"/> class.
        /// </summary>
        public StyleControl()
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
