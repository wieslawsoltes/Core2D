// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    public class DxfInsert : DxfObject<DxfInsert>
    {
        public DxfInsert(DxfAcadVer version, int id)
            : base(version, id)
        {
            Add(0, DxfCodeName.Insert);

            Entity();

            if (Version > DxfAcadVer.AC1009)
                Subclass(SubclassMarker.BlockReference);
        }

        public DxfInsert Block(string name)
        {
            Add(2, name);
            return this;
        }

        public DxfInsert Layer(string layer)
        {
            Add(8, layer);
            return this;
        }

        public DxfInsert Insertion(Vector3 point)
        {
            Add(10, point.X);
            Add(20, point.Y);
            Add(30, point.Z);
            return this;
        }

        public DxfInsert Scale(Vector3 factor)
        {
            Add(41, factor.X);
            Add(42, factor.Y);
            Add(43, factor.Z);
            return this;
        }

        public DxfInsert Rotation(double angle)
        {
            Add(50, angle);
            return this;
        }

        public DxfInsert Columns(int count)
        {
            Add(70, count);
            return this;
        }

        public DxfInsert Rows(int count)
        {
            Add(71, count);
            return this;
        }

        public DxfInsert ColumnSpacing(double value)
        {
            Add(44, value);
            return this;
        }

        public DxfInsert RowSpacing(double value)
        {
            Add(45, value);
            return this;
        }

        public DxfInsert AttributesBegin()
        {
            Add(66, "1"); // attributes follow: 0 - no, 1 - yes
            return this;
        }

        public DxfInsert AddAttribute(DxfAttrib attrib)
        {
            Append(attrib.ToString());
            return this;
        }

        public DxfInsert AttributesEnd(int id, string layer)
        {
            Add(0, DxfCodeName.SeqEnd);

            if (Version > DxfAcadVer.AC1009)
            {
                Handle(id);
                Subclass(SubclassMarker.Entity);
                Add(8, layer);
            }

            return this;
        }
    }
}
