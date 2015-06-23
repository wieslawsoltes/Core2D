// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Test2d
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
        /// 
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
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static Database Create(
            string name, 
            ImmutableArray<Column> columns)
        {
            return new Database()
            {
                Name = name,
                Columns = columns,
                Records = ImmutableArray.Create<Record>()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="columns"></param>
        /// <param name="records"></param>
        /// <returns></returns>
        public static Database Create(
            string name,
            ImmutableArray<Column> columns,
            ImmutableArray<Record> records)
        {
            return new Database()
            {
                Name = name,
                Columns = columns,
                Records = records
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static Database Create(string name, IEnumerable<string[]> fields)
        {
            var db = Database.Create(name);
            var tempColumns = fields.FirstOrDefault().Select(c => Column.Create(c));
            var columns = ImmutableArray.CreateRange<Column>(tempColumns);

            if (columns.Length >= 1 && columns[0].Name == "Id")
            {
                // use existing record Id's
                var tempRecords = fields
                    .Skip(1)
                    .Select(v =>
                            Record.Create(
                                v.FirstOrDefault(),
                                columns,
                                ImmutableArray.CreateRange<Value>(v.Select(c => Value.Create(c)))));
                var records = ImmutableArray.CreateRange<Record>(tempRecords);

                db.Columns = columns;
                db.Records = records;
            }
            else
            {
                // create records with new Id's
                var tempRecords = fields
                    .Skip(1)
                    .Select(v =>
                            Record.Create(
                                columns,
                                ImmutableArray.CreateRange<Value>(v.Select(c => Value.Create(c)))));
                var records = ImmutableArray.CreateRange<Record>(tempRecords);

                db.Columns = columns;
                db.Records = records;
            }

            return db;
        }
    }
}
