using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Avalonia.Views.Containers
{
    /// <summary>
    /// Interaction logic for <see cref="LibraryControl"/> xaml.
    /// </summary>
    public class LibraryControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LibraryControl"/> class.
        /// </summary>
        public LibraryControl()
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
