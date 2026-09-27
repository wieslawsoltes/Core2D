// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using Core2D.Controls.Studio;

namespace Core2D.Behaviors;

/// <summary>Keeps bounds subscriptions scoped to the realized editor and its current document.</summary>
public sealed class StudioBoundsEditorBehavior : Behavior<StudioBoundsEditor>
{
    private bool _attached;

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is not { } editor) return;
        editor.AttachedToVisualTree += OnVisualAttached;
        editor.DetachedFromVisualTree += OnVisualDetached;
        editor.PropertyChanged += OnChanged;
        _attached = editor.GetVisualRoot() is not null;
        editor.Rebind(_attached);
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (AssociatedObject is { } editor)
        {
            editor.AttachedToVisualTree -= OnVisualAttached;
            editor.DetachedFromVisualTree -= OnVisualDetached;
            editor.PropertyChanged -= OnChanged;
            editor.Rebind(false);
        }
        _attached = false;
        base.OnDetaching();
    }

    private void OnVisualAttached(object? sender, VisualTreeAttachmentEventArgs e)
    {
        _attached = true;
        AssociatedObject?.Rebind(true);
    }

    private void OnVisualDetached(object? sender, VisualTreeAttachmentEventArgs e)
    {
        _attached = false;
        AssociatedObject?.Rebind(false);
    }

    private void OnChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == StudioBoundsEditor.StartProperty || e.Property == StudioBoundsEditor.EndProperty || e.Property == StudioEditContext.HistoryProperty)
            AssociatedObject?.Rebind(_attached);
    }
}
