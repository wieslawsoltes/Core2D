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

        /// <summary>
        /// Default Id column name.
        /// </summary>
        public const string DefaultIdColumnName = "Id";

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

        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class.
        /// </summary>
        public Database()
            : base()
        {
            _columns = ImmutableArray.Create<IColumn>();
            _records = ImmutableArray.Create<IRecord>();
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="Database"/> instance.
        /// </summary>
        /// <param name="name">The database name.</param>
        /// <param name="idColumnName">The Id column name.</param>
        /// <returns>The new instance of the <see cref="Database"/> class.</returns>
        public static IDatabase Create(string name, string idColumnName = DefaultIdColumnName)
        {
            return new Database()
            {
                Name = name,
                IdColumnName = idColumnName,
                Columns = ImmutableArray.Create<IColumn>(),
                Records = ImmutableArray.Create<IRecord>()
            };
        }

        /// <summary>
        /// Creates a new <see cref="Database"/> instance.
        /// </summary>
        /// <param name="name">The database name.</param>
        /// <param name="columns">The database columns.</param>
        /// <param name="idColumnName">The Id column name.</param>
        /// <returns>The new instance of the <see cref="Database"/> class.</returns>
        public static IDatabase Create(string name, ImmutableArray<IColumn> columns, string idColumnName = DefaultIdColumnName)
        {
            return new Database()
            {
                Name = name,
                IdColumnName = idColumnName,
                Columns = columns,
                Records = ImmutableArray.Create<IRecord>()
            };
        }

        /// <summary>
        /// Creates a new <see cref="Database"/> instance.
        /// </summary>
        /// <param name="name">The database name.</param>
        /// <param name="columns">The database columns.</param>
        /// <param name="records">The database records.</param>
        /// <param name="idColumnName">The Id column name.</param>
        /// <returns>The new instance of the <see cref="Database"/> class.</returns>
        public static IDatabase Create(string name, ImmutableArray<IColumn> columns, ImmutableArray<IRecord> records, string idColumnName = DefaultIdColumnName)
        {
            return new Database()
            {
                Name = name,
                IdColumnName = idColumnName,
                Columns = columns,
                Records = records
            };
        }

        /// <summary>
        /// Creates a new <see cref="Database"/> instance.
        /// </summary>
        /// <param name="name">The database name.</param>
        /// <param name="fields">The fields collection.</param>
        /// <param name="idColumnName">The Id column name.</param>
        /// <returns>The new instance of the <see cref="Database"/> class.</returns>
        public static IDatabase FromFields(string name, IEnumerable<string[]> fields, string idColumnName = DefaultIdColumnName)
        {
            var db = Database.Create(name, idColumnName);
            var tempColumns = fields.FirstOrDefault().Select(c => Column.Create(db, c));
            var columns = ImmutableArray.CreateRange<IColumn>(tempColumns);

            if (columns.Length >= 1 && columns[0].Name == idColumnName)
            {
                db.Columns = columns;

                // Use existing record Id.
                var tempRecords = fields
                    .Skip(1)
                    .Select(v =>
                            Record.Create(
                                db,
                                v.FirstOrDefault(),
                                ImmutableArray.CreateRange<IValue>(v.Select(c => Value.Create(c)))));

                db.Records = ImmutableArray.CreateRange<IRecord>(tempRecords);
            }
            else
            {
                db.Columns = columns;

                // Create records with new Id.
                var tempRecords = fields
                    .Skip(1)
                    .Select(v =>
                            Record.Create(
                                db,
                                ImmutableArray.CreateRange<IValue>(v.Select(c => Value.Create(c)))));

                db.Records = ImmutableArray.CreateRange<IRecord>(tempRecords);
            }

            return db;
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
