#region netDxf, Copyright(C) 2015 Daniel Carvajal, Licensed under LGPL.
// 
//                         netDxf library
//  Copyright (C) 2009-2015 Daniel Carvajal (haplokuon@gmail.com)
//  
//  This library is free software; you can redistribute it and/or
//  modify it under the terms of the GNU Lesser General Public
//  License as published by the Free Software Foundation; either
//  version 2.1 of the License, or (at your option) any later version.
//  
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//  FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//  COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//  IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//  CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Text;
using System.Threading;

namespace netDxf
{
    /// <summary>
    /// Represents a 3x3 double precision matrix.
    /// </summary>
    public struct Matrix3
    {
        #region private fields

        private double mM11;
        private double mM12;
        private double mM13;
        private double mM21;
        private double mM22;
        private double mM23;
        private double mM31;
        private double mM32;
        private double mM33;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of Matrix3.
        /// </summary>
        /// <param name="m11">Element [0,0].</param>
        /// <param name="m12">Element [0,1].</param>
        /// <param name="m13">Element [0,1].</param>
        /// <param name="m21">Element [1,0].</param>
        /// <param name="m22">Element [1,1].</param>
        /// <param name="m23">Element [1,2].</param>
        /// <param name="m31">Element [2,0].</param>
        /// <param name="m32">Element [2,1].</param>
        /// <param name="m33">Element [2,2].</param>
        public Matrix3(double m11, double m12, double m13, double m21, double m22, double m23, double m31, double m32, double m33)
        {
            this.mM11 = m11;
            this.mM12 = m12;
            this.mM13 = m13;

            this.mM21 = m21;
            this.mM22 = m22;
            this.mM23 = m23;

            this.mM31 = m31;
            this.mM32 = m32;
            this.mM33 = m33;
        }

        #endregion

        #region constants

        /// <summary>
        /// Gets the zero matrix.
        /// </summary>
        public static Matrix3 Zero
        {
            get { return new Matrix3(0, 0, 0, 0, 0, 0, 0, 0, 0); }
        }

        /// <summary>
        /// Gets the identity matrix.
        /// </summary>
        public static Matrix3 Identity
        {
            get { return new Matrix3(1, 0, 0, 0, 1, 0, 0, 0, 1); }
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the matrix element [0,0].
        /// </summary>
        public double M11
        {
            get { return this.mM11; }
            set { this.mM11 = value; }
        }

        /// <summary>
        /// Gets or sets the matrix element [0,1].
        /// </summary>
        public double M12
        {
            get { return this.mM12; }
            set { this.mM12 = value; }
        }

        /// <summary>
        /// Gets or sets the matrix element [0,2].
        /// </summary>
        public double M13
        {
            get { return this.mM13; }
            set { this.mM13 = value; }
        }

        /// <summary>
        /// Gets or sets the matrix element [1,0].
        /// </summary>
        public double M21
        {
            get { return this.mM21; }
            set { this.mM21 = value; }
        }

        /// <summary>
        /// Gets or sets the matrix element [1,1].
        /// </summary>
        public double M22
        {
            get { return this.mM22; }
            set { this.mM22 = value; }
        }

        /// <summary>
        /// Gets or sets the matrix element [1,2].
        /// </summary>
        public double M23
        {
            get { return this.mM23; }
            set { this.mM23 = value; }
        }

        /// <summary>
        /// Gets or sets the matrix element [2,0].
        /// </summary>
        public double M31
        {
            get { return this.mM31; }
            set { this.mM31 = value; }
        }

        /// <summary>
        /// Gets or sets the matrix element [2,1].
        /// </summary>
        public double M32
        {
            get { return this.mM32; }
            set { this.mM32 = value; }
        }

        /// <summary>
        /// Gets or sets the matrix element [2,2].
        /// </summary>
        public double M33
        {
            get { return this.mM33; }
            set { this.mM33 = value; }
        }

        #endregion

        #region overloaded operators

        /// <summary>
        /// Matrix addition.
        /// </summary>
        /// <param name="a">Matrix3.</param>
        /// <param name="b">Matrix3.</param>
        /// <returns>Matrix3.</returns>
        public static Matrix3 operator +(Matrix3 a, Matrix3 b)
        {
            return new Matrix3(a.M11 + b.M11, a.M12 + b.M12, a.M13 + b.M13, a.M21 + b.M21, a.M22 + b.M22, a.M23 + b.M23, a.M31 + b.M31, a.M32 + b.M32, a.M33 + b.M33);
        }

        /// <summary>
        /// Matrix subtraction.
        /// </summary>
        /// <param name="a">Matrix3.</param>
        /// <param name="b">Matrix3.</param>
        /// <returns>Matrix3.</returns>
        public static Matrix3 operator -(Matrix3 a, Matrix3 b)
        {
            return new Matrix3(a.M11 - b.M11, a.M12 - b.M12, a.M13 - b.M13, a.M21 - b.M21, a.M22 - b.M22, a.M23 - b.M23, a.M31 - b.M31, a.M32 - b.M32, a.M33 - b.M33);
        }

        /// <summary>
        /// Product of two matrices.
        /// </summary>
        /// <param name="a">Matrix3.</param>
        /// <param name="b">Matrix3.</param>
        /// <returns>Matrix3.</returns>
        public static Matrix3 operator *(Matrix3 a, Matrix3 b)
        {
            return new Matrix3(a.M11*b.M11 + a.M12*b.M21 + a.M13*b.M31, a.M11*b.M12 + a.M12*b.M22 + a.M13*b.M32, a.M11*b.M13 + a.M12*b.M23 + a.M13*b.M33, a.M21*b.M11 + a.M22*b.M21 + a.M23*b.M31,
                               a.M21*b.M12 + a.M22*b.M22 + a.M23*b.M32, a.M21*b.M13 + a.M22*b.M23 + a.M23*b.M33, a.M31*b.M11 + a.M32*b.M21 + a.M33*b.M31, a.M31*b.M12 + a.M32*b.M22 + a.M33*b.M32,
                               a.M31*b.M13 + a.M32*b.M23 + a.M33*b.M33);
        }

        /// <summary>
        /// Product of a matrix with a vector.
        /// </summary>
        /// <param name="a">Matrix3.</param>
        /// <param name="u">Vector3d.</param>
        /// <returns>Matrix3.</returns>
        /// <remarks>Matrix3 adopts the convention of using column vectors to represent three dimensional points.</remarks>
        public static Vector3 operator *(Matrix3 a, Vector3 u)
        {
            return new Vector3(a.M11*u.X + a.M12*u.Y + a.M13*u.Z, a.M21*u.X + a.M22*u.Y + a.M23*u.Z, a.M31*u.X + a.M32*u.Y + a.M33*u.Z);
        }

        /// <summary>
        /// Product of a matrix with a scalar.
        /// </summary>
        /// <param name="m">Matrix3.</param>
        /// <param name="a">Scalar.</param>
        /// <returns>Matrix3.</returns>
        public static Matrix3 operator *(Matrix3 m, double a)
        {
            return new Matrix3(m.M11*a, m.M12*a, m.M13*a, m.M21*a, m.M22*a, m.M23*a, m.M31*a, m.M32*a, m.M33*a);
        }

        #endregion

        #region public methods

        /// <summary>
        /// Calculate the determinant of the actual matrix.
        /// </summary>
        /// <returns>Determinant.</returns>
        public double Determinant()
        {
            return this.mM11*this.mM22*this.mM33 + this.mM12*this.mM23*this.mM31 + this.mM13*this.mM21*this.mM32 - this.mM13*this.mM22*this.mM31 - this.mM11*this.mM23*this.mM32 - this.mM12*this.mM21*this.mM33;
        }

        /// <summary>
        /// Calculates the inverse matrix.
        /// </summary>
        /// <returns>Inverse Matrix3.</returns>
        public Matrix3 Inverse()
        {
            double det = this.Determinant();
            Matrix3 resultado = new Matrix3();
            if (MathHelper.IsZero(det))
            {
                throw new ArithmeticException("The matrix is not invertible.");
            }
            det = 1/det;

            resultado.M11 = det*(this.mM22*this.mM33 - this.mM23*this.mM32);
            resultado.M12 = det*(this.mM13*this.mM32 - this.mM12*this.mM33);
            resultado.M13 = det*(this.mM12*this.mM23 - this.mM13*this.mM22);

            resultado.M21 = det*(this.mM23*this.mM31 - this.mM21*this.mM33);
            resultado.M22 = det*(this.mM11*this.mM33 - this.mM13*this.mM31);
            resultado.M23 = det*(this.mM13*this.mM21 - this.mM11*this.mM23);

            resultado.M31 = det*(this.mM21*this.mM32 - this.mM22*this.mM31);
            resultado.M32 = det*(this.mM12*this.mM31 - this.mM11*this.mM32);
            resultado.M33 = det*(this.mM11*this.mM22 - this.mM12*this.mM21);

            return resultado;
        }

        /// <summary>
        /// Obtains the transpose matrix.
        /// </summary>
        /// <returns>Transpose matrix.</returns>
        public Matrix3 Traspose()
        {
            return new Matrix3(this.mM11, this.mM21, this.mM31, this.mM12, this.mM22, this.mM32, this.mM13, this.mM23, this.mM33);
        }

        #endregion

        #region static methods

        /// <summary>
        /// Builds a rotation matrix for a rotation around the x-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting Matrix3 instance.</returns>
        /// <remarks>Matrix3 adopts the convention of using column vectors to represent three dimensional points.</remarks>
        public static Matrix3 RotationX(double angle)
        {
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            return new Matrix3(1, 0, 0, 0, cos, -sin, 0, sin, cos);
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the y-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting Matrix3 instance.</returns>
        /// <remarks>Matrix3 adopts the convention of using column vectors to represent three dimensional points.</remarks>
        public static Matrix3 RotationY(double angle)
        {
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            return new Matrix3(cos, 0, sin, 0, 1, 0, -sin, 0, cos);
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the z-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting Matrix3 instance.</returns>
        /// <remarks>Matrix3 adopts the convention of using column vectors to represent three dimensional points.</remarks>
        public static Matrix3 RotationZ(double angle)
        {
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            return new Matrix3(cos, -sin, 0, sin, cos, 0, 0, 0, 1);
        }

        /// <summary>
        /// Build a scaling matrix.
        /// </summary>
        /// <param name="scale">Single scale factor for x, y, and z axes.</param>
        /// <returns>A scaling matrix.</returns>
        public static Matrix3 Scale(double scale)
        {
            return Scale(scale, scale, scale);
        }

        /// <summary>
        /// Build a scaling matrix.
        /// </summary>
        /// <param name="scale">Scale factors for x, y, and z axes.</param>
        /// <returns>A scaling matrix.</returns>
        public static Matrix3 Scale(Vector3 scale)
        {
            return Scale(scale.X, scale.Y, scale.Z);
        }

        /// <summary>
        /// Build a scaling matrix.
        /// </summary>
        /// <param name="x">Scale factor for x-axis.</param>
        /// <param name="y">Scale factor for y-axis.</param>
        /// <param name="z">Scale factor for z-axis.</param>
        /// <returns>A scaling matrix.</returns>
        public static Matrix3 Scale(double x, double y, double z)
        {
            return new Matrix3(x, 0, 0, 0, y, 0, 0, 0, z);
        }

        #endregion

        #region overrides

        /// <summary>
        /// Obtains a string that represents the matrix.
        /// </summary>
        /// <returns>A string text.</returns>
        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append(string.Format("|{0}{3}{1}{3}{2}|" + Environment.NewLine, this.mM11, this.mM12, this.mM13, Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator));
            s.Append(string.Format("|{0}{3}{1}{3}{2}|" + Environment.NewLine, this.mM21, this.mM22, this.mM23, Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator));
            s.Append(string.Format("|{0}{3}{1}{3}{2}|" + Environment.NewLine, this.mM31, this.mM32, this.mM33, Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator));
            return s.ToString();
        }

        /// <summary>
        /// Obtains a string that represents the matrix.
        /// </summary>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information. </param>
        /// <returns>A string text.</returns>
        public string ToString(IFormatProvider provider)
        {
            string separator = Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator;
            StringBuilder s = new StringBuilder();
            s.Append(string.Format("|{0}{3}{1}{3}{2}|" + Environment.NewLine, this.mM11.ToString(provider), this.mM12.ToString(provider), this.mM13.ToString(provider), separator));
            s.Append(string.Format("|{0}{3}{1}{3}{2}|" + Environment.NewLine, this.mM21.ToString(provider), this.mM22.ToString(provider), this.mM23.ToString(provider), separator));
            s.Append(string.Format("|{0}{3}{1}{3}{2}|" + Environment.NewLine, this.mM31.ToString(provider), this.mM32.ToString(provider), this.mM33.ToString(provider), separator));
            return s.ToString();
        }

        #endregion
    }
}