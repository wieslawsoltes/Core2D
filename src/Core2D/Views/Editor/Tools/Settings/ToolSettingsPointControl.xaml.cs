using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Editor.Tools.Settings
{
    /// <summary>
    /// Interaction logic for <see cref="ToolSettingsPointControl"/> xaml.
    /// </summary>
    public class ToolSettingsPointControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolSettingsPointControl"/> class.
        /// </summary>
        public ToolSettingsPointControl()
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
