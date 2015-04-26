// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    public class DxfDimstyle : DxfObject
    {
        public string Name { get; set; }
        public DxfDimstyleStandardFlags DimstyleStandardFlags { get; set; }

        public DxfDimstyle(DxfAcadVer version, int id)
            : base(version, id)
        {
        }

        public void Defaults()
        {
            Name = string.Empty;
            DimstyleStandardFlags = DxfDimstyleStandardFlags.Default;
        }

        public override string Create()
        {
            Reset();

            Add(0, DxfCodeName.Dimstyle);

            if (Version > DxfAcadVer.AC1009)
            {
                Add(105, Id.ToDxfHandle()); // Dimstyle handle code is 105 instead of 5
                Subclass(DxfSubclassMarker.SymbolTableRecord);
                Subclass(DxfSubclassMarker.DimStyleTableRecord);
            }

            Add(2, Name);
            Add(70, (int)DimstyleStandardFlags);

            return Build();
        }
    }
}
