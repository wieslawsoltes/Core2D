// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using Core2D.Controls.Studio;

namespace Core2D.Behaviors;

/// <summary>Routes native text editing to the draft/commit contract and reconnects retained templates.</summary>
public sealed class StudioTextFieldBehavior : Behavior<StudioTextField>
{
    private TextBox? _input;
    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is not { } field) return;
        field.TemplateApplied += OnTemplateApplied;
        field.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
        AttachInput(field.GetVisualDescendants().OfType<TextBox>().FirstOrDefault(x => x.Name == "PART_Input" && ReferenceEquals(x.TemplatedParent, field)));
    }
    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (AssociatedObject is { } field)
        {
            field.TemplateApplied -= OnTemplateApplied;
            field.RemoveHandler(InputElement.KeyDownEvent, OnKeyDown);
            field.CancelEdit();
        }
        AttachInput(null);
        base.OnDetaching();
    }
    private void OnTemplateApplied(object? sender, TemplateAppliedEventArgs e) => AttachInput(e.NameScope.Find<TextBox>("PART_Input"));
    private void AttachInput(TextBox? input)
    {
        if (_input is not null) _input.LostFocus -= OnLostFocus;
        _input = input;
        if (_input is not null) _input.LostFocus += OnLostFocus;
    }
    private bool IsComposing() => _input?.GetVisualDescendants().OfType<TextPresenter>().Any(x => !string.IsNullOrEmpty(x.PreeditText)) == true;
    private void OnLostFocus(object? sender, RoutedEventArgs e)
    {
        if (!IsComposing() && AssociatedObject?.GetVisualRoot() is not null) AssociatedObject.TryCommit();
    }
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (AssociatedObject is not { } field || IsComposing()) return;
        if (e.Key == Key.Escape)
        {
            field.CancelEdit(); _input?.SelectAll(); e.Handled = true;
        }
        else if (e.Key == Key.Enter)
        {
            if (field.TryCommit()) _input?.SelectAll();
            e.Handled = true;
        }
        else if (e.Key == Key.F2)
        {
            _input?.Focus(); _input?.SelectAll(); e.Handled = true;
        }
    }
}
