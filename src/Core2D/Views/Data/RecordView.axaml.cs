using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Data
{
    public class RecordView : UserControl
    {
        public RecordView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
