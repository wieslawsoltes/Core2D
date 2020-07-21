#nullable enable
using System;
using System.Globalization;

namespace Spatial
{
    public struct PointD : IComparable<PointD>
    {
        public readonly decimal X;
        public readonly decimal Y;

        public PointD(decimal x, decimal y)
        {
            this.X = x;
            this.Y = y;
        }

        public void Deconstruct(out decimal x, out decimal y)
        {
            x = this.X;
            y = this.Y;
        }

        public static PointD FromXY(decimal x, decimal y)
        {
            return new PointD(x, y);
        }

        public static PointD operator +(PointD point1, PointD point2)
        {
            return new PointD(point1.X + point2.X, point1.Y + point2.Y);
        }

        public static PointD operator -(PointD point1, PointD point2)
        {
            return new PointD(point1.X - point2.X, point1.Y - point2.Y);
        }

        public decimal DistanceTo(PointD other)
        {
            decimal dx = X - other.X;
            decimal dy = Y - other.Y;
            return (decimal)Math.Sqrt((double)((dx * dx) + (dy * dy)));
        }

        public decimal AngleBetween(PointD other)
        {
            decimal angle = (decimal)Math.Atan2((double)(other.Y - Y), (double)(other.X - X));
            decimal result = angle * 180m / (decimal)Math.PI;
            if (result < 0m)
                result += 360m;
            return result;
        }

        public PointD RotateAt(PointD center, decimal angleInDegrees)
        {
            decimal angleInRadians = angleInDegrees * ((decimal)Math.PI / 180m);
            decimal cosTheta = (decimal)Math.Cos((double)angleInRadians);
            decimal sinTheta = (decimal)Math.Sin((double)angleInRadians);
            return new PointD(
                (cosTheta * (X - center.X)) - (sinTheta * (Y - center.Y)) + center.X,
                (sinTheta * (X - center.X)) + (cosTheta * (Y - center.Y)) + center.Y);
        }

        public PointD ProjectOnLine(PointD a, PointD b)
        {
            decimal x1 = a.X, y1 = a.Y, x2 = b.X, y2 = b.Y, x3 = X, y3 = Y;
            decimal px = x2 - x1, py = y2 - y1, dAB = (px * px) + (py * py);
            decimal u = (((x3 - x1) * px) + ((y3 - y1) * py)) / dAB;
            decimal x = x1 + (u * px), y = y1 + (u * py);
            return new PointD(x, y);
        }

        public PointD NearestOnLine(PointD a, PointD b)
        {
            decimal ax = X - a.X;
            decimal ay = Y - a.Y;
            decimal bx = b.X - a.X;
            decimal by = b.Y - a.Y;
            decimal t = ((ax * bx) + (ay * by)) / ((bx * bx) + (by * by));
            if (t < 0m)
            {
                return new PointD(a.X, a.Y);
            }
            else if (t > 1m)
            {
                return new PointD(b.X, b.Y);
            }
            return new PointD((bx * t) + a.X, (by * t) + a.Y);
        }

        public bool IsOnLine(PointD a, PointD b)
        {
            decimal minX = (decimal)Math.Min(a.X, b.X);
            decimal maxX = (decimal)Math.Max(a.X, b.X);
            decimal minY = (decimal)Math.Min(a.Y, b.Y);
            decimal maxY = (decimal)Math.Max(a.Y, b.Y);
            return minX <= X && X <= maxX && minY <= Y && Y <= maxY;
        }

        public Rect2 ExpandToRect(decimal radius)
        {
            decimal size = radius * 2m;
            return new Rect2((double)(X - radius), (double)(Y - radius), (double)size, (double)size);
        }

        public static bool operator <(PointD p1, PointD p2)
        {
            return p1.X < p2.X || (p1.X == p2.X && p1.Y < p2.Y);
        }

        public static bool operator >(PointD p1, PointD p2)
        {
            return p1.X > p2.X || (p1.X == p2.X && p1.Y > p2.Y);
        }

        public int CompareTo(PointD other)
        {
            return (this > other) ? -1 : ((this < other) ? 1 : 0);
        }

        public bool Equals(PointD other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object? obj)
        {
            return obj is PointD ? Equals((PointD)obj) : false;
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
