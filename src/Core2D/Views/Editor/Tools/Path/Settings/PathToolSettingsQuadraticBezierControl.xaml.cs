using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Editor.Tools.Path.Settings
{
    /// <summary>
    /// Interaction logic for <see cref="PathToolSettingsQuadraticBezierControl"/> xaml.
    /// </summary>
    public class PathToolSettingsQuadraticBezierControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathToolSettingsQuadraticBezierControl"/> class.
        /// </summary>
        public PathToolSettingsQuadraticBezierControl()
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
