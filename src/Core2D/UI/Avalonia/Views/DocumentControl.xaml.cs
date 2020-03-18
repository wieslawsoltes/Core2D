using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Avalonia.Views
{
    /// <summary>
    /// Interaction logic for <see cref="DocumentControl"/> xaml.
    /// </summary>
    public class DocumentControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentControl"/> class.
        /// </summary>
        public DocumentControl()
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
