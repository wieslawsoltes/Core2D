using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Avalonia.Views.Settings.Tools
{
    /// <summary>
    /// Interaction logic for <see cref="LineSettingsControl"/> xaml.
    /// </summary>
    public class LineSettingsControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineSettingsControl"/> class.
        /// </summary>
        public LineSettingsControl()
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
