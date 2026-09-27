// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.DataGridSearching;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;

namespace Core2D.Behaviors;

/// <summary>Connects layer search to the existing grid search model and keyboard navigation.</summary>
public sealed class StudioLayerSearchBehavior : Behavior<TextBox>
{
    /// <summary>Defines the view containing the project hierarchy.</summary>
    public static readonly StyledProperty<Control?> TargetProperty =
        AvaloniaProperty.Register<StudioLayerSearchBehavior, Control?>(nameof(Target));

    private Control? _observedTarget;

    /// <summary>Gets or sets the project hierarchy view.</summary>
    public Control? Target
    {
        get => GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is { } field)
        {
            field.TextChanged += OnTextChanged;
            field.KeyDown += OnKeyDown;
            field.AttachedToVisualTree += OnVisualAttached;
        }
        ObserveTarget(Target);
        Dispatcher.UIThread.Post(ApplySearch, DispatcherPriority.Loaded);
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (AssociatedObject is { } field)
        {
            field.TextChanged -= OnTextChanged;
            field.KeyDown -= OnKeyDown;
            field.AttachedToVisualTree -= OnVisualAttached;
        }
        ObserveTarget(null);
        base.OnDetaching();
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TargetProperty && AssociatedObject is not null)
        {
            ObserveTarget(Target);
            Dispatcher.UIThread.Post(ApplySearch, DispatcherPriority.Loaded);
        }
    }

    private void ObserveTarget(Control? target)
    {
        if (_observedTarget is not null)
        {
            _observedTarget.DataContextChanged -= OnTargetContextChanged;
        }
        _observedTarget = target;
        if (target is not null)
        {
            target.DataContextChanged += OnTargetContextChanged;
        }
    }

    private ISearchModel? SearchModel => Target?.GetVisualDescendants().OfType<DataGrid>().FirstOrDefault()?.SearchModel;

    private void OnVisualAttached(object? sender, VisualTreeAttachmentEventArgs e) =>
        Dispatcher.UIThread.Post(ApplySearch, DispatcherPriority.Loaded);

    private void OnTargetContextChanged(object? sender, EventArgs e) =>
        Dispatcher.UIThread.Post(ApplySearch, DispatcherPriority.Loaded);

    private void OnTextChanged(object? sender, TextChangedEventArgs e) => ApplySearch();

    private void ApplySearch()
    {
        if (AssociatedObject is not { } field || SearchModel is not { } model)
        {
            return;
        }
        var query = field.Text?.Trim();
        if (string.IsNullOrEmpty(query))
        {
            model.Clear();
            return;
        }
        model.UpdateSelectionOnNavigate = true;
        model.WrapNavigation = true;
        model.SetOrUpdate(new SearchDescriptor(query, comparison: StringComparison.OrdinalIgnoreCase));
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape && AssociatedObject is { } field)
        {
            field.SetCurrentValue(TextBox.TextProperty, string.Empty);
            e.Handled = true;
        }
        else if (e.Key is Key.Enter or Key.F3 && SearchModel is { } model)
        {
            if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
            {
                model.MovePrevious();
            }
            else
            {
                model.MoveNext();
            }
            e.Handled = true;
        }
    }
}
