// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System.ComponentModel;
using Avalonia;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using Core2D.Controls.Studio;

namespace Core2D.Behaviors;

/// <summary>Observes item display changes only while its native label is attached.</summary>
public sealed class StudioItemLabelBehavior : Behavior<StudioItemLabel>
{
    private INotifyPropertyChanged? _source;
    private int _generation;

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is not { } label) return;
        label.PropertyChanged += OnLabelChanged;
        Reconnect();
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (AssociatedObject is { } label) label.PropertyChanged -= OnLabelChanged;
        Disconnect();
        base.OnDetaching();
    }

    private void OnLabelChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == StudioItemLabel.ItemProperty) Reconnect();
    }

    private void Disconnect()
    {
        _generation++;
        if (_source is not null) _source.PropertyChanged -= OnItemChanged;
        _source = null;
    }

    private void Reconnect()
    {
        Disconnect();
        _source = AssociatedObject?.Item as INotifyPropertyChanged;
        if (_source is not null) _source.PropertyChanged += OnItemChanged;
        Refresh();
    }

    private void OnItemChanged(object? sender, PropertyChangedEventArgs e)
    {
        int generation = _generation;
        if (Dispatcher.UIThread.CheckAccess()) Refresh();
        else Dispatcher.UIThread.Post(() => { if (generation == _generation) Refresh(); });
    }

    private void Refresh()
    {
        if (AssociatedObject is { } label) label.Text = GridTextHelper.GetDisplayText(label.Item);
    }
}
