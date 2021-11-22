using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Demo.Views
{
    public class ToolsView : UserControl
    {
        public ToolsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
