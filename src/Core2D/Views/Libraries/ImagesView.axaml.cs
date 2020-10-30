using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Libraries
{
    public class ImagesView : UserControl
    {
        public ImagesView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
