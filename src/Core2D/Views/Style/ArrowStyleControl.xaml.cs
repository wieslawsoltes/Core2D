using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Style
{
    /// <summary>
    /// Interaction logic for <see cref="ArrowStyleControl"/> xaml.
    /// </summary>
    public class ArrowStyleControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrowStyleControl"/> class.
        /// </summary>
        public ArrowStyleControl()
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
