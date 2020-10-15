using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Shapes
{
    /// <summary>
    /// Interaction logic for <see cref="GroupShapeControl"/> xaml.
    /// </summary>
    public class GroupShapeControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupShapeControl"/> class.
        /// </summary>
        public GroupShapeControl()
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
