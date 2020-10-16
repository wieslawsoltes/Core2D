using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Data
{
    public class PropertiesControl : UserControl
    {
        public PropertiesControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
