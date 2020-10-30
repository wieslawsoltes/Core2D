using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views
{
    public class StatusView : UserControl
    {
        public StatusView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
