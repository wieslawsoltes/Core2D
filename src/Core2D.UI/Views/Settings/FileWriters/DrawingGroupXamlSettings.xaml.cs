using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Settings.FileWriters
{
    /// <summary>
    /// Interaction logic for <see cref="DrawingGroupXamlSettings"/> xaml.
    /// </summary>
    public class DrawingGroupXamlSettings : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DrawingGroupXamlSettings"/> class.
        /// </summary>
        public DrawingGroupXamlSettings()
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
