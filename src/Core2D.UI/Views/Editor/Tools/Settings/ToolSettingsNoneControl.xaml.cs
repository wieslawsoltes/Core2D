using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Editor.Tools.Settings
{
    /// <summary>
    /// Interaction logic for <see cref="ToolSettingsNoneControl"/> xaml.
    /// </summary>
    public class ToolSettingsNoneControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolSettingsNoneControl"/> class.
        /// </summary>
        public ToolSettingsNoneControl()
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
