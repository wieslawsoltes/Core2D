using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Editor.Tools.Path.Settings
{
    /// <summary>
    /// Interaction logic for <see cref="PathToolSettingsMoveControl"/> xaml.
    /// </summary>
    public class PathToolSettingsMoveControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathToolSettingsMoveControl"/> class.
        /// </summary>
        public PathToolSettingsMoveControl()
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
