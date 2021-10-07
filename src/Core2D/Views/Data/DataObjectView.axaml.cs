using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Data;

public class DataObjectView : UserControl
{
    public DataObjectView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}