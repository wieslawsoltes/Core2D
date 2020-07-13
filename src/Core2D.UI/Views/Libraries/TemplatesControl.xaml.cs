using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Libraries
{
    /// <summary>
    /// Interaction logic for <see cref="TemplatesControl"/> xaml.
    /// </summary>
    public class TemplatesControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplatesControl"/> class.
        /// </summary>
        public TemplatesControl()
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
