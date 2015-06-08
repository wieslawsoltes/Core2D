// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    /// <summary>
    /// 
    /// </summary>
    public class DxfLwpolyline : DxfObject
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
        public DxfLwpolylineFlags LwpolylineFlags { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double ConstantWidth { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double Elevation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfLwpolylineVertex [] Vertices { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfVector3 ExtrusionDirection { get; set; }
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <param name="id"></param>
        public DxfLwpolyline(DxfAcadVer version, int id)
            : base(version, id)
        {
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void Defaults()
        {
            // TODO: Implement Defaults().
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string Create()
        {
            Reset();

            Add(0, DxfCodeName.Lwpolyline);

            Entity();

            if (Version > DxfAcadVer.AC1009)
                Subclass(DxfSubclassMarker.Lwpolyline);
            
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

            return Build();
        }
    }
}
