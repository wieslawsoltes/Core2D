// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;

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
    }
}
