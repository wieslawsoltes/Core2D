using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Editor.Tools.Settings
{
    /// <summary>
    /// Interaction logic for <see cref="PointSettingsControl"/> xaml.
    /// </summary>
    public class PointSettingsControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PointSettingsControl"/> class.
        /// </summary>
        public PointSettingsControl()
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
