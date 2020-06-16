using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Settings.FileWriters
{
    /// <summary>
    /// Interaction logic for <see cref="PdfSharpSettings"/> xaml.
    /// </summary>
    public class PdfSharpSettings : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSharpSettings"/> class.
        /// </summary>
        public PdfSharpSettings()
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
