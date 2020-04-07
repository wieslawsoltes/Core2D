using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Shapes
{
    /// <summary>
    /// Interaction logic for <see cref="PointControl"/> xaml.
    /// </summary>
    public class PointControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PointControl"/> class.
        /// </summary>
        public PointControl()
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
