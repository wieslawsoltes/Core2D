#nullable disable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Reactive.Disposables;

namespace Core2D.ViewModels.Data
{
    public partial class DatabaseViewModel : ViewModelBase
    {
        [AutoNotify] private string _idColumnName;
        [AutoNotify] private ImmutableArray<ColumnViewModel> _columns;
        [AutoNotify] private ImmutableArray<RecordViewModel> _records;
        [AutoNotify] private RecordViewModel _currentRecord;

        public DatabaseViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            var columns = this._columns.Copy(shared).ToImmutable();
            var records = this._records.Copy(shared).ToImmutable();
            var currentRecordIndex = _records.IndexOf(_currentRecord);

            return new DatabaseViewModel(_serviceProvider)
            {
                Name = this.Name,
                IdColumnName = this.IdColumnName,
                Columns = columns,
                Records = records,
                CurrentRecord = records[currentRecordIndex]
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

        public override IDisposable Subscribe(IObserver<(object sender, PropertyChangedEventArgs e)> observer)
        {
            var mainDisposable = new CompositeDisposable();
            var disposablePropertyChanged = default(IDisposable);
            var disposableColumns = default(CompositeDisposable);
            var disposableRecords = default(CompositeDisposable);

            ObserveSelf(Handler, ref disposablePropertyChanged, mainDisposable);
            ObserveList(_columns, ref disposableColumns, mainDisposable, observer);
            ObserveList(_records, ref disposableRecords, mainDisposable, observer);

            void Handler(object sender, PropertyChangedEventArgs e)
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
