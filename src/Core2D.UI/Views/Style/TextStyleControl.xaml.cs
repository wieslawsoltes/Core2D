using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Style
{
    /// <summary>
    /// Interaction logic for <see cref="TextStyleControl"/> xaml.
    /// </summary>
    public class TextStyleControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextStyleControl"/> class.
        /// </summary>
        public TextStyleControl()
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
