// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core2D.Spatial;
using Core2D.Shape;
using Core2D.Shapes;

namespace Core2D.Editor.Bounds
{
    /// <summary>
    /// Hit test shapes using selection bounds.
    /// </summary>
    public static class ShapeHitTestSelection
    {
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
            if (ShapeBounds.GetPointBounds(point, threshold, dx, dy).IntersectsWith(rect))
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
            double x0clip, y0clip, x1clip, y1clip;
            if (ShapeBounds.GetPointBounds(line.Start, threshold, dx, dy).IntersectsWith(rect)
                || ShapeBounds.GetPointBounds(line.End, threshold, dx, dy).IntersectsWith(rect)
                || Line2.LineIntersectsWithRect(new Point2(line.Start.X, line.Start.Y), new Point2(line.End.X, line.End.Y), rect, out x0clip, out y0clip, out x1clip, out y1clip))
            {                if (selected != null)
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
            if (ShapeBounds.GetEllipseBounds(ellipse, dx, dy).IntersectsWith(rect))
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
            if (ShapeBounds.GetRectangleBounds(rectangle, dx, dy).IntersectsWith(rect))
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
            if (ShapeBounds.GetArcBounds(arc, dx, dy).IntersectsWith(rect))
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
            if (ShapeBounds.Overlap(selection, points, dx, dy))
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
            if (ShapeBounds.Overlap(selection, points, dx, dy))
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
            if (ShapeBounds.GetTextBounds(text, dx, dy).IntersectsWith(rect))
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
            if (ShapeBounds.GetImageBounds(image, dx, dy).IntersectsWith(rect))
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
                if (ShapeBounds.Overlap(selection, points, dx, dy))
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
