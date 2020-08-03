using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Editors
{
    /// <summary>
    /// Interaction logic for <see cref="TextBindingEditorWindow"/> xaml.
    /// </summary>
    public class TextBindingEditorWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextBindingEditorWindow"/> class.
        /// </summary>
        public TextBindingEditorWindow()
        {
            InitializeComponent();
            this.AttachDevTools();
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
