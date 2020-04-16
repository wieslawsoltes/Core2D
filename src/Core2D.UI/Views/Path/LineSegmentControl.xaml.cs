using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Path
{
    /// <summary>
    /// Interaction logic for <see cref="LineSegmentControl"/> xaml.
    /// </summary>
    public class LineSegmentControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineSegmentControl"/> class.
        /// </summary>
        public LineSegmentControl()
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
