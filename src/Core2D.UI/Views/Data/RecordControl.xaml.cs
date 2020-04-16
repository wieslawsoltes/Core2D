using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Data
{
    /// <summary>
    /// Interaction logic for <see cref="RecordControl"/> xaml.
    /// </summary>
    public class RecordControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecordControl"/> class.
        /// </summary>
        public RecordControl()
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
