// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
            throw new NotImplementedException();
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
        public virtual bool ShouldSerializeColumns() => _columns.IsEmpty == false;

        /// <summary>
        /// Check whether the <see cref="Records"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeRecords() => _records.IsEmpty == false;

        /// <summary>
        /// Check whether the <see cref="CurrentRecord"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeCurrentRecord() => _currentRecord != null;
    }
}
