// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    public class DxfBlockRecord : DxfObject<DxfBlockRecord>
    {
        public string Name { get; set; }

        public DxfBlockRecord(DxfAcadVer version, int id)
            : base(version, id)
        {
        }

        public DxfBlockRecord Defaults()
        {
            Name = string.Empty;
            return this;
        }

        public DxfBlockRecord Create()
        {
            Add(0, CodeName.BlockRecord);

            if (Version > DxfAcadVer.AC1009)
            {
                Handle(Id);
                Subclass(SubclassMarker.SymbolTableRecord);
                Subclass(SubclassMarker.BlockTableRecord);
            }

            Add(2, Name);

            return this;
        }
    }
}
