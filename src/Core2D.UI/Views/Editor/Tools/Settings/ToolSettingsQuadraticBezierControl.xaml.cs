using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Editor.Tools.Settings
{
    /// <summary>
    /// Interaction logic for <see cref="ToolSettingsQuadraticBezierControl"/> xaml.
    /// </summary>
    public class ToolSettingsQuadraticBezierControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolSettingsQuadraticBezierControl"/> class.
        /// </summary>
        public ToolSettingsQuadraticBezierControl()
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
