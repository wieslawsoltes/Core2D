// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    public class DxfFile : DxfObject<DxfFile>
    {
        public DxfFile(DxfAcadVer version, int id)
            : base(version, id)
        {
        }

        public DxfFile Header(DxfHeader header)
        {
            Append(header.ToString());
            return this;
        }

        public DxfFile Classes(DxfClasses classes)
        {
            Append(classes.ToString());
            return this;
        }

        public DxfFile Tables(DxfTables tables)
        {
            Append(tables.ToString());
            return this;
        }

        public DxfFile Blocks(DxfBlocks blocks)
        {
            Append(blocks.ToString());
            return this;
        }

        public DxfFile Entities(DxfEntities entities)
        {
            Append(entities.ToString());
            return this;
        }

        public DxfFile Objects(DxfObjects objects)
        {
            Append(objects.ToString());
            return this;
        }

        public DxfFile Eof()
        {
            Add(0, "EOF");
            return this;
        }
    }
}
