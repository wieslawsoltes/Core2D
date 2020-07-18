using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Renderer
{
    /// <summary>
    /// Interaction logic for <see cref="GridControl"/> xaml.
    /// </summary>
    public class GridControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridControl"/> class.
        /// </summary>
        public GridControl()
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
