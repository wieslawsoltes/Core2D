using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Docking.Tools.Properties;

public class PagePropertiesView : UserControl
{
    public PagePropertiesView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}