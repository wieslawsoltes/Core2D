using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Libraries
{
    /// <summary>
    /// Interaction logic for <see cref="ScriptsControl"/> xaml.
    /// </summary>
    public class ScriptsControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptsControl"/> class.
        /// </summary>
        public ScriptsControl()
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
