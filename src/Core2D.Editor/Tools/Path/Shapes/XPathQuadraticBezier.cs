// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Shapes.Interfaces;

namespace Core2D.Editor.Tools.Path.Shapes
{
    internal class PathShapeQuadraticBezier : IQuadraticBezierShape
    {
        public PointShape Point1 { get; set; }
        public PointShape Point2 { get; set; }
        public PointShape Point3 { get; set; }
    }
}
