// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System.Collections;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Xaml.Interactivity;
using Core2D.Behaviors;
using Core2D.Model;
using Core2D.ViewModels;

namespace Core2D.Controls.Studio;

/// <summary>Searchable object navigation and contextual design/data details over original model instances.</summary>
public sealed class StudioCollectionInspector : TemplatedControl
{
    /// <summary>Defines the original collection; filtering never reorders or clones it.</summary>
    public static readonly DirectProperty<StudioCollectionInspector, IEnumerable?> ItemsProperty =
        AvaloniaProperty.RegisterDirect<StudioCollectionInspector, IEnumerable?>(nameof(Items), x => x.Items, (x, value) => x.Items = value);
    /// <summary>Defines the selected original object.</summary>
    public static readonly DirectProperty<StudioCollectionInspector, ViewModelBase?> SelectedItemProperty =
        AvaloniaProperty.RegisterDirect<StudioCollectionInspector, ViewModelBase?>(nameof(SelectedItem), x => x.SelectedItem, (x, value) => x.SelectedItem = value);
    /// <summary>Defines the section title.</summary>
    public static readonly DirectProperty<StudioCollectionInspector, string> TitleProperty =
        AvaloniaProperty.RegisterDirect<StudioCollectionInspector, string>(nameof(Title), x => x.Title, (x, value) => x.Title = value);
    /// <summary>Defines whether an object is selected.</summary>
    public static readonly DirectProperty<StudioCollectionInspector, bool> HasSelectionProperty =
        AvaloniaProperty.RegisterDirect<StudioCollectionInspector, bool>(nameof(HasSelection), x => x.HasSelection);
    /// <summary>Defines whether the selected model has custom data.</summary>
    public static readonly DirectProperty<StudioCollectionInspector, bool> HasDataProperty =
        AvaloniaProperty.RegisterDirect<StudioCollectionInspector, bool>(nameof(HasData), x => x.HasData);
    /// <summary>Defines the bounded browser height independently of selected detail content.</summary>
    public static readonly StyledProperty<double> BrowserHeightProperty =
        AvaloniaProperty.Register<StudioCollectionInspector, double>(nameof(BrowserHeight), 220, validate: value => double.IsFinite(value) && value >= 120);
    private IEnumerable? _items;
    private ViewModelBase? _selectedItem;
    private string _title = "Objects";
    private bool _hasSelection, _hasData;
    /// <summary>Initializes selection membership and native clear-selection behavior.</summary>
    public StudioCollectionInspector() => Interaction.GetBehaviors(this).Add(new StudioCollectionInspectorBehavior());
    /// <summary>Gets or sets the original object collection.</summary>
    public IEnumerable? Items { get => _items; set => SetAndRaise(ItemsProperty, ref _items, value); }
    /// <summary>Gets or sets the selected original object.</summary>
    public ViewModelBase? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (!SetAndRaise(SelectedItemProperty, ref _selectedItem, value)) return;
            SetAndRaise(HasSelectionProperty, ref _hasSelection, _selectedItem is not null);
            SetAndRaise(HasDataProperty, ref _hasData, _selectedItem is IDataObject);
        }
    }
    /// <summary>Gets or sets the heading.</summary>
    public string Title { get => _title; set => SetAndRaise(TitleProperty, ref _title, value); }
    /// <summary>Gets whether details can be displayed.</summary>
    public bool HasSelection => _hasSelection;
    /// <summary>Gets whether the selected object supports data editing.</summary>
    public bool HasData => _hasData;
    /// <summary>Gets or sets the browser height in DIPs.</summary>
    public double BrowserHeight { get => GetValue(BrowserHeightProperty); set => SetValue(BrowserHeightProperty, value); }
}
