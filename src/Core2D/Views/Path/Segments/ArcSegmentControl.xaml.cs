using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Path.Segments
{
    /// <summary>
    /// Interaction logic for <see cref="ArcSegmentControl"/> xaml.
    /// </summary>
    public class ArcSegmentControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArcSegmentControl"/> class.
        /// </summary>
        public ArcSegmentControl()
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
