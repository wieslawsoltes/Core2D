using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Editor.Tools.Settings
{
    /// <summary>
    /// Interaction logic for <see cref="ToolSettingsArcControl"/> xaml.
    /// </summary>
    public class ToolSettingsArcControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolSettingsArcControl"/> class.
        /// </summary>
        public ToolSettingsArcControl()
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
