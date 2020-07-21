#nullable enable
using System;
using System.Globalization;
using Spatial;

namespace Spatial
{
    public struct MatrixD
    {
        public readonly decimal M11;
        public readonly decimal M12;
        public readonly decimal M21;
        public readonly decimal M22;
        public readonly decimal OffsetX;
        public readonly decimal OffsetY;

        public MatrixD(decimal m11, decimal m12, decimal m21, decimal m22, decimal offsetX, decimal offsetY)
        {
            this.M11 = m11;
            this.M12 = m12;
            this.M21 = m21;
            this.M22 = m22;
            this.OffsetX = offsetX;
            this.OffsetY = offsetY;
        }

        public void Deconstruct(out decimal m11, out decimal m12, out decimal m21, out decimal m22, out decimal offsetX, out decimal offsetY)
        {
            m11 = this.M11;
            m12 = this.M12;
            m21 = this.M21;
            m22 = this.M22;
            offsetX = this.OffsetX;
            offsetY = this.OffsetY;
        }

        public static readonly MatrixD Identity = new MatrixD(1m, 0m, 0m, 1m, 0m, 0m);

        public bool IsIdentity
        {
            get { return Equals(Identity); }
        }

        public bool HasInverse
        {
            get { return Determinant != 0m; }
        }

        public static MatrixD operator *(MatrixD value1, MatrixD value2)
        {
            return new MatrixD(
                (value1.M11 * value2.M11) + (value1.M12 * value2.M21),
                (value1.M11 * value2.M12) + (value1.M12 * value2.M22),
                (value1.M21 * value2.M11) + (value1.M22 * value2.M21),
                (value1.M21 * value2.M12) + (value1.M22 * value2.M22),
                (value1.OffsetX * value2.M11) + (value1.OffsetY * value2.M21) + value2.OffsetX,
                (value1.OffsetX * value2.M12) + (value1.OffsetY * value2.M22) + value2.OffsetY);
        }

        public static MatrixD operator -(MatrixD value)
        {
            return value.Invert();
        }

        public static bool operator ==(MatrixD value1, MatrixD value2)
        {
            return value1.Equals(value2);
        }

        public static bool operator !=(MatrixD value1, MatrixD value2)
        {
            return !value1.Equals(value2);
        }

        public static MatrixD Translate(decimal offsetX, decimal offsetY)
        {
            return new MatrixD(1m, 0m, 0m, 1m, offsetX, offsetY);
        }

        public static MatrixD TranslatePrepend(MatrixD matrix, decimal offsetX, decimal offsetY)
        {
            return Translate(offsetX, offsetY) * matrix;
        }

        public static MatrixD Scale(decimal scaleX, decimal scaleY)
        {
            return new MatrixD(scaleX, 0m, 0m, scaleY, 0m, 0m);
        }

        public static MatrixD ScaleAt(decimal scaleX, decimal scaleY, decimal centerX, decimal centerY)
        {
            return new MatrixD(scaleX, 0m, 0m, scaleY, centerX - (scaleX * centerX), centerY - (scaleY * centerY));
        }

        public static MatrixD ScaleAtPrepend(MatrixD matrix, decimal scaleX, decimal scaleY, decimal centerX, decimal centerY)
        {
            return ScaleAt(scaleX, scaleY, centerX, centerY) * matrix;
        }

        public static MatrixD Skew(decimal angleX, decimal angleY)
        {
            return new MatrixD(1m, (decimal)Math.Tan((double)angleX), (decimal)Math.Tan((double)angleY), 1m, 0m, 0m);
        }

        public static MatrixD Rotation(decimal radians)
        {
            decimal cos = (decimal)Math.Cos((double)radians);
            decimal sin = (decimal)Math.Sin((double)radians);
            return new MatrixD(cos, sin, -sin, cos, 0m, 0m);
        }

        public static MatrixD Rotation(decimal angle, decimal centerX, decimal centerY)
        {
            return Translate(-centerX, -centerY) * Rotation(angle) * Translate(centerX, centerY);
        }

        public static MatrixD Rotation(decimal angle, Vector2 center)
        {
            return Translate((decimal)-center.X, (decimal)-center.Y) * Rotation(angle) * Translate((decimal)center.X, (decimal)center.Y);
        }

        public static PointD TransformPoint(MatrixD matrix, PointD point)
        {
            return new PointD(
                (point.X * matrix.M11) + (point.Y * matrix.M21) + matrix.OffsetX,
                (point.X * matrix.M12) + (point.Y * matrix.M22) + matrix.OffsetY);
        }

        public decimal Determinant
        {
            get { return (M11 * M22) - (M12 * M21); }
        }

        public bool Equals(MatrixD other)
        {
            return
                M11 == other.M11 &&
                M12 == other.M12 &&
                M21 == other.M21 &&
                M22 == other.M22 &&
                OffsetX == other.OffsetX &&
                OffsetY == other.OffsetY;
        }

        public override bool Equals(object? obj)
        {
            return obj is MatrixD ? Equals((MatrixD)obj) : false;
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

        public MatrixD Invert()
        {
            if (Determinant == 0)
            {
                throw new InvalidOperationException("Transform is not invertible.");
            }
            decimal d = Determinant;
            return new MatrixD(
                M22 / d,
                -M12 / d,
                -M21 / d,
                M11 / d,
                ((M21 * OffsetY) - (M22 * OffsetX)) / d,
                ((M12 * OffsetX) - (M11 * OffsetY)) / d);
        }
    }
}
