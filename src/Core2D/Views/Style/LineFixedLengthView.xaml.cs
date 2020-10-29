using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Style
{
    public class LineFixedLengthView : UserControl
    {
        public LineFixedLengthView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
