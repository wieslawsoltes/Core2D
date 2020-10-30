using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Containers
{
    public class OptionsView : UserControl
    {
        public OptionsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
