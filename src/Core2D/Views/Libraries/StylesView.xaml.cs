using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Libraries
{
    public class StylesView : UserControl
    {
        public StylesView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
