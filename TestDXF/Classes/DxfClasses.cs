// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Dxf
{
    public class DxfClasses : DxfObject
    {
        public IList<DxfClass> Classes { get; set; }

        public DxfClasses(DxfAcadVer version, int id)
            : base(version, id)
        {
            Classes = new List<DxfClass>();
        }

        public override string Create()
        {
            Reset();

            Add(0, DxfCodeName.Section);
            Add(2, "CLASSES");

            foreach(var cls in Classes)
            {
                Append(cls.Create());
            }

            Add(0, DxfCodeName.EndSec);

            return Build();
        }
    }
}
