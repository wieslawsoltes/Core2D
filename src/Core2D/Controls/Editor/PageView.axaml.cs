using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Controls.Editor
{
    public class PageView : UserControl
    {
        public PageView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
