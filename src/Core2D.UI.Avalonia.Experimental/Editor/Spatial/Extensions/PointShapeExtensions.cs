// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Shape;
using Spatial;

namespace Core2D.Shapes
{
    public static class PointShapeExtensions
    {
        public static Point2 ToPoint2(this PointShape point)
        {
            return new Point2(point.X, point.Y);
        }

        public static PointShape FromPoint2(this Point2 point, BaseShape template = null)
        {
            return new PointShape(point.X, point.Y, template);
        }
    }
}
