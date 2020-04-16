using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Shapes
{
    /// <summary>
    /// Interaction logic for <see cref="CubicBezierControl"/> xaml.
    /// </summary>
    public class CubicBezierControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CubicBezierControl"/> class.
        /// </summary>
        public CubicBezierControl()
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
