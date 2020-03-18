using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Avalonia.Views.Path
{
    /// <summary>
    /// Interaction logic for <see cref="PolyLineSegmentControl"/> xaml.
    /// </summary>
    public class PolyLineSegmentControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PolyLineSegmentControl"/> class.
        /// </summary>
        public PolyLineSegmentControl()
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
