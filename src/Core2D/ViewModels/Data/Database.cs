using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.Serialization;

namespace Core2D.Data
{
    [DataContract(IsReference = true)]
    public class Database : ViewModelBase
    {
        private string _idColumnName;
        private ImmutableArray<Column> _columns;
        private ImmutableArray<Record> _records;
        private Record _currentRecord;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public string IdColumnName
        {
            get => _idColumnName;
            set => RaiseAndSetIfChanged(ref _idColumnName, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public ImmutableArray<Column> Columns
        {
            get => _columns;
            set => RaiseAndSetIfChanged(ref _columns, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public ImmutableArray<Record> Records
        {
            get => _records;
            set => RaiseAndSetIfChanged(ref _records, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
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
    }
}
