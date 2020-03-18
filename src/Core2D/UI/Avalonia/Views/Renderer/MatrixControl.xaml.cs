using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Avalonia.Views.Renderer
{
    /// <summary>
    /// Interaction logic for <see cref="MatrixControl"/> xaml.
    /// </summary>
    public class MatrixControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatrixControl"/> class.
        /// </summary>
        public MatrixControl()
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
