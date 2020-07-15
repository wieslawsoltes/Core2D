using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Shapes
{
    /// <summary>
    /// Interaction logic for <see cref="TextShapeControl"/> xaml.
    /// </summary>
    public class TextShapeControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextShapeControl"/> class.
        /// </summary>
        public TextShapeControl()
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
