using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Settings.FileWriters
{
    /// <summary>
    /// Interaction logic for <see cref="SvgSkiaSharpSettings"/> xaml.
    /// </summary>
    public class SvgSkiaSharpSettings : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SvgSkiaSharpSettings"/> class.
        /// </summary>
        public SvgSkiaSharpSettings()
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
