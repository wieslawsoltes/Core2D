// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Dxf
{
    public class DxfBlock : DxfObject
    {
        public string Name { get; set; }
        public string Layer { get; set; }
        public DxfBlockTypeFlags BlockTypeFlags { get; set; }
        public DxfVector3 BasePoint { get; set; }
        public string XrefPathName { get; set; }
        public string Description { get; set; }
        public int EndId { get; set; }
        public string EndLayer { get; set; }
        public IList<object> Entities { get; set; }

        public DxfBlock(DxfAcadVer version, int id)
            : base(version, id)
        {
        }

        public void Defaults()
        {
            Name = string.Empty;
            Layer = "0";
            BlockTypeFlags = DxfBlockTypeFlags.Default;
            BasePoint = new DxfVector3(0.0, 0.0, 0.0);
            XrefPathName = null;
            Description = null;
            EndId = 0;
            EndLayer = "0";
            Entities = null;
        }

        public override string Create()
        {
            Reset();

            Add(0, DxfCodeName.Block);

            if (Version > DxfAcadVer.AC1009)
            {
                Handle(Id);
                Subclass(DxfSubclassMarker.Entity);
            }

            Add(8, Layer);

            if (Version > DxfAcadVer.AC1009)
            {
                Subclass(DxfSubclassMarker.BlockBegin);
            }

            Add(2, Name);
            Add(70, (int)BlockTypeFlags);

            Add(10, BasePoint.X);
            Add(20, BasePoint.Y);
            Add(30, BasePoint.Z);

            Add(3, Name);

            if (XrefPathName != null)
            {
                Add(1, XrefPathName);
            }

            if (Version > DxfAcadVer.AC1014 && Description != null)
            {
                Add(4, Description);
            }

            if (Entities != null)
            {
                foreach (var entity in Entities)
                {
                    Append(entity.ToString());
                }
            }

            Add(0, DxfCodeName.Endblk);

            if (Version > DxfAcadVer.AC1009)
            {
                Handle(EndId);
                Subclass(DxfSubclassMarker.Entity);
                Add(8, EndLayer);
                Subclass(DxfSubclassMarker.BlockEnd);
            }

            return Build();
        }
    }
}
