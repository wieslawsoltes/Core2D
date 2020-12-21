#nullable disable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

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

            foreach (var column in Columns)
            {
                isDirty |= column.IsDirty();
            }

            foreach (var record in Records)
            {
                isDirty |= record.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            foreach (var column in Columns)
            {
                column.Invalidate();
            }

            foreach (var record in Records)
            {
                record.Invalidate();
            }
        }
    }
}
