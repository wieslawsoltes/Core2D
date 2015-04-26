// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    public class DxfFile : DxfObject
    {
        public DxfHeader Header { get; set; }
        public DxfClasses Classes { get; set; }
        public DxfTables Tables { get; set; }
        public DxfBlocks Blocks { get; set; }
        public DxfEntities Entities { get; set; }
        public DxfObjects Objects { get; set; }

        public DxfFile(DxfAcadVer version, int id)
            : base(version, id)
        {
        }

        public override string Create()
        {
            Reset();

            Append(Header.Create());

            if (Version > DxfAcadVer.AC1009)
            {
                Append(Classes.Create());
            }

            Append(Tables.Create());
            Append(Blocks.Create());
            Append(Entities.Create());

            if (Version > DxfAcadVer.AC1009)
            {
                Append(Objects.Create());
            }

            Add(0, DxfCodeName.Eof);

            return Build();
        }
    }
}
