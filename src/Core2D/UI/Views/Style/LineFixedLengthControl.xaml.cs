using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Style
{
    /// <summary>
    /// Interaction logic for <see cref="LineFixedLengthControl"/> xaml.
    /// </summary>
    public class LineFixedLengthControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineFixedLengthControl"/> class.
        /// </summary>
        public LineFixedLengthControl()
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
