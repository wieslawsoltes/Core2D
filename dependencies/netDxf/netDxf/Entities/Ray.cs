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
using netDxf.Tables;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a ray <see cref="EntityObject">entity</see>.
    /// </summary>
    /// <remarks>A ray is a line in three-dimensional space that starts in the specified origin and extends to infinity.</remarks>
    public class Ray :
        EntityObject
    {
        #region private fields

        private Vector3 origin;
        private Vector3 direction;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>Ray</c> class.
        /// </summary>
        public Ray()
            : this(Vector3.Zero, Vector3.UnitX)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Ray</c> class.
        /// </summary>
        /// <param name="origin">Ray <see cref="Vector2">start point.</see></param>
        /// <param name="direction">Ray <see cref="Vector2">end point.</see></param>
        public Ray(Vector2 origin, Vector2 direction)
            : this(new Vector3(origin.X, origin.Y, 0.0), new Vector3(direction.X, direction.Y, 0.0))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Ray</c> class.
        /// </summary>
        /// <param name="origin">Ray start <see cref="Vector3">point.</see></param>
        /// <param name="direction">Ray end <see cref="Vector3">point.</see></param>
        public Ray(Vector3 origin, Vector3 direction) 
            : base(EntityType.Ray, DxfObjectCode.Ray)
        {
            this.origin = origin;
            this.direction = direction;
            this.direction.Normalize();
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the ray <see cref="netDxf.Vector3">origin</see>.
        /// </summary>
        public Vector3 Origin
        {
            get { return this.origin; }
            set { this.origin = value; }
        }

        /// <summary>
        /// Gets or sets the ray <see cref="netDxf.Vector3">direction</see>.
        /// </summary>
        public Vector3 Direction
        {
            get { return this.direction; }
            set
            {
                if (value == Vector3.Zero)
                    throw new ArgumentNullException(nameof(value), "The direction can not be the zero vector.");
                this.direction = value;
                this.direction.Normalize();
            }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new Ray that is a copy of the current instance.
        /// </summary>
        /// <returns>A new Ray that is a copy of this instance.</returns>
        public override object Clone()
        {
            Ray entity = new Ray
            {
                //EntityObject properties
                Layer = (Layer)this.layer.Clone(),
                LineType = (LineType)this.lineType.Clone(),
                Color = (AciColor)this.color.Clone(),
                Lineweight = (Lineweight)this.lineweight.Clone(),
                Transparency = (Transparency)this.transparency.Clone(),
                LineTypeScale = this.lineTypeScale,
                Normal = this.normal,
                //Ray properties
                Origin = this.origin,
                Direction = this.direction,
            };

            foreach (XData data in this.XData.Values)
                entity.XData.Add((XData)data.Clone());

            return entity;

        }

        #endregion
    }
}