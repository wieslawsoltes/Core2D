using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Attributes;

namespace Core2D.Data
{
    /// <summary>
    /// Records database.
    /// </summary>
    public class Database : ObservableObject, IDatabase
    {
        private string _idColumnName;
        private ImmutableArray<IColumn> _columns;
        private ImmutableArray<IRecord> _records;
        private IRecord _currentRecord;

        /// <inheritdoc/>
        public string IdColumnName
        {
            get => _idColumnName;
            set => Update(ref _idColumnName, value);
        }

        /// <inheritdoc/>
        public ImmutableArray<IColumn> Columns
        {
            get => _columns;
            set => Update(ref _columns, value);
        }

        /// <inheritdoc/>
        [Content]
        public ImmutableArray<IRecord> Records
        {
            get => _records;
            set => Update(ref _records, value);
        }

        /// <inheritdoc/>
        public IRecord CurrentRecord
        {
            get => _currentRecord;
            set => Update(ref _currentRecord, value);
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
