// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using SkiaSharp;
using static System.Math;

namespace Renderer.SkiaSharp
{
    /// <summary>
    /// SkiaSharp Matrix helper methods.
    /// </summary>
    public static class MatrixHelper
    {
        private static SKMatrix ToSKMatrix(double m11, double m12, double m21, double m22, double m31, double m32)
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

        /// <summary>
        /// Multiplies two matrices together and returns the resulting matrix.
        /// </summary>
        /// <param name="value1">The first source matrix.</param>
        /// <param name="value2">The second source matrix.</param>
        /// <returns>The product matrix.</returns>
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

        /// <summary>
        /// Gets the identity matrix.
        /// </summary>
        /// <value>The identity matrix.</value>
        public static readonly SKMatrix Identity = ToSKMatrix(1.0, 0.0, 0.0, 1.0, 0.0, 0.0);

        /// <summary>
        /// Creates a translation matrix using the specified offsets.
        /// </summary>
        /// <param name="offsetX">X-coordinate offset.</param>
        /// <param name="offsetY">Y-coordinate offset.</param>
        /// <returns>The created translation matrix.</returns>
        public static SKMatrix Translate(double offsetX, double offsetY)
        {
            return ToSKMatrix(1.0, 0.0, 0.0, 1.0, offsetX, offsetY);
        }

        /// <summary>
        /// Prepends a translation around the center of provided matrix.
        /// </summary>
        /// <param name="matrix">The matrix to prepend translation.</param>
        /// <param name="offsetX">X-coordinate offset.</param>
        /// <param name="offsetY">Y-coordinate offset.</param>
        /// <returns>The created translation matrix.</returns>
        public static SKMatrix TranslatePrepend(SKMatrix matrix, double offsetX, double offsetY)
        {
            return Multiply(Translate(offsetX, offsetY), matrix);
        }

        /// <summary>
        /// Creates a matrix that scales along the x-axis and y-axis.
        /// </summary>
        /// <param name="scaleX">Scaling factor that is applied along the x-axis.</param>
        /// <param name="scaleY">Scaling factor that is applied along the y-axis.</param>
        /// <returns>The created scaling matrix.</returns>
        public static SKMatrix Scale(double scaleX, double scaleY)
        {
            return ToSKMatrix(scaleX, 0, 0, scaleY, 0.0, 0.0);
        }

        /// <summary>
        /// Creates a matrix that is scaling from a specified center.
        /// </summary>
        /// <param name="scaleX">Scaling factor that is applied along the x-axis.</param>
        /// <param name="scaleY">Scaling factor that is applied along the y-axis.</param>
        /// <param name="centerX">The center X-coordinate of the scaling.</param>
        /// <param name="centerY">The center Y-coordinate of the scaling.</param>
        /// <returns>The created scaling matrix.</returns>
        public static SKMatrix ScaleAt(double scaleX, double scaleY, double centerX, double centerY)
        {
            return ToSKMatrix(scaleX, 0, 0, scaleY, centerX - scaleX * centerX, centerY - scaleY * centerY);
        }

        /// <summary>
        /// Prepends a scale around the center of provided matrix.
        /// </summary>
        /// <param name="matrix">The matrix to prepend scale.</param>
        /// <param name="scaleX">Scaling factor that is applied along the x-axis.</param>
        /// <param name="scaleY">Scaling factor that is applied along the y-axis.</param>
        /// <param name="centerX">The center X-coordinate of the scaling.</param>
        /// <param name="centerY">The center Y-coordinate of the scaling.</param>
        /// <returns>The created scaling matrix.</returns>
        public static SKMatrix ScaleAtPrepend(SKMatrix matrix, double scaleX, double scaleY, double centerX, double centerY)
        {
            return Multiply(ScaleAt(scaleX, scaleY, centerX, centerY), matrix);
        }

        /// <summary>
        /// Creates a skew matrix.
        /// </summary>
        /// <param name="angleX">Angle of skew along the X-axis in radians.</param>
        /// <param name="angleY">Angle of skew along the Y-axis in radians.</param>
        /// <returns>When the method completes, contains the created skew matrix.</returns>
        public static SKMatrix Skew(float angleX, float angleY)
        {
            return ToSKMatrix(1.0, Tan(angleX), Tan(angleY), 1.0, 0.0, 0.0);
        }

        /// <summary>
        /// Creates a matrix that rotates.
        /// </summary>
        /// <param name="radians">Angle of rotation in radians. Angles are measured clockwise when looking along the rotation axis.</param>
        /// <returns>The created rotation matrix.</returns>
        public static SKMatrix Rotation(double radians)
        {
            double cos = Cos(radians);
            double sin = Sin(radians);
            return ToSKMatrix(cos, sin, -sin, cos, 0, 0);
        }

        /// <summary>
        /// Creates a matrix that rotates about a specified center.
        /// </summary>
        /// <param name="angle">Angle of rotation in radians.</param>
        /// <param name="centerX">The center X-coordinate of the rotation.</param>
        /// <param name="centerY">The center Y-coordinate of the rotation.</param>
        /// <returns>The created rotation matrix.</returns>
        public static SKMatrix Rotation(double angle, double centerX, double centerY)
        {
            return Multiply(
                Multiply(
                    Translate(-centerX, -centerY), 
                    Rotation(angle)),
                Translate(centerX, centerY));
        }

        /// <summary>
        /// Creates a matrix that rotates about a specified center.
        /// </summary>
        /// <param name="angle">Angle of rotation in radians.</param>
        /// <param name="center">The center of the rotation.</param>
        /// <returns>The created rotation matrix.</returns>
        public static SKMatrix Rotation(double angle, SKPoint center)
        {
            return Multiply(
                Multiply(
                    Translate(-center.X, -center.Y), 
                    Rotation(angle)), 
                Translate(center.X, center.Y));
        }

        /// <summary>
        /// Transforms a point by this matrix.
        /// </summary>
        /// <param name="matrix">The matrix to use as a transformation matrix.</param>
        /// <param name="point">>The original point to apply the transformation.</param>
        /// <returns>The result of the transformation for the input point.</returns>
        public static SKPoint TransformPoint(SKMatrix matrix, SKPoint point)
        {
            return new SKPoint(
                (point.X * matrix.ScaleX) + (point.Y * matrix.SkewX) + matrix.TransX,
                (point.X * matrix.SkewY) + (point.Y * matrix.ScaleY) + matrix.TransY);
        }
    }
}
