using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Avalonia.Views.Settings
{
    /// <summary>
    /// Interaction logic for <see cref="ToolsControl"/> xaml.
    /// </summary>
    public class ToolsControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolsControl"/> class.
        /// </summary>
        public ToolsControl()
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
