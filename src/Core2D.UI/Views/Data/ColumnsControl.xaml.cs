using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Data
{
    /// <summary>
    /// Interaction logic for <see cref="ColumnsControl"/> xaml.
    /// </summary>
    public class ColumnsControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnsControl"/> class.
        /// </summary>
        public ColumnsControl()
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
