// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class Record : ObservableObject
    {
        private Guid _id;
        private ImmutableArray<Column> _columns;
        private ImmutableArray<Value> _values;

        /// <summary>
        /// 
        /// </summary>
        public Guid Id
        {
            get { return _id; }
            set { Update(ref _id, value); }
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
        public ImmutableArray<Value> Values
        {
            get { return _values; }
            set { Update(ref _values, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Record Create(
            ImmutableArray<Column> columns, 
            ImmutableArray<Value> values)
        {
            return new Record()
            {
                Id = Guid.NewGuid(),
                Columns = columns,
                Values = values
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="columns"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Record Create(
            string id,
            ImmutableArray<Column> columns,
            ImmutableArray<Value> values)
        {
            return new Record()
            {
                Id = Guid.Parse(id),
                Columns = columns,
                Values = values
            };
        }
    }
}
