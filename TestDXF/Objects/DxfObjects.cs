// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Dxf
{
    public class DxfObjects : DxfObject<DxfObjects>
    {
        public DxfObjects(DxfAcadVer version, int id)
            : base(version, id)
        {
        }

        public DxfObjects Begin()
        {
            Add(0, "SECTION");
            Add(2, "OBJECTS");
            return this;
        }

        public DxfObjects Add<T>(T obj)
        {
            Append(obj.ToString());
            return this;
        }

        public DxfObjects Add<T>(IEnumerable<T> objects)
        {
            foreach (var obj in objects)
            {
                Add(obj);
            }

            return this;
        }

        public DxfObjects End()
        {
            Add(0, "ENDSEC");
            return this;
        }
    }
}
