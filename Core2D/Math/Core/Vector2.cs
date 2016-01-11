// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Globalization;

namespace Core2D
{
    /// <summary>
    /// 
    /// </summary>
    public struct Vector2 : IComparable<Vector2>
    {
        /// <summary>
        /// 
        /// </summary>
        public double X { get; }

        /// <summary>
        /// 
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// 
        /// </summary>
        public static Vector2 One => new Vector2(1.0);

        /// <summary>
        /// 
        /// </summary>
        public static Vector2 Zero => new Vector2(0.0);

        /// <summary>
        /// 
        /// </summary>
        public static Vector2 UnitX => new Vector2(1.0, 0.0);

        /// <summary>
        /// 
        /// </summary>
        public static Vector2 UnitY => new Vector2(0.0, 1.0);

        /// <summary>
        /// Initializes a new <see cref="Vector2"/> instance.
        /// </summary>
        /// <param name="value"></param>
        public Vector2(double value)
            : this()
        {
            X = value;
            Y = value;
        }

        /// <summary>
        /// Initializes a new <see cref="Vector2"/> instance.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vector2(double x, double y)
            : this()
        {
            X = x;
            Y = y;
        }

        /// <inheritdoc/>
        public override string ToString() => string.Concat(X, CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, Y);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator <(Vector2 v1, Vector2 v2) => v1.X < v2.X || (v1.X == v2.X && v1.Y < v2.Y);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator >(Vector2 v1, Vector2 v2) => v1.X > v2.X || (v1.X == v2.X && v1.Y > v2.Y);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public int CompareTo(Vector2 v) => (this > v) ? -1 : ((this < v) ? 1 : 0);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public bool Equals(Vector2 v) => X == v.X && Y == v.Y;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => obj is Vector2 ? Equals((Vector2)obj) : false;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => X.GetHashCode() + Y.GetHashCode();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector2 Negate() => new Vector2(-X, -Y);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector2 Perpendicular() => new Vector2(-Y, X);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector2 Subtract(Vector2 v) => new Vector2(X - v.X, Y - v.Y);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector2 Add(Vector2 v) => new Vector2(X + v.X, Y + v.Y);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public Vector2 Multiply(double scalar) => new Vector2(X * scalar, Y * scalar);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector2 Multiply(Vector2 v) => new Vector2(X * v.X, Y * v.Y);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public Vector2 Divide(double scalar)
        {
            double value = 1.0 / scalar;
            return new Vector2(X * value, Y * value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector2 Divide(Vector2 v) => new Vector2(X / v.X, Y / v.Y);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public double Dot(Vector2 v) => (X * v.X) + (Y * v.Y);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public double Angle(Vector2 v) => Math.Acos(Dot(v));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public double Cross(Vector2 v) => (X * v.Y) - (Y * v.X);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator ==(Vector2 v1, Vector2 v2) => v1.X == v2.X && v1.Y == v2.Y;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator !=(Vector2 v1, Vector2 v2) => v1.X != v2.X || v1.Y != v2.Y;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2 operator -(Vector2 v) => new Vector2(-v.X, -v.Y);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector2 operator -(Vector2 v1, Vector2 v2) => new Vector2(v1.X - v2.X, v1.Y - v2.Y);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector2 operator +(Vector2 v1, Vector2 v2) => new Vector2(v1.X + v2.X, v1.Y + v2.Y);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vector2 operator *(Vector2 v, double scalar) => new Vector2(v.X * scalar, v.Y * scalar);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2 operator *(double scalar, Vector2 v) => new Vector2(v.X * scalar, v.Y * scalar);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector2 operator *(Vector2 v1, Vector2 v2) => new Vector2(v1.X * v2.X, v1.Y * v2.Y);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vector2 operator /(Vector2 v, double scalar)
        {
            double value = 1.0 / scalar;
            return new Vector2(v.X * value, v.Y * value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector2 operator /(Vector2 v1, Vector2 v2) => new Vector2(v1.X / v2.X, v1.Y / v2.Y);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double Magnitude() => Math.Sqrt(X * X + Y * Y);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double MagnitudeSquared() => X * X + Y * Y;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double Length() => Math.Sqrt(X * X + Y * Y);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double LengthSquared() => X * X + Y * Y;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector2 Normalize() => this / Length();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public double Component(Vector2 v) => Dot(v.Normalize());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <remarks>
        /// Alternative implementation: v.Normalize() * Component(v)
        /// </remarks>
        /// <returns></returns>
        public Vector2 Project(Vector2 v) => v * (Dot(v) / v.Dot(v));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="normal"></param>
        /// <returns></returns>
        public Vector2 Reflect(Vector2 normal)
        {
            double dot = Dot(normal);
            return new Vector2(X - 2.0 * dot * normal.X, Y - 2.0 * dot * normal.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector2 Min(Vector2 v) => new Vector2(X < v.X ? X : v.X, Y < v.Y ? Y : v.Y);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector2 Max(Vector2 v) => new Vector2(X > v.X ? X : v.X, Y > v.Y ? Y : v.Y);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public Vector2 Lerp(Vector2 v, double amount) => this + (v - this) * amount;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public Vector2 Slerp(Vector2 v, double amount)
        {
            double dot = Clamp(this.Dot(v), -1.0, 1.0);
            double theta = Math.Acos(dot) * amount;
            Vector2 relative = (v - this * dot).Normalize();
            return ((this * Math.Cos(theta)) + (relative * Math.Sin(theta)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public Vector2 Nlerp(Vector2 v, double amount) => Lerp(v, amount).Normalize();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public double Distance(Vector2 v)
        {
            double dx = X - v.X;
            double dy = Y - v.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector2 Middle(Vector2 v) => new Vector2((X + v.X) / 2.0, (Y + v.Y) / 2.0);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public Vector2 NearestPointOnLine(Vector2 a, Vector2 b) => (this - a).Project(b - a) + a;

        /// <summary>
        /// 
        /// </summary>
        public const double RadiansToDegrees = 180.0 / Math.PI;

        /// <summary>
        /// 
        /// </summary>
        public const double DegreesToRadians = Math.PI / 180.0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static double Clamp(double value, double min, double max) => value > max ? max : value < min ? min : value;
    }
}
