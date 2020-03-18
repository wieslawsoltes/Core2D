using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Avalonia.Views.Settings.Tools
{
    /// <summary>
    /// Interaction logic for <see cref="ImageSettingsControl"/> xaml.
    /// </summary>
    public class ImageSettingsControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageSettingsControl"/> class.
        /// </summary>
        public ImageSettingsControl()
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
