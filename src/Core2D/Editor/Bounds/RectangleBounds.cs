// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Math;
using Core2D.Shapes;
using static System.Math;

namespace Core2D.Editor.Bounds
{
    /// <summary>
    /// Calculate shape bounds using rectangles.
    /// </summary>
    public static class RectangleBounds
    {
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
            return Rect2.Create(rectangle.TopLeft, rectangle.BottomRight, dx, dy);
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
            return Rect2.Create(ellipse.TopLeft, ellipse.BottomRight, dx, dy);
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
            return Rect2.Create(text.TopLeft, text.BottomRight, dx, dy);
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
            return Rect2.Create(image.TopLeft, image.BottomRight, dx, dy);
        }
    }
}
