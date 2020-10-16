using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Data
{
    public class RecordControl : UserControl
    {
        public RecordControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
