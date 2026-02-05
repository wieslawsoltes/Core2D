// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Runtime.Serialization;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Editor;

namespace Core2D.ViewModels.Data;

public partial class DatabaseViewModel : ViewModelBase
{
    [AutoNotify] private string? _idColumnName;
    [AutoNotify] private ImmutableArray<ColumnViewModel> _columns;
    [AutoNotify] private ImmutableArray<RecordViewModel> _records;
    [AutoNotify] private RecordViewModel? _currentRecord;
    [AutoNotify(IgnoreDataMember = true)] private string? _recordFilterText;
    [AutoNotify(IgnoreDataMember = true)] private string? _columnFilterText;

    public DatabaseViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
        AddColumn = new RelayCommand<DatabaseViewModel?>(x => GetProject()?.OnAddColumn(x));
            
        RemoveColumn = new RelayCommand<ColumnViewModel?>(x => GetProject()?.OnRemoveColumn(x));
            
        AddRecord = new RelayCommand<DatabaseViewModel?>(x => GetProject()?.OnAddRecord(x));
            
        RemoveRecord = new RelayCommand<RecordViewModel?>(x => GetProject()?.OnRemoveRecord(x));

        ApplyRecord = new RelayCommand<RecordViewModel?>(x => GetProject()?.OnApplyRecord(x));

        ProjectContainerViewModel? GetProject() => ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
    }

    [IgnoreDataMember]
    public ICommand AddColumn { get; }
        
    [IgnoreDataMember]
    public ICommand RemoveColumn { get; }
        
    [IgnoreDataMember]
    public ICommand AddRecord { get; }
        
    [IgnoreDataMember]
    public ICommand RemoveRecord { get; }
        
    [IgnoreDataMember]
    public ICommand ApplyRecord { get; }

    public override object Copy(IDictionary<object, object>? shared)
    {
        var columns = _columns.CopyShared(shared).ToImmutable();
        var records = _records.CopyShared(shared).ToImmutable();
        var currentRecord = _currentRecord.GetCurrentItem(ref _records, ref records);

        var copy = new DatabaseViewModel(ServiceProvider)
        {
            Name = Name,
            IdColumnName = IdColumnName,
            Columns = columns,
            Records = records,
            CurrentRecord = currentRecord
        };

        return copy;
    }

    public override bool IsDirty()
    {
        var isDirty = base.IsDirty();

        foreach (var column in _columns)
        {
            isDirty |= column.IsDirty();
        }

        foreach (var record in _records)
        {
            isDirty |= record.IsDirty();
        }

        return isDirty;
    }

    public override void Invalidate()
    {
        base.Invalidate();

        foreach (var column in _columns)
        {
            column.Invalidate();
        }

        foreach (var record in _records)
        {
            record.Invalidate();
        }
    }

    public override IDisposable Subscribe(IObserver<(object? sender, PropertyChangedEventArgs e)> observer)
    {
        var mainDisposable = new CompositeDisposable();
        var disposablePropertyChanged = default(IDisposable);
        var disposableColumns = default(CompositeDisposable);
        var disposableRecords = default(CompositeDisposable);

        ObserveSelf(Handler, ref disposablePropertyChanged, mainDisposable);
        ObserveList(_columns, ref disposableColumns, mainDisposable, observer);
        ObserveList(_records, ref disposableRecords, mainDisposable, observer);

        void Handler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Columns))
            {
                ObserveList(_columns, ref disposableColumns, mainDisposable, observer);
            }

            if (e.PropertyName == nameof(Records))
            {
                ObserveList(_records, ref disposableRecords, mainDisposable, observer);
            }

            observer.OnNext((sender, e));
        }

        return mainDisposable;
    }
}
