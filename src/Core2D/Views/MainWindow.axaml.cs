using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Core2D.Screenshot;

namespace Core2D.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.AttachDevTools();
            this.AttachScreenshot();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
