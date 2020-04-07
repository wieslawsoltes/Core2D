using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Data
{
    /// <summary>
    /// Interaction logic for <see cref="RecordsControl"/> xaml.
    /// </summary>
    public class RecordsControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecordsControl"/> class.
        /// </summary>
        public RecordsControl()
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
