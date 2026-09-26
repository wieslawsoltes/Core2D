// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Core2D.Behaviors;

namespace Core2D.Controls.Studio;

/// <summary>A keyboard-first action picker whose declarative MenuItems retain compiled command bindings.</summary>
public class StudioCommandPalette : ItemsControl
{
    /// <summary>Defines whether the picker is open.</summary>
    public static readonly DirectProperty<StudioCommandPalette, bool> IsOpenProperty =
        AvaloniaProperty.RegisterDirect<StudioCommandPalette, bool>(nameof(IsOpen), x => x.IsOpen, (x, value) => x.IsOpen = value);
    /// <summary>Defines whether opening the picker is permitted, for example while another dialog is active.</summary>
    public static readonly DirectProperty<StudioCommandPalette, bool> CanOpenProperty =
        AvaloniaProperty.RegisterDirect<StudioCommandPalette, bool>(nameof(CanOpen), x => x.CanOpen, (x, value) => x.CanOpen = value);
    private bool _isOpen;
    private bool _canOpen = true;
    /// <summary>Gets or sets picker visibility without changing editor selection.</summary>
    public bool IsOpen
    {
        get => _isOpen;
        set
        {
            if (value && !CanOpen) return;
            SetCurrentValue(IsVisibleProperty, value);
            SetAndRaise(IsOpenProperty, ref _isOpen, value);
        }
    }
    /// <summary>Gets or sets whether opening is permitted; disabling closes the picker.</summary>
    public bool CanOpen
    {
        get => _canOpen;
        set { SetAndRaise(CanOpenProperty, ref _canOpen, value); if (!value) IsOpen = false; }
    }
    /// <summary>Gets or sets the action/category query.</summary>
    public static readonly DirectProperty<StudioCommandPalette, string?> QueryProperty =
        AvaloniaProperty.RegisterDirect<StudioCommandPalette, string?>(nameof(Query), x => x.Query, (x, value) => x.Query = value);
    private string? _query = null;
    /// <summary>Gets or sets the action/category query.</summary>
    public string? Query { get => _query; set => SetAndRaise(QueryProperty, ref _query, value); }

    /// <summary>Gets the number of matching actions.</summary>
    public static readonly DirectProperty<StudioCommandPalette, int> ResultCountProperty =
        AvaloniaProperty.RegisterDirect<StudioCommandPalette, int>(nameof(ResultCount), x => x.ResultCount, (x, value) => x.ResultCount = value);
    private int _resultCount = 0;
    /// <summary>Gets the number of matching actions.</summary>
    public int ResultCount { get => _resultCount; set => SetAndRaise(ResultCountProperty, ref _resultCount, value); }
    /// <inheritdoc />
    protected override Type StyleKeyOverride => typeof(StudioCommandPalette);
    /// <summary>Initializes the picker and its scoped input/lifecycle behavior.</summary>
    public StudioCommandPalette()
    {
        IsVisible = false;
        Interaction.GetBehaviors(this).Add(new StudioCommandPaletteBehavior());
    }
}
