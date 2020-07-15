using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Editor.Tools.Settings
{
    /// <summary>
    /// Interaction logic for <see cref="SelectionSettingsControl"/> xaml.
    /// </summary>
    public class SelectionSettingsControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionSettingsControl"/> class.
        /// </summary>
        public SelectionSettingsControl()
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
