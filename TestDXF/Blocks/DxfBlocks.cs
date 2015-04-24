// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Dxf
{
    public class DxfBlocks : DxfObject<DxfBlocks>
    {
        public DxfBlocks(DxfAcadVer version, int id)
            : base(version, id)
        {
        }

        public DxfBlocks Begin()
        {
            Add(0, CodeName.Section);
            Add(2, "BLOCKS");
            return this;
        }

        public DxfBlocks Add(DxfBlock block)
        {
            Append(block.ToString());
            return this;
        }

        public DxfBlocks Add(IEnumerable<DxfBlock> blocks)
        {
            foreach (var block in blocks)
                Add(block);

            return this;
        }

        public DxfBlocks End()
        {
            Add(0, CodeName.EndSec);
            return this;
        }
    }
}
