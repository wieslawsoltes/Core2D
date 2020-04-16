using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Data
{
    /// <summary>
    /// Interaction logic for <see cref="DatabaseControl"/> xaml.
    /// </summary>
    public class DatabaseControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseControl"/> class.
        /// </summary>
        public DatabaseControl()
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
