using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Containers
{
    /// <summary>
    /// Interaction logic for <see cref="ProjectControl"/> xaml.
    /// </summary>
    public class ProjectControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectControl"/> class.
        /// </summary>
        public ProjectControl()
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
