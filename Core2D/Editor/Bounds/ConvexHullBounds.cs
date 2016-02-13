// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Math;
using Core2D.Math.ConvexHull;
using Core2D.Math.Sat;
using Core2D.Shapes;
using System.Collections.Generic;
using System.Linq;

namespace Core2D.Editor.Bounds
{
    /// <summary>
    /// Calculate shape bounds using convex hulls.
    /// </summary>
    public static class ConvexHullBounds
    {
        private static MonotoneChain mc = new MonotoneChain();
        private static SeparatingAxisTheorem sat = new SeparatingAxisTheorem();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="k"></param>
        /// <param name="convexHull"></param>
        public static void ToConvexHull(IList<XPoint> points, double dx, double dy, out int k, out Vector2[] convexHull)
        {
            Vector2[] vertices = new Vector2[points.Count];

            for (int i = 0; i < points.Count; i++)
            {
                vertices[i] = new Vector2(points[i].X + dx, points[i].Y + dy);
            }

            mc.ConvexHull(vertices, out convexHull, out k);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="convexHull"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static bool Contains(double x, double y, Vector2[] convexHull, int k)
        {
            bool contains = false;
            for (int i = 0, j = k - 2; i < k - 1; j = i++)
            {
                if (((convexHull[i].Y > y) != (convexHull[j].Y > y))
                    && (x < (convexHull[j].X - convexHull[i].X) * (y - convexHull[i].Y) / (convexHull[j].Y - convexHull[i].Y) + convexHull[i].X))
                {
                    contains = !contains;
                }
            }
            return contains;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="v"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool Contains(IList<XPoint> points, Vector2 v, double dx, double dy)
        {
            int k;
            Vector2[] convexHull;
            ToConvexHull(points, dx, dy, out k, out convexHull);
            return Contains(v.X, v.Y, convexHull, k);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static Vector2[] GetVertices(IList<XPoint> points, double dx, double dy)
        {
            int k;
            Vector2[] convexHull;
            ToConvexHull(points, dx, dy, out k, out convexHull);
            return convexHull.Take(k).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selection"></param>
        /// <param name="points"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool Overlap(Vector2[] selection, IList<XPoint> points, double dx, double dy)
        {
            Vector2[] vertices = GetVertices(points, dx, dy);
            return sat.Overlap(selection, vertices);
        }
    }
}
