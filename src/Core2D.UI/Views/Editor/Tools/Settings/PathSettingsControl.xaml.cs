using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Editor.Tools.Settings
{
    /// <summary>
    /// Interaction logic for <see cref="PathSettingsControl"/> xaml.
    /// </summary>
    public class PathSettingsControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathSettingsControl"/> class.
        /// </summary>
        public PathSettingsControl()
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
