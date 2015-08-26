// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Test2d
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
        /// <param name="bezier"></param>
        /// <param name="point"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool Contains(XBezier bezier, Vector2 point, double dx, double dy)
        {
            Vector2[] vertices = new Vector2[4];
            int k;
            Vector2[] convexHull;

            vertices[0] = new Vector2(bezier.Point1.X + dx, bezier.Point1.Y + dy);
            vertices[1] = new Vector2(bezier.Point2.X + dx, bezier.Point2.Y + dy);
            vertices[2] = new Vector2(bezier.Point3.X + dx, bezier.Point3.Y + dy);
            vertices[3] = new Vector2(bezier.Point4.X + dx, bezier.Point4.Y + dy);

            mc.ConvexHull(vertices, out convexHull, out k);

            return Contains(point.X, point.Y, convexHull, k);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qbezier"></param>
        /// <param name="point"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool Contains(XQBezier qbezier, Vector2 point, double dx, double dy)
        {
            Vector2[] vertices = new Vector2[3];
            int k;
            Vector2[] convexHull;

            vertices[0] = new Vector2(qbezier.Point1.X + dx, qbezier.Point1.Y + dy);
            vertices[1] = new Vector2(qbezier.Point2.X + dx, qbezier.Point2.Y + dy);
            vertices[2] = new Vector2(qbezier.Point3.X + dx, qbezier.Point3.Y + dy);

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
        /// <param name="bezier"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static Vector2[] GetVertices(XBezier bezier, double dx, double dy)
        {
            Vector2[] vertices = new Vector2[4];
            int k;
            Vector2[] convexHull;

            vertices[0] = new Vector2(bezier.Point1.X + dx, bezier.Point1.Y + dy);
            vertices[1] = new Vector2(bezier.Point2.X + dx, bezier.Point2.Y + dy);
            vertices[2] = new Vector2(bezier.Point3.X + dx, bezier.Point3.Y + dy);
            vertices[3] = new Vector2(bezier.Point4.X + dx, bezier.Point4.Y + dy);

            mc.ConvexHull(vertices, out convexHull, out k);

            return convexHull.Take(k).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qbezier"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static Vector2[] GetVertices(XQBezier qbezier, double dx, double dy)
        {
            Vector2[] vertices = new Vector2[3];
            int k;
            Vector2[] convexHull;

            vertices[0] = new Vector2(qbezier.Point1.X + dx, qbezier.Point1.Y + dy);
            vertices[1] = new Vector2(qbezier.Point2.X + dx, qbezier.Point2.Y + dy);
            vertices[2] = new Vector2(qbezier.Point3.X + dx, qbezier.Point3.Y + dy);

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
