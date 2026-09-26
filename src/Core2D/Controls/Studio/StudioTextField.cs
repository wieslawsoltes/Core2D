// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Xaml.Interactivity;
using Core2D.Behaviors;

namespace Core2D.Controls.Studio;

/// <summary>A native single-line text editor that separates drafts from committed model values.</summary>
public class StudioTextField : TemplatedControl
{
    /// <summary>Defines the committed text.</summary>
    public static readonly DirectProperty<StudioTextField, string?> ValueProperty = AvaloniaProperty.RegisterDirect<StudioTextField, string?>(
        nameof(Value), x => x.Value, (x, value) => x.Value = value, defaultBindingMode: BindingMode.TwoWay);
    /// <summary>Defines the current uncommitted draft.</summary>
    public static readonly DirectProperty<StudioTextField, string> DraftTextProperty = AvaloniaProperty.RegisterDirect<StudioTextField, string>(
        nameof(DraftText), x => x.DraftText, (x, value) => x.DraftText = value);
    /// <summary>Defines the current validation message.</summary>
    public static readonly DirectProperty<StudioTextField, string?> ErrorProperty = AvaloniaProperty.RegisterDirect<StudioTextField, string?>(nameof(Error), x => x.Error);
    /// <summary>Defines the empty-field hint.</summary>
    public static readonly StyledProperty<string?> WatermarkProperty = AvaloniaProperty.Register<StudioTextField, string?>(nameof(Watermark));
    /// <summary>Defines whether user commits are allowed.</summary>
    public static readonly StyledProperty<bool> IsReadOnlyProperty = AvaloniaProperty.Register<StudioTextField, bool>(nameof(IsReadOnly));
    private string? _value;
    private string _draftText = string.Empty;
    private string? _error;

    /// <summary>Initializes native text input, commit, cancel and reattachment behavior.</summary>
    public StudioTextField() => Interaction.GetBehaviors(this).Add(new StudioTextFieldBehavior());
    /// <summary>Gets or sets the committed value.</summary>
    public string? Value { get => _value; set { if (SetAndRaise(ValueProperty, ref _value, value)) CancelEdit(); } }
    /// <summary>Gets or sets uncommitted input without mutating the model.</summary>
    public string DraftText { get => _draftText; set { if (SetAndRaise(DraftTextProperty, ref _draftText, value ?? string.Empty)) SetError(null); } }
    /// <summary>Gets the current validation error.</summary>
    public string? Error => _error;
    /// <summary>Gets or sets the watermark.</summary>
    public string? Watermark { get => GetValue(WatermarkProperty); set => SetValue(WatermarkProperty, value); }
    /// <summary>Gets or sets read-only mode.</summary>
    public bool IsReadOnly { get => GetValue(IsReadOnlyProperty); set => SetValue(IsReadOnlyProperty, value); }
    /// <summary>Occurs once after a user commit changes the value, not for external model updates.</summary>
    public event EventHandler<StudioTextCommittedEventArgs>? Committed;

    /// <summary>Validates and commits the draft through the existing binding.</summary>
    public bool TryCommit()
    {
        if (IsReadOnly || !IsEffectivelyEnabled) return false;
        if (DraftText == (Value ?? string.Empty)) { CancelEdit(); return true; }
        if (!TryNormalize(DraftText, out string normalized, out string? error)) { SetError(error); return false; }
        string? previous = Value;
        SetCurrentValue(ValueProperty, normalized);
        CancelEdit();
        if (previous != Value) Committed?.Invoke(this, new StudioTextCommittedEventArgs(previous, Value));
        return true;
    }

    /// <summary>Restores the most recent model value.</summary>
    public void CancelEdit() { DraftText = Value ?? string.Empty; SetError(null); }

    /// <summary>Validates a bounded single-line draft; specialized editors may normalize further.</summary>
    protected virtual bool TryNormalize(string draft, out string normalized, out string? error)
    {
        normalized = draft;
        error = draft.Length > 4096 || draft.IndexOfAny(new[] { '\r', '\n' }) >= 0 ? "Enter a single line of at most 4096 characters." : null;
        return error is null;
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == DataContextProperty || (change.Property == IsReadOnlyProperty && IsReadOnly)) CancelEdit();
    }

    private void SetError(string? error)
    {
        SetAndRaise(ErrorProperty, ref _error, error);
        PseudoClasses.Set(":error", error is not null);
    }
}
