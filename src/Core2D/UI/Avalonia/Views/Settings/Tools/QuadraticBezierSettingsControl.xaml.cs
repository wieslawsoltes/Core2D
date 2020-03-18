using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Avalonia.Views.Settings.Tools
{
    /// <summary>
    /// Interaction logic for <see cref="QuadraticBezierSettingsControl"/> xaml.
    /// </summary>
    public class QuadraticBezierSettingsControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuadraticBezierSettingsControl"/> class.
        /// </summary>
        public QuadraticBezierSettingsControl()
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
