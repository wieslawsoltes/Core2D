// Browser design system. Fluent-derived support resources: LICENSE.Avalonia.txt.
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Avalonia.Themes.Browser;

/// <summary>Control density in device-independent pixels.</summary>
public enum BrowserDensity
{
    /// <summary>28-DIP inputs for dense authoring applications.</summary>
    Compact,
    /// <summary>36-DIP inputs for ordinary desktop/web-style interfaces.</summary>
    Comfortable,
    /// <summary>44-DIP inputs for larger pointer targets.</summary>
    Touch
}

/// <summary>A standalone, complete browser-style theme for Avalonia's built-in controls.</summary>
public sealed class BrowserTheme : Styles, IResourceNode
{
    private readonly ResourceDictionary[] _densities;
    private BrowserDensity _density = BrowserDensity.Comfortable;
    /// <summary>Defines the live control-density selection.</summary>
    public static readonly DirectProperty<BrowserTheme, BrowserDensity> DensityProperty =
        AvaloniaProperty.RegisterDirect<BrowserTheme, BrowserDensity>(nameof(Density), x => x.Density, (x, value) => x.Density = value);
    /// <summary>Loads all local template and support resources; no FluentTheme instance is required.</summary>
    public BrowserTheme(IServiceProvider? serviceProvider = null)
    {
        AvaloniaXamlLoader.Load(serviceProvider, this);
        _densities = new[] { Take("CompactStyles"), Take("ComfortableStyles"), Take("TouchStyles") };
        Palettes = Resources.MergedDictionaries.OfType<ColorPaletteResourcesCollection>().Single();
    }
    /// <summary>Gets or sets live density without recreating control templates or their input state.</summary>
    public BrowserDensity Density
    {
        get => _density;
        set
        {
            if (!Enum.IsDefined(value)) throw new ArgumentOutOfRangeException(nameof(value));
            if (SetAndRaise(DensityProperty, ref _density, value)) Owner?.NotifyHostedResourcesChanged(ResourcesChangedEventArgs.Empty);
        }
    }
    /// <summary>Legacy base-palette customization for Fluent-compatible third-party resources.</summary>
    public IDictionary<ThemeVariant, ColorPaletteResources> Palettes { get; }
    private ResourceDictionary Take(string name)
    {
        ResourceDictionary dictionary = (ResourceDictionary)Resources[name]!;
        Resources.Remove(name);
        return dictionary;
    }
    bool IResourceNode.TryGetResource(object key, ThemeVariant? theme, out object? value)
    {
        if (_densities is not null && _densities[(int)_density].TryGetResource(key, theme, out value)) return true;
        return base.TryGetResource(key, theme, out value);
    }
}
