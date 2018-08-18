// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Shapes;

namespace Core2D.Editor.Tools.Path.Shapes
{
    internal class PathShapeCubicBezier : PathShapeBase, ICubicBezierShape
    {
        public override Type TargetType => typeof(ICubicBezierShape);
        public IPointShape Point1 { get; set; }
        public IPointShape Point2 { get; set; }
        public IPointShape Point3 { get; set; }
        public IPointShape Point4 { get; set; }
    }
}
