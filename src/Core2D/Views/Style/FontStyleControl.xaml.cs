using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Style
{
    /// <summary>
    /// Interaction logic for <see cref="FontStyleControl"/> xaml.
    /// </summary>
    public class FontStyleControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FontStyleControl"/> class.
        /// </summary>
        public FontStyleControl()
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
