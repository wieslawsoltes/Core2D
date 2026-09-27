// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System.ComponentModel;
using Avalonia;
using Avalonia.Xaml.Interactivity;
using Core2D.Controls.Studio;
using Core2D.ViewModels;

namespace Core2D.Behaviors;

/// <summary>Observes only the currently realized object identity row.</summary>
public sealed class StudioObjectSummaryBehavior : Behavior<StudioObjectSummary>
{
    private ViewModelBase? _source;
    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is { } control) { control.PropertyChanged += OnChanged; Connect(); }
    }
    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (AssociatedObject is { } control) control.PropertyChanged -= OnChanged;
        if (_source is not null) _source.PropertyChanged -= OnModelChanged;
        _source = null;
        base.OnDetaching();
    }
    private void OnChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    { if (e.Property == StudioObjectSummary.SourceProperty) Connect(); }
    private void Connect()
    {
        if (_source is not null) _source.PropertyChanged -= OnModelChanged;
        _source = AssociatedObject?.Source;
        if (_source is not null) _source.PropertyChanged += OnModelChanged;
        AssociatedObject?.Refresh();
    }
    private void OnModelChanged(object? sender, PropertyChangedEventArgs e)
    { if (e.PropertyName is nameof(ViewModelBase.Name) or null or "") AssociatedObject?.Refresh(); }
}
