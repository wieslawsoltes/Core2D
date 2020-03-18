using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Avalonia.Views
{
    /// <summary>
    /// Interaction logic for <see cref="ScriptControl"/> xaml.
    /// </summary>
    public class ScriptControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptControl"/> class.
        /// </summary>
        public ScriptControl()
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
