using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Avalonia.Views
{
    /// <summary>
    /// Interaction logic for <see cref="StatusControl"/> xaml.
    /// </summary>
    public class StatusControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusControl"/> class.
        /// </summary>
        public StatusControl()
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
