using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Docking.Tools.Options;

public class ImageOptionsView : UserControl
{
    public ImageOptionsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}