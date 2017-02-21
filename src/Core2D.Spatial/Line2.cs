// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Core2D.Spatial
{
    public struct Line2
    {
        public readonly Point2 A;
        public readonly Point2 B;

        public Line2(Point2 a, Point2 b)
        {
            this.A = a;
            this.B = b;
        }

        public Line2(double ax, double ay, double bx, double by)
        {
            this.A = new Point2(ax, ay);
            this.B = new Point2(bx, by);
        }

        public static Line2 FromPoints(double ax, double ay, double bx, double by, double dx = 0.0, double dy = 0.0)
        {
            return new Line2(ax + dx, ay + dy, bx + dx, by + dy);
        }

        public static Line2 FromPoints(Point2 a, Point2 b, double dx = 0.0, double dy = 0.0)
        {
            return FromPoints(a.X, a.Y, b.X, b.Y, dx, dy);
        }

        public static double AngleBetween(Point2 a0, Point2 b0, Point2 a1, Point2 b1)
        {
            double angle1 = (double)Math.Atan2(a0.Y - b0.Y, a0.X - b0.X);
            double angle2 = (double)Math.Atan2(a1.Y - b1.Y, a1.X - b1.X);
            double result = (angle2 - angle1) * 180.0 / Math.PI;
            if (result < 0.0d)
                result += 360.0;
            return result;
        }

        public static Point2 Middle(Point2 a, Point2 b)
        {
            return new Point2((a.X + b.X) / 2.0, (a.Y + b.Y) / 2.0);
        }

        public static bool LineIntersectWithLine(Point2 a0, Point2 b0, Point2 a1, Point2 b1, out Point2 clip)
        {
            double A0 = b0.Y - a0.Y;
            double B0 = a0.X - b0.X;
            double C0 = A0 * a0.X + B0 * a0.Y;
            double A1 = b1.Y - a1.Y;
            double B1 = a1.X - b1.X;
            double C1 = A1 * a1.X + B1 * a1.Y;
            double det = A0 * B1 - A1 * B0;
            if (det != 0.0)
            {
                double x = (B1 * C0 - B0 * C1) / det;
                double y = (A0 * C1 - A1 * C0) / det;
                var point = new Point2(x, y);
                if (point.IsOnLine(a0, b0) && point.IsOnLine(a1, b1))
                {
                    clip = point;
                    return true;
                }
            }
            clip = default(Point2);
            return false;
        }

        public static bool LineIntersectsWithEllipse(Point2 a, Point2 b, Rect2 rect, bool onlySegment, out IList<Point2> points)
        {
            if ((rect.Width == 0) || (rect.Height == 0) || ((a.X == b.X) && (a.Y == b.Y)))
            {
                points = null;
                return false;
            }

            if (rect.Width < 0)
                throw new ArgumentException("Rectangle Width must be positive value.");

            if (rect.Height < 0)
                throw new ArgumentException("Rectangle Height must be positive value.");

            double cx = rect.Left + rect.Width / 2.0;
            double cy = rect.Top + rect.Height / 2.0;

            double p1X = a.X - cx;
            double p1Y = a.Y - cy;
            double p2X = b.X - cx;
            double p2Y = b.Y - cy;

            double rx = rect.Width / 2.0;
            double ry = rect.Height / 2.0;

            double A = (p2X - p1X) * (p2X - p1X) / rx / rx + (p2Y - p1Y) * (p2Y - p1Y) / ry / ry;
            double B = 2 * p1X * (p2X - p1X) / rx / rx + 2 * p1Y * (p2Y - p1Y) / ry / ry;
            double C = p1X * p1X / rx / rx + p1Y * p1Y / ry / ry - 1;

            var solutions = new List<double>();

            double discriminant = B * B - 4 * A * C;
            if (discriminant == 0)
            {
                solutions.Add(-B / 2 / A);
            }
            else if (discriminant > 0)
            {
                solutions.Add((-B + (double)Math.Sqrt(discriminant)) / 2 / A);
                solutions.Add((-B - (double)Math.Sqrt(discriminant)) / 2 / A);
            }

            var result = new List<Point2>();

            foreach (var t in solutions)
            {
                if (!onlySegment || ((t >= 0f) && (t <= 1f)))
                {
                    double x = p1X + (p2X - p1X) * t + cx;
                    double y = p1Y + (p2Y - p1Y) * t + cy;
                    result.Add(new Point2(x, y));
                }
            }

            if (result.Count > 0)
            {
                points = result;
                return true;
            }
            else
            {
                points = null;
                return false;
            }
        }

        public static bool LineIntersectsWithRect(Point2 a, Point2 b, Rect2 rect, out double x0clip, out double y0clip, out double x1clip, out double y1clip)
        {
            x0clip = 0.0;
            y0clip = 0.0;
            x1clip = 0.0;
            y1clip = 0.0;

            double left = rect.Left;
            double right = rect.Right;
            double bottom = rect.Bottom;
            double top = rect.Top;
            double x0 = a.X;
            double y0 = a.Y;
            double x1 = b.X;
            double y1 = b.Y;

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

            x0clip = x0 + t0 * dx;
            y0clip = y0 + t0 * dy;
            x1clip = x0 + t1 * dx;
            y1clip = y0 + t1 * dy;

            return true;
        }
    }
}
