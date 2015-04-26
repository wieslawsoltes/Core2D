// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Dxf
{
    public class DxfObjects : DxfObject
    {
        public IList<DxfDictionary> Objects { get; set; }

        public DxfObjects(DxfAcadVer version, int id)
            : base(version, id)
        {
            Objects = new List<DxfDictionary>();
        }

        public override string Create()
        {
            Reset();

            Add(0, DxfCodeName.Section);
            Add(2, "OBJECTS");

            foreach(var obj in Objects)
            {
                Append(obj.Create());
            }

            Add(0, DxfCodeName.EndSec);

            return Build();
        }
    }
}
