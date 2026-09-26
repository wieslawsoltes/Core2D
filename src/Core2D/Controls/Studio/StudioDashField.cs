// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Xaml.Interactivity;
using Core2D.Behaviors;
using Core2D.Model.Style;

namespace Core2D.Controls.Studio;

/// <summary>A validated dash-pattern editor with named presets and a native vector preview.</summary>
public class StudioDashField : StudioTextField
{
    private static readonly ReadOnlyCollection<StrokePatternPreset> Choices = Array.AsReadOnly(new[]
    {
        new StrokePatternPreset("Solid", ""), new StrokePatternPreset("Dashed", "8 4"),
        new StrokePatternPreset("Dotted", "1 3"), new StrokePatternPreset("Dash dot", "8 3 1 3")
    });
    /// <summary>Defines the selected named pattern, or null for custom patterns.</summary>
    public static readonly DirectProperty<StudioDashField, StrokePatternPreset?> SelectedPresetProperty =
        AvaloniaProperty.RegisterDirect<StudioDashField, StrokePatternPreset?>(nameof(SelectedPreset), x => x.SelectedPreset, (x, value) => x.SelectedPreset = value);
    private StrokePatternPreset? _selectedPreset = Choices[0];
    private bool _syncing;

    /// <summary>Creates the field and its explicit document-history adapter.</summary>
    public StudioDashField()
    {
        Watermark = "Solid";
        Interaction.GetBehaviors(this).Add(new StudioTextHistoryBehavior());
    }
    /// <summary>Gets the immutable preset collection.</summary>
    public ReadOnlyCollection<StrokePatternPreset> Presets => Choices;
    /// <summary>Gets or sets the selected preset. Choosing a preset commits one complete pattern.</summary>
    public StrokePatternPreset? SelectedPreset
    {
        get => _selectedPreset;
        set
        {
            if (!_syncing && (IsReadOnly || !IsEffectivelyEnabled)) return;
            if (!SetAndRaise(SelectedPresetProperty, ref _selectedPreset, value) || _syncing || value is null) return;
            DraftText = value.Pattern;
            TryCommit();
        }
    }
    /// <inheritdoc />
    protected override bool TryNormalize(string draft, out string normalized, out string? error)
    {
        bool valid = DashPattern.TryNormalize(draft, out normalized, out _);
        error = valid ? null : "Use up to 32 non-negative lengths with a positive total (for example 8 4). Use a dot for decimals.";
        return valid;
    }
    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property != ValueProperty) return;
        StrokePatternPreset? match = null;
        if (DashPattern.TryNormalize(Value, out string normalized, out _))
            foreach (StrokePatternPreset preset in Choices) if (preset.Pattern == normalized) { match = preset; break; }
        _syncing = true;
        try { SetAndRaise(SelectedPresetProperty, ref _selectedPreset, match); }
        finally { _syncing = false; }
    }
}
