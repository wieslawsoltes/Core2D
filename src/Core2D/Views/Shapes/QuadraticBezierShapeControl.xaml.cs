using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Shapes
{
    /// <summary>
    /// Interaction logic for <see cref="QuadraticBezierShapeControl"/> xaml.
    /// </summary>
    public class QuadraticBezierShapeControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuadraticBezierShapeControl"/> class.
        /// </summary>
        public QuadraticBezierShapeControl()
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
