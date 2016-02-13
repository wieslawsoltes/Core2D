// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Math;
using Core2D.Project;
using Core2D.Shape;
using Core2D.Shapes;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Core2D.Editor.Bounds
{
    /// <summary>
    /// Hit test shapes using bounds.
    /// </summary>
    public static class ShapeHitTest
    {
        /// <summary>
        /// Hit test point in <see cref="BaseShape"/> shape bounds.
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="p"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static BaseShape HitTest(BaseShape shape, Vector2 p, double threshold, double dx, double dy)
        {
            if (shape is XPoint)
            {
                return HitTestPoint(shape, p, threshold, dx, dy);
            }
            else if (shape is XLine)
            {
                return HitTestLine(shape, p, threshold, dx, dy);
            }
            else if (shape is XRectangle)
            {
                return HitTestRectangle(shape, p, threshold, dx, dy);
            }
            else if (shape is XEllipse)
            {
                return HitTestEllipse(shape, p, threshold, dx, dy);
            }
            else if (shape is XArc)
            {
                return HitTestArc(shape, p, threshold, dx, dy);
            }
            else if (shape is XCubicBezier)
            {
                return HitTestCubicBezier(shape, p, threshold, dx, dy);
            }
            else if (shape is XQuadraticBezier)
            {
                return HitTestQuadraticBezier(shape, p, threshold, dx, dy);
            }
            else if (shape is XText)
            {
                return HitTestText(shape, p, threshold, dx, dy);
            }
            else if (shape is XImage)
            {
                return HitTestImage(shape, p, threshold, dx, dy);
            }
            else if (shape is XPath)
            {
                return HitTestPath(shape, p, threshold, dx, dy);
            }
            else if (shape is XGroup)
            {
                return HitTestGroup(shape, p, threshold, dx, dy);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="p"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static BaseShape HitTestPoint(BaseShape shape, Vector2 p, double threshold, double dx, double dy)
        {
            if (RectangleBounds.GetPointBounds(shape as XPoint, threshold, dx, dy).Contains(p))
            {
                return shape;
            }

            return null;
        }

        /// <summary>
        /// Hit test point in <see cref="XLine"/> shape bounds.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="p"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool HitTestLine(XLine line, Vector2 p, double threshold, double dx, double dy)
        {
            var a = new Vector2(line.Start.X + dx, line.Start.Y + dy);
            var b = new Vector2(line.End.X + dx, line.End.Y + dy);
            var nearest = MathHelpers.NearestPointOnLine(a, b, p);
            double distance = MathHelpers.Distance(p.X, p.Y, nearest.X, nearest.Y);
            return distance < threshold;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="p"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static BaseShape HitTestLine(BaseShape shape, Vector2 p, double threshold, double dx, double dy)
        {
            var line = shape as XLine;

            if (RectangleBounds.GetPointBounds(line.Start, threshold, dx, dy).Contains(p))
            {
                return line.Start;
            }

            if (RectangleBounds.GetPointBounds(line.End, threshold, dx, dy).Contains(p))
            {
                return line.End;
            }

            if (HitTestLine(line, p, threshold, dx, dy))
            {
                return line;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="p"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static BaseShape HitTestRectangle(BaseShape shape, Vector2 p, double threshold, double dx, double dy)
        {
            var rectangle = shape as XRectangle;

            if (RectangleBounds.GetPointBounds(rectangle.TopLeft, threshold, dx, dy).Contains(p))
            {
                return rectangle.TopLeft;
            }

            if (RectangleBounds.GetPointBounds(rectangle.BottomRight, threshold, dx, dy).Contains(p))
            {
                return rectangle.BottomRight;
            }

            if (RectangleBounds.GetRectangleBounds(rectangle, dx, dy).Contains(p))
            {
                return rectangle;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="p"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static BaseShape HitTestEllipse(BaseShape shape, Vector2 p, double threshold, double dx, double dy)
        {
            var ellipse = shape as XEllipse;

            if (RectangleBounds.GetPointBounds(ellipse.TopLeft, threshold, dx, dy).Contains(p))
            {
                return ellipse.TopLeft;
            }

            if (RectangleBounds.GetPointBounds(ellipse.BottomRight, threshold, dx, dy).Contains(p))
            {
                return ellipse.BottomRight;
            }

            if (RectangleBounds.GetEllipseBounds(ellipse, dx, dy).Contains(p))
            {
                return ellipse;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="p"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static BaseShape HitTestArc(BaseShape shape, Vector2 p, double threshold, double dx, double dy)
        {
            var arc = shape as XArc;

            if (RectangleBounds.GetPointBounds(arc.Point1, threshold, dx, dy).Contains(p))
            {
                return arc.Point1;
            }

            if (RectangleBounds.GetPointBounds(arc.Point2, threshold, dx, dy).Contains(p))
            {
                return arc.Point2;
            }

            if (RectangleBounds.GetPointBounds(arc.Point3, threshold, dx, dy).Contains(p))
            {
                return arc.Point3;
            }

            if (RectangleBounds.GetPointBounds(arc.Point4, threshold, dx, dy).Contains(p))
            {
                return arc.Point4;
            }

            if (RectangleBounds.GetArcBounds(arc, dx, dy).Contains(p))
            {
                return arc;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="p"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static BaseShape HitTestCubicBezier(BaseShape shape, Vector2 p, double threshold, double dx, double dy)
        {
            var cubicBezier = shape as XCubicBezier;

            if (RectangleBounds.GetPointBounds(cubicBezier.Point1, threshold, dx, dy).Contains(p))
            {
                return cubicBezier.Point1;
            }

            if (RectangleBounds.GetPointBounds(cubicBezier.Point2, threshold, dx, dy).Contains(p))
            {
                return cubicBezier.Point2;
            }

            if (RectangleBounds.GetPointBounds(cubicBezier.Point3, threshold, dx, dy).Contains(p))
            {
                return cubicBezier.Point3;
            }

            if (RectangleBounds.GetPointBounds(cubicBezier.Point4, threshold, dx, dy).Contains(p))
            {
                return cubicBezier.Point4;
            }

            if (ConvexHullBounds.Contains(cubicBezier.GetPoints().ToImmutableArray(), p, dx, dy))
            {
                return cubicBezier;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="p"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static BaseShape HitTestQuadraticBezier(BaseShape shape, Vector2 p, double threshold, double dx, double dy)
        {
            var quadraticBezier = shape as XQuadraticBezier;

            if (RectangleBounds.GetPointBounds(quadraticBezier.Point1, threshold, dx, dy).Contains(p))
            {
                return quadraticBezier.Point1;
            }

            if (RectangleBounds.GetPointBounds(quadraticBezier.Point2, threshold, dx, dy).Contains(p))
            {
                return quadraticBezier.Point2;
            }

            if (RectangleBounds.GetPointBounds(quadraticBezier.Point3, threshold, dx, dy).Contains(p))
            {
                return quadraticBezier.Point3;
            }

            if (ConvexHullBounds.Contains(quadraticBezier.GetPoints().ToImmutableArray(), p, dx, dy))
            {
                return quadraticBezier;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="p"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static BaseShape HitTestText(BaseShape shape, Vector2 p, double threshold, double dx, double dy)
        {
            var text = shape as XText;

            if (RectangleBounds.GetPointBounds(text.TopLeft, threshold, dx, dy).Contains(p))
            {
                return text.TopLeft;
            }

            if (RectangleBounds.GetPointBounds(text.BottomRight, threshold, dx, dy).Contains(p))
            {
                return text.BottomRight;
            }

            if (RectangleBounds.GetTextBounds(text, dx, dy).Contains(p))
            {
                return text;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="p"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static BaseShape HitTestImage(BaseShape shape, Vector2 p, double threshold, double dx, double dy)
        {
            var image = shape as XImage;

            if (RectangleBounds.GetPointBounds(image.TopLeft, threshold, dx, dy).Contains(p))
            {
                return image.TopLeft;
            }

            if (RectangleBounds.GetPointBounds(image.BottomRight, threshold, dx, dy).Contains(p))
            {
                return image.BottomRight;
            }

            if (RectangleBounds.GetImageBounds(image, dx, dy).Contains(p))
            {
                return image;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="p"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static BaseShape HitTestPath(BaseShape shape, Vector2 p, double threshold, double dx, double dy)
        {
            var path = shape as XPath;

            if (path.Geometry != null)
            {
                var points = path.GetPoints().ToImmutableArray();
                foreach (var point in points)
                {
                    if (RectangleBounds.GetPointBounds(point, threshold, dx, dy).Contains(p))
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="p"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static BaseShape HitTestGroup(BaseShape shape, Vector2 p, double threshold, double dx, double dy)
        {
            var group = shape as XGroup;

            foreach (var connector in group.Connectors.Reverse())
            {
                if (RectangleBounds.GetPointBounds(connector, threshold, dx, dy).Contains(p))
                {
                    return connector;
                }
            }

            var result = HitTest(group.Shapes.Reverse(), p, threshold, dx, dy);
            if (result != null)
            {
                return shape;
            }

            return null;
        }

        /// <summary>
        /// Hit test point in <see cref="BaseShape"/> shapes bounds.
        /// </summary>
        /// <param name="shapes"></param>
        /// <param name="p"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static BaseShape HitTest(IEnumerable<BaseShape> shapes, Vector2 p, double threshold, double dx, double dy)
        {
            foreach (var shape in shapes)
            {
                var result = HitTest(shape, p, threshold, dx, dy);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Hit test point in <see cref="XContainer"/> shapes bounds.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="p"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static BaseShape HitTest(XContainer container, Vector2 p, double threshold)
        {
            var result = HitTest(container.CurrentLayer.Shapes.Reverse(), p, threshold, 0, 0);
            if (result != null)
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// Hit test rectangle in <see cref="XContainer"/> shapes bounds.
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="rect"></param>
        /// <param name="selection"></param>
        /// <param name="selected"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool HitTest(BaseShape shape, Rect2 rect, Vector2[] selection, ISet<BaseShape> selected, double threshold, double dx, double dy)
        {
            if (shape is XPoint)
            {
                return HitTestPoint(shape, rect, selected, threshold, dx, dy);
            }
            else if (shape is XLine)
            {
                return HitTestLine(shape, rect, selected, threshold, dx, dy);
            }
            else if (shape is XEllipse)
            {
                return HitTestEllipse(shape, rect, selected, dx, dy);
            }
            else if (shape is XRectangle)
            {
                return HitTestRectangle(shape, rect, selected, dx, dy);
            }
            else if (shape is XArc)
            {
                return HitTestArc(shape, rect, selected, dx, dy);
            }
            else if (shape is XCubicBezier)
            {
                return HitTestCubicBezier(shape, selection, selected, dx, dy);
            }
            else if (shape is XQuadraticBezier)
            {
                return HitTestQadraticBezier(shape, selection, selected, dx, dy);
            }
            else if (shape is XText)
            {
                return HitTestText(shape, rect, selected, dx, dy);
            }
            else if (shape is XImage)
            {
                return HitTestImage(shape, rect, selected, dx, dy);
            }
            else if (shape is XPath)
            {
                return HitTestPath(shape, selection, selected, dx, dy);
            }
            else if (shape is XGroup)
            {
                return HitTestGroup(shape, rect, selection, selected, threshold, dx, dy);
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="rect"></param>
        /// <param name="selected"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool HitTestPoint(BaseShape shape, Rect2 rect, ISet<BaseShape> selected, double threshold, double dx, double dy)
        {
            if (RectangleBounds.GetPointBounds(shape as XPoint, threshold, dx, dy).IntersectsWith(rect))
            {
                if (selected != null)
                {
                    selected.Add(shape);
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="rect"></param>
        /// <param name="selected"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool HitTestLine(BaseShape shape, Rect2 rect, ISet<BaseShape> selected, double threshold, double dx, double dy)
        {
            var line = shape as XLine;
            if (RectangleBounds.GetPointBounds(line.Start, threshold, dx, dy).IntersectsWith(rect)
                || RectangleBounds.GetPointBounds(line.End, threshold, dx, dy).IntersectsWith(rect)
                || MathHelpers.LineIntersectsWithRect(rect, new Point2(line.Start.X, line.Start.Y), new Point2(line.End.X, line.End.Y)))
            {
                if (selected != null)
                {
                    selected.Add(line);
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="rect"></param>
        /// <param name="selected"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool HitTestEllipse(BaseShape shape, Rect2 rect, ISet<BaseShape> selected, double dx, double dy)
        {
            if (RectangleBounds.GetEllipseBounds(shape as XEllipse, dx, dy).IntersectsWith(rect))
            {
                if (selected != null)
                {
                    selected.Add(shape);
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="rect"></param>
        /// <param name="selected"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool HitTestRectangle(BaseShape shape, Rect2 rect, ISet<BaseShape> selected, double dx, double dy)
        {
            if (RectangleBounds.GetRectangleBounds(shape as XRectangle, dx, dy).IntersectsWith(rect))
            {
                if (selected != null)
                {
                    selected.Add(shape);
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="rect"></param>
        /// <param name="selected"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool HitTestArc(BaseShape shape, Rect2 rect, ISet<BaseShape> selected, double dx, double dy)
        {
            if (RectangleBounds.GetArcBounds(shape as XArc, dx, dy).IntersectsWith(rect))
            {
                if (selected != null)
                {
                    selected.Add(shape);
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="selection"></param>
        /// <param name="selected"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool HitTestCubicBezier(BaseShape shape, Vector2[] selection, ISet<BaseShape> selected, double dx, double dy)
        {
            var points = shape.GetPoints().ToImmutableArray();
            if (ConvexHullBounds.Overlap(selection, points, dx, dy))
            {
                if (selected != null)
                {
                    selected.Add(shape);
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="selection"></param>
        /// <param name="selected"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool HitTestQadraticBezier(BaseShape shape, Vector2[] selection, ISet<BaseShape> selected, double dx, double dy)
        {
            var points = shape.GetPoints().ToImmutableArray();
            if (ConvexHullBounds.Overlap(selection, points, dx, dy))
            {
                if (selected != null)
                {
                    selected.Add(shape);
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="rect"></param>
        /// <param name="selected"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool HitTestText(BaseShape shape, Rect2 rect, ISet<BaseShape> selected, double dx, double dy)
        {
            if (RectangleBounds.GetTextBounds(shape as XText, dx, dy).IntersectsWith(rect))
            {
                if (selected != null)
                {
                    selected.Add(shape);
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="rect"></param>
        /// <param name="selected"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool HitTestImage(BaseShape shape, Rect2 rect, ISet<BaseShape> selected, double dx, double dy)
        {
            if (RectangleBounds.GetImageBounds(shape as XImage, dx, dy).IntersectsWith(rect))
            {
                if (selected != null)
                {
                    selected.Add(shape);
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="selection"></param>
        /// <param name="selected"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool HitTestPath(BaseShape shape, Vector2[] selection, ISet<BaseShape> selected, double dx, double dy)
        {
            if ((shape as XPath).Geometry != null)
            {
                var points = shape.GetPoints().ToImmutableArray();
                if (ConvexHullBounds.Overlap(selection, points, dx, dy))
                {
                    if (selected != null)
                    {
                        selected.Add(shape);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="rect"></param>
        /// <param name="selection"></param>
        /// <param name="selected"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool HitTestGroup(BaseShape shape, Rect2 rect, Vector2[] selection, ISet<BaseShape> selected, double threshold, double dx, double dy)
        {
            if (HitTest((shape as XGroup).Shapes.Reverse(), rect, selection, null, threshold, dx, dy) == true)
            {
                if (selected != null)
                {
                    selected.Add(shape);
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Hit test rectangle if intersects with any <see cref="BaseShape"/> shape bounds.
        /// </summary>
        /// <param name="shapes"></param>
        /// <param name="rect"></param>
        /// <param name="selection"></param>
        /// <param name="selected"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool HitTest(IEnumerable<BaseShape> shapes, Rect2 rect, Vector2[] selection, ISet<BaseShape> selected, double threshold, double dx, double dy)
        {
            foreach (var shape in shapes)
            {
                var result = HitTest(shape, rect, selection, selected, threshold, dx, dy);
                if (result == true)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Hit test rectangle if intersects with any <see cref="XContainer"/> shape bounds.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="rect"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static ImmutableHashSet<BaseShape> HitTest(XContainer container, Rect2 rect, double threshold)
        {
            var selected = ImmutableHashSet.CreateBuilder<BaseShape>();

            var selection = new Vector2[]
            {
                new Vector2(rect.X, rect.Y),
                new Vector2(rect.X + rect.Width, rect.Y),
                new Vector2(rect.X + rect.Width, rect.Y + rect.Height),
                new Vector2(rect.X, rect.Y + rect.Height)
            };

            HitTest(container.CurrentLayer.Shapes.Reverse(), rect, selection, selected, threshold, 0, 0);

            return selected.ToImmutableHashSet();
        }
    }
}
