// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Shapes;

namespace Core2D.Editor.Tools.Path.Shapes
{
    internal class PathShapeQuadraticBezier : PathShapeBase, IQuadraticBezierShape
    {
        public IPointShape Point1 { get; set; }
        public IPointShape Point2 { get; set; }
        public IPointShape Point3 { get; set; }
    }
}
