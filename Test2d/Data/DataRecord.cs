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
    public class DataRecord : ObservableObject
    {
        private IList<string> _columns;
        private IList<string> _data;

        /// <summary>
        /// 
        /// </summary>
        public IList<string> Columns
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
        public IList<string> Data
        {
            get { return _data; }
            set
            {
                if (value != _data)
                {
                    _data = value;
                    Notify("Data");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataRecord Create(
            IList<string> columns, 
            IEnumerable<string> data)
        {
            return new DataRecord()
            {
                Columns = columns,
                Data = new ObservableCollection<string>(data)
            };
        }
    }
}
