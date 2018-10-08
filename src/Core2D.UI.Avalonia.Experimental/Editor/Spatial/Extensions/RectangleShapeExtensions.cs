// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Spatial;

namespace Core2D.Shapes
{
    public static class RectangleShapeExtensions
    {
        public static Rect2 ToRect2(this RectangleShape rectangle, double dx = 0.0, double dy = 0.0)
        {
            return Rect2.FromPoints(
                rectangle.TopLeft.X, rectangle.TopLeft.Y, 
                rectangle.BottomRight.X, rectangle.BottomRight.Y, 
                dx, dy);
        }

        public static RectangleShape FromRect2(this Rect2 rect)
        {
            return new RectangleShape(rect.TopLeft.FromPoint2(), rect.BottomRight.FromPoint2());
        }
    }
}
