using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Controls.Dialog;

public class DialogView : UserControl
{
    public DialogView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
