using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Path
{
    /// <summary>
    /// Interaction logic for <see cref="PathGeometryControl"/> xaml.
    /// </summary>
    public class PathGeometryControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathGeometryControl"/> class.
        /// </summary>
        public PathGeometryControl()
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
