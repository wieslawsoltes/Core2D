using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Shapes
{
    /// <summary>
    /// Interaction logic for <see cref="ArcShapeControl"/> xaml.
    /// </summary>
    public class ArcShapeControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArcShapeControl"/> class.
        /// </summary>
        public ArcShapeControl()
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
