// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    public class DxfView : DxfObject<DxfView>
    {
        public DxfView(DxfAcadVer version, int id)
            : base(version, id)
        {
            Add(0, DxfCodeName.View);

            if (version > DxfAcadVer.AC1009)
            {
                Handle(id);
                Subclass(DxfSubclassMarker.SymbolTableRecord);
                Subclass(DxfSubclassMarker.ViewTableRecord);
            }
        }

        public DxfView Name(string name)
        {
            Add(2, name);
            return this;
        }

        public DxfView StandardFlags(DxfViewStandardFlags flags)
        {
            Add(70, (int)flags);
            return this;
        }

        public DxfView Height(double height)
        {
            Add(40, height);
            return this;
        }

        public DxfView Width(double width)
        {
            Add(41, width);
            return this;
        }

        public DxfView Center(DxfVector2 point)
        {
            Add(10, point.X);
            Add(20, point.Y);
            return this;
        }

        public DxfView ViewDirection(Vector3 wcs)
        {
            Add(11, wcs.X);
            Add(21, wcs.Y);
            Add(31, wcs.Z);
            return this;
        }

        public DxfView TargetPoint(Vector3 wcs)
        {
            Add(12, wcs.X);
            Add(22, wcs.Y);
            Add(32, wcs.Z);
            return this;
        }

        public DxfView LensLength(double length)
        {
            Add(42, length);
            return this;
        }

        public DxfView FrontClippingPlane(double offset)
        {
            Add(43, offset);
            return this;
        }

        public DxfView BackClippingPlane(double offset)
        {
            Add(44, offset);
            return this;
        }

        public DxfView Twist(double angle)
        {
            Add(50, angle);
            return this;
        }
    }
}
