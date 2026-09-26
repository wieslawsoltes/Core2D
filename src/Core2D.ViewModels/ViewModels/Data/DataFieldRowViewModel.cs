// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.ComponentModel;
using System.Windows.Input;
using Core2D.Model.History;
using ReactiveUI;

namespace Core2D.ViewModels.Data;

/// <summary>A stable, identity-preserving pair of a field name and its value.</summary>
public sealed class DataFieldRowViewModel : ReactiveObject, IDisposable
{
    private readonly ViewModelBase? _nameTarget;
    private readonly PropertyViewModel? _property;
    private readonly ValueViewModel? _value;
    private readonly IHistory? _history;
    private readonly string _fallbackName;
    private bool _disposed;

    /// <summary>Adapts a custom property; removal is delegated to its existing owner command.</summary>
    public DataFieldRowViewModel(PropertyViewModel property, ICommand? remove, IHistory? history)
    {
        ArgumentNullException.ThrowIfNull(property);
        Model = property;
        _nameTarget = _property = property;
        _history = history;
        _fallbackName = string.Empty;
        RemoveCommand = remove;
        Observe();
    }

    /// <summary>Pairs record fields by schema position without dropping unmatched values or columns.</summary>
    public DataFieldRowViewModel(ColumnViewModel? column, ValueViewModel? value, int index, IHistory? history)
    {
        if (column is null && value is null) throw new ArgumentException("A column or value is required.");
        Model = (ViewModelBase?)value ?? column!;
        _nameTarget = column;
        _value = value;
        _history = history;
        _fallbackName = $"Field {index + 1}";
        Issue = column is null ? "No matching column" : value is null ? "Missing record value" : string.Empty;
        Observe();
    }

    /// <summary>Gets the original field object, never a cloned document value.</summary>
    public ViewModelBase Model { get; }
    /// <summary>Gets schema mismatch guidance; mismatches never shift later field/value pairs.</summary>
    public string Issue { get; } = string.Empty;
    /// <summary>Gets whether a schema mismatch exists for this row.</summary>
    public bool HasIssue => Issue.Length != 0;
    /// <summary>Gets whether the field has an editable original name.</summary>
    public bool CanEditName => !_disposed && _nameTarget is not null;
    /// <summary>Gets whether the field has an editable original value.</summary>
    public bool CanEditValue => !_disposed && (_property is not null || _value is not null);
    /// <summary>Gets the existing owner command for removing a custom property.</summary>
    public ICommand? RemoveCommand { get; }
    /// <summary>Gets whether a remove action should be presented.</summary>
    public bool CanRemove => !_disposed && RemoveCommand is not null;
    /// <summary>Gets or sets the original field name in one history operation.</summary>
    public string Name
    {
        get => _nameTarget?.Name ?? _fallbackName;
        set
        {
            if (!CanEditName || _nameTarget is not { } target || target.Name == value) return;
            _history?.Snapshot(target.Name, value, text => target.Name = text);
            target.Name = value;
        }
    }
    /// <summary>Gets or sets the original value in one history operation, preserving null on no-op edits.</summary>
    public string? Value
    {
        get => _property is not null ? _property.Value : _value?.Content;
        set
        {
            if (!CanEditValue || Value == value) return;
            if (_property is { } property)
            {
                _history?.Snapshot(property.Value, value, text => property.Value = text);
                property.Value = value;
            }
            else if (_value is { } recordValue)
            {
                _history?.Snapshot(recordValue.Content, value, text => recordValue.Content = text);
                recordValue.Content = value;
            }
        }
    }
    private void Observe()
    {
        if (_nameTarget is not null) _nameTarget.PropertyChanged += OnChanged;
        if (!ReferenceEquals(Model, _nameTarget)) Model.PropertyChanged += OnChanged;
    }
    private void OnChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(ViewModelBase.Name) or null or "") this.RaisePropertyChanged(nameof(Name));
        if (e.PropertyName is nameof(PropertyViewModel.Value) or nameof(ValueViewModel.Content) or null or "") this.RaisePropertyChanged(nameof(Value));
    }
    /// <summary>Releases subscriptions and disables stale adapters; recorded history retains only original models.</summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_nameTarget is not null) _nameTarget.PropertyChanged -= OnChanged;
        if (!ReferenceEquals(Model, _nameTarget)) Model.PropertyChanged -= OnChanged;
        this.RaisePropertyChanged(nameof(CanEditName));
        this.RaisePropertyChanged(nameof(CanEditValue));
        this.RaisePropertyChanged(nameof(CanRemove));
    }
}
