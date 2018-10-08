// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Spatial;

namespace Core2D.Shapes
{
    public static class EllipseShapeExtensions
    {
        public static Rect2 ToRect2(this EllipseShape ellipse, double dx = 0.0, double dy = 0.0)
        {
            return Rect2.FromPoints(
                ellipse.TopLeft.X, ellipse.TopLeft.Y,
                ellipse.BottomRight.X, ellipse.BottomRight.Y,
                dx, dy);
        }

        public static EllipseShape FromRect2(this Rect2 rect)
        {
            return new EllipseShape(rect.TopLeft.FromPoint2(), rect.BottomRight.FromPoint2());
        }
    }
}
