// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Globalization;

namespace Core2D.Spatial
{
    public struct Point2 : IComparable<Point2>
    {
        public readonly double X;
        public readonly double Y;

        public Point2(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public void Deconstruct(out double x, out double y)
        {
            x = this.X;
            y = this.Y;
        }

        public static Point2 FromXY(double x, double y)
        {
            return new Point2(x, y);
        }

        public static Point2 operator +(Point2 point1, Point2 point2)
        {
            return new Point2(point1.X + point2.X, point1.Y + point2.Y);
        }

        public static Point2 operator -(Point2 point1, Point2 point2)
        {
            return new Point2(point1.X - point2.X, point1.Y - point2.Y);
        }

        public double DistanceTo(Point2 other)
        {
            double dx = X - other.X;
            double dy = Y - other.Y;
            return (double)Math.Sqrt(dx * dx + dy * dy);
        }

        public double AngleBetween(Point2 other)
        {
            double angle = (double)Math.Atan2(other.Y - Y, other.X - X);
            double result = angle * 180.0 / Math.PI;
            if (result < 0.0)
                result += 360.0;
            return result;
        }

        public Point2 RotateAt(Point2 center, double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * (Math.PI / 180.0);
            double cosTheta = (double)Math.Cos(angleInRadians);
            double sinTheta = (double)Math.Sin(angleInRadians);
            return new Point2(
                (cosTheta * (X - center.X) - sinTheta * (Y - center.Y) + center.X),
                (sinTheta * (X - center.X) + cosTheta * (Y - center.Y) + center.Y));
        }

        public Point2 ProjectOnLine(Point2 a, Point2 b)
        {
            double x1 = a.X, y1 = a.Y, x2 = b.X, y2 = b.Y, x3 = X, y3 = Y;
            double px = x2 - x1, py = y2 - y1, dAB = px * px + py * py;
            double u = ((x3 - x1) * px + (y3 - y1) * py) / dAB;
            double x = x1 + u * px, y = y1 + u * py;
            return new Point2(x, y);
        }

        public Point2 NearestOnLine(Point2 a, Point2 b)
        {
            double ax = X - a.X;
            double ay = Y - a.Y;
            double bx = b.X - a.X;
            double by = b.Y - a.Y;
            double t = (ax * bx + ay * by) / (bx * bx + by * by);
            if (t < 0.0)
            {
                return new Point2(a.X, a.Y);
            }
            else if (t > 1.0)
            {
                return new Point2(b.X, b.Y);
            }
            return new Point2(bx * t + a.X, by * t + a.Y);
        }

        public bool IsOnLine(Point2 a, Point2 b)
        {
            double minX = (double)Math.Min(a.X, b.X);
            double maxX = (double)Math.Max(a.X, b.X);
            double minY = (double)Math.Min(a.Y, b.Y);
            double maxY = (double)Math.Max(a.Y, b.Y);
            return minX <= X && X <= maxX && minY <= Y && Y <= maxY;
        }

        public Rect2 ExpandToRect(double radius)
        {
            double size = radius * 2;
            return new Rect2(X - radius, Y - radius, size, size);
        }

        public static bool operator <(Point2 p1, Point2 p2)
        {
            return p1.X < p2.X || (p1.X == p2.X && p1.Y < p2.Y);
        }

        public static bool operator >(Point2 p1, Point2 p2)
        {
            return p1.X > p2.X || (p1.X == p2.X && p1.Y > p2.Y);
        }

        public int CompareTo(Point2 other)
        {
            return (this > other) ? -1 : ((this < other) ? 1 : 0);
        }

        public bool Equals(Point2 other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is Point2 ? Equals((Point2)obj) : false;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode();
        }

        public override string ToString()
        {
            return string.Concat(X, CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, Y);
        }
    }
}
