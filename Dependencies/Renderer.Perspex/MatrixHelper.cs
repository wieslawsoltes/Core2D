// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Perspex;

namespace Dependencies
{
    /// <summary>
    /// Perspex Matrix helper methods.
    /// </summary>
    /// <remarks>
    /// Based on code from https://github.com/sharpdx/SharpDX/blob/master/Source/SharpDX.Mathematics/Matrix3x2.cs
    /// </remarks>
    internal static class MatrixHelper
    {
        /// <summary>
        /// Creates a matrix that rotates about a specified center.
        /// </summary>
        /// <param name="angle">Angle of rotation in radians.</param>
        /// <param name="center">The center of the rotation.</param>
        /// <returns>The created rotation matrix.</returns>
        public static Matrix Rotation(double angle, Vector center)
        {
            return
                Matrix.CreateTranslation(-center)
                * Matrix.CreateRotation(angle)
                * Matrix.CreateTranslation(center);
        }

        /// <summary>
        /// Transforms a point by this matrix.
        /// </summary>
        /// <param name="matrix">The matrix to use as a transformation matrix.</param>
        /// <param name="point">>The original point to apply the transformation.</param>
        /// <returns>The result of the transformation for the input point.</returns>
        public static Point TransformPoint(Matrix matrix, Point point)
        {
            return new Point(
                (point.X * matrix.M11) + (point.Y * matrix.M21) + matrix.M31,
                (point.X * matrix.M12) + (point.Y * matrix.M22) + matrix.M32);
        }
    }
}
