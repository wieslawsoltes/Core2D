// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    public class DxfLtype : DxfObject<DxfLtype>
    {
        public string Name { get; set; }
        public DxfLtypeStandardFlags LtypeStandardFlags { get; set; }
        public string Description { get; set; }
        public int DashLengthItems { get; set; }
        public double TotalPatternLength { get; set; }
        public double[] DashLengths { get; set; }

        public DxfLtype(DxfAcadVer version, int id)
            : base(version, id)
        {
        }

        public DxfLtype Defaults()
        {
            Name = string.Empty;
            LtypeStandardFlags = DxfLtypeStandardFlags.Default;
            Description = string.Empty;
            DashLengthItems = 0;
            TotalPatternLength = 0.0;
            DashLengths = null;

            return this;
        }

        public DxfLtype Create()
        {
            Add(0, DxfCodeName.Ltype);

            if (Version > DxfAcadVer.AC1009)
            {
                Handle(Id);
                Subclass(DxfSubclassMarker.SymbolTableRecord);
                Subclass(DxfSubclassMarker.LinetypeTableRecord);
            }

            Add(2, Name);
            Add(70, (int)LtypeStandardFlags);
            Add(3, Description);
            Add(72, 65); // alignment code; value is always 65, the ASCII code for A

            Add(73, DashLengthItems);
            Add(40, TotalPatternLength);

            if (DashLengths != null)
            {
                // dash length 0,1,2...n-1 = DashLengthItems
                foreach (var length in DashLengths)
                {
                    Add(49, length);
                }
            }

            if (Version > DxfAcadVer.AC1009)
            {
                // TODO: multiple complex linetype elements
                // 74
                // 75
                // 340
                // 46
                // 50
                // 44
                // 45
                // 9
            }

            return this;
        }
    }
}
