// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System.Collections.Specialized;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using Core2D.Controls.Studio;

namespace Core2D.Behaviors;

/// <summary>Prevents stale details after collection replacement and reconnects retained template parts.</summary>
public sealed class StudioCollectionInspectorBehavior : Behavior<StudioCollectionInspector>
{
    private Button? _clear;
    private TabControl? _tabs;
    private INotifyCollectionChanged? _source;
    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is not { } control) return;
        control.PropertyChanged += OnChanged;
        control.TemplateApplied += OnTemplate;
        ConnectSource();
        ConnectParts(control.GetVisualDescendants().OfType<Button>().FirstOrDefault(x => x.Name == "PART_ClearSelection" && ReferenceEquals(x.TemplatedParent, control)),
            control.GetVisualDescendants().OfType<TabControl>().FirstOrDefault(x => x.Name == "PART_Details" && ReferenceEquals(x.TemplatedParent, control)));
    }
    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (AssociatedObject is { } control)
        { control.PropertyChanged -= OnChanged; control.TemplateApplied -= OnTemplate; }
        if (_source is not null) _source.CollectionChanged -= OnCollectionChanged;
        _source = null;
        ConnectParts(null, null);
        base.OnDetaching();
    }
    private void OnTemplate(object? sender, TemplateAppliedEventArgs e) =>
        ConnectParts(e.NameScope.Find<Button>("PART_ClearSelection"), e.NameScope.Find<TabControl>("PART_Details"));
    private void ConnectParts(Button? clear, TabControl? tabs)
    {
        if (_clear is not null) _clear.Click -= OnClear;
        _clear = clear; _tabs = tabs;
        if (_clear is not null) _clear.Click += OnClear;
    }
    private void OnClear(object? sender, RoutedEventArgs e)
    { if (AssociatedObject is { } control) control.SelectedItem = null; e.Handled = true; }
    private void OnChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == StudioCollectionInspector.ItemsProperty) ConnectSource();
        else if (e.Property == StudioCollectionInspector.SelectedItemProperty)
        { ValidateSelection(); if (_tabs is not null) _tabs.SelectedIndex = 0; }
    }
    private void ConnectSource()
    {
        if (_source is not null) _source.CollectionChanged -= OnCollectionChanged;
        _source = AssociatedObject?.Items as INotifyCollectionChanged;
        if (_source is not null) _source.CollectionChanged += OnCollectionChanged;
        ValidateSelection();
    }
    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => ValidateSelection();
    private void ValidateSelection()
    {
        if (AssociatedObject is not { SelectedItem: { } selected } control) return;
        if (control.Items is { } items)
            foreach (object? item in items) if (ReferenceEquals(item, selected)) return;
        control.SelectedItem = null;
    }
}
