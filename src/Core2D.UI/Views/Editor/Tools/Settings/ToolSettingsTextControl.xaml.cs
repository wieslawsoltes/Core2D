using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Editor.Tools.Settings
{
    /// <summary>
    /// Interaction logic for <see cref="ToolSettingsTextControl"/> xaml.
    /// </summary>
    public class ToolSettingsTextControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolSettingsTextControl"/> class.
        /// </summary>
        public ToolSettingsTextControl()
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
