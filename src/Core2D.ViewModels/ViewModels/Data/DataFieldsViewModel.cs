// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using Core2D.ViewModels.Shapes;
using System.ComponentModel;
using Core2D.Model;
using Core2D.Model.History;
using ReactiveUI;

namespace Core2D.ViewModels.Data;

/// <summary>Owns the live property/record projection and the lifetime of its row adapters.</summary>
public sealed class DataFieldsViewModel : ReactiveObject, IDisposable
{
    private readonly object? _source;
    private readonly IHistory? _history;
    private DatabaseViewModel? _database;
    private ImmutableArray<DataFieldRowViewModel> _rows = ImmutableArray<DataFieldRowViewModel>.Empty;
    private bool _disposed;
    private readonly bool _includeChildProperties;
    private readonly List<BaseShapeViewModel> _children = new();

    /// <summary>Adapts custom properties or a record. Unsupported/null sources produce an empty projection.</summary>
    public DataFieldsViewModel(object? source, IHistory? history, bool includeChildProperties = false)
    {
        _source = source;
        _includeChildProperties = includeChildProperties;
        _history = history;
        if (source is INotifyPropertyChanged observable) observable.PropertyChanged += OnSourceChanged;
        Rebuild();
    }
    /// <summary>Gets current row adapters in original schema/property order.</summary>
    public ImmutableArray<DataFieldRowViewModel> Rows => _rows;
    /// <summary>Gets how many record fields do not have both a column and a value.</summary>
    public int IssueCount { get; private set; }
    /// <summary>Gets whether record schema mismatches need to be shown.</summary>
    public bool HasIssues => IssueCount != 0;

    private void OnSourceChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(BlockShapeViewModel.Shapes) or nameof(IDataObject.Properties) or nameof(RecordViewModel.Values) or nameof(ViewModelBase.Owner) or null or "") Rebuild();
    }
    private void OnSchemaChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(DatabaseViewModel.Columns) or null or "") Rebuild();
    }
    private void Rebuild()
    {
        if (_disposed) return;
        DetachChildren();
        if (_database is not null) _database.PropertyChanged -= OnSchemaChanged;
        foreach (DataFieldRowViewModel row in _rows) row.Dispose();
        _database = (_source as RecordViewModel)?.Owner as DatabaseViewModel;
        if (_database is not null) _database.PropertyChanged += OnSchemaChanged;
        var rows = ImmutableArray.CreateBuilder<DataFieldRowViewModel>();
        IssueCount = 0;
        if (_includeChildProperties && _source is BlockShapeViewModel block)
        {
            var seen = new HashSet<BaseShapeViewModel>(ReferenceEqualityComparer.Instance);
            if (!block.Shapes.IsDefault)
                foreach (BaseShapeViewModel child in block.Shapes)
                {
                    if (!seen.Add(child)) continue;
                    _children.Add(child);
                    child.PropertyChanged += OnChildChanged;
                    if (!child.Properties.IsDefault)
                        foreach (PropertyViewModel property in child.Properties)
                            rows.Add(new DataFieldRowViewModel(property, child.RemoveProperty, _history, child));
                }
        }
        else if (_source is RecordViewModel record)
        {
            var columns = _database?.Columns ?? ImmutableArray<ColumnViewModel>.Empty;
            var values = record.Values;
            int count = Math.Max(columns.IsDefault ? 0 : columns.Length, values.IsDefault ? 0 : values.Length);
            for (int index = 0; index < count; index++)
            {
                var row = new DataFieldRowViewModel(!columns.IsDefault && index < columns.Length ? columns[index] : null,
                    !values.IsDefault && index < values.Length ? values[index] : null, index, _history);
                rows.Add(row);
                if (row.HasIssue) IssueCount++;
            }
        }
        else if (_source is IDataObject data && !data.Properties.IsDefault)
        {
            foreach (PropertyViewModel property in data.Properties)
                rows.Add(new DataFieldRowViewModel(property, data.RemoveProperty, _history));
        }
        _rows = rows.ToImmutable();
        this.RaisePropertyChanged(nameof(Rows));
        this.RaisePropertyChanged(nameof(IssueCount));
        this.RaisePropertyChanged(nameof(HasIssues));
    }
    private void OnChildChanged(object? sender, PropertyChangedEventArgs e)
    { if (e.PropertyName is nameof(IDataObject.Properties) or null or "") Rebuild(); }
    private void DetachChildren()
    {
        foreach (BaseShapeViewModel child in _children) child.PropertyChanged -= OnChildChanged;
        _children.Clear();
    }
    /// <summary>Unsubscribes from the source and schema and invalidates all row adapters.</summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        DetachChildren();
        if (_source is INotifyPropertyChanged observable) observable.PropertyChanged -= OnSourceChanged;
        if (_database is not null) _database.PropertyChanged -= OnSchemaChanged;
        _database = null;
        foreach (DataFieldRowViewModel row in _rows) row.Dispose();
    }
}
