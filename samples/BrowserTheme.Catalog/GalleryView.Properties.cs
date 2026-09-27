using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.Themes.Browser;
namespace BrowserTheme.Catalog;
/// <summary>Interactive gallery properties control only the sample's presentation.</summary>
public sealed partial class GalleryView : UserControl
{
    public static readonly DirectProperty<GalleryView, int> AppearanceIndexProperty =
        AvaloniaProperty.RegisterDirect<GalleryView, int>(nameof(AppearanceIndex), x => x.AppearanceIndex, (x, v) => x.AppearanceIndex = v);
    public static readonly DirectProperty<GalleryView, int> DensityIndexProperty =
        AvaloniaProperty.RegisterDirect<GalleryView, int>(nameof(DensityIndex), x => x.DensityIndex, (x, v) => x.DensityIndex = v);
    private int _appearance, _density = 1;
    public IReadOnlyList<string> FontNames { get; } = new[] { "Inter", "Arial", "Georgia", "Monospace" };
    public int AppearanceIndex
    {
        get => _appearance;
        set
        {
            if (value < 0 || value > 2 || !SetAndRaise(AppearanceIndexProperty, ref _appearance, value)) return;
            if (TopLevel.GetTopLevel(this) is Window window)
                window.RequestedThemeVariant = value == 0 ? ThemeVariant.Default : value == 1 ? ThemeVariant.Light : ThemeVariant.Dark;
        }
    }
    public int DensityIndex
    {
        get => _density;
        set
        {
            if (value < 0 || value > 2 || !SetAndRaise(DensityIndexProperty, ref _density, value)) return;
            var theme = Application.Current?.Styles.OfType<Avalonia.Themes.Browser.BrowserTheme>().SingleOrDefault();
            if (theme is not null) theme.Density = (BrowserDensity)value;
        }
    }
}
