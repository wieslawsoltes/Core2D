// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    public class DxfLwpolylineVertex
    {
        public Vector2 Coordinates { get; private set; }
        public double StartWidth { get; private set; }
        public double EndWidth { get; private set; }
        public double Bulge { get; private set; }

        public DxfLwpolylineVertex(Vector2 coordinates, double startWidth, double endWidth, double bulge)
        {
            Coordinates = coordinates;
            StartWidth = startWidth;
            EndWidth = endWidth;
            Bulge = bulge;
        }
    }
}
