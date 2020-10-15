using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Editor.Tools.Settings
{
    /// <summary>
    /// Interaction logic for <see cref="ToolSettingsLineControl"/> xaml.
    /// </summary>
    public class ToolSettingsLineControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolSettingsLineControl"/> class.
        /// </summary>
        public ToolSettingsLineControl()
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
