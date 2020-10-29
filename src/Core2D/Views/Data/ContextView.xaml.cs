using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Data
{
    public class ContextView : UserControl
    {
        public ContextView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
