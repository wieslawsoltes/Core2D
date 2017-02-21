// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Globalization;

namespace Core2D.Spatial
{
    public struct Matrix2
    {
        public readonly double M11;
        public readonly double M12;
        public readonly double M21;
        public readonly double M22;
        public readonly double OffsetX;
        public readonly double OffsetY;

        public Matrix2(double m11, double m12, double m21, double m22, double offsetX, double offsetY)
        {
            this.M11 = m11;
            this.M12 = m12;
            this.M21 = m21;
            this.M22 = m22;
            this.OffsetX = offsetX;
            this.OffsetY = offsetY;
        }

        public static readonly Matrix2 Identity = new Matrix2(1.0, 0.0, 0.0, 1.0, 0.0, 0.0);

        public bool IsIdentity
        {
            get { return Equals(Identity); }
        }

        public bool HasInverse
        {
            get { return Determinant != 0; }
        }

        public static Matrix2 operator *(Matrix2 value1, Matrix2 value2)
        {
            return new Matrix2(
                (value1.M11 * value2.M11) + (value1.M12 * value2.M21),
                (value1.M11 * value2.M12) + (value1.M12 * value2.M22),
                (value1.M21 * value2.M11) + (value1.M22 * value2.M21),
                (value1.M21 * value2.M12) + (value1.M22 * value2.M22),
                (value1.OffsetX * value2.M11) + (value1.OffsetY * value2.M21) + value2.OffsetX,
                (value1.OffsetX * value2.M12) + (value1.OffsetY * value2.M22) + value2.OffsetY);
        }

        public static Matrix2 operator -(Matrix2 value)
        {
            return value.Invert();
        }

        public static bool operator ==(Matrix2 value1, Matrix2 value2)
        {
            return value1.Equals(value2);
        }

        public static bool operator !=(Matrix2 value1, Matrix2 value2)
        {
            return !value1.Equals(value2);
        }

        public static Matrix2 Translate(double offsetX, double offsetY)
        {
            return new Matrix2(1.0, 0.0, 0.0, 1.0, offsetX, offsetY);
        }

        public static Matrix2 TranslatePrepend(Matrix2 matrix, double offsetX, double offsetY)
        {
            return Translate(offsetX, offsetY) * matrix;
        }

        public static Matrix2 Scale(double scaleX, double scaleY)
        {
            return new Matrix2(scaleX, 0, 0, scaleY, 0.0, 0.0);
        }

        public static Matrix2 ScaleAt(double scaleX, double scaleY, double centerX, double centerY)
        {
            return new Matrix2(scaleX, 0, 0, scaleY, centerX - scaleX * centerX, centerY - scaleY * centerY);
        }

        public static Matrix2 ScaleAtPrepend(Matrix2 matrix, double scaleX, double scaleY, double centerX, double centerY)
        {
            return ScaleAt(scaleX, scaleY, centerX, centerY) * matrix;
        }

        public static Matrix2 Skew(float angleX, float angleY)
        {
            return new Matrix2(1.0, (double)Math.Tan(angleX), (double)Math.Tan(angleY), 1.0, 0.0, 0.0);
        }

        public static Matrix2 Rotation(double radians)
        {
            double cos = (double)Math.Cos(radians);
            double sin = (double)Math.Sin(radians);
            return new Matrix2(cos, sin, -sin, cos, 0, 0);
        }

        public static Matrix2 Rotation(double angle, double centerX, double centerY)
        {
            return Translate(-centerX, -centerY) * Rotation(angle) * Translate(centerX, centerY);
        }

        public static Matrix2 Rotation(double angle, Vector2 center)
        {
            return Translate(-center.X, -center.Y) * Rotation(angle) * Translate(center.X, center.Y);
        }

        public static Point2 TransformPoint(Matrix2 matrix, Point2 point)
        {
            return new Point2(
                (point.X * matrix.M11) + (point.Y * matrix.M21) + matrix.OffsetX,
                (point.X * matrix.M12) + (point.Y * matrix.M22) + matrix.OffsetY);
        }

        public double Determinant
        {
            get { return (M11 * M22) - (M12 * M21); }
        }

        public bool Equals(Matrix2 other)
        {
            return
                M11 == other.M11 &&
                M12 == other.M12 &&
                M21 == other.M21 &&
                M22 == other.M22 &&
                OffsetX == other.OffsetX &&
                OffsetY == other.OffsetY;
        }

        public override bool Equals(object obj)
        {
            return obj is Matrix2 ? Equals((Matrix2)obj) : false;
        }

        public override int GetHashCode()
        {
            return
                M11.GetHashCode() + M12.GetHashCode() +
                M21.GetHashCode() + M22.GetHashCode() +
                OffsetY.GetHashCode() + OffsetY.GetHashCode();
        }

        public override string ToString()
        {
            var ci = CultureInfo.CurrentCulture;
            return string.Format(
                ci,
                "{{ {{M11:{0} M12:{1}}} {{M21:{2} M22:{3}}} {{M31:{4} M32:{5}}} }}",
                M11.ToString(ci),
                M12.ToString(ci),
                M21.ToString(ci),
                M22.ToString(ci),
                OffsetX.ToString(ci),
                OffsetY.ToString(ci));
        }

        public Matrix2 Invert()
        {
            if (Determinant == 0)
            {
                throw new InvalidOperationException("Transform is not invertible.");
            }
            double d = Determinant;
            return new Matrix2(
                M22 / d,
                -M12 / d,
                -M21 / d,
                M11 / d,
                ((M21 * OffsetY) - (M22 * OffsetX)) / d,
                ((M12 * OffsetX) - (M11 * OffsetY)) / d);
        }
    }
}
