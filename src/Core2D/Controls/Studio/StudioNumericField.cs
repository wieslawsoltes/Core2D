// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Xaml.Interactivity;
using Core2D.Behaviors;
using Core2D.Model.Expressions;

namespace Core2D.Controls.Studio;

/// <summary>A decimal property editor with a private text draft and atomic model commits.</summary>
public class StudioNumericField : TemplatedControl
{
    /// <summary>Defines the committed value; null represents a mixed selection.</summary>
    public static readonly StyledProperty<decimal?> ValueProperty =
        AvaloniaProperty.Register<StudioNumericField, decimal?>(nameof(Value), 0m, defaultBindingMode: BindingMode.TwoWay);
    /// <summary>Defines the smallest allowed committed value.</summary>
    public static readonly StyledProperty<decimal> MinimumProperty =
        AvaloniaProperty.Register<StudioNumericField, decimal>(nameof(Minimum), decimal.MinValue);
    /// <summary>Defines the largest allowed committed value.</summary>
    public static readonly StyledProperty<decimal> MaximumProperty =
        AvaloniaProperty.Register<StudioNumericField, decimal>(nameof(Maximum), decimal.MaxValue);
    /// <summary>Defines one keyboard or scrub step.</summary>
    public static readonly StyledProperty<decimal> IncrementProperty =
        AvaloniaProperty.Register<StudioNumericField, decimal>(nameof(Increment), 1m, validate: value => value > 0);
    /// <summary>Defines the prefix used as a scrub handle.</summary>
    public static readonly StyledProperty<string?> PrefixProperty =
        AvaloniaProperty.Register<StudioNumericField, string?>(nameof(Prefix));
    /// <summary>Defines an optional unit suffix.</summary>
    public static readonly StyledProperty<string?> SuffixProperty =
        AvaloniaProperty.Register<StudioNumericField, string?>(nameof(Suffix));
    /// <summary>Defines whether the user may edit the value.</summary>
    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        AvaloniaProperty.Register<StudioNumericField, bool>(nameof(IsReadOnly));
    /// <summary>Defines the editable, uncommitted text.</summary>
    public static readonly DirectProperty<StudioNumericField, string> DraftTextProperty =
        AvaloniaProperty.RegisterDirect<StudioNumericField, string>(nameof(DraftText), x => x.DraftText, (x, value) => x.DraftText = value);
    /// <summary>Defines the validation message for the draft.</summary>
    public static readonly DirectProperty<StudioNumericField, string?> ErrorProperty =
        AvaloniaProperty.RegisterDirect<StudioNumericField, string?>(nameof(Error), x => x.Error);

    private string _draftText = "0";
    private string? _error;
    private decimal _basis;

    /// <summary>Initializes native text-input and scrub interaction.</summary>
    public StudioNumericField() => Interaction.GetBehaviors(this).Add(new StudioNumericFieldBehavior());

    /// <summary>Gets or sets the committed value without exposing incomplete input to its binding.</summary>
    public decimal? Value { get => GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
    /// <summary>Gets or sets the inclusive lower bound.</summary>
    public decimal Minimum { get => GetValue(MinimumProperty); set => SetValue(MinimumProperty, value); }
    /// <summary>Gets or sets the inclusive upper bound.</summary>
    public decimal Maximum { get => GetValue(MaximumProperty); set => SetValue(MaximumProperty, value); }
    /// <summary>Gets or sets the keyboard and scrub increment.</summary>
    public decimal Increment { get => GetValue(IncrementProperty); set => SetValue(IncrementProperty, value); }
    /// <summary>Gets or sets the prefix.</summary>
    public string? Prefix { get => GetValue(PrefixProperty); set => SetValue(PrefixProperty, value); }
    /// <summary>Gets or sets the suffix.</summary>
    public string? Suffix { get => GetValue(SuffixProperty); set => SetValue(SuffixProperty, value); }
    /// <summary>Gets or sets the read-only state.</summary>
    public bool IsReadOnly { get => GetValue(IsReadOnlyProperty); set => SetValue(IsReadOnlyProperty, value); }
    /// <summary>Gets or sets the draft. Changing it never changes <see cref="Value"/>.</summary>
    public string DraftText
    {
        get => _draftText;
        set
        {
            if (SetAndRaise(DraftTextProperty, ref _draftText, value ?? string.Empty)) SetError(null);
        }
    }
    /// <summary>Gets the draft validation error, or null.</summary>
    public string? Error => _error;

    /// <summary>Commits a valid draft, preserving the existing value binding.</summary>
    public bool TryCommit()
    {
        if (IsReadOnly || !IsEffectivelyEnabled) return false;
        if (!TryGetCandidate(out decimal result)) return false;
        CommitValue(result);
        return true;
    }

    /// <summary>Discards the draft and displays the most recent model value.</summary>
    public void CancelEdit()
    {
        _basis = Value ?? 0m;
        DraftText = Value?.ToString("G", CultureInfo.CurrentCulture) ?? string.Empty;
        SetError(null);
    }

    /// <summary>Validates a draft without mutating the model.</summary>
    public bool TryGetCandidate(out decimal result)
    {
        if (Minimum > Maximum)
        {
            result = _basis;
            SetError("The minimum must not exceed the maximum.");
            return false;
        }
        if (Value is null && string.IsNullOrWhiteSpace(DraftText))
        {
            result = 0;
            return false;
        }
        if (!NumericExpression.TryEvaluate(DraftText, _basis, CultureInfo.CurrentCulture, out result))
        {
            SetError("Enter a number or arithmetic expression. Use +=, -=, *= or /= for relative edits.");
            return false;
        }
        if (result < Minimum || result > Maximum)
        {
            SetError($"Enter a value between {Minimum} and {Maximum}.");
            return false;
        }
        SetError(null);
        return true;
    }

    /// <summary>Commits one bounded value after keyboard stepping or scrubbing.</summary>
    public void CommitValue(decimal value)
    {
        if (IsReadOnly || !IsEffectivelyEnabled || Minimum > Maximum) return;
        SetCurrentValue(ValueProperty, Math.Clamp(value, Minimum, Maximum));
        CancelEdit();
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ValueProperty || change.Property == DataContextProperty) CancelEdit();
        // An external model update wins over a pending draft, including a selection change.
        if (change.Property == IsReadOnlyProperty && IsReadOnly) CancelEdit();
    }

    private void SetError(string? message)
    {
        SetAndRaise(ErrorProperty, ref _error, message);
        PseudoClasses.Set(":error", message is not null);
    }
}
