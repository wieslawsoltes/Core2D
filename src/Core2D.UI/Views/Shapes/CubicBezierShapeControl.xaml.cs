using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Shapes
{
    /// <summary>
    /// Interaction logic for <see cref="CubicBezierShapeControl"/> xaml.
    /// </summary>
    public class CubicBezierShapeControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CubicBezierShapeControl"/> class.
        /// </summary>
        public CubicBezierShapeControl()
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
