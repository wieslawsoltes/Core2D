// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Test2d
{
    public static class ShapeBounds
    {
        #region Math
        
        private static bool LineIntersectsWithRect(ref Rect2 rect, XPoint p0, XPoint p1)
        {
            double left = rect.Left; 
            double right = rect.Right;
            double bottom = rect.Bottom; 
            double top = rect.Top;
            double x0 = p0.X; 
            double y0 = p0.Y;
            double x1 = p1.X; 
            double y1 = p1.Y;

            // Liang-Barsky line clipping algorithm
            double t0 = 0.0;
            double t1 = 1.0;
            double dx = x1 - x0;
            double dy = y1 - y0;
            double p = 0.0, q = 0.0, r;

            for (int edge = 0; edge < 4; edge++)
            {
                if (edge == 0)
                {
                    p = -dx;
                    q = -(left - x0);
                }
                if (edge == 1)
                {
                    p = dx;
                    q = (right - x0);
                }
                if (edge == 2)
                {
                    p = dy;
                    q = (bottom - y0);
                }
                if (edge == 3)
                {
                    p = -dy;
                    q = -(top - y0);
                }

                r = q / p;

                if (p == 0.0 && q < 0.0)
                {
                    return false;
                }

                if (p < 0.0)
                {
                    if (r > t1)
                    {
                        return false;
                    }
                    else if (r > t0)
                    {
                        t0 = r;
                    }
                }
                else if (p > 0.0)
                {
                    if (r < t0)
                    {
                        return false;
                    }
                    else if (r < t1)
                    {
                        t1 = r;
                    }
                }
            }

            // clipped line
            //double x0clip = x0 + t0 * dx;
            //double y0clip = y0 + t0 * dy;
            //double x1clip = x0 + t1 * dx;
            //double y1clip = y0 + t1 * dy;

            return true;
        }

        private static Vector2 NearestPointOnLine(Vector2 a, Vector2 b, Vector2 p)
        {
            double ax = p.X - a.X;
            double ay = p.Y - a.Y;
            double bx = b.X - a.X;
            double by = b.Y - a.Y;
            double t = (ax * bx + ay * by) / (bx * bx + by * by);
            if (t < 0.0)
            {
                return new Vector2(a.X, a.Y);
            }
            else if (t > 1.0)
            {
                return new Vector2(b.X, b.Y);
            }
            return new Vector2(bx * t + a.X, by * t + a.Y);
        }

        private static double Distance(double x1, double y1, double x2, double y2)
        {
            double dx = x1 - x2;
            double dy = y1 - y2;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private static Vector2 Middle(double x1, double y1, double x2, double y2)
        {
            return new Vector2((x1 + x2) / 2.0, (y1 + y2) / 2.0);
        }

        #endregion

        #region Bounds

        private static Rect2 GetPointBounds(XPoint point, double treshold)
        {
            double radius = treshold / 2.0;
            return new Rect2(
                point.X - radius, 
                point.Y - radius, 
                treshold, 
                treshold);
        }

        private static Rect2 GetRectangleBounds(XRectangle rectangle)
        {
            return Rect2.Create(rectangle.TopLeft, rectangle.BottomRight);
        }

        private static Rect2 GetEllipseBounds(XEllipse ellipse)
        {
            return Rect2.Create(ellipse.TopLeft, ellipse.BottomRight);
        }

        private static Rect2 GetArcBounds(XArc arc, double dx, double dy)
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

        private static Rect2 GetTextBounds(XText text)
        {
            return Rect2.Create(text.TopLeft, text.BottomRight);
        }

        private static Rect2 GetImageBounds(XImage image)
        {
            return Rect2.Create(image.TopLeft, image.BottomRight);
        }

        #endregion

        #region HitTest Point

        private static bool HitTest(XLine line, Vector2 p, double treshold)
        {
            var a = new Vector2(line.Start.X, line.Start.Y);
            var b = new Vector2(line.End.X, line.End.Y);
            var nearest = NearestPointOnLine(a, b, p);
            double distance = Distance(p.X, p.Y, nearest.X, nearest.Y);
            return distance < treshold;
        }

        private static BaseShape HitTest(IEnumerable<BaseShape> shapes, Vector2 p, double treshold)
        {
            foreach (var shape in shapes)
            {
                if (shape is XPoint)
                {
                    if (GetPointBounds(shape as XPoint, treshold).Contains(p))
                    {
                        return shape;
                    }
                    continue;
                }
                else if (shape is XLine)
                {
                    var line = shape as XLine;

                    if (GetPointBounds(line.Start, treshold).Contains(p))
                    {
                        return line.Start;
                    }

                    if (GetPointBounds(line.End, treshold).Contains(p))
                    {
                        return line.End;
                    }

                    if (HitTest(line, p, treshold))
                    {
                        return line;
                    }

                    continue;
                }
                else if (shape is XRectangle)
                { 
                    var rectangle = shape as XRectangle;

                    if (GetPointBounds(rectangle.TopLeft, treshold).Contains(p))
                    {
                        return rectangle.TopLeft;
                    }

                    if (GetPointBounds(rectangle.BottomRight, treshold).Contains(p))
                    {
                        return rectangle.BottomRight;
                    }
                    
                    if (GetRectangleBounds(rectangle).Contains(p))
                    {
                        return rectangle;
                    }
                    continue;
                }
                else if (shape is XEllipse)
                {
                    var ellipse = shape as XEllipse;

                    if (GetPointBounds(ellipse.TopLeft, treshold).Contains(p))
                    {
                        return ellipse.TopLeft;
                    }

                    if (GetPointBounds(ellipse.BottomRight, treshold).Contains(p))
                    {
                        return ellipse.BottomRight;
                    }
                    
                    if (GetEllipseBounds(ellipse).Contains(p))
                    {
                        return ellipse;
                    }
                    continue;
                }
                else if (shape is XArc)
                {
                    var arc = shape as XArc;

                    if (GetPointBounds(arc.Point1, treshold).Contains(p))
                    {
                        return arc.Point1;
                    }
       
                    if (GetPointBounds(arc.Point2, treshold).Contains(p))
                    {
                        return arc.Point2;
                    }
                    
                    if (GetArcBounds(arc, 0.0, 0.0).Contains(p))
                    {
                        return arc;
                    }
                    continue;
                }
                else if (shape is XBezier)
                {  
                    var bezier = shape as XBezier;

                    if (GetPointBounds(bezier.Point1, treshold).Contains(p))
                    {
                        return bezier.Point1;
                    }
       
                    if (GetPointBounds(bezier.Point2, treshold).Contains(p))
                    {
                        return bezier.Point2;
                    }
                    
                    if (GetPointBounds(bezier.Point3, treshold).Contains(p))
                    {
                        return bezier.Point3;
                    }
                    
                    if (GetPointBounds(bezier.Point4, treshold).Contains(p))
                    {
                        return bezier.Point4;
                    }
                       
                    if (ConvexHullBounds.Contains(bezier, p))
                    {
                        return bezier;
                    }
                    continue;
                }
                else if (shape is XQBezier)
                {
                    var qbezier = shape as XQBezier;

                    if (GetPointBounds(qbezier.Point1, treshold).Contains(p))
                    {
                        return qbezier.Point1;
                    }
       
                    if (GetPointBounds(qbezier.Point2, treshold).Contains(p))
                    {
                        return qbezier.Point2;
                    }
                     
                    if (GetPointBounds(qbezier.Point3, treshold).Contains(p))
                    {
                        return qbezier.Point3;
                    }
                    
                    if (ConvexHullBounds.Contains(qbezier, p))
                    {
                        return qbezier;
                    }
                    continue;
                }
                else if (shape is XText)
                {
                    var text = shape as XText;

                    if (GetPointBounds(text.TopLeft, treshold).Contains(p))
                    {
                        return text.TopLeft;
                    }

                    if (GetPointBounds(text.BottomRight, treshold).Contains(p))
                    {
                        return text.BottomRight;
                    }
                    
                    if (GetTextBounds(text).Contains(p))
                    {
                        return text;
                    }
                    continue;
                }
                else if (shape is XImage)
                {
                    var image = shape as XImage;

                    if (GetPointBounds(image.TopLeft, treshold).Contains(p))
                    {
                        return image.TopLeft;
                    }

                    if (GetPointBounds(image.BottomRight, treshold).Contains(p))
                    {
                        return image.BottomRight;
                    }

                    if (GetImageBounds(image).Contains(p))
                    {
                        return image;
                    }
                    continue;
                }
                else if (shape is XGroup)
                {
                    var group = shape as XGroup;

                    foreach (var connector in group.Connectors)
                    {
                        if (GetPointBounds(connector, treshold).Contains(p))
                        {
                            return connector;
                        }
                    }

                    var result = HitTest(group.Shapes, p, treshold);
                    if (result != null)
                    {
                        return shape;
                    }
                    continue;
                }
            }

            return null;
        }

        public static BaseShape HitTest(Container container, Vector2 p, double treshold)
        {
            //foreach (var layer in container.Layers)
            //{
            //    var shape = HitTest(layer.Shapes, p, treshold);
            //    if (shape != null)
            //    {
            //        return shape;
            //    }
            //}
            var shape = HitTest(container.CurrentLayer.Shapes, p, treshold);
            if (shape != null)
            {
                return shape;
            }

            return null;
        }
      
        #endregion

        #region HitTest Rect

        private static bool HitTest(IEnumerable<BaseShape> shapes, Rect2 rect, Vector2[] selection, ICollection<BaseShape> hs, double treshold)
        {
            foreach (var shape in shapes)
            {
                if (shape is XPoint)
                {
                    if (GetPointBounds(shape as XPoint, treshold).IntersectsWith(rect))
                    {
                        if (hs != null)
                        {
                            hs.Add(shape);
                        }
                        else
                        {
                            return true;
                        }
                    }
                    continue;
                }
                else if (shape is XLine)
                {
                    var line = shape as XLine;
                    if (GetPointBounds(line.Start, treshold).IntersectsWith(rect)
                        || GetPointBounds(line.End, treshold).IntersectsWith(rect)
                        || LineIntersectsWithRect(ref rect, line.Start, line.End))
                    {
                        if (hs != null)
                        {
                            hs.Add(line);
                            continue;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    continue;  
                }
                else if (shape is XEllipse)
                {
                    if (GetEllipseBounds(shape as XEllipse).IntersectsWith(rect))
                    {
                        if (hs != null)
                        {
                            hs.Add(shape);
                            continue;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    continue;
                }
                else if (shape is XRectangle)
                {
                    if (GetRectangleBounds(shape as XRectangle).IntersectsWith(rect))
                    {
                        if (hs != null)
                        {
                            hs.Add(shape);
                            continue;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    continue;
                }
                else if (shape is XArc)
                {
                    if (GetArcBounds(shape as XArc, 0.0, 0.0).IntersectsWith(rect))
                    {
                        if (hs != null)
                        {
                            hs.Add(shape);
                            continue;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    continue;
                }
                else if (shape is XBezier)
                {
                    if (ConvexHullBounds.Overlap(selection, ConvexHullBounds.GetVertices(shape as XBezier)))
                    {
                        if (hs != null)
                        {
                            hs.Add(shape);
                            continue;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    continue;
                }
                else if (shape is XQBezier)
                {
                    if (ConvexHullBounds.Overlap(selection, ConvexHullBounds.GetVertices(shape as XQBezier)))
                    {
                        if (hs != null)
                        {
                            hs.Add(shape);
                            continue;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    continue;
                }
                else if (shape is XText)
                {
                    if (GetTextBounds(shape as XText).IntersectsWith(rect))
                    {
                        if (hs != null)
                        {
                            hs.Add(shape);
                            continue;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    continue;
                }
                else if (shape is XImage)
                {
                    if (GetImageBounds(shape as XImage).IntersectsWith(rect))
                    {
                        if (hs != null)
                        {
                            hs.Add(shape);
                            continue;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    continue;
                }
                else if (shape is XGroup)
                {
                    if (HitTest((shape as XGroup).Shapes, rect, selection, null, treshold) == true)
                    {
                        if (hs != null)
                        {
                            hs.Add(shape);
                            continue;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    continue;
                }
            }

            return false;
        }

        public static ICollection<BaseShape> HitTest(Container container, Rect2 rect, double treshold)
        {
            var hs = new HashSet<BaseShape>();

            var selection = new Vector2[]
            {
                new Vector2(rect.X, rect.Y),
                new Vector2(rect.X + rect.Width, rect.Y),
                new Vector2(rect.X + rect.Width, rect.Y + rect.Height),
                new Vector2(rect.X, rect.Y + rect.Height)
            };

            //foreach (var layer in container.Layers) 
            //{
            //    HitTest(layer.Shapes, rect, selection, hs, treshold);
            //}
            HitTest(container.CurrentLayer.Shapes, rect, selection, hs, treshold);
      
            return hs;
        }

        #endregion
    }
}
