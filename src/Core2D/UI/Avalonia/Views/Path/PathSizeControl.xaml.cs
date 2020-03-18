using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Avalonia.Views.Path
{
    /// <summary>
    /// Interaction logic for <see cref="PathSizeControl"/> xaml.
    /// </summary>
    public class PathSizeControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathSizeControl"/> class.
        /// </summary>
        public PathSizeControl()
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
