// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dxf
{
    public class DxfTable<T>
    {
        public int Id { get; set; }
        public IList<T> Items { get; set; }

        public DxfTable()
        {
            Items = new List<T>();
        }

        public DxfTable(int id)
            : this()
        {
            Id = id;
        }
    }
}
