// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System.Collections.Specialized;
using Avalonia;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using Core2D.Controls.Studio;

namespace Core2D.Behaviors;

/// <summary>Scopes multi-selection subscriptions to the document and the realized control.</summary>
public sealed class StudioSelectionEditorBehavior : Behavior<StudioSelectionEditor>
{
    private INotifyCollectionChanged? _source;
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
        Rebind();
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
        if (_source is not null) _source.CollectionChanged -= OnCollectionChanged;
        _source = null;
        _attached = false;
        base.OnDetaching();
    }

    private void OnVisualAttached(object? sender, VisualTreeAttachmentEventArgs e) { _attached = true; Rebind(); }
    private void OnVisualDetached(object? sender, VisualTreeAttachmentEventArgs e) { _attached = false; Rebind(); }
    private void OnChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == StudioSelectionEditor.SelectionProperty || e.Property == StudioEditContext.HistoryProperty) Rebind();
    }
    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => AssociatedObject?.Rebind(_attached);
    private void Rebind()
    {
        if (_source is not null) _source.CollectionChanged -= OnCollectionChanged;
        _source = _attached ? AssociatedObject?.Selection as INotifyCollectionChanged : null;
        if (_source is not null) _source.CollectionChanged += OnCollectionChanged;
        AssociatedObject?.Rebind(_attached);
    }
}
