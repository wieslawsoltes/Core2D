// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public static class ShapeBounds
    {
        #region Bounds

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="treshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static Rect2 GetPointBounds(XPoint point, double treshold, double dx, double dy)
        {
            double radius = treshold / 2.0;
            return new Rect2(
                point.X - radius + dx, 
                point.Y - radius + dy, 
                treshold, 
                treshold);
        }

        /// <summary>
        /// 
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
        /// 
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
        /// 
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

            double r = Math.Sqrt((x1 - x0) * (x1 - x0) + (y1 - y0) * (y1 - y0));
            double x = x0 - r;
            double y = y0 - r;
            double width = 2.0 * r;
            double height = 2.0 * r;

            return new Rect2(x, y, width, height);
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static Rect2 GetImageBounds(XImage image, double dx, double dy)
        {
            return Rect2.Create(image.TopLeft, image.BottomRight, dx, dy);
        }

        #endregion

        #region HitTest Point

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="p"></param>
        /// <param name="treshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool HitTestLine(XLine line, Vector2 p, double treshold, double dx, double dy)
        {
            var a = new Vector2(line.Start.X + dx, line.Start.Y + dy);
            var b = new Vector2(line.End.X + dx, line.End.Y + dy);
            var nearest = MathHelpers.NearestPointOnLine(a, b, p);
            double distance = MathHelpers.Distance(p.X, p.Y, nearest.X, nearest.Y);
            return distance < treshold;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="p"></param>
        /// <param name="treshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static BaseShape HitTest(BaseShape shape, Vector2 p, double treshold, double dx, double dy)
        {
            if (shape is XPoint)
            {
                if (GetPointBounds(shape as XPoint, treshold, dx, dy).Contains(p))
                {
                    return shape;
                }
                return null;
            }
            else if (shape is XLine)
            {
                var line = shape as XLine;

                if (GetPointBounds(line.Start, treshold, dx, dy).Contains(p))
                {
                    return line.Start;
                }

                if (GetPointBounds(line.End, treshold, dx, dy).Contains(p))
                {
                    return line.End;
                }

                if (HitTestLine(line, p, treshold, dx, dy))
                {
                    return line;
                }

                return null;
            }
            else if (shape is XRectangle)
            {
                var rectangle = shape as XRectangle;

                if (GetPointBounds(rectangle.TopLeft, treshold, dx, dy).Contains(p))
                {
                    return rectangle.TopLeft;
                }

                if (GetPointBounds(rectangle.BottomRight, treshold, dx, dy).Contains(p))
                {
                    return rectangle.BottomRight;
                }

                if (GetRectangleBounds(rectangle, dx, dy).Contains(p))
                {
                    return rectangle;
                }
                return null;
            }
            else if (shape is XEllipse)
            {
                var ellipse = shape as XEllipse;

                if (GetPointBounds(ellipse.TopLeft, treshold, dx, dy).Contains(p))
                {
                    return ellipse.TopLeft;
                }

                if (GetPointBounds(ellipse.BottomRight, treshold, dx, dy).Contains(p))
                {
                    return ellipse.BottomRight;
                }

                if (GetEllipseBounds(ellipse, dx, dy).Contains(p))
                {
                    return ellipse;
                }
                return null;
            }
            else if (shape is XArc)
            {
                var arc = shape as XArc;

                if (GetPointBounds(arc.Point1, treshold, dx, dy).Contains(p))
                {
                    return arc.Point1;
                }

                if (GetPointBounds(arc.Point2, treshold, dx, dy).Contains(p))
                {
                    return arc.Point2;
                }

                if (GetPointBounds(arc.Point3, treshold, dx, dy).Contains(p))
                {
                    return arc.Point3;
                }

                if (GetPointBounds(arc.Point4, treshold, dx, dy).Contains(p))
                {
                    return arc.Point4;
                }

                if (GetArcBounds(arc, dx, dy).Contains(p))
                {
                    return arc;
                }
                return null;
            }
            else if (shape is XBezier)
            {
                var bezier = shape as XBezier;

                if (GetPointBounds(bezier.Point1, treshold, dx, dy).Contains(p))
                {
                    return bezier.Point1;
                }

                if (GetPointBounds(bezier.Point2, treshold, dx, dy).Contains(p))
                {
                    return bezier.Point2;
                }

                if (GetPointBounds(bezier.Point3, treshold, dx, dy).Contains(p))
                {
                    return bezier.Point3;
                }

                if (GetPointBounds(bezier.Point4, treshold, dx, dy).Contains(p))
                {
                    return bezier.Point4;
                }

                if (ConvexHullBounds.Contains(bezier, p, dx, dy))
                {
                    return bezier;
                }
                return null;
            }
            else if (shape is XQBezier)
            {
                var qbezier = shape as XQBezier;

                if (GetPointBounds(qbezier.Point1, treshold, dx, dy).Contains(p))
                {
                    return qbezier.Point1;
                }

                if (GetPointBounds(qbezier.Point2, treshold, dx, dy).Contains(p))
                {
                    return qbezier.Point2;
                }

                if (GetPointBounds(qbezier.Point3, treshold, dx, dy).Contains(p))
                {
                    return qbezier.Point3;
                }

                if (ConvexHullBounds.Contains(qbezier, p, dx, dy))
                {
                    return qbezier;
                }
                return null;
            }
            else if (shape is XText)
            {
                var text = shape as XText;

                if (GetPointBounds(text.TopLeft, treshold, dx, dy).Contains(p))
                {
                    return text.TopLeft;
                }

                if (GetPointBounds(text.BottomRight, treshold, dx, dy).Contains(p))
                {
                    return text.BottomRight;
                }

                if (GetTextBounds(text, dx, dy).Contains(p))
                {
                    return text;
                }
                return null;
            }
            else if (shape is XImage)
            {
                var image = shape as XImage;

                if (GetPointBounds(image.TopLeft, treshold, dx, dy).Contains(p))
                {
                    return image.TopLeft;
                }

                if (GetPointBounds(image.BottomRight, treshold, dx, dy).Contains(p))
                {
                    return image.BottomRight;
                }

                if (GetImageBounds(image, dx, dy).Contains(p))
                {
                    return image;
                }
                return null;
            }
            else if (shape is XPath)
            {
                var path = shape as XPath;

                if (path.Geometry != null)
                {
                    var points = path.GetAllPoints();
                    foreach (var point in points)
                    {
                        if (GetPointBounds(point, treshold, dx, dy).Contains(p))
                        {
                            return point;
                        }
                    }

                    if (ConvexHullBounds.Contains(points, p, dx, dy))
                    {
                        return path;
                    }
                }
                return null;
            }
            else if (shape is XGroup)
            {
                var group = shape as XGroup;

                foreach (var connector in group.Connectors.Reverse())
                {
                    if (GetPointBounds(connector, treshold, dx, dy).Contains(p))
                    {
                        return connector;
                    }
                }

                var result = HitTest(group.Shapes.Reverse(), p, treshold, dx, dy);
                if (result != null)
                {
                    return shape;
                }
                return null;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapes"></param>
        /// <param name="p"></param>
        /// <param name="treshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static BaseShape HitTest(IEnumerable<BaseShape> shapes, Vector2 p, double treshold, double dx, double dy)
        {
            foreach (var shape in shapes)
            {
                var result = HitTest(shape, p, treshold, dx, dy);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="p"></param>
        /// <param name="treshold"></param>
        /// <returns></returns>
        public static BaseShape HitTest(Container container, Vector2 p, double treshold)
        {
            var result = HitTest(container.CurrentLayer.Shapes.Reverse(), p, treshold, 0, 0);
            if (result != null)
            {
                return result;
            }

            return null;
        }
      
        #endregion

        #region HitTest Rect

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="rect"></param>
        /// <param name="selection"></param>
        /// <param name="builder"></param>
        /// <param name="treshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        private static bool HitTest(
            BaseShape shape, 
            Rect2 rect, 
            Vector2[] selection, 
            ImmutableHashSet<BaseShape>.Builder builder, 
            double treshold,
            double dx,
            double dy)
        {
            if (shape is XPoint)
            {
                if (GetPointBounds(shape as XPoint, treshold, dx, dy).IntersectsWith(rect))
                {
                    if (builder != null)
                    {
                        builder.Add(shape);
                    }
                    else
                    {
                        return true;
                    }
                }
                return false;
            }
            else if (shape is XLine)
            {
                var line = shape as XLine;
                if (GetPointBounds(line.Start, treshold, dx, dy).IntersectsWith(rect)
                    || GetPointBounds(line.End, treshold, dx, dy).IntersectsWith(rect)
                    || MathHelpers.LineIntersectsWithRect(rect, new Point2(line.Start.X, line.Start.Y), new Point2(line.End.X, line.End.Y)))
                {
                    if (builder != null)
                    {
                        builder.Add(line);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                return false;
            }
            else if (shape is XEllipse)
            {
                if (GetEllipseBounds(shape as XEllipse, dx, dy).IntersectsWith(rect))
                {
                    if (builder != null)
                    {
                        builder.Add(shape);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                return false;
            }
            else if (shape is XRectangle)
            {
                if (GetRectangleBounds(shape as XRectangle, dx, dy).IntersectsWith(rect))
                {
                    if (builder != null)
                    {
                        builder.Add(shape);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                return false;
            }
            else if (shape is XArc)
            {
                if (GetArcBounds(shape as XArc, dx, dy).IntersectsWith(rect))
                {
                    if (builder != null)
                    {
                        builder.Add(shape);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                return false;
            }
            else if (shape is XBezier)
            {
                if (ConvexHullBounds.Overlap(selection, ConvexHullBounds.GetVertices(shape as XBezier, dx, dy)))
                {
                    if (builder != null)
                    {
                        builder.Add(shape);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                return false;
            }
            else if (shape is XQBezier)
            {
                if (ConvexHullBounds.Overlap(selection, ConvexHullBounds.GetVertices(shape as XQBezier, dx, dy)))
                {
                    if (builder != null)
                    {
                        builder.Add(shape);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                return false;
            }
            else if (shape is XText)
            {
                if (GetTextBounds(shape as XText, dx, dy).IntersectsWith(rect))
                {
                    if (builder != null)
                    {
                        builder.Add(shape);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                return false;
            }
            else if (shape is XImage)
            {
                if (GetImageBounds(shape as XImage, dx, dy).IntersectsWith(rect))
                {
                    if (builder != null)
                    {
                        builder.Add(shape);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                return false;
            }
            else if (shape is XPath)
            {
                if ((shape as XPath).Geometry != null)
                {
                    var points = (shape as XPath).GetAllPoints();
                    if (ConvexHullBounds.Overlap(selection, ConvexHullBounds.GetVertices(points, dx, dy)))
                    {
                        if (builder != null)
                        {
                            builder.Add(shape);
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            else if (shape is XGroup)
            {
                if (HitTest((shape as XGroup).Shapes.Reverse(), rect, selection, null, treshold, dx, dy) == true)
                {
                    if (builder != null)
                    {
                        builder.Add(shape);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                return false;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapes"></param>
        /// <param name="rect"></param>
        /// <param name="selection"></param>
        /// <param name="builder"></param>
        /// <param name="treshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        private static bool HitTest(
            IEnumerable<BaseShape> shapes, 
            Rect2 rect, 
            Vector2[] selection, 
            ImmutableHashSet<BaseShape>.Builder builder, 
            double treshold,
            double dx,
            double dy)
        {
            foreach (var shape in shapes)
            {
                var result = HitTest(shape, rect, selection, builder, treshold, dx, dy);
                if (result == true)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="rect"></param>
        /// <param name="treshold"></param>
        /// <returns></returns>
        public static ImmutableHashSet<BaseShape> HitTest(Container container, Rect2 rect, double treshold)
        {
            var builder = ImmutableHashSet.CreateBuilder<BaseShape>();

            var selection = new Vector2[]
            {
                new Vector2(rect.X, rect.Y),
                new Vector2(rect.X + rect.Width, rect.Y),
                new Vector2(rect.X + rect.Width, rect.Y + rect.Height),
                new Vector2(rect.X, rect.Y + rect.Height)
            };

            HitTest(container.CurrentLayer.Shapes.Reverse(), rect, selection, builder, treshold, 0, 0);

            return builder.ToImmutableHashSet();
        }

        #endregion
    }
}
