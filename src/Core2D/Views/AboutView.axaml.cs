using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views
{
    public class AboutView : UserControl
    {
        public AboutView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
