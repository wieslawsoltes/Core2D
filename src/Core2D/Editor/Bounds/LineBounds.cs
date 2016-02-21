// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Math;
using Core2D.Shapes;

namespace Core2D.Editor.Bounds
{
    /// <summary>
    /// Calculate line shape bounds.
    /// </summary>
    public static class LineBounds
    {
        /// <summary>
        /// Checks if line contains point.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="v"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool Contains(XLine line, Vector2 v, double threshold, double dx, double dy)
        {
            var a = new Vector2(line.Start.X + dx, line.Start.Y + dy);
            var b = new Vector2(line.End.X + dx, line.End.Y + dy);
            var nearest = MathHelpers.NearestPointOnLine(a, b, v);
            double distance = MathHelpers.Distance(v.X, v.Y, nearest.X, nearest.Y);
            return distance < threshold;
        }
    }
}
