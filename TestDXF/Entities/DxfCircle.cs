// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    public class DxfCircle : DxfObject<DxfCircle>
    {
        public string Layer { get; set; }
        public string Color { get; set; }
        public double Thickness { get; set; }
        public Vector3 CenterPoint { get; set; }
        public double Radius { get; set; }
        public Vector3 ExtrusionDirection { get; set; }

        public DxfCircle(DxfAcadVer version, int id)
            : base(version, id)
        {
        }

        public DxfCircle Defaults()
        {
            Layer = "0";
            Color = "0";
            Thickness = 0.0;
            CenterPoint = new Vector3(0.0, 0.0, 0.0);
            Radius = 0.0;
            ExtrusionDirection = new Vector3(0.0, 0.0, 1.0);

            return this;
        }

        public DxfCircle Create()
        {
            Add(0, DxfCodeName.Circle);

            Entity();

            if (Version > DxfAcadVer.AC1009)
                Subclass(DxfSubclassMarker.Circle);

            Add(8, Layer);
            Add(62, Color);

            Add(39, Thickness);

            Add(10, CenterPoint.X);
            Add(20, CenterPoint.Y);
            Add(30, CenterPoint.Z);

            Add(40, Radius);

            Add(210, ExtrusionDirection.X);
            Add(220, ExtrusionDirection.Y);
            Add(230, ExtrusionDirection.Z);

            return this;
        }
    }
}
