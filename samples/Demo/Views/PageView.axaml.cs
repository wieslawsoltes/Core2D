using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Demo.Views
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
