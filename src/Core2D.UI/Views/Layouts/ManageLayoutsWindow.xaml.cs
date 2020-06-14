using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Layouts
{
    /// <summary>
    /// Interaction logic for <see cref="ManageLayoutsWindow"/> xaml.
    /// </summary>
    public class ManageLayoutsWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManageLayoutsWindow"/> class.
        /// </summary>
        public ManageLayoutsWindow()
        {
            this.InitializeComponent();
            this.AttachDevTools();
            // FIXME: App.Selector.EnableThemes(this);
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
