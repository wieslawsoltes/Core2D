using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Controls.Editor
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
