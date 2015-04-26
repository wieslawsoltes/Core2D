// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Dxf
{
    public class DxfInsert : DxfObject
    {
        public string BlockName { get; set; }
        public string Layer { get; set; }
        public DxfVector3 InsertionPoint { get; set; }
        public DxfVector3 ScaleFactor { get; set; }
        public double RotationAngle { get; set; }
        public int ColumnCount { get; set; }
        public int RowCount { get; set; }
        public double ColumnSpacing { get; set; }
        public double RowSpacing { get; set; }
        public DxfVector3 ExtrusionDirection { get; set; }
        public IList<DxfAttrib> Attributes { get; set; }

        public DxfInsert(DxfAcadVer version, int id)
            : base(version, id)
        {
            Attributes = new List<DxfAttrib>();
        }

        public void Defaults()
        {
            BlockName = string.Empty;
            Layer = "0";
            InsertionPoint = new DxfVector3(0.0, 0.0, 0.0);
            ScaleFactor = new DxfVector3(1.0, 1.0, 1.0);
            RotationAngle = 0.0;
            ColumnCount = 1;
            RowCount = 1;
            ColumnSpacing = 0.0;
            RowSpacing = 0.0;
            ExtrusionDirection = new DxfVector3(0.0, 0.0, 1.0);
        }

        public override string Create()
        {
            Reset();

            Add(0, DxfCodeName.Insert);

            Entity();

            if (Version > DxfAcadVer.AC1009)
                Subclass(DxfSubclassMarker.BlockReference);

            Add(2, BlockName);
            Add(8, Layer);

            Add(10, InsertionPoint.X);
            Add(20, InsertionPoint.Y);
            Add(30, InsertionPoint.Z);

            Add(41, ScaleFactor.X);
            Add(42, ScaleFactor.Y);
            Add(43, ScaleFactor.Z);

            Add(50, RotationAngle);

            Add(70, ColumnCount);
            Add(71, RowCount);
            Add(44, ColumnSpacing);
            Add(45, RowSpacing);

            if (Version > DxfAcadVer.AC1009)
            {
                Add(210, ExtrusionDirection.X);
                Add(220, ExtrusionDirection.Y);
                Add(230, ExtrusionDirection.Z);
            }

            if (Attributes != null && Attributes.Count > 0)
            {
                Add(66, "1"); // attributes follow: 0 - no, 1 - yes

                foreach (var attrib in Attributes)
                {
                    Append(attrib.Create());
                }

                Add(0, DxfCodeName.SeqEnd);

                if (Version > DxfAcadVer.AC1009)
                {
                    Handle(Id);
                    Subclass(DxfSubclassMarker.Entity);
                    Add(8, Layer);
                }
            }

            return Build();
        }
    }
}
