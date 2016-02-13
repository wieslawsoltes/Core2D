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
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="v"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static BaseShape HitTestPoint(XPoint point, Vector2 v, double threshold, double dx, double dy)
        {
            if (RectangleBounds.GetPointBounds(point, threshold, dx, dy).Contains(v))
            {
                return point;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="v"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static BaseShape HitTestLine(XLine line, Vector2 v, double threshold, double dx, double dy)
        {
            if (RectangleBounds.GetPointBounds(line.Start, threshold, dx, dy).Contains(v))
            {
                return line.Start;
            }

            if (RectangleBounds.GetPointBounds(line.End, threshold, dx, dy).Contains(v))
            {
                return line.End;
            }

            if (LineBounds.Contains(line, v, threshold, dx, dy))
            {
                return line;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="v"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static BaseShape HitTestRectangle(XRectangle rectangle, Vector2 v, double threshold, double dx, double dy)
        {
            if (RectangleBounds.GetPointBounds(rectangle.TopLeft, threshold, dx, dy).Contains(v))
            {
                return rectangle.TopLeft;
            }

            if (RectangleBounds.GetPointBounds(rectangle.BottomRight, threshold, dx, dy).Contains(v))
            {
                return rectangle.BottomRight;
            }

            if (RectangleBounds.GetRectangleBounds(rectangle, dx, dy).Contains(v))
            {
                return rectangle;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ellipse"></param>
        /// <param name="v"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static BaseShape HitTestEllipse(XEllipse ellipse, Vector2 v, double threshold, double dx, double dy)
        {
            if (RectangleBounds.GetPointBounds(ellipse.TopLeft, threshold, dx, dy).Contains(v))
            {
                return ellipse.TopLeft;
            }

            if (RectangleBounds.GetPointBounds(ellipse.BottomRight, threshold, dx, dy).Contains(v))
            {
                return ellipse.BottomRight;
            }

            if (RectangleBounds.GetEllipseBounds(ellipse, dx, dy).Contains(v))
            {
                return ellipse;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arc"></param>
        /// <param name="v"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static BaseShape HitTestArc(XArc arc, Vector2 v, double threshold, double dx, double dy)
        {
            if (RectangleBounds.GetPointBounds(arc.Point1, threshold, dx, dy).Contains(v))
            {
                return arc.Point1;
            }

            if (RectangleBounds.GetPointBounds(arc.Point2, threshold, dx, dy).Contains(v))
            {
                return arc.Point2;
            }

            if (RectangleBounds.GetPointBounds(arc.Point3, threshold, dx, dy).Contains(v))
            {
                return arc.Point3;
            }

            if (RectangleBounds.GetPointBounds(arc.Point4, threshold, dx, dy).Contains(v))
            {
                return arc.Point4;
            }

            if (RectangleBounds.GetArcBounds(arc, dx, dy).Contains(v))
            {
                return arc;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cubicBezier"></param>
        /// <param name="v"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static BaseShape HitTestCubicBezier(XCubicBezier cubicBezier, Vector2 v, double threshold, double dx, double dy)
        {
            if (RectangleBounds.GetPointBounds(cubicBezier.Point1, threshold, dx, dy).Contains(v))
            {
                return cubicBezier.Point1;
            }

            if (RectangleBounds.GetPointBounds(cubicBezier.Point2, threshold, dx, dy).Contains(v))
            {
                return cubicBezier.Point2;
            }

            if (RectangleBounds.GetPointBounds(cubicBezier.Point3, threshold, dx, dy).Contains(v))
            {
                return cubicBezier.Point3;
            }

            if (RectangleBounds.GetPointBounds(cubicBezier.Point4, threshold, dx, dy).Contains(v))
            {
                return cubicBezier.Point4;
            }

            if (ConvexHullBounds.Contains(cubicBezier.GetPoints().ToImmutableArray(), v, dx, dy))
            {
                return cubicBezier;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quadraticBezier"></param>
        /// <param name="v"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static BaseShape HitTestQuadraticBezier(XQuadraticBezier quadraticBezier, Vector2 v, double threshold, double dx, double dy)
        {
            if (RectangleBounds.GetPointBounds(quadraticBezier.Point1, threshold, dx, dy).Contains(v))
            {
                return quadraticBezier.Point1;
            }

            if (RectangleBounds.GetPointBounds(quadraticBezier.Point2, threshold, dx, dy).Contains(v))
            {
                return quadraticBezier.Point2;
            }

            if (RectangleBounds.GetPointBounds(quadraticBezier.Point3, threshold, dx, dy).Contains(v))
            {
                return quadraticBezier.Point3;
            }

            if (ConvexHullBounds.Contains(quadraticBezier.GetPoints().ToImmutableArray(), v, dx, dy))
            {
                return quadraticBezier;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="v"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static BaseShape HitTestText(XText text, Vector2 v, double threshold, double dx, double dy)
        {
            if (RectangleBounds.GetPointBounds(text.TopLeft, threshold, dx, dy).Contains(v))
            {
                return text.TopLeft;
            }

            if (RectangleBounds.GetPointBounds(text.BottomRight, threshold, dx, dy).Contains(v))
            {
                return text.BottomRight;
            }

            if (RectangleBounds.GetTextBounds(text, dx, dy).Contains(v))
            {
                return text;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="v"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static BaseShape HitTestImage(XImage image, Vector2 v, double threshold, double dx, double dy)
        {
            if (RectangleBounds.GetPointBounds(image.TopLeft, threshold, dx, dy).Contains(v))
            {
                return image.TopLeft;
            }

            if (RectangleBounds.GetPointBounds(image.BottomRight, threshold, dx, dy).Contains(v))
            {
                return image.BottomRight;
            }

            if (RectangleBounds.GetImageBounds(image, dx, dy).Contains(v))
            {
                return image;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="v"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static BaseShape HitTestPath(XPath path, Vector2 v, double threshold, double dx, double dy)
        {
            if (path.Geometry != null)
            {
                var points = path.GetPoints().ToImmutableArray();
                foreach (var point in points)
                {
                    if (RectangleBounds.GetPointBounds(point, threshold, dx, dy).Contains(v))
                    {
                        return point;
                    }
                }

                if (ConvexHullBounds.Contains(points, v, dx, dy))
                {
                    return path;
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="v"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static BaseShape HitTestGroup(XGroup group, Vector2 v, double threshold, double dx, double dy)
        {
            foreach (var connector in group.Connectors.Reverse())
            {
                if (RectangleBounds.GetPointBounds(connector, threshold, dx, dy).Contains(v))
                {
                    return connector;
                }
            }

            var result = HitTest(group.Shapes.Reverse(), v, threshold, dx, dy);
            if (result != null)
            {
                return group;
            }

            return null;
        }

        /// <summary>
        /// Hit test point in shape bounds.
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="v"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static BaseShape HitTest(BaseShape shape, Vector2 v, double threshold, double dx, double dy)
        {
            if (shape is XPoint)
            {
                return HitTestPoint(shape as XPoint, v, threshold, dx, dy);
            }
            else if (shape is XLine)
            {
                return HitTestLine(shape as XLine, v, threshold, dx, dy);
            }
            else if (shape is XRectangle)
            {
                return HitTestRectangle(shape as XRectangle, v, threshold, dx, dy);
            }
            else if (shape is XEllipse)
            {
                return HitTestEllipse(shape as XEllipse, v, threshold, dx, dy);
            }
            else if (shape is XArc)
            {
                return HitTestArc(shape as XArc, v, threshold, dx, dy);
            }
            else if (shape is XCubicBezier)
            {
                return HitTestCubicBezier(shape as XCubicBezier, v, threshold, dx, dy);
            }
            else if (shape is XQuadraticBezier)
            {
                return HitTestQuadraticBezier(shape as XQuadraticBezier, v, threshold, dx, dy);
            }
            else if (shape is XText)
            {
                return HitTestText(shape as XText, v, threshold, dx, dy);
            }
            else if (shape is XImage)
            {
                return HitTestImage(shape as XImage, v, threshold, dx, dy);
            }
            else if (shape is XPath)
            {
                return HitTestPath(shape as XPath, v, threshold, dx, dy);
            }
            else if (shape is XGroup)
            {
                return HitTestGroup(shape as XGroup, v, threshold, dx, dy);
            }

            return null;
        }

        /// <summary>
        /// Hit test point in shapes bounds.
        /// </summary>
        /// <param name="shapes"></param>
        /// <param name="v"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static BaseShape HitTest(IEnumerable<BaseShape> shapes, Vector2 v, double threshold, double dx, double dy)
        {
            foreach (var shape in shapes)
            {
                var result = HitTest(shape, v, threshold, dx, dy);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Hit test point in shapes bounds.
        /// </summary>
        /// <param name="shapes"></param>
        /// <param name="v"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static BaseShape HitTest(IEnumerable<BaseShape> shapes, Vector2 v, double threshold)
        {
            var result = HitTest(shapes.Reverse(), v, threshold, 0, 0);
            if (result != null)
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="rect"></param>
        /// <param name="selected"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool HitTestPoint(XPoint point, Rect2 rect, ISet<BaseShape> selected, double threshold, double dx, double dy)
        {
            if (RectangleBounds.GetPointBounds(point, threshold, dx, dy).IntersectsWith(rect))
            {
                if (selected != null)
                {
                    selected.Add(point);
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
        /// <param name="line"></param>
        /// <param name="rect"></param>
        /// <param name="selected"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool HitTestLine(XLine line, Rect2 rect, ISet<BaseShape> selected, double threshold, double dx, double dy)
        {
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
        /// <param name="ellipse"></param>
        /// <param name="rect"></param>
        /// <param name="selected"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool HitTestEllipse(XEllipse ellipse, Rect2 rect, ISet<BaseShape> selected, double dx, double dy)
        {
            if (RectangleBounds.GetEllipseBounds(ellipse, dx, dy).IntersectsWith(rect))
            {
                if (selected != null)
                {
                    selected.Add(ellipse);
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
        /// <param name="rectangle"></param>
        /// <param name="rect"></param>
        /// <param name="selected"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool HitTestRectangle(XRectangle rectangle, Rect2 rect, ISet<BaseShape> selected, double dx, double dy)
        {
            if (RectangleBounds.GetRectangleBounds(rectangle, dx, dy).IntersectsWith(rect))
            {
                if (selected != null)
                {
                    selected.Add(rectangle);
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
        /// <param name="arc"></param>
        /// <param name="rect"></param>
        /// <param name="selected"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool HitTestArc(XArc arc, Rect2 rect, ISet<BaseShape> selected, double dx, double dy)
        {
            if (RectangleBounds.GetArcBounds(arc, dx, dy).IntersectsWith(rect))
            {
                if (selected != null)
                {
                    selected.Add(arc);
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
        /// <param name="cubicBezier"></param>
        /// <param name="selection"></param>
        /// <param name="selected"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool HitTestCubicBezier(XCubicBezier cubicBezier, Vector2[] selection, ISet<BaseShape> selected, double dx, double dy)
        {
            var points = cubicBezier.GetPoints().ToImmutableArray();
            if (ConvexHullBounds.Overlap(selection, points, dx, dy))
            {
                if (selected != null)
                {
                    selected.Add(cubicBezier);
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
        /// <param name="quadraticBezier"></param>
        /// <param name="selection"></param>
        /// <param name="selected"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool HitTestQadraticBezier(XQuadraticBezier quadraticBezier, Vector2[] selection, ISet<BaseShape> selected, double dx, double dy)
        {
            var points = quadraticBezier.GetPoints().ToImmutableArray();
            if (ConvexHullBounds.Overlap(selection, points, dx, dy))
            {
                if (selected != null)
                {
                    selected.Add(quadraticBezier);
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
        /// <param name="text"></param>
        /// <param name="rect"></param>
        /// <param name="selected"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool HitTestText(XText text, Rect2 rect, ISet<BaseShape> selected, double dx, double dy)
        {
            if (RectangleBounds.GetTextBounds(text, dx, dy).IntersectsWith(rect))
            {
                if (selected != null)
                {
                    selected.Add(text);
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
        /// <param name="image"></param>
        /// <param name="rect"></param>
        /// <param name="selected"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool HitTestImage(XImage image, Rect2 rect, ISet<BaseShape> selected, double dx, double dy)
        {
            if (RectangleBounds.GetImageBounds(image, dx, dy).IntersectsWith(rect))
            {
                if (selected != null)
                {
                    selected.Add(image);
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
        /// <param name="path"></param>
        /// <param name="selection"></param>
        /// <param name="selected"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool HitTestPath(XPath path, Vector2[] selection, ISet<BaseShape> selected, double dx, double dy)
        {
            if (path.Geometry != null)
            {
                var points = path.GetPoints().ToImmutableArray();
                if (ConvexHullBounds.Overlap(selection, points, dx, dy))
                {
                    if (selected != null)
                    {
                        selected.Add(path);
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
        /// <param name="group"></param>
        /// <param name="rect"></param>
        /// <param name="selection"></param>
        /// <param name="selected"></param>
        /// <param name="threshold"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static bool HitTestGroup(XGroup group, Rect2 rect, Vector2[] selection, ISet<BaseShape> selected, double threshold, double dx, double dy)
        {
            if (HitTest(group.Shapes.Reverse(), rect, selection, null, threshold, dx, dy) == true)
            {
                if (selected != null)
                {
                    selected.Add(group);
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
        /// Hit test rectangle if intersects with any of the shape bounds.
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
                return HitTestPoint(shape as XPoint, rect, selected, threshold, dx, dy);
            }
            else if (shape is XLine)
            {
                return HitTestLine(shape as XLine, rect, selected, threshold, dx, dy);
            }
            else if (shape is XEllipse)
            {
                return HitTestEllipse(shape as XEllipse, rect, selected, dx, dy);
            }
            else if (shape is XRectangle)
            {
                return HitTestRectangle(shape as XRectangle, rect, selected, dx, dy);
            }
            else if (shape is XArc)
            {
                return HitTestArc(shape as XArc, rect, selected, dx, dy);
            }
            else if (shape is XCubicBezier)
            {
                return HitTestCubicBezier(shape as XCubicBezier, selection, selected, dx, dy);
            }
            else if (shape is XQuadraticBezier)
            {
                return HitTestQadraticBezier(shape as XQuadraticBezier, selection, selected, dx, dy);
            }
            else if (shape is XText)
            {
                return HitTestText(shape as XText, rect, selected, dx, dy);
            }
            else if (shape is XImage)
            {
                return HitTestImage(shape as XImage, rect, selected, dx, dy);
            }
            else if (shape is XPath)
            {
                return HitTestPath(shape as XPath, selection, selected, dx, dy);
            }
            else if (shape is XGroup)
            {
                return HitTestGroup(shape as XGroup, rect, selection, selected, threshold, dx, dy);
            }

            return false;
        }

        /// <summary>
        /// Hit test rectangle if intersects with any of the shape bounds.
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
        /// Hit test rectangle if intersects with any of the shape bounds.
        /// </summary>
        /// <param name="shapes"></param>
        /// <param name="rect"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static ImmutableHashSet<BaseShape> HitTest(IEnumerable<BaseShape> shapes, Rect2 rect, double threshold)
        {
            var selected = ImmutableHashSet.CreateBuilder<BaseShape>();

            var selection = new Vector2[]
            {
                new Vector2(rect.X, rect.Y),
                new Vector2(rect.X + rect.Width, rect.Y),
                new Vector2(rect.X + rect.Width, rect.Y + rect.Height),
                new Vector2(rect.X, rect.Y + rect.Height)
            };

            HitTest(shapes.Reverse(), rect, selection, selected, threshold, 0, 0);

            return selected.ToImmutableHashSet();
        }
    }
}
