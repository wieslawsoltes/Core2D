using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Layouts
{
    /// <summary>
    /// Interaction logic for <see cref="SaveLayoutWindow"/> xaml.
    /// </summary>
    public class SaveLayoutWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SaveLayoutWindow"/> class.
        /// </summary>
        public SaveLayoutWindow()
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
