using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Shapes
{
    /// <summary>
    /// Interaction logic for <see cref="EllipseShapeControl"/> xaml.
    /// </summary>
    public class EllipseShapeControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EllipseShapeControl"/> class.
        /// </summary>
        public EllipseShapeControl()
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
