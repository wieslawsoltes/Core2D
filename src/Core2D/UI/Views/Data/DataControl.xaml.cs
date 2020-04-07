using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Data
{
    /// <summary>
    /// Interaction logic for <see cref="DataControl"/> xaml.
    /// </summary>
    public class DataControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataControl"/> class.
        /// </summary>
        public DataControl()
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
