// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    public class DxfBlockRecord : DxfObject
    {
        public string Name { get; set; }

        public DxfBlockRecord(DxfAcadVer version, int id)
            : base(version, id)
        {
        }

        public void Defaults()
        {
            Name = string.Empty;
        }

        public override string Create()
        {
            Reset();

            Add(0, DxfCodeName.BlockRecord);

            if (Version > DxfAcadVer.AC1009)
            {
                Handle(Id);
                Subclass(DxfSubclassMarker.SymbolTableRecord);
                Subclass(DxfSubclassMarker.BlockTableRecord);
            }

            Add(2, Name);

            return Build();
        }
    }
}
