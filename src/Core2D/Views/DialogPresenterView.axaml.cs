using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views
{
    public class DialogPresenterView : UserControl
    {
        public DialogPresenterView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
