using SkiaSharp;
using static System.Math;

namespace Core2D.Renderer.SkiaSharp
{
    public static class MatrixHelper
    {
        public static SKMatrix ToSKMatrix(double m11, double m12, double m21, double m22, double m31, double m32)
        {
            return new SKMatrix
            {
                ScaleX = (float)m11,
                SkewX = (float)m21,
                TransX = (float)m31,
                SkewY = (float)m12,
                ScaleY = (float)m22,
                TransY = (float)m32,
                Persp0 = 0,
                Persp1 = 0,
                Persp2 = 1
            };
        }

        public static SKMatrix Multiply(SKMatrix value1, SKMatrix value2)
        {
            return ToSKMatrix(
                (value1.ScaleX * value2.ScaleX) + (value1.SkewY * value2.SkewX),
                (value1.ScaleX * value2.SkewY) + (value1.SkewY * value2.ScaleY),
                (value1.SkewX * value2.ScaleX) + (value1.ScaleY * value2.SkewX),
                (value1.SkewX * value2.SkewY) + (value1.ScaleY * value2.ScaleY),
                (value1.TransX * value2.ScaleX) + (value1.TransY * value2.SkewX) + value2.TransX,
                (value1.TransX * value2.SkewY) + (value1.TransY * value2.ScaleY) + value2.TransY);
        }

        public static readonly SKMatrix Identity = ToSKMatrix(1.0, 0.0, 0.0, 1.0, 0.0, 0.0);

        public static SKMatrix Translate(double offsetX, double offsetY)
        {
            return ToSKMatrix(1.0, 0.0, 0.0, 1.0, offsetX, offsetY);
        }

        public static SKMatrix TranslatePrepend(SKMatrix matrix, double offsetX, double offsetY)
        {
            return Multiply(Translate(offsetX, offsetY), matrix);
        }

        public static SKMatrix Scale(double scaleX, double scaleY)
        {
            return ToSKMatrix(scaleX, 0, 0, scaleY, 0.0, 0.0);
        }

        public static SKMatrix ScaleAt(double scaleX, double scaleY, double centerX, double centerY)
        {
            return ToSKMatrix(scaleX, 0, 0, scaleY, centerX - scaleX * centerX, centerY - scaleY * centerY);
        }

        public static SKMatrix ScaleAtPrepend(SKMatrix matrix, double scaleX, double scaleY, double centerX, double centerY)
        {
            return Multiply(ScaleAt(scaleX, scaleY, centerX, centerY), matrix);
        }

        public static SKMatrix Skew(float angleX, float angleY)
        {
            return ToSKMatrix(1.0, Tan(angleX), Tan(angleY), 1.0, 0.0, 0.0);
        }

        public static SKMatrix Rotation(double radians)
        {
            double cos = Cos(radians);
            double sin = Sin(radians);
            return ToSKMatrix(cos, sin, -sin, cos, 0, 0);
        }

        public static SKMatrix Rotation(double angle, double centerX, double centerY)
        {
            return Multiply(
                Multiply(
                    Translate(-centerX, -centerY),
                    Rotation(angle)),
                Translate(centerX, centerY));
        }

        public static SKMatrix Rotation(double angle, SKPoint center)
        {
            return Multiply(
                Multiply(
                    Translate(-center.X, -center.Y),
                    Rotation(angle)),
                Translate(center.X, center.Y));
        }

        public static SKPoint TransformPoint(SKMatrix matrix, SKPoint point)
        {
            return new SKPoint(
                (point.X * matrix.ScaleX) + (point.Y * matrix.SkewX) + matrix.TransX,
                (point.X * matrix.SkewY) + (point.Y * matrix.ScaleY) + matrix.TransY);
        }
    }
}
