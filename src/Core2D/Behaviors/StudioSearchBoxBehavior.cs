// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using Core2D.Controls.Studio;

namespace Core2D.Behaviors;

/// <summary>Clears the search through its existing binding and restores text focus.</summary>
public sealed class StudioSearchBoxBehavior : Behavior<StudioSearchBox>
{
    private Button? _clear;

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is { } box) box.TemplateApplied += OnTemplateApplied;
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (AssociatedObject is { } box) box.TemplateApplied -= OnTemplateApplied;
        DetachButton();
        base.OnDetaching();
    }

    private void OnTemplateApplied(object? sender, TemplateAppliedEventArgs e)
    {
        DetachButton();
        _clear = e.NameScope.Find<Button>("PART_Clear");
        if (_clear is not null) _clear.Click += OnClear;
    }

    private void DetachButton()
    {
        if (_clear is not null) _clear.Click -= OnClear;
        _clear = null;
    }

    private void OnClear(object? sender, RoutedEventArgs e)
    {
        if (AssociatedObject is not { IsReadOnly: false, IsEffectivelyEnabled: true } box) return;
        box.SetCurrentValue(TextBox.TextProperty, string.Empty);
        box.Focus();
        e.Handled = true;
    }
}
