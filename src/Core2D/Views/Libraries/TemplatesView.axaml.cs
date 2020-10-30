using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Libraries
{
    public class TemplatesView : UserControl
    {
        public TemplatesView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
