using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Editor.Tools.Path.Settings
{
    /// <summary>
    /// Interaction logic for <see cref="PathToolSettingsLineControl"/> xaml.
    /// </summary>
    public class PathToolSettingsLineControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathToolSettingsLineControl"/> class.
        /// </summary>
        public PathToolSettingsLineControl()
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
