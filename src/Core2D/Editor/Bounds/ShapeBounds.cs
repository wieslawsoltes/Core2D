// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
using Core2D.Spatial;
using Core2D.Spatial.ConvexHull;
using Core2D.Spatial.Sat;
using Core2D.Shapes;
using static System.Math;

namespace Core2D.Editor.Bounds
{
    /// <summary>
    /// Calculate shape bounds.
    /// </summary>
    public static class ShapeBounds
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
            var a = new Point2(line.Start.X + dx, line.Start.Y + dy);
            var b = new Point2(line.End.X + dx, line.End.Y + dy);
            var target = new Point2(v.X, v.Y);
            var nearest = target.NearestOnLine(a, b);
            double distance = target.DistanceTo(nearest);
            return distance < threshold;
        }

        /// <summary>
        /// Get the bounding rectangle for <see cref="XPoint"/> shape.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static Rect2 GetPointBounds(XPoint point, double threshold, double dx, double dy)
        {
            double radius = threshold / 2.0;
            return new Rect2(
                point.X - radius + dx,
                point.Y - radius + dy,
                threshold,
                threshold);
        }

        /// <summary>
        /// Get the bounding rectangle for <see cref="XRectangle"/> shape.
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static Rect2 GetRectangleBounds(XRectangle rectangle, double dx, double dy)
        {
            return Rect2.FromPoints(
                rectangle.TopLeft.X,
                rectangle.TopLeft.Y,
                rectangle.BottomRight.X,
                rectangle.BottomRight.Y,
                dx, dy);
        }

        /// <summary>
        /// Get the bounding rectangle for <see cref="XEllipse"/> shape.
        /// </summary>
        /// <param name="ellipse"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static Rect2 GetEllipseBounds(XEllipse ellipse, double dx, double dy)
        {
            return Rect2.FromPoints(
                ellipse.TopLeft.X,
                ellipse.TopLeft.Y,
                ellipse.BottomRight.X,
                ellipse.BottomRight.Y,
                dx, dy);
        }

        /// <summary>
        /// Get the bounding rectangle for <see cref="XArc"/> shape.
        /// </summary>
        /// <param name="arc"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static Rect2 GetArcBounds(XArc arc, double dx, double dy)
        {
            double x1 = arc.Point1.X + dx;
            double y1 = arc.Point1.Y + dy;
            double x2 = arc.Point2.X + dx;
            double y2 = arc.Point2.Y + dy;

            double x0 = (x1 + x2) / 2.0;
            double y0 = (y1 + y2) / 2.0;

            double r = Sqrt((x1 - x0) * (x1 - x0) + (y1 - y0) * (y1 - y0));
            double x = x0 - r;
            double y = y0 - r;
            double width = 2.0 * r;
            double height = 2.0 * r;

            return new Rect2(x, y, width, height);
        }

        /// <summary>
        /// Get the bounding rectangle for <see cref="XText"/> shape.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static Rect2 GetTextBounds(XText text, double dx, double dy)
        {
            return Rect2.FromPoints(
                text.TopLeft.X,
                text.TopLeft.Y,
                text.BottomRight.X,
                text.BottomRight.Y,
                dx, dy);
        }

        /// <summary>
        /// Get the bounding rectangle for <see cref="XImage"/> shape.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static Rect2 GetImageBounds(XImage image, double dx, double dy)
        {
            return Rect2.FromPoints(
                image.TopLeft.X,
                image.TopLeft.Y,
                image.BottomRight.X,
                image.BottomRight.Y,
                dx, dy);
        }
    }
}
