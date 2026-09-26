// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Collections.Generic;
using Avalonia;
using Core2D.Controls.Studio;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;

namespace Core2D.Behaviors;

/// <summary>Routes existing shortcuts after native input and canvas gestures, without changing command bindings.</summary>
public sealed class StudioScopedKeyBindingsBehavior : Behavior<Control>
{
    private readonly List<KeyBinding> _bindings = new();

    /// <summary>Defines whether unhandled primary-modifier shortcuts remain available from native text editors.</summary>
    public static readonly DirectProperty<StudioScopedKeyBindingsBehavior, bool> AllowModifiedTextBindingsProperty =
        AvaloniaProperty.RegisterDirect<StudioScopedKeyBindingsBehavior, bool>(nameof(AllowModifiedTextBindings), x => x.AllowModifiedTextBindings, (x, value) => x.AllowModifiedTextBindings = value);
    private bool _allowModifiedTextBindings;
    /// <summary>Gets or sets support for application file shortcuts after native text input has had priority.</summary>
    public bool AllowModifiedTextBindings { get => _allowModifiedTextBindings; set => SetAndRaise(AllowModifiedTextBindingsProperty, ref _allowModifiedTextBindings, value); }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is not { } root) return;
        root.Initialized += OnInitialized;
        root.Loaded += OnLoaded;
        root.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Bubble);
        if (root.IsInitialized) CaptureBindings();
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (AssociatedObject is { } root)
        {
            root.Initialized -= OnInitialized;
            root.Loaded -= OnLoaded;
            root.RemoveHandler(InputElement.KeyDownEvent, OnKeyDown);
            foreach (KeyBinding binding in _bindings)
                if (!root.KeyBindings.Contains(binding)) root.KeyBindings.Add(binding);
        }
        _bindings.Clear();
        base.OnDetaching();
    }

    private void OnInitialized(object? sender, EventArgs e) => CaptureBindings();
    private void OnLoaded(object? sender, RoutedEventArgs e) => CaptureBindings();

    private void CaptureBindings()
    {
        if (AssociatedObject is not { } root) return;
        // KeyBinding is not a visual. Keeping the original objects preserves their compiled bindings.
        foreach (KeyBinding binding in root.KeyBindings)
            if (!_bindings.Contains(binding)) _bindings.Add(binding);
        root.KeyBindings.Clear();
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Handled || AssociatedObject is not { IsEffectivelyEnabled: true } root) return;
        for (var current = e.Source as Avalonia.Visual; current is not null; current = current.GetVisualParent())
        {
            if (current is StudioCommandPalette) return;
            if (current is TextBox or AutoCompleteBox or AvaloniaEdit.Editing.TextArea)
            {
                if (!AllowModifiedTextBindings || (e.KeyModifiers & (KeyModifiers.Control | KeyModifiers.Meta)) == 0) return;
            }
            if (ReferenceEquals(current, root)) break;
        }
        // A command can replace the current view and detach this behavior during execution.
        foreach (KeyBinding binding in _bindings.ToArray())
        {
            binding.TryHandle(e);
            if (e.Handled) break;
        }
    }
}
