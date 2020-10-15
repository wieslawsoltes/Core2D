using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views
{
    /// <summary>
    /// Interaction logic for <see cref="EditorMenuControl"/> xaml.
    /// </summary>
    public class EditorMenuControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditorMenuControl"/> class.
        /// </summary>
        public EditorMenuControl()
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
