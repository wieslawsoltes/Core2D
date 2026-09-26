// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System.Collections.Specialized;
using Avalonia;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using Core2D.Controls.Studio;

namespace Core2D.Behaviors;

/// <summary>Owns state-adapter subscriptions through selection changes and docking reattachment.</summary>
public sealed class StudioShapeStateEditorBehavior : Behavior<StudioShapeStateEditor>
{
    private INotifyCollectionChanged? _collection;
    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is not { } control) return;
        control.PropertyChanged += OnChanged;
        control.AttachedToVisualTree += OnVisualAttached;
        Reconnect();
    }
    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (AssociatedObject is { } control)
        {
            control.PropertyChanged -= OnChanged;
            control.AttachedToVisualTree -= OnVisualAttached;
            control.Rebind(false);
        }
        if (_collection is not null) _collection.CollectionChanged -= OnCollectionChanged;
        _collection = null;
        base.OnDetaching();
    }
    private void OnVisualAttached(object? sender, VisualTreeAttachmentEventArgs e) => Reconnect();
    private void OnChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == StudioShapeStateEditor.SourceProperty || e.Property == StudioShapeStateEditor.SelectionProperty
            || e.Property == StudioEditContext.HistoryProperty) Reconnect();
    }
    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (AssociatedObject is { } control) control.Rebind(control.GetVisualRoot() is not null);
    }
    private void Reconnect()
    {
        if (_collection is not null) _collection.CollectionChanged -= OnCollectionChanged;
        _collection = AssociatedObject?.Selection as INotifyCollectionChanged;
        if (_collection is not null) _collection.CollectionChanged += OnCollectionChanged;
        if (AssociatedObject is { } control) control.Rebind(control.GetVisualRoot() is not null);
    }
}
