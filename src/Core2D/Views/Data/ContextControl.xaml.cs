using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Data
{
    /// <summary>
    /// Interaction logic for <see cref="ContextControl"/> xaml.
    /// </summary>
    public class ContextControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContextControl"/> class.
        /// </summary>
        public ContextControl()
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
