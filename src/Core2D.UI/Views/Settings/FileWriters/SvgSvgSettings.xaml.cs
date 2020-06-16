using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Settings.FileWriters
{
    /// <summary>
    /// Interaction logic for <see cref="SvgSvgSettings"/> xaml.
    /// </summary>
    public class SvgSvgSettings : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SvgSvgSettings"/> class.
        /// </summary>
        public SvgSvgSettings()
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
