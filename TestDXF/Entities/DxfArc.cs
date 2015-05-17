// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    /// <summary>
    /// 
    /// </summary>
    public class DxfArc : DxfObject
    {
        /// <summary>
        /// 
        /// </summary>
        public string Layer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double Thickness { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfVector3 CenterPoint { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double Radius { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double StartAngle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double EndAngle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfVector3 ExtrusionDirection { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <param name="id"></param>
        public DxfArc(DxfAcadVer version, int id)
            : base(version, id)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void Defaults()
        {
            Layer = "0";
            Color = "0";
            Thickness = 0.0;
            CenterPoint = new DxfVector3(0.0, 0.0, 0.0);
            Radius = 0.0;
            StartAngle = 0.0;
            EndAngle = 0.0;
            ExtrusionDirection = new DxfVector3(0.0, 0.0, 1.0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string Create()
        {
            Reset();

            Add(0, DxfCodeName.Arc);

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

            if (Version > DxfAcadVer.AC1009)
                Subclass(DxfSubclassMarker.Arc);

            Add(50, StartAngle);
            Add(51, EndAngle);

            return Build();
        }
    }
}
