// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public struct Vector2 : IComparable<Vector2>
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public double X { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public double Y { get; private set; }

        #endregion

        #region Vectors

        /// <summary>
        /// 
        /// </summary>
        public static Vector2 One { get { return new Vector2(1.0); } }
        /// <summary>
        /// 
        /// </summary>
        public static Vector2 Zero { get { return new Vector2(0.0); } }
        /// <summary>
        /// 
        /// </summary>
        public static Vector2 UnitX { get { return new Vector2(1.0, 0.0); } }
        /// <summary>
        /// 
        /// </summary>
        public static Vector2 UnitY { get { return new Vector2(0.0, 1.0); } }

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public Vector2(double value)
            : this()
        {
            this.X = value;
            this.Y = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vector2(double x, double y)
            : this()
        {
            this.X = x;
            this.Y = y;
        }

        #endregion

        #region ToString

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Concat(X, System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, Y);
        }

        #endregion

        #region IComparable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator <(Vector2 v1, Vector2 v2)
        {
            return v1.X < v2.X || (v1.X == v2.X && v1.Y < v2.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator >(Vector2 v1, Vector2 v2)
        {
            return v1.X > v2.X || (v1.X == v2.X && v1.Y > v2.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public int CompareTo(Vector2 v)
        {
            return (this > v) ? -1 : ((this < v) ? 1 : 0);
        } 

        #endregion

        #region Equals

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public bool Equals(Vector2 v)
        {
            return this.X == v.X && this.Y == v.Y;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Vector2)
            {
                return this.Equals((Vector2)obj);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.X.GetHashCode() + this.Y.GetHashCode();
        }

        #endregion

        #region Arithmetic

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector2 Negate()
        {
            return new Vector2(-this.X, -this.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector2 Perpendicular()
        {
            return new Vector2(-this.Y, this.X);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector2 Subtract(Vector2 v)
        {
            return new Vector2(this.X - v.X, this.Y - v.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector2 Add(Vector2 v)
        {
            return new Vector2(this.X + v.X, this.Y + v.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public Vector2 Multiply(double scalar)
        {
            return new Vector2(
                this.X * scalar,
                this.Y * scalar);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector2 Multiply(Vector2 v)
        {
            return new Vector2(
                this.X * v.X,
                this.Y * v.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public Vector2 Divide(double scalar)
        {
            double value = 1.0 / scalar;
            return new Vector2(
                this.X * value,
                this.Y * value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector2 Divide(Vector2 v)
        {
            return new Vector2(
                this.X / v.X,
                this.Y / v.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public double Dot(Vector2 v)
        {
            return (this.X * v.X) + (this.Y * v.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public double Angle(Vector2 v)
        {
            return Math.Acos(this.Dot(v));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public double Cross(Vector2 v)
        {
            return (this.X * v.Y) - (this.Y * v.X);
        }

        #endregion

        #region Operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator ==(Vector2 v1, Vector2 v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator !=(Vector2 v1, Vector2 v2)
        {
            return v1.X != v2.X || v1.Y != v2.Y;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2 operator -(Vector2 v)
        {
            return new Vector2(-v.X, -v.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X - v2.X, v1.Y - v2.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X + v2.X, v1.Y + v2.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vector2 operator *(Vector2 v, double scalar)
        {
            return new Vector2(
                v.X * scalar,
                v.Y * scalar);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2 operator *(double scalar, Vector2 v)
        {
            return new Vector2(
                v.X * scalar,
                v.Y * scalar);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector2 operator *(Vector2 v1, Vector2 v2)
        {
            return new Vector2(
                v1.X * v2.X,
                v1.Y * v2.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vector2 operator /(Vector2 v, double scalar)
        {
            double value = 1.0 / scalar;
            return new Vector2(
                v.X * value,
                v.Y * value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector2 operator /(Vector2 v1, Vector2 v2)
        {
            return new Vector2(
                v1.X / v2.X,
                v1.Y / v2.Y);
        }

        #endregion

        #region Vector

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double Magnitude()
        {
            return Math.Sqrt(this.X * this.X + this.Y * this.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double MagnitudeSquared()
        {
            return this.X * this.X + this.Y * this.Y;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double Length()
        {
            return Math.Sqrt(this.X * this.X + this.Y * this.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double LengthSquared()
        {
            return this.X * this.X + this.Y * this.Y;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector2 Normalize()
        {
            return this / this.Length();
        }

        //public double Component(Vector2 v)
        //{
        //    return this.Dot(v.Normalize());
        //}

        //public Vector2 Project(Vector2 v)
        //{
        //    return v.Normalize() * this.Component(v);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector2 Project(Vector2 v)
        {
            return v * (this.Dot(v) / v.Dot(v));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="normal"></param>
        /// <returns></returns>
        public Vector2 Reflect(Vector2 normal)
        {
            double dot = this.Dot(normal);
            return new Vector2(
                this.X - 2.0 * dot * normal.X,
                this.Y - 2.0 * dot * normal.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector2 Min(Vector2 v)
        {
            return new Vector2(
                this.X < v.X ? this.X : v.X,
                this.Y < v.Y ? this.Y : v.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector2 Max(Vector2 v)
        {
            return new Vector2(
                this.X > v.X ? this.X : v.X,
                this.Y > v.Y ? this.Y : v.Y);
        }

        #endregion

        #region Interpolation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public Vector2 Lerp(Vector2 v, double amount)
        {
            return this + (v - this) * amount;
        }

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
        public Vector2 Nlerp(Vector2 v, double amount)
        {
            return this.Lerp(v, amount).Normalize();
        }

        #endregion

        #region Point

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public double Distance(Vector2 v)
        {
            double dx = this.X - v.X;
            double dy = this.Y - v.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector2 Middle(Vector2 v)
        {
            return new Vector2(
                (this.X + v.X) / 2.0,
                (this.Y + v.Y) / 2.0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public Vector2 NearestPointOnLine(Vector2 a, Vector2 b)
        {
            return (this - a).Project(b - a) + a;
        }

        #endregion

        #region Math

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
        public static double Clamp(double value, double min, double max)
        {
            return value > max ? max : value < min ? min : value;
        }

        #endregion
    }
}
