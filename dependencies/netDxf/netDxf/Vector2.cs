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
using System.Collections.Generic;
using System.Threading;

namespace netDxf
{
    /// <summary>
    /// Represent a two component vector of double precision.
    /// </summary>
    public struct Vector2 :
        IEquatable<Vector2>
    {
        #region private fields

        private double x;
        private double y;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of Vector3.
        /// </summary>
        /// <param name="value">X, Y component.</param>
        public Vector2(double value)
        {
            this.x = value;
            this.y = value;
        }

        /// <summary>
        /// Initializes a new instance of Vector2.
        /// </summary>
        /// <param name="x">X component.</param>
        /// <param name="y">Y component.</param>
        public Vector2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Initializes a new instance of Vector2.
        /// </summary>
        /// <param name="array">Array of two elements that represents the vector.</param>
        public Vector2(IList<double> array)
        {
            if (array.Count != 2)
                throw new ArgumentOutOfRangeException(nameof(array), array.Count, "The dimension of the array must be two");
            this.x = array[0];
            this.y = array[1];
        }

        #endregion

        #region constants

        /// <summary>
        /// Zero vector.
        /// </summary>
        public static Vector2 Zero
        {
            get { return new Vector2(0, 0); }
        }

        /// <summary>
        /// Unit X vector.
        /// </summary>
        public static Vector2 UnitX
        {
            get { return new Vector2(1, 0); }
        }

        /// <summary>
        /// Unit Y vector.
        /// </summary>
        public static Vector2 UnitY
        {
            get { return new Vector2(0, 1); }
        }

        /// <summary>
        /// Represents a vector with not a number components.
        /// </summary>
        public static Vector2 NaN
        {
            get { return new Vector2(double.NaN, double.NaN); }
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the X component.
        /// </summary>
        public double X
        {
            get { return this.x; }
            set { this.x = value; }
        }

        /// <summary>
        /// Gets or sets the Y component.
        /// </summary>
        public double Y
        {
            get { return this.y; }
            set { this.y = value; }
        }

        /// <summary>
        /// Gets or sets a vector element defined by its index.
        /// </summary>
        /// <param name="index">Index of the element.</param>
        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.x;
                    case 1:

                        return this.y;
                    default:

                        throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.x = value;
                        break;
                    case 1:

                        this.y = value;
                        break;
                    default:

                        throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
        }

        #endregion

        #region static methods

        /// <summary>
        ///  Returns a value indicating if any component of the specified vector evaluates to a value that is not a number <see cref="System.Double.NaN"/>.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <returns>Returns true if any component of the specified vector evaluates to <see cref="System.Double.NaN"/>; otherwise, false.</returns>
        public static bool IsNaN(Vector2 u)
        {
            if (double.IsNaN(u.X) || double.IsNaN(u.Y))
                return true;
            return false;
        }

        /// <summary>
        /// Obtains the dot product of two vectors.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>The dot product.</returns>
        public static double DotProduct(Vector2 u, Vector2 v)
        {
            return u.X*v.X + u.Y*v.Y;
        }

        /// <summary>
        /// Obtains the cross product of two vectors.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>Vector2.</returns>
        public static double CrossProduct(Vector2 u, Vector2 v)
        {
            return u.X*v.Y - u.Y*v.X;
        }

        /// <summary>
        /// Obtains the counter clockwise perpendicular vector .
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <returns>Vector2.</returns>
        public static Vector2 Perpendicular(Vector2 u)
        {
            return new Vector2(-u.Y, u.X);
        }

        /// <summary>
        /// Obtains a the polar point of another point. 
        /// </summary>
        /// <param name="u">Reference point.</param>
        /// <param name="distance">Distance from point u.</param>
        /// <param name="angle">Angle in radians.</param>
        /// <returns>The polar point of the specified point.</returns>
        public static Vector2 Polar(Vector2 u, double distance, double angle)
        {
            Vector2 dir = new Vector2(Math.Cos(angle), Math.Sin(angle));
            return u + dir*distance;
        }

        /// <summary>
        /// Obtains the distance between two points.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>Distance.</returns>
        public static double Distance(Vector2 u, Vector2 v)
        {
            return Math.Sqrt((u.X - v.X)*(u.X - v.X) + (u.Y - v.Y)*(u.Y - v.Y));
        }

        /// <summary>
        /// Obtains the square distance between two points.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>Square distance.</returns>
        public static double SquareDistance(Vector2 u, Vector2 v)
        {
            return (u.X - v.X)*(u.X - v.X) + (u.Y - v.Y)*(u.Y - v.Y);
        }

        /// <summary>
        /// Obtains the angle of a vector.
        /// </summary>
        /// <param name="u">A Vector2.</param>
        /// <returns>Angle in radians.</returns>
        public static double Angle(Vector2 u)
        {
            double angle = Math.Atan2(u.Y, u.X);
            if (angle < 0)
                return MathHelper.TwoPI + angle;
            return angle;
        }

        /// <summary>
        /// Obtains the angle of a line defined by two points.
        /// </summary>
        /// <param name="u">A Vector2.</param>
        /// <param name="v">A Vector2.</param>
        /// <returns>Angle in radians.</returns>
        public static double Angle(Vector2 u, Vector2 v)
        {
            Vector2 dir = v - u;
            return Angle(dir);
        }

        /// <summary>
        /// Obtains the angle between two vectors.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>Angle in radians.</returns>
        public static double AngleBetween(Vector2 u, Vector2 v)
        {
            double cos = DotProduct(u, v) / (u.Modulus() * v.Modulus());
            if (cos >= 1.0)
                return 0.0;
            if (cos <= -1.0)
                return Math.PI;

            return Math.Acos(cos);
        }

        /// <summary>
        /// Obtains the midpoint.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>Vector2.</returns>
        public static Vector2 MidPoint(Vector2 u, Vector2 v)
        {
            return new Vector2((v.X + u.X)*0.5F, (v.Y + u.Y)*0.5F);
        }

        /// <summary>
        /// Checks if two vectors are perpendicular.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <param name="threshold">Tolerance used.</param>
        /// <returns>True if are perpendicular or false in any other case.</returns>
        public static bool ArePerpendicular(Vector2 u, Vector2 v, double threshold = MathHelper.Epsilon)
        {
            return MathHelper.IsZero(DotProduct(u, v), threshold);
        }

        /// <summary>
        /// Checks if two vectors are parallel.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <param name="threshold">Tolerance used.</param>
        /// <returns>True if are parallel or false in any other case.</returns>
        public static bool AreParallel(Vector2 u, Vector2 v, double threshold = MathHelper.Epsilon)
        {
            double a = u.X*v.Y - u.Y*v.X;
            return MathHelper.IsZero(a, threshold);
        }

        /// <summary>
        /// Rounds the components of a vector.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="numDigits">Number of decimal places in the return value.</param>
        /// <returns>Vector2.</returns>
        public static Vector2 Round(Vector2 u, int numDigits)
        {
            return new Vector2(Math.Round(u.X, numDigits), Math.Round(u.Y, numDigits));
        }

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        /// <param name="u">Vector to normalize</param>
        /// <returns>A normalized vector.</returns>
        public static Vector2 Normalize(Vector2 u)
        {
            double mod = u.Modulus();
            if (MathHelper.IsOne(mod))
                return u; // the vector is already normalized
            if (MathHelper.IsZero(mod))
                return NaN;
            double modInv = 1 / mod;
            return new Vector2(u.x * modInv, u.y * modInv);
        }

        #endregion

        #region overloaded operators

        /// <summary>
        /// Check if the components of two vectors are equal.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>True if the two components are equal or false in any other case.</returns>
        public static bool operator ==(Vector2 u, Vector2 v)
        {
            return Equals(u, v);
        }

        /// <summary>
        /// Check if the components of two vectors are different.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>True if the two components are different or false in any other case.</returns>
        public static bool operator !=(Vector2 u, Vector2 v)
        {
            return !Equals(u, v);
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>The addition of u plus v.</returns>
        public static Vector2 operator +(Vector2 u, Vector2 v)
        {
            return new Vector2(u.X + v.X, u.Y + v.Y);
        }

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name="u">Vector3.</param>
        /// <param name="v">Vector3.</param>
        /// <returns>The subtraction of u minus v.</returns>
        public static Vector2 operator -(Vector2 u, Vector2 v)
        {
            return new Vector2(u.X - v.X, u.Y - v.Y);
        }

        /// <summary>
        /// Negates a vector.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <returns>The negative vector of u.</returns>
        public static Vector2 operator -(Vector2 u)
        {
            return new Vector2(-u.X, -u.Y);
        }

        /// <summary>
        /// Multiplies a vector with an scalar (same as a*u, commutative property).
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="a">Scalar.</param>
        /// <returns>The multiplication of u times a.</returns>
        public static Vector2 operator *(Vector2 u, double a)
        {
            return new Vector2(u.X*a, u.Y*a);
        }

        /// <summary>
        /// Multiplies an scalar with a vector (same as u*a, commutative property).
        /// </summary>
        /// <param name="a">Scalar.</param>
        /// <param name="u">Vector3.</param>
        /// <returns>The multiplication of a times u.</returns>
        public static Vector2 operator *(double a, Vector2 u)
        {
            return new Vector2(u.X*a, u.Y*a);
        }

        /// <summary>
        /// Divides a vector with an scalar (not same as a/v).
        /// </summary>
        /// <param name="a">Vector3.</param>
        /// <param name="u">Scalar.</param>
        /// <returns>The multiplication of a times u.</returns>
        public static Vector2 operator /(Vector2 u, double a)
        {
            double invEscalar = 1/a;
            return new Vector2(u.X*invEscalar, u.Y*invEscalar);
        }

        /// <summary>
        /// Divides an scalar with a vector (not same as v/a).
        /// </summary>
        /// <param name="a">Vector3.</param>
        /// <param name="u">Scalar.</param>
        /// <returns>The multiplication of a times u.</returns>
        public static Vector2 operator /(double a, Vector2 u)
        {
            double invEscalar = 1/a;
            return new Vector2(u.X*invEscalar, u.Y*invEscalar);
        }

        #endregion

        #region public methods

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        public void Normalize()
        {
            double mod = this.Modulus();
            if (MathHelper.IsOne(mod))
                return; // the vector is already normalized
            if (MathHelper.IsZero(mod))
                this = NaN;
            else
            {
                double modInv = 1/mod;
                this.x *= modInv;
                this.y *= modInv;
            }
        }

        /// <summary>
        /// Obtains the modulus of the vector.
        /// </summary>
        /// <returns>Vector modulus.</returns>
        public double Modulus()
        {
            return Math.Sqrt(DotProduct(this, this));
        }

        /// <summary>
        /// Returns an array that represents the vector.
        /// </summary>
        /// <returns>Array.</returns>
        public double[] ToArray()
        {
            double[] u = {this.x, this.y};
            return u;
        }

        #endregion

        #region comparison methods

        /// <summary>
        /// Check if the components of two vectors are approximate equals.
        /// </summary>
        /// <param name="obj">Another Vector2 to compare to.</param>
        /// <returns>True if the three components are almost equal or false in any other case.</returns>
        public bool Equals(Vector2 obj)
        {
            return this.Equals(obj, MathHelper.Epsilon);
        }

        /// <summary>
        /// Check if the components of two vectors are approximate equals.
        /// </summary>
        /// <param name="obj">Another Vector2 to compare to.</param>
        /// <param name="threshold">Maximum tolerance.</param>
        /// <returns>True if the three components are almost equal or false in any other case.</returns>
        public bool Equals(Vector2 obj, double threshold)
        {
            return MathHelper.IsEqual(obj.X, this.x, threshold) && MathHelper.IsEqual(obj.Y, this.y, threshold);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>True if obj and this instance are the same type and represent the same value; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Vector2)
                return this.Equals((Vector2) obj);
            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode();
        }

        #endregion

        #region overrides

        /// <summary>
        /// Obtains a string that represents the vector.
        /// </summary>
        /// <returns>A string text.</returns>
        public override string ToString()
        {
            return string.Format("{0}{2} {1}", this.x, this.y, Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator);
        }

        /// <summary>
        /// Obtains a string that represents the vector.
        /// </summary>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information. </param>
        /// <returns>A string text.</returns>
        public string ToString(IFormatProvider provider)
        {
            return string.Format("{0}{2} {1}", this.x.ToString(provider), this.y.ToString(provider), Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator);
        }

        #endregion
    }
}