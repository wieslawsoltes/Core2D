using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Avalonia.Views.Settings.Tools.Path
{
    /// <summary>
    /// Interaction logic for <see cref="CubicBezierSettingsControl"/> xaml.
    /// </summary>
    public class CubicBezierSettingsControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CubicBezierSettingsControl"/> class.
        /// </summary>
        public CubicBezierSettingsControl()
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
