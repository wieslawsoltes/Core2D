using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Avalonia.Views.Containers
{
    /// <summary>
    /// Interaction logic for <see cref="OptionsControl"/> xaml.
    /// </summary>
    public class OptionsControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsControl"/> class.
        /// </summary>
        public OptionsControl()
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
