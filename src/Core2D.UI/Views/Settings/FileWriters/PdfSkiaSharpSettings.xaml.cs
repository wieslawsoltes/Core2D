using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Settings.FileWriters
{
    /// <summary>
    /// Interaction logic for <see cref="PdfSkiaSharpSettings"/> xaml.
    /// </summary>
    public class PdfSkiaSharpSettings : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSkiaSharpSettings"/> class.
        /// </summary>
        public PdfSkiaSharpSettings()
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
