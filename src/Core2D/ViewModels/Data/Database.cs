using System.Collections.Generic;
using System.Collections.Immutable;

namespace Core2D.Data
{
    /// <summary>
    /// Records database.
    /// </summary>
    public class Database : ObservableObject
    {
        private string _idColumnName;
        private ImmutableArray<Column> _columns;
        private ImmutableArray<Record> _records;
        private Record _currentRecord;

        /// <inheritdoc/>
        public string IdColumnName
        {
            get => _idColumnName;
            set => RaiseAndSetIfChanged(ref _idColumnName, value);
        }

        /// <inheritdoc/>
        public ImmutableArray<Column> Columns
        {
            get => _columns;
            set => RaiseAndSetIfChanged(ref _columns, value);
        }

        /// <inheritdoc/>
        public ImmutableArray<Record> Records
        {
            get => _records;
            set => RaiseAndSetIfChanged(ref _records, value);
        }

        /// <inheritdoc/>
        public Record CurrentRecord
        {
            get => _currentRecord;
            set => RaiseAndSetIfChanged(ref _currentRecord, value);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <summary>
        /// Check whether the <see cref="IdColumnName"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeIdColumnName() => !string.IsNullOrWhiteSpace(_idColumnName);

        /// <summary>
        /// Check whether the <see cref="Columns"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeColumns() => true;

        /// <summary>
        /// Check whether the <see cref="Records"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeRecords() => true;

        /// <summary>
        /// Check whether the <see cref="CurrentRecord"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeCurrentRecord() => _currentRecord != null;
    }
}
