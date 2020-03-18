using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Avalonia.Views.Shapes
{
    /// <summary>
    /// Interaction logic for <see cref="LineControl"/> xaml.
    /// </summary>
    public class LineControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineControl"/> class.
        /// </summary>
        public LineControl()
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
