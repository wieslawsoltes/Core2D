using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Path.Segments
{
    /// <summary>
    /// Interaction logic for <see cref="CubicBezierSegmentControl"/> xaml.
    /// </summary>
    public class CubicBezierSegmentControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CubicBezierSegmentControl"/> class.
        /// </summary>
        public CubicBezierSegmentControl()
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
