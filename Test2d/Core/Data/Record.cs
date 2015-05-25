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
    public class Record : ObservableObject
    {
        private Guid _id;
        private IList<Column> _columns;
        private IList<Value> _values;

        /// <summary>
        /// 
        /// </summary>
        public Guid Id
        {
            get { return _id; }
            set
            {
                if (value != _id)
                {
                    _id = value;
                    Notify("Id");
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
        public IList<Value> Values
        {
            get { return _values; }
            set
            {
                if (value != _values)
                {
                    _values = value;
                    Notify("Values");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Record Create(
            IList<Column> columns, 
            IEnumerable<Value> values)
        {
            return new Record()
            {
                Id = Guid.NewGuid(),
                Columns = columns,
                Values = new ObservableCollection<Value>(values)
            };
        }
    }
}
