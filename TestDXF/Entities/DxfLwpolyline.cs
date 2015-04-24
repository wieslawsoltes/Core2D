// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    public class DxfLwpolyline : DxfObject<DxfLwpolyline>
    {
        public string Layer { get; set; }
        public string Color { get; set; }
        public double Thickness { get; set; }
        public DxfLwpolylineFlags LwpolylineFlags { get; set; }
        public double ConstantWidth { get; set; }
        public double Elevation { get; set; }
        public DxfLwpolylineVertex [] Vertices { get; set; }
        public Vector3 ExtrusionDirection { get; set; }
    
        public DxfLwpolyline(DxfAcadVer version, int id)
            : base(version, id)
        {
        }
        
        public DxfLwpolyline Defaults()
        {
            // TODO:

            return this;
        }
        
        public DxfLwpolyline Create()
        {
            Add(0, CodeName.Lwpolyline);

            Entity();

            if (Version > DxfAcadVer.AC1009)
                Subclass(SubclassMarker.Lwpolyline);
            
            Add(8, Layer);
            Add(62, Color);

            Add(90, Vertices != null ? Vertices.Length : 0);
            Add(70, (int)LwpolylineFlags);
            Add(43, ConstantWidth);
            Add(38, Elevation);
            Add(39, Thickness);
 
            foreach(var vertex in Vertices)
            {
                Add(10, vertex.Coordinates.X);
                Add(20, vertex.Coordinates.Y);

                if (ConstantWidth == 0.0)
                {
                    Add(40, vertex.StartWidth);
                    Add(41, vertex.EndWidth);
                }

                Add(42, vertex.Bulge);
            }
    
            Add(210, ExtrusionDirection.X);
            Add(220, ExtrusionDirection.Y);
            Add(230, ExtrusionDirection.Z);

            return this;
        }
    }
}
