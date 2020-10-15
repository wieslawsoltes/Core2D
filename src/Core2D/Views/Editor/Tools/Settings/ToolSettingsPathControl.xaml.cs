using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Editor.Tools.Settings
{
    /// <summary>
    /// Interaction logic for <see cref="ToolSettingsPathControl"/> xaml.
    /// </summary>
    public class ToolSettingsPathControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolSettingsPathControl"/> class.
        /// </summary>
        public ToolSettingsPathControl()
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
