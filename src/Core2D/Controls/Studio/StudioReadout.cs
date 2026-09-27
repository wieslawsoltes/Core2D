// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls.Primitives;

namespace Core2D.Controls.Studio;

/// <summary>A compact, selectable diagnostic value that is visually distinct from editable property fields.</summary>
public sealed class StudioReadout : TemplatedControl
{
    /// <summary>Defines the diagnostic label.</summary>
    public static readonly DirectProperty<StudioReadout, string?> LabelProperty =
        AvaloniaProperty.RegisterDirect<StudioReadout, string?>(nameof(Label), x => x.Label, (x, value) => x.Label = value);
    /// <summary>Defines the already-formatted display value.</summary>
    public static readonly DirectProperty<StudioReadout, string?> ValueProperty =
        AvaloniaProperty.RegisterDirect<StudioReadout, string?>(nameof(Value), x => x.Value, (x, value) => x.Value = value);
    private string? _label, _value;
    /// <summary>Gets or sets the label.</summary>
    public string? Label { get => _label; set => SetAndRaise(LabelProperty, ref _label, value); }
    /// <summary>Gets or sets the formatted value without implying that it is editable.</summary>
    public string? Value { get => _value; set => SetAndRaise(ValueProperty, ref _value, value); }
}
