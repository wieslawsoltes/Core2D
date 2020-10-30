using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Containers
{
    public class DocumentContainerView : UserControl
    {
        public DocumentContainerView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
