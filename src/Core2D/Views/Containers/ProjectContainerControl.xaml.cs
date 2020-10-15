using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Containers
{
    /// <summary>
    /// Interaction logic for <see cref="ProjectContainerControl"/> xaml.
    /// </summary>
    public class ProjectContainerControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectContainerControl"/> class.
        /// </summary>
        public ProjectContainerControl()
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
