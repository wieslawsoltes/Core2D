using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Libraries
{
    public class GroupsView : UserControl
    {
        public GroupsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
