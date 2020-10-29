using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Shapes
{
    public class GroupShapeView : UserControl
    {
        public GroupShapeView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
