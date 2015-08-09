// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dxf
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DxfTable<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IList<T> Items { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DxfTable()
        {
            Items = new List<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public DxfTable(int id)
            : this()
        {
            Id = id;
        }
    }
}
