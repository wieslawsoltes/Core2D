// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;

namespace Test2d
{
    internal static class ConvexHullBounds
    {
        private static MonotoneChain mc = new MonotoneChain();
        private static SeparatingAxisTheorem sat = new SeparatingAxisTheorem();

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

        public static bool Contains(XBezier bezier, Vector2 point)
        {
            Vector2[] vertices = new Vector2[4];
            int k;
            Vector2[] convexHull;

            vertices[0] = new Vector2(bezier.Point1.X, bezier.Point1.Y);
            vertices[1] = new Vector2(bezier.Point2.X, bezier.Point2.Y);
            vertices[2] = new Vector2(bezier.Point3.X, bezier.Point3.Y);
            vertices[3] = new Vector2(bezier.Point4.X, bezier.Point4.Y);

            mc.ConvexHull(vertices, out convexHull, out k);

            return Contains(point.X, point.Y, convexHull, k);
        }

        public static bool Contains(XQBezier qbezier, Vector2 point)
        {
            Vector2[] vertices = new Vector2[3];
            int k;
            Vector2[] convexHull;

            vertices[0] = new Vector2(qbezier.Point1.X, qbezier.Point1.Y);
            vertices[1] = new Vector2(qbezier.Point2.X, qbezier.Point2.Y);
            vertices[2] = new Vector2(qbezier.Point3.X, qbezier.Point3.Y);

            mc.ConvexHull(vertices, out convexHull, out k);

            return Contains(point.X, point.Y, convexHull, k);
        }

        public static Vector2[] GetVertices(XBezier bezier)
        {
            Vector2[] vertices = new Vector2[4];
            int k;
            Vector2[] convexHull;

            vertices[0] = new Vector2(bezier.Point1.X, bezier.Point1.Y);
            vertices[1] = new Vector2(bezier.Point2.X, bezier.Point2.Y);
            vertices[2] = new Vector2(bezier.Point3.X, bezier.Point3.Y);
            vertices[3] = new Vector2(bezier.Point4.X, bezier.Point4.Y);

            mc.ConvexHull(vertices, out convexHull, out k);

            return convexHull.Take(k).ToArray();
        }

        public static Vector2[] GetVertices(XQBezier qbezier)
        {
            Vector2[] vertices = new Vector2[3];
            int k;
            Vector2[] convexHull;

            vertices[0] = new Vector2(qbezier.Point1.X, qbezier.Point1.Y);
            vertices[1] = new Vector2(qbezier.Point2.X, qbezier.Point2.Y);
            vertices[2] = new Vector2(qbezier.Point3.X, qbezier.Point3.Y);

            mc.ConvexHull(vertices, out convexHull, out k);

            return convexHull.Take(k).ToArray();
        }

        public static bool Overlap(Vector2[] selection, Vector2[] vertices)
        {
            return sat.Overlap(selection, vertices);
        }
    }
}
