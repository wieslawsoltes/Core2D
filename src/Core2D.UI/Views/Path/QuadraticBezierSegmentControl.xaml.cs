using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Path
{
    /// <summary>
    /// Interaction logic for <see cref="QuadraticBezierSegmentControl"/> xaml.
    /// </summary>
    public class QuadraticBezierSegmentControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuadraticBezierSegmentControl"/> class.
        /// </summary>
        public QuadraticBezierSegmentControl()
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
