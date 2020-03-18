using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Avalonia.Views.Settings.Tools
{
    /// <summary>
    /// Interaction logic for <see cref="TextSettingsControl"/> xaml.
    /// </summary>
    public class TextSettingsControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextSettingsControl"/> class.
        /// </summary>
        public TextSettingsControl()
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
