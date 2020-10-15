using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Editor.Tools.Settings
{
    /// <summary>
    /// Interaction logic for <see cref="ToolSettingsEllipseControl"/> xaml.
    /// </summary>
    public class ToolSettingsEllipseControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolSettingsEllipseControl"/> class.
        /// </summary>
        public ToolSettingsEllipseControl()
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
