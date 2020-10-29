using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Style
{
    public class LineStyleView : UserControl
    {
        public LineStyleView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
