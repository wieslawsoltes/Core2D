// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Shape;
using Core2D.Style;
using Spatial;
using static System.Math;

namespace Core2D.Shapes
{
    /// <summary>
    /// Point shape extension methods.
    /// </summary>
    public static class PointShapeExtentions
    {
        /// <summary>
        /// Calculates distance between points.
        /// </summary>
        /// <param name="point">The point instance.</param>
        /// <param name="other">The other point.</param>
        /// <returns>The distance between points.</returns>
        public static double DistanceTo(this PointShape point, PointShape other)
        {
            double dx = point.X - other.X;
            double dy = point.Y - other.Y;
            return Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Creates a new <see cref="PointShape"/> instance.
        /// </summary>
        /// <param name="point">The source point.</param>
        /// <returns>The new instance of the <see cref="PointShape"/> class.</returns>
        public static PointShape AsPointShape(this Point2 point)
        {
            return new PointShape()
            {
                Name = "",
                Style = default(ShapeStyle),
                X = point.X,
                Y = point.Y,
                Alignment = PointAlignment.None,
                Shape = null
            };
        }
    }
}
