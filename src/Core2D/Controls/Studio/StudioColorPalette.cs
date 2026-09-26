// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;

namespace Core2D.Controls.Studio;

/// <summary>A keyboard-accessible preset palette that changes RGB without discarding opacity.</summary>
public class StudioColorPalette : ListBox
{
    /// <summary>Defines the selected color.</summary>
    public static readonly StyledProperty<Color> ColorProperty = AvaloniaProperty.Register<StudioColorPalette, Color>(
        nameof(Color), Colors.Black, defaultBindingMode: BindingMode.TwoWay);
    /// <summary>Gets the immutable default RGB preset collection.</summary>
    public static IReadOnlyList<Color> Presets { get; } = Array.AsReadOnly(new[]
    {
        Colors.White, Color.Parse("#D9D9D9"), Color.Parse("#808080"), Colors.Black,
        Color.Parse("#F24822"), Color.Parse("#FFCD29"), Color.Parse("#14AE5C"), Color.Parse("#0D99FF"),
        Color.Parse("#9747FF"), Color.Parse("#FF6AC1"), Color.Parse("#FF8577"), Color.Parse("#FFE8A3"),
        Color.Parse("#AFF4C6"), Color.Parse("#BDE3FF"), Color.Parse("#D9B8FF"), Color.Parse("#FDD2EB")
    });
    private bool _synchronizing;

    /// <summary>Creates the native single-selection palette.</summary>
    public StudioColorPalette()
    {
        ItemsSource = Presets;
        SelectionMode = SelectionMode.Single;
        AutoScrollToSelectedItem = false;
        Synchronize();
    }

    /// <summary>Gets or sets the full ARGB value. Selecting a preset preserves its alpha.</summary>
    public Color Color { get => GetValue(ColorProperty); set => SetValue(ColorProperty, value); }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (_synchronizing) return;
        if (change.Property == ColorProperty) Synchronize();
        else if (change.Property == SelectedItemProperty && SelectedItem is Color color)
            SetCurrentValue(ColorProperty, Color.FromArgb(Color.A, color.R, color.G, color.B));
    }

    private void Synchronize()
    {
        _synchronizing = true;
        try
        {
            int selected = -1;
            for (int i = 0; i < Items.Count; i++)
                if (Items[i] is Color c && c.R == Color.R && c.G == Color.G && c.B == Color.B) { selected = i; break; }
            SelectedIndex = selected;
        }
        finally { _synchronizing = false; }
    }
}
