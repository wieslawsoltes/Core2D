// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Dxf
{
    /// <summary>
    /// 
    /// </summary>
    public class DxfBlock : DxfObject
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Layer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfBlockTypeFlags BlockTypeFlags { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfVector3 BasePoint { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string XrefPathName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int EndId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EndLayer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IList<object> Entities { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <param name="id"></param>
        public DxfBlock(DxfAcadVer version, int id)
            : base(version, id)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void Defaults()
        {
            Name = string.Empty;
            Layer = "0";
            BlockTypeFlags = DxfBlockTypeFlags.Default;
            BasePoint = new DxfVector3(0.0, 0.0, 0.0);
            XrefPathName = default(string);
            Description = default(string);
            EndId = 0;
            EndLayer = "0";
            Entities = default(IList<object>);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
