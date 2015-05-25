// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class Database : ObservableObject
    {
        private string _name;
        private IList<Column> _columns;
        private IList<Record> _records;

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    Notify("Name");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IList<Column> Columns
        {
            get { return _columns; }
            set
            {
                if (value != _columns)
                {
                    _columns = value;
                    Notify("Columns");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IList<Record> Records
        {
            get { return _records; }
            set
            {
                if (value != _records)
                {
                    _records = value;
                    Notify("Records");
                }
            }
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
                Columns = new ObservableCollection<Column>(),
                Records = new ObservableCollection<Record>()
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
            IList<Column> columns, 
            IList<Record> records)
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
