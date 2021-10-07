using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Docking.Tools.Properties;

public class StylePropertiesView : UserControl
{
    public StylePropertiesView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}