// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
        public static double DistanceTo(this IPointShape point, IPointShape other)
        {
            double dx = point.X - other.X;
            double dy = point.Y - other.Y;
            return Sqrt(dx * dx + dy * dy);
        }
    }
}
