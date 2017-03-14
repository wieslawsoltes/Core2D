// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Globalization;

namespace Core2D.Spatial
{
    public struct Vector2 : IComparable<Vector2>
    {
        public readonly double X;
        public readonly double Y;

        public static Vector2 One
        {
            get { return new Vector2(1.0); }
        }

        public static Vector2 Zero
        {
            get { return new Vector2(0.0); }
        }

        public static Vector2 UnitX
        {
            get { return new Vector2(1.0, 0.0); }
        }

        public static Vector2 UnitY
        {
            get { return new Vector2(0.0, 1.0); }
        }

        public const double RadiansToDegrees = 180.0 / Math.PI;

        public const double DegreesToRadians = Math.PI / 180.0;

        public static double ToRadians(double degrees)
        {
            return degrees * DegreesToRadians;
        }

        public static double ToDegrees(double radians)
        {
            return radians * RadiansToDegrees;
        }

        public Vector2(double value)
            : this()
        {
            this.X = value;
            this.Y = value;
        }

        public Vector2(double x, double y)
            : this()
        {
            this.X = x;
            this.Y = y;
        }

        public void Deconstruct(out double x, out double y)
        {
            x = this.X;
            y = this.Y;
        }

        public Vector2 Negate()
        {
            return new Vector2(-X, -Y);
        }

        public Vector2 Perpendicular()
        {
            return new Vector2(-Y, X);
        }

        public Vector2 Subtract(Vector2 v)
        {
            return new Vector2(X - v.X, Y - v.Y);
        }

        public Vector2 Add(Vector2 v)
        {
            return new Vector2(X + v.X, Y + v.Y);
        }

        public Vector2 Multiply(double scalar)
        {
            return new Vector2(X * scalar, Y * scalar);
        }

        public Vector2 Multiply(Vector2 v)
        {
            return new Vector2(X * v.X, Y * v.Y);
        }

        public Vector2 Divide(double scalar)
        {
            double value = 1.0 / scalar;
            return new Vector2(X * value, Y * value);
        }

        public Vector2 Divide(Vector2 v)
        {
            return new Vector2(X / v.X, Y / v.Y);
        }

        public double Dot(Vector2 v)
        {
            return (X * v.X) + (Y * v.Y);
        }

        public double Angle(Vector2 v)
        {
            return Math.Acos(Dot(v));
        }

        public double Cross(Vector2 v)
        {
            return (X * v.Y) - (Y * v.X);
        }

        public static bool operator ==(Vector2 v1, Vector2 v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y;
        }

        public static bool operator !=(Vector2 v1, Vector2 v2)
        {
            return v1.X != v2.X || v1.Y != v2.Y;
        }

        public static Vector2 operator -(Vector2 v)
        {
            return new Vector2(-v.X, -v.Y);
        }

        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2 operator *(Vector2 v, double scalar)
        {
            return new Vector2(v.X * scalar, v.Y * scalar);
        }

        public static Vector2 operator *(double scalar, Vector2 v)
        {
            return new Vector2(v.X * scalar, v.Y * scalar);
        }

        public static Vector2 operator *(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X * v2.X, v1.Y * v2.Y);
        }

        public static Vector2 operator /(Vector2 v, double scalar)
        {
            double value = 1.0 / scalar;
            return new Vector2(v.X * value, v.Y * value);
        }

        public static Vector2 operator /(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X / v2.X, v1.Y / v2.Y);
        }

        public double Magnitude()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        public double MagnitudeSquared()
        {
            return X * X + Y * Y;
        }

        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        public double LengthSquared()
        {
            return X * X + Y * Y;
        }

        public Vector2 Normalize()
        {
            return this / Length();
        }

        public double Component(Vector2 v)
        {
            return Dot(v.Normalize());
        }

        public Vector2 Project(Vector2 v)
        {
            return v * (Dot(v) / v.Dot(v));
        }

        public Vector2 Reflect(Vector2 normal)
        {
            double dot = Dot(normal);
            return new Vector2(X - 2.0 * dot * normal.X, Y - 2.0 * dot * normal.Y);
        }

        public Vector2 Min(Vector2 v)
        {
            return new Vector2(X < v.X ? X : v.X, Y < v.Y ? Y : v.Y);
        }

        public Vector2 Max(Vector2 v)
        {
            return new Vector2(X > v.X ? X : v.X, Y > v.Y ? Y : v.Y);
        }

        public Vector2 Lerp(Vector2 v, double amount)
        {
            return this + (v - this) * amount;
        }

        public Vector2 Slerp(Vector2 v, double amount)
        {
            double dot = Clamp(this.Dot(v), -1.0, 1.0);
            double theta = Math.Acos(dot) * amount;
            Vector2 relative = (v - this * dot).Normalize();
            return ((this * Math.Cos(theta)) + (relative * Math.Sin(theta)));
        }

        public Vector2 Nlerp(Vector2 v, double amount)
        {
            return Lerp(v, amount).Normalize();
        }

        public double Distance(Vector2 v)
        {
            double dx = X - v.X;
            double dy = Y - v.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public Vector2 Middle(Vector2 v)
        {
            return new Vector2((X + v.X) / 2.0, (Y + v.Y) / 2.0);
        }

        public Vector2 NearestOnLine(Vector2 a, Vector2 b)
        {
            return (this - a).Project(b - a) + a;
        }

        public static double Clamp(double value, double min, double max)
        {
            return value > max ? max : value < min ? min : value;
        }

        public static bool operator <(Vector2 v1, Vector2 v2)
        {
            return v1.X < v2.X || (v1.X == v2.X && v1.Y < v2.Y);
        }

        public static bool operator >(Vector2 v1, Vector2 v2)
        {
            return v1.X > v2.X || (v1.X == v2.X && v1.Y > v2.Y);
        }

        public int CompareTo(Vector2 other)
        {
            return (this > other) ? -1 : ((this < other) ? 1 : 0);
        }

        public bool Equals(Vector2 other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector2 ? Equals((Vector2)obj) : false;
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
