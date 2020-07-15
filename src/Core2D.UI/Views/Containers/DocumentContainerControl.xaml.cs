using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Containers
{
    /// <summary>
    /// Interaction logic for <see cref="DocumentContainerControl"/> xaml.
    /// </summary>
    public class DocumentContainerControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentContainerControl"/> class.
        /// </summary>
        public DocumentContainerControl()
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
