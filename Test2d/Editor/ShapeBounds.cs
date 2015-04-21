// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test2d
{
    public static class ShapeBounds
    {
        #region Math
        
        public static bool LineIntersectsWithRect(ref Rect2 rect, XPoint p0, XPoint p1)
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

        public static Vector2 NearestPointOnLine(Vector2 a, Vector2 b, Vector2 p)
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

        public static double Distance(double x1, double y1, double x2, double y2)
        {
            double dx = x1 - x2;
            double dy = y1 - y2;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static Vector2 Middle(double x1, double y1, double x2, double y2)
        {
            return new Vector2((x1 + x2) / 2.0, (y1 + y2) / 2.0);
        }

        #endregion

        #region Bounds

        public static Rect2 CreateRect(XPoint tl, XPoint br, double dx, double dy)
        {
            double tlx = Math.Min(tl.X, br.X);
            double tly = Math.Min(tl.Y, br.Y);
            double brx = Math.Max(tl.X, br.X);
            double bry = Math.Max(tl.Y, br.Y);
            double x = tlx + dx;
            double y = tly + dy;
            double width = (brx + dx) - x;
            double height = (bry + dy) - y;
            return new Rect2(x, y, width, height);
        }
        
        public static Rect2 GetPointBounds(XPoint point, double treshold)
        {
            double radius = treshold / 2.0;
            return new Rect2(
                point.X - radius, 
                point.Y - radius, 
                treshold, 
                treshold);
        }
 
        public static Rect2 GetRectangleBounds(XRectangle rectangle)
        {
            return CreateRect(rectangle.TopLeft, rectangle.BottomRight, 0.0, 0.0);
        }
        
        public static Rect2 GetEllipseBounds(XEllipse ellipse)
        {
            return CreateRect(ellipse.TopLeft, ellipse.BottomRight, 0.0, 0.0);
        }

        public static Rect2 GetTextBounds(XText text)
        {
            return CreateRect(text.TopLeft, text.BottomRight, 0.0, 0.0);
        }

        #endregion

        #region HitTest
   
        public static bool HitTest(XLine line, Vector2 p, double treshold)
        {
            var a = new Vector2(line.Start.X, line.Start.Y);
            var b = new Vector2(line.End.X, line.End.Y);
            var nearest = NearestPointOnLine(a, b, p);
            double distance = Distance(p.X, p.Y, nearest.X, nearest.Y);
            return distance < treshold;
        }

        public static BaseShape HitTest(IEnumerable<BaseShape> shapes, Vector2 p, double treshold)
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
                        return line;
                    }

                    if (GetPointBounds(line.End, treshold).Contains(p))
                    {
                        return line;
                    }

                    if (HitTest(line, p, treshold))
                    {
                        return line;
                    }

                    continue;
                }
                else if (shape is XRectangle)
                {
                    if (GetRectangleBounds(shape as XRectangle).Contains(p))
                    {
                        return shape;
                    }
                    continue;
                }
                else if (shape is XEllipse)
                {
                    if (GetEllipseBounds(shape as XEllipse).Contains(p))
                    {
                        return shape;
                    }
                    continue;
                }
                else if (shape is XArc)
                {
                    // TODO:
                }
                else if (shape is XBezier)
                {
                    // TODO:
                }
                else if (shape is XQBezier)
                {
                    // TODO:
                }
                else if (shape is XText)
                {
                    if (GetTextBounds(shape as XText).Contains(p))
                    {
                        return shape;
                    }
                    continue;
                }
                else if (shape is XGroup)
                {
                    var result = HitTest((shape as XGroup).Shapes, p, treshold);
                    if (result != null)
                    {
                        return shape;
                    }
                    continue;
                }
            }

            return null;
        }

        public static bool HitTest(IEnumerable<BaseShape> shapes, Rect2 rect, ICollection<BaseShape> hs, double treshold)
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
                    // TODO:
                }
                else if (shape is XBezier)
                {
                    // TODO:
                }
                else if (shape is XQBezier)
                {
                    // TODO:
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
                else if (shape is XGroup)
                {
                    if (HitTest((shape as XGroup).Shapes, rect, null, treshold) == true)
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

        public static BaseShape HitTest(Container container, Vector2 p, double treshold)
        {
            foreach (var layer in container.Layers) 
            {
                var shape = HitTest(layer.Shapes, p, treshold);
                if (shape != null)
                {
                    return shape;
                }
            }

            return null;
        }
        
        public static ICollection<BaseShape> HitTest(Container container, Rect2 rect, double treshold)
        {
            var hs = new HashSet<BaseShape>();
            
            foreach (var layer in container.Layers) 
            {
                HitTest(layer.Shapes, rect, hs, treshold);
            }
      
            return hs;
        }

        #endregion
    }
}
