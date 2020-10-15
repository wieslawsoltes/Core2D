using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Editor.Tools.Settings
{
    /// <summary>
    /// Interaction logic for <see cref="ToolSettingsRectangleControl"/> xaml.
    /// </summary>
    public class ToolSettingsRectangleControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolSettingsRectangleControl"/> class.
        /// </summary>
        public ToolSettingsRectangleControl()
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
