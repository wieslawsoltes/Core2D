using Avalonia.Controls;
using Avalonia.Markup.Xaml;
namespace BrowserTheme.Catalog;
public sealed partial class CatalogWindow : Window
{
    public CatalogWindow() => AvaloniaXamlLoader.Load(this);
}
