#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Reactive.Disposables;

namespace Core2D.ViewModels.Data
{
    public partial class DatabaseViewModel : ViewModelBase
    {
        [AutoNotify] private string? _idColumnName;
        [AutoNotify] private ImmutableArray<ColumnViewModel> _columns;
        [AutoNotify] private ImmutableArray<RecordViewModel> _records;
        [AutoNotify] private RecordViewModel? _currentRecord;

        public DatabaseViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override object Copy(IDictionary<object, object>? shared)
        {
            var columns = _columns.Copy(shared).ToImmutable();
            var records = _records.Copy(shared).ToImmutable();
            var currentRecordIndex = _currentRecord is null ? -1 : _records.IndexOf(_currentRecord);
            var currentRecord = currentRecordIndex == -1 ? null : records[currentRecordIndex];

            return new DatabaseViewModel(ServiceProvider)
            {
                Name = Name,
                IdColumnName = IdColumnName,
                Columns = columns,
                Records = records,
                CurrentRecord = currentRecord
            };
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
}
