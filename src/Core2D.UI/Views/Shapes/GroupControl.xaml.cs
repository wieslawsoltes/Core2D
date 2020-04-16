using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Shapes
{
    /// <summary>
    /// Interaction logic for <see cref="GroupControl"/> xaml.
    /// </summary>
    public class GroupControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupControl"/> class.
        /// </summary>
        public GroupControl()
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
