using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Avalonia.Views.Shapes
{
    /// <summary>
    /// Interaction logic for <see cref="EllipseControl"/> xaml.
    /// </summary>
    public class EllipseControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EllipseControl"/> class.
        /// </summary>
        public EllipseControl()
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
