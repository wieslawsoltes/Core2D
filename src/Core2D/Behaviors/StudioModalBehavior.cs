// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using Core2D.ViewModels.Editor;

namespace Core2D.Behaviors;

/// <summary>Constrains modal chrome to the viewport and restores focus without bypassing native input.</summary>
public sealed class StudioModalBehavior : Behavior<Control>
{
    private IInputElement? _previousFocus;
    private TopLevel? _topLevel;
    private int _generation;

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is not { } root) return;
        root.Loaded += OnLoaded;
        root.PropertyChanged += OnChanged;
        root.AddHandler(InputElement.KeyDownEvent, OnKey, RoutingStrategies.Bubble);
    }
    /// <inheritdoc />
    protected override void OnDetaching()
    {
        _generation++;
        if (AssociatedObject is { } root)
        {
            root.Loaded -= OnLoaded;
            root.PropertyChanged -= OnChanged;
            root.RemoveHandler(InputElement.KeyDownEvent, OnKey);
            IInputElement? focused = _topLevel?.FocusManager?.GetFocusedElement();
            if (focused is null || focused is Visual visual && visual.GetVisualAncestors().Contains(root))
                if (_previousFocus is InputElement { IsEffectivelyEnabled: true, IsEffectivelyVisible: true } previous) previous.Focus();
        }
        _previousFocus = null; _topLevel = null;
        base.OnDetaching();
    }
    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (AssociatedObject is not { } root) return;
        _topLevel = TopLevel.GetTopLevel(root);
        _previousFocus = _topLevel?.FocusManager?.GetFocusedElement();
        Constrain();
        int version = ++_generation;
        Dispatcher.UIThread.Post(() =>
        {
            if (version != _generation || !ReferenceEquals(root, AssociatedObject)) return;
            var content = root.FindControl<ContentControl>("DialogContent");
            Control? input = content?.GetVisualDescendants().OfType<Control>().FirstOrDefault(x => x.Focusable && x.IsEffectivelyEnabled && x.IsEffectivelyVisible);
            (input ?? root.FindControl<Button>("CloseButton"))?.Focus();
        }, DispatcherPriority.Input);
    }
    private void OnChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == Visual.BoundsProperty) Constrain();
    }
    private void Constrain()
    {
        if (AssociatedObject is not { } root || root.GetVisualRoot() is null || root.FindControl<Border>("DragBorder") is not { } surface) return;
        surface.MaxWidth = Math.Min(920, Math.Max(0, root.Bounds.Width - 32));
        surface.MaxHeight = Math.Min(760, Math.Max(0, root.Bounds.Height - 32));
    }
    private void OnKey(object? sender, KeyEventArgs e)
    {
        if (!e.Handled && e.Key == Key.Escape && AssociatedObject?.DataContext is DialogViewModel model)
        {
            if (((ICommand)model.CloseCommand).CanExecute(null)) ((ICommand)model.CloseCommand).Execute(null);
            e.Handled = true;
        }
    }
}
