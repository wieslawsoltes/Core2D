using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Settings.FileWriters
{
    /// <summary>
    /// Interaction logic for <see cref="EmfSettings"/> xaml.
    /// </summary>
    public class EmfSettings : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmfSettings"/> class.
        /// </summary>
        public EmfSettings()
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
