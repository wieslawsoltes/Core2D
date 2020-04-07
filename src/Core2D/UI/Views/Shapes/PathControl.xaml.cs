using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Shapes
{
    /// <summary>
    /// Interaction logic for <see cref="PathControl"/> xaml.
    /// </summary>
    public class PathControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathControl"/> class.
        /// </summary>
        public PathControl()
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
