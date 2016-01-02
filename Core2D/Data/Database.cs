// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Core2D
{
    /// <summary>
    /// 
    /// </summary>
    public class Database : ObservableObject
    {
        private string _name;
        private ImmutableArray<Column> _columns;
        private ImmutableArray<Record> _records;
        private Record _currentRecord;

        /// <summary>
        /// 
        /// </summary>
        public const string IdColumnName = "Id";

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { Update(ref _name, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ImmutableArray<Column> Columns
        {
            get { return _columns; }
            set { Update(ref _columns, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ImmutableArray<Record> Records
        {
            get { return _records; }
            set { Update(ref _records, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Record CurrentRecord
        {
            get { return _currentRecord; }
            set { Update(ref _currentRecord, value); }
        }

        /// <summary>
        /// Creates a new <see cref="Database"/> instance.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Database Create(string name)
        {
            return new Database()
            {
                Name = name,
                Columns = ImmutableArray.Create<Column>(),
                Records = ImmutableArray.Create<Record>()
            };
        }

        /// <summary>
        /// Creates a new <see cref="Database"/> instance.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static Database Create(string name, ImmutableArray<Column> columns)
        {
            return new Database()
            {
                Name = name,
                Columns = columns,
                Records = ImmutableArray.Create<Record>()
            };
        }

        /// <summary>
        /// Creates a new <see cref="Database"/> instance.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="columns"></param>
        /// <param name="records"></param>
        /// <returns></returns>
        public static Database Create(string name, ImmutableArray<Column> columns, ImmutableArray<Record> records)
        {
            return new Database()
            {
                Name = name,
                Columns = columns,
                Records = records
            };
        }

        /// <summary>
        /// Creates a new <see cref="Database"/> instance.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static Database Create(string name, IEnumerable<string[]> fields)
        {
            var db = Database.Create(name);
            var tempColumns = fields.FirstOrDefault().Select(c => Column.Create(db, c));
            var columns = ImmutableArray.CreateRange<Column>(tempColumns);

            if (columns.Length >= 1 && columns[0].Name == IdColumnName)
            {
                // Use existing record Id.
                var tempRecords = fields
                    .Skip(1)
                    .Select(v =>
                            Record.Create(
                                db,
                                v.FirstOrDefault(),
                                columns,
                                ImmutableArray.CreateRange<Value>(v.Select(c => Value.Create(c)))));
                var records = ImmutableArray.CreateRange<Record>(tempRecords);

                db.Columns = columns;
                db.Records = records;
            }
            else
            {
                // Create records with new Id.
                var tempRecords = fields
                    .Skip(1)
                    .Select(v =>
                            Record.Create(
                                db,
                                columns,
                                ImmutableArray.CreateRange<Value>(v.Select(c => Value.Create(c)))));
                var records = ImmutableArray.CreateRange<Record>(tempRecords);

                db.Columns = columns;
                db.Records = records;
            }

            return db;
        }

        /// <summary>
        /// Update the destination database using data from source database using Id column as identification.
        /// </summary>
        /// <param name="destination">The destination database.</param>
        /// <param name="source">The source database.</param>
        /// <param name="records">The updated records from destination database.</param>
        /// <returns>True if destination database was updated.</returns>
        public static bool Update(Database destination, Database source, out ImmutableArray<Record>.Builder records)
        {
            bool isDirty = false;
            records = null;

            if (source == null || destination == null)
            {
                return isDirty;
            }

            // Check the number of source database columns.
            if (source.Columns.Length <= 1)
            {
                return isDirty;
            }

            // Check for presence of the Id column in the source database.
            if (source.Columns[0].Name != Database.IdColumnName)
            {
                return isDirty;
            }

            // Check for matching columns length.
            if (source.Columns.Length - 1 != destination.Columns.Length)
            {
                return isDirty;
            }

            // Check for matching column names.
            for (int i = 1; i < source.Columns.Length; i++)
            {
                if (source.Columns[i].Name != destination.Columns[i - 1].Name)
                {
                    return isDirty;
                }
            }

            // Create updated records builder.
            records = destination.Records.ToBuilder();

            // Update or remove existing records.
            for (int i = 0; i < destination.Records.Length; i++)
            {
                var record = destination.Records[i];
                var result = source.Records.FirstOrDefault(r => r.Id == record.Id);
                if (result != null)
                {
                    // Update existing record.
                    for (int j = 1; j < result.Values.Length; j++)
                    {
                        var valuesBuilder = record.Values.ToBuilder();
                        valuesBuilder[j - 1] = result.Values[j];
                        record.Values = valuesBuilder.ToImmutable();
                    }
                    isDirty = true;
                }
                else
                {
                    // Remove existing record.
                    records.Remove(record);
                    isDirty = true;
                }
            }

            // Add new records.
            for (int i = 0; i < source.Records.Length; i++)
            {
                var record = source.Records[i];
                var result = destination.Records.FirstOrDefault(r => r.Id == record.Id);
                if (result == null)
                {
                    var r = source.Records[i];

                    // Use existing columns.
                    r.Columns = destination.Columns;

                    // Skip Id column.
                    r.Values = r.Values.Skip(1).ToImmutableArray();

                    // Add new record.
                    records.Add(r);
                    isDirty = true;
                }
            }

            return isDirty;
        }
    }
}
