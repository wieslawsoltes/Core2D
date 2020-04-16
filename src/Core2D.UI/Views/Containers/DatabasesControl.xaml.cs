using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Containers
{
    /// <summary>
    /// Interaction logic for <see cref="DatabasesControl"/> xaml.
    /// </summary>
    public class DatabasesControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabasesControl"/> class.
        /// </summary>
        public DatabasesControl()
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
