using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Libraries
{
    /// <summary>
    /// Interaction logic for <see cref="ImagesControl"/> xaml.
    /// </summary>
    public class ImagesControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImagesControl"/> class.
        /// </summary>
        public ImagesControl()
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
