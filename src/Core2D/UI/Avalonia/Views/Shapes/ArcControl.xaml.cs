using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Avalonia.Views.Shapes
{
    /// <summary>
    /// Interaction logic for <see cref="ArcControl"/> xaml.
    /// </summary>
    public class ArcControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArcControl"/> class.
        /// </summary>
        public ArcControl()
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
