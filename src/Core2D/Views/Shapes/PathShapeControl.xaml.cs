using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Shapes
{
    /// <summary>
    /// Interaction logic for <see cref="PathShapeControl"/> xaml.
    /// </summary>
    public class PathShapeControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathShapeControl"/> class.
        /// </summary>
        public PathShapeControl()
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
