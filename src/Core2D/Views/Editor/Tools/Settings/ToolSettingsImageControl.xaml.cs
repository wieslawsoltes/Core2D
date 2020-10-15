using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Editor.Tools.Settings
{
    /// <summary>
    /// Interaction logic for <see cref="ToolSettingsImageControl"/> xaml.
    /// </summary>
    public class ToolSettingsImageControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolSettingsImageControl"/> class.
        /// </summary>
        public ToolSettingsImageControl()
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
