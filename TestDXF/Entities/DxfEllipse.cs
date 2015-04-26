// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    public class DxfEllipse : DxfObject
    {
        public string Layer { get; set; }
        public string Color { get; set; }
        public DxfVector3 CenterPoint { get; set; }
        public DxfVector3 EndPoint { get; set; }
        public DxfVector3 ExtrusionDirection { get; set; }
        public double Ratio { get; set; }
        public double StartParameter { get; set; }
        public double EndParameter { get; set; }

        public DxfEllipse(DxfAcadVer version, int id)
            : base(version, id)
        {
        }

        public void Defaults()
        {
            Layer = "0";
            Color = "0";
            CenterPoint = new DxfVector3(0.0, 0.0, 0.0);
            EndPoint = new DxfVector3(0.0, 0.0, 0.0);
            ExtrusionDirection = new DxfVector3(0.0, 0.0, 1.0);
            Ratio = 0.0;
            StartParameter = 0.0;
            EndParameter = 2.0 * Math.PI;
        }

        public override string Create()
        {
            Reset();

            Add(0, DxfCodeName.Ellipse);

            Entity();

            if (Version > DxfAcadVer.AC1009)
                Subclass(DxfSubclassMarker.Ellipse);

            Add(8, Layer);
            Add(62, Color);

            Add(10, CenterPoint.X);
            Add(20, CenterPoint.Y);
            Add(30, CenterPoint.Z);

            Add(11, EndPoint.X);
            Add(21, EndPoint.Y);
            Add(31, EndPoint.Z);

            Add(210, ExtrusionDirection.X);
            Add(220, ExtrusionDirection.Y);
            Add(230, ExtrusionDirection.Z);

            Add(40, Ratio);
            Add(41, StartParameter);
            Add(42, EndParameter);

            return Build();
        }
    }
}
