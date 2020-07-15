using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Editor.Tools.Path.Settings
{
    /// <summary>
    /// Interaction logic for <see cref="PathToolSettingsCubicBezierControl"/> xaml.
    /// </summary>
    public class PathToolSettingsCubicBezierControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathToolSettingsCubicBezierControl"/> class.
        /// </summary>
        public PathToolSettingsCubicBezierControl()
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
