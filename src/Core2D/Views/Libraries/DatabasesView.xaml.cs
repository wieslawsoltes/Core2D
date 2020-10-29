using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Libraries
{
    public class DatabasesView : UserControl
    {
        public DatabasesView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
