using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Avalonia.Views.Menus
{
    /// <summary>
    /// Interaction logic for <see cref="FormatMenuItem"/> xaml.
    /// </summary>
    public class FormatMenuItem : MenuItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatMenuItem"/> class.
        /// </summary>
        public FormatMenuItem()
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
