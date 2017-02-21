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
    /// Hit test shapes using point.
    /// </summary>
    public static class ShapeHitTestPoint
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
            if (ShapeBounds.GetPointBounds(point, threshold, dx, dy).Contains(v.X, v.Y))
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
            if (ShapeBounds.GetPointBounds(line.Start, threshold, dx, dy).Contains(v.X, v.Y))
            {
                return line.Start;
            }

            if (ShapeBounds.GetPointBounds(line.End, threshold, dx, dy).Contains(v.X, v.Y))
            {
                return line.End;
            }

            if (ShapeBounds.Contains(line, v, threshold, dx, dy))
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
            if (ShapeBounds.GetPointBounds(rectangle.TopLeft, threshold, dx, dy).Contains(v.X, v.Y))
            {
                return rectangle.TopLeft;
            }

            if (ShapeBounds.GetPointBounds(rectangle.BottomRight, threshold, dx, dy).Contains(v.X, v.Y))
            {
                return rectangle.BottomRight;
            }

            if (ShapeBounds.GetRectangleBounds(rectangle, dx, dy).Contains(v.X, v.Y))
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
            if (ShapeBounds.GetPointBounds(ellipse.TopLeft, threshold, dx, dy).Contains(v.X, v.Y))
            {
                return ellipse.TopLeft;
            }

            if (ShapeBounds.GetPointBounds(ellipse.BottomRight, threshold, dx, dy).Contains(v.X, v.Y))
            {
                return ellipse.BottomRight;
            }

            if (ShapeBounds.GetEllipseBounds(ellipse, dx, dy).Contains(v.X, v.Y))
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
            if (ShapeBounds.GetPointBounds(arc.Point1, threshold, dx, dy).Contains(v.X, v.Y))
            {
                return arc.Point1;
            }

            if (ShapeBounds.GetPointBounds(arc.Point2, threshold, dx, dy).Contains(v.X, v.Y))
            {
                return arc.Point2;
            }

            if (ShapeBounds.GetPointBounds(arc.Point3, threshold, dx, dy).Contains(v.X, v.Y))
            {
                return arc.Point3;
            }

            if (ShapeBounds.GetPointBounds(arc.Point4, threshold, dx, dy).Contains(v.X, v.Y))
            {
                return arc.Point4;
            }

            if (ShapeBounds.GetArcBounds(arc, dx, dy).Contains(v.X, v.Y))
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
            if (ShapeBounds.GetPointBounds(cubicBezier.Point1, threshold, dx, dy).Contains(v.X, v.Y))
            {
                return cubicBezier.Point1;
            }

            if (ShapeBounds.GetPointBounds(cubicBezier.Point2, threshold, dx, dy).Contains(v.X, v.Y))
            {
                return cubicBezier.Point2;
            }

            if (ShapeBounds.GetPointBounds(cubicBezier.Point3, threshold, dx, dy).Contains(v.X, v.Y))
            {
                return cubicBezier.Point3;
            }

            if (ShapeBounds.GetPointBounds(cubicBezier.Point4, threshold, dx, dy).Contains(v.X, v.Y))
            {
                return cubicBezier.Point4;
            }

            if (ShapeBounds.Contains(cubicBezier.GetPoints().ToImmutableArray(), v, dx, dy))
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
            if (ShapeBounds.GetPointBounds(quadraticBezier.Point1, threshold, dx, dy).Contains(v.X, v.Y))
            {
                return quadraticBezier.Point1;
            }

            if (ShapeBounds.GetPointBounds(quadraticBezier.Point2, threshold, dx, dy).Contains(v.X, v.Y))
            {
                return quadraticBezier.Point2;
            }

            if (ShapeBounds.GetPointBounds(quadraticBezier.Point3, threshold, dx, dy).Contains(v.X, v.Y))
            {
                return quadraticBezier.Point3;
            }

            if (ShapeBounds.Contains(quadraticBezier.GetPoints().ToImmutableArray(), v, dx, dy))
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
            if (ShapeBounds.GetPointBounds(text.TopLeft, threshold, dx, dy).Contains(v.X, v.Y))
            {
                return text.TopLeft;
            }

            if (ShapeBounds.GetPointBounds(text.BottomRight, threshold, dx, dy).Contains(v.X, v.Y))
            {
                return text.BottomRight;
            }

            if (ShapeBounds.GetTextBounds(text, dx, dy).Contains(v.X, v.Y))
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
            if (ShapeBounds.GetPointBounds(image.TopLeft, threshold, dx, dy).Contains(v.X, v.Y))
            {
                return image.TopLeft;
            }

            if (ShapeBounds.GetPointBounds(image.BottomRight, threshold, dx, dy).Contains(v.X, v.Y))
            {
                return image.BottomRight;
            }

            if (ShapeBounds.GetImageBounds(image, dx, dy).Contains(v.X, v.Y))
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
                    if (ShapeBounds.GetPointBounds(point, threshold, dx, dy).Contains(v.X, v.Y))
                    {
                        return point;
                    }
                }

                if (ShapeBounds.Contains(points, v, dx, dy))
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
                if (ShapeBounds.GetPointBounds(connector, threshold, dx, dy).Contains(v.X, v.Y))
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
    }
}
