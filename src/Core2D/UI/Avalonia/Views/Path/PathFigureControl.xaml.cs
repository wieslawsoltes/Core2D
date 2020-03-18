using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Avalonia.Views.Path
{
    /// <summary>
    /// Interaction logic for <see cref="PathFigureControl"/> xaml.
    /// </summary>
    public class PathFigureControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathFigureControl"/> class.
        /// </summary>
        public PathFigureControl()
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
