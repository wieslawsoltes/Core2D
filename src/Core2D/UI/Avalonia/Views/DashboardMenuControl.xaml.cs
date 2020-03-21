using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Avalonia.Views
{
    /// <summary>
    /// Interaction logic for <see cref="DashboardMenuControl"/> xaml.
    /// </summary>
    public class DashboardMenuControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardMenuControl"/> class.
        /// </summary>
        public DashboardMenuControl()
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
