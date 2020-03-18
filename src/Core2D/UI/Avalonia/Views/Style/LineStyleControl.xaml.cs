using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Avalonia.Views.Style
{
    /// <summary>
    /// Interaction logic for <see cref="LineStyleControl"/> xaml.
    /// </summary>
    public class LineStyleControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineStyleControl"/> class.
        /// </summary>
        public LineStyleControl()
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
