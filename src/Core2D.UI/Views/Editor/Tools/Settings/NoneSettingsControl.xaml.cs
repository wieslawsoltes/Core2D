using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Editor.Tools.Settings
{
    /// <summary>
    /// Interaction logic for <see cref="NoneSettingsControl"/> xaml.
    /// </summary>
    public class NoneSettingsControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoneSettingsControl"/> class.
        /// </summary>
        public NoneSettingsControl()
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
