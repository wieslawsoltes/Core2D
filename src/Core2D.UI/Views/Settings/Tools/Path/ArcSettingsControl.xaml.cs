using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Settings.Tools.Path
{
    /// <summary>
    /// Interaction logic for <see cref="ArcSettingsControl"/> xaml.
    /// </summary>
    public class ArcSettingsControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArcSettingsControl"/> class.
        /// </summary>
        public ArcSettingsControl()
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
