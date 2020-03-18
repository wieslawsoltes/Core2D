using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Avalonia.Views
{
    /// <summary>
    /// Interaction logic for <see cref="DocumentMenuControl"/> xaml.
    /// </summary>
    public class DocumentMenuControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentMenuControl"/> class.
        /// </summary>
        public DocumentMenuControl()
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
