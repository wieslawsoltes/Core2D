using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Libraries
{
    /// <summary>
    /// Interaction logic for <see cref="GroupsControl"/> xaml.
    /// </summary>
    public class GroupsControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupsControl"/> class.
        /// </summary>
        public GroupsControl()
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
