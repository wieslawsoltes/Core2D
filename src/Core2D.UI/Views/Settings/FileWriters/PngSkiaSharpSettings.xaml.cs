using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Settings.FileWriters
{
    /// <summary>
    /// Interaction logic for <see cref="PngSkiaSharpSettings"/> xaml.
    /// </summary>
    public class PngSkiaSharpSettings : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PngSkiaSharpSettings"/> class.
        /// </summary>
        public PngSkiaSharpSettings()
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
