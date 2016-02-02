// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;

namespace Core2D
{
    /// <summary>
    /// 
    /// </summary>
    internal static class ConvexHullBounds
    {
        private static MonotoneChain mc = new MonotoneChain();
        private static SeparatingAxisTheorem sat = new SeparatingAxisTheorem();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="convexHull"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        private static bool Contains(double x, double y, Vector2[] convexHull, int k)
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
        /// <param name="point"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool Contains(IList<XPoint> points, Vector2 point, double dx, double dy)
        {
            Vector2[] vertices = new Vector2[points.Count];
            int k;
            Vector2[] convexHull;

            for (int i = 0; i < points.Count; i++)
            {
                vertices[i] = new Vector2(points[i].X + dx, points[i].Y + dy);
            }

            mc.ConvexHull(vertices, out convexHull, out k);

            return Contains(point.X, point.Y, convexHull, k);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cubicBezier"></param>
        /// <param name="point"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool Contains(XCubicBezier cubicBezier, Vector2 point, double dx, double dy)
        {
            Vector2[] vertices = new Vector2[4];
            int k;
            Vector2[] convexHull;

            vertices[0] = new Vector2(cubicBezier.Point1.X + dx, cubicBezier.Point1.Y + dy);
            vertices[1] = new Vector2(cubicBezier.Point2.X + dx, cubicBezier.Point2.Y + dy);
            vertices[2] = new Vector2(cubicBezier.Point3.X + dx, cubicBezier.Point3.Y + dy);
            vertices[3] = new Vector2(cubicBezier.Point4.X + dx, cubicBezier.Point4.Y + dy);

            mc.ConvexHull(vertices, out convexHull, out k);

            return Contains(point.X, point.Y, convexHull, k);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quadraticBezier"></param>
        /// <param name="point"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool Contains(XQuadraticBezier quadraticBezier, Vector2 point, double dx, double dy)
        {
            Vector2[] vertices = new Vector2[3];
            int k;
            Vector2[] convexHull;

            vertices[0] = new Vector2(quadraticBezier.Point1.X + dx, quadraticBezier.Point1.Y + dy);
            vertices[1] = new Vector2(quadraticBezier.Point2.X + dx, quadraticBezier.Point2.Y + dy);
            vertices[2] = new Vector2(quadraticBezier.Point3.X + dx, quadraticBezier.Point3.Y + dy);

            mc.ConvexHull(vertices, out convexHull, out k);

            return Contains(point.X, point.Y, convexHull, k);
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
            Vector2[] vertices = new Vector2[points.Count];
            int k;
            Vector2[] convexHull;

            for (int i = 0; i < points.Count; i++)
            {
                vertices[i] = new Vector2(points[i].X + dx, points[i].Y + dy);
            }

            mc.ConvexHull(vertices, out convexHull, out k);

            return convexHull.Take(k).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cubicBezier"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static Vector2[] GetVertices(XCubicBezier cubicBezier, double dx, double dy)
        {
            Vector2[] vertices = new Vector2[4];
            int k;
            Vector2[] convexHull;

            vertices[0] = new Vector2(cubicBezier.Point1.X + dx, cubicBezier.Point1.Y + dy);
            vertices[1] = new Vector2(cubicBezier.Point2.X + dx, cubicBezier.Point2.Y + dy);
            vertices[2] = new Vector2(cubicBezier.Point3.X + dx, cubicBezier.Point3.Y + dy);
            vertices[3] = new Vector2(cubicBezier.Point4.X + dx, cubicBezier.Point4.Y + dy);

            mc.ConvexHull(vertices, out convexHull, out k);

            return convexHull.Take(k).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quadraticBezier"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static Vector2[] GetVertices(XQuadraticBezier quadraticBezier, double dx, double dy)
        {
            Vector2[] vertices = new Vector2[3];
            int k;
            Vector2[] convexHull;

            vertices[0] = new Vector2(quadraticBezier.Point1.X + dx, quadraticBezier.Point1.Y + dy);
            vertices[1] = new Vector2(quadraticBezier.Point2.X + dx, quadraticBezier.Point2.Y + dy);
            vertices[2] = new Vector2(quadraticBezier.Point3.X + dx, quadraticBezier.Point3.Y + dy);

            mc.ConvexHull(vertices, out convexHull, out k);

            return convexHull.Take(k).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selection"></param>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static bool Overlap(Vector2[] selection, Vector2[] vertices)
        {
            return sat.Overlap(selection, vertices);
        }
    }
}
