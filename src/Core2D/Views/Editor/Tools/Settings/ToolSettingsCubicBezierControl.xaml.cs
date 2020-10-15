using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Editor.Tools.Settings
{
    /// <summary>
    /// Interaction logic for <see cref="ToolSettingsCubicBezierControl"/> xaml.
    /// </summary>
    public class ToolSettingsCubicBezierControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolSettingsCubicBezierControl"/> class.
        /// </summary>
        public ToolSettingsCubicBezierControl()
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
