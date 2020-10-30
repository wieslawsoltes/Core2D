using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Containers
{
    public class ProjectContainerView : UserControl
    {
        public ProjectContainerView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
