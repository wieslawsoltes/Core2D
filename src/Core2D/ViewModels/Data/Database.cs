using System.Collections.Generic;
using System.Collections.Immutable;

namespace Core2D.Data
{
    public class Database : ObservableObject
    {
        private string _idColumnName;
        private ImmutableArray<Column> _columns;
        private ImmutableArray<Record> _records;
        private Record _currentRecord;

        public string IdColumnName
        {
            get => _idColumnName;
            set => RaiseAndSetIfChanged(ref _idColumnName, value);
        }

        public ImmutableArray<Column> Columns
        {
            get => _columns;
            set => RaiseAndSetIfChanged(ref _columns, value);
        }

        public ImmutableArray<Record> Records
        {
            get => _records;
            set => RaiseAndSetIfChanged(ref _records, value);
        }

        public Record CurrentRecord
        {
            get => _currentRecord;
            set => RaiseAndSetIfChanged(ref _currentRecord, value);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            var columns = this._columns.Copy(shared).ToImmutable();
            var records = this._records.Copy(shared).ToImmutable();
            var currentRecordIndex = _records.IndexOf(_currentRecord);

            return new Database()
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

        public virtual bool ShouldSerializeIdColumnName() => !string.IsNullOrWhiteSpace(_idColumnName);

        public virtual bool ShouldSerializeColumns() => true;

        public virtual bool ShouldSerializeRecords() => true;

        public virtual bool ShouldSerializeCurrentRecord() => _currentRecord != null;
    }
}
