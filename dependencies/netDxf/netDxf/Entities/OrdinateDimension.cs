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

using netDxf.Blocks;
using netDxf.Tables;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents an ordinate dimension <see cref="EntityObject">entity</see>.
    /// </summary>
    public class OrdinateDimension
        : Dimension
    {
        #region private fields

        private Vector3 origin;
        private Vector2 referencePoint;
        private double length;
        private double rotation;
        private OrdinateDimensionAxis axis;
        private Vector3 firstPoint;
        private Vector3 secondPoint;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>OrdinateDimension</c> class.
        /// </summary>
        public OrdinateDimension()
            : this(Vector3.Zero, Vector2.UnitX, 0.1, 0.0, OrdinateDimensionAxis.Y)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>OrdinateDimension</c> class.
        /// </summary>
        /// <param name="origin">Origin <see cref="Vector3">point</see> of the ordinate dimension.</param>
        /// <param name="referencePoint">Base location <see cref="Vector3">point</see> in local coordinates of the ordinate dimension.</param>
        /// <param name="length">Length of the dimension line.</param>
        /// <param name="axis">Length of the dimension line.</param>
        /// <param name="rotation">Angle of rotation in degrees of the dimension lines.</param>
        /// <remarks>The local coordinate system of the dimension is defined by the dimension normal and the rotation value.</remarks>
        public OrdinateDimension(Vector3 origin, Vector2 referencePoint, double length, double rotation, OrdinateDimensionAxis axis)
            : this(origin, referencePoint, length, axis, rotation, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>OrdinateDimension</c> class.
        /// </summary>
        /// <param name="origin">Origin <see cref="Vector3">point</see> in world coordinates of the ordinate dimension.</param>
        /// <param name="referencePoint">Base location <see cref="Vector3">point</see> in local coordinates of the ordinate dimension.</param>
        /// <param name="length">Length of the dimension line.</param>
        /// <param name="axis">Local axis that measures the ordinate dimension.</param>
        /// <param name="rotation">Angle of rotation in degrees of the dimension lines.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        /// <remarks>The local coordinate system of the dimension is defined by the dimension normal and the rotation value.</remarks>
        public OrdinateDimension(Vector3 origin, Vector2 referencePoint, double length, OrdinateDimensionAxis axis, double rotation, DimensionStyle style)
            : base(DimensionType.Ordinate)
        {
            this.origin = origin;
            this.rotation = MathHelper.NormalizeAngle(rotation);
            this.length = length;
            this.referencePoint = referencePoint;
            this.axis = axis;
            this.style = style;
        }

        #endregion

        #region internal properties

        internal Vector3 FirstPoint
        {
            get { return this.firstPoint; }
            set { this.firstPoint = value; }
        }

        internal Vector3 SecondPoint
        {
            get { return this.secondPoint; }
            set { this.secondPoint = value; }
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the origin <see cref="Vector3">point</see> in world coordinates of the ordinate dimension.
        /// </summary>
        public Vector3 Origin
        {
            get { return this.origin; }
            set { this.origin = value; }
        }

        /// <summary>
        /// Gets or sets the base location <see cref="Vector3">point</see> in local coordinates of the ordinate dimension.
        /// </summary>
        public Vector2 ReferencePoint
        {
            get { return this.referencePoint; }
            set { this.referencePoint = value; }
        }

        /// <summary>
        /// Gets or sets the angle of rotation in degrees of the ordinate dimension local coordinate system.
        /// </summary>
        public double Rotation
        {
            get { return this.rotation; }
            set { MathHelper.NormalizeAngle(this.rotation = value); }
        }

        /// <summary>
        /// Gets or sets the length of the dimension line.
        /// </summary>
        public double Length
        {
            get { return this.length; }
            set { this.length = value; }
        }

        /// <summary>
        /// Gets or sets the local axis that measures the ordinate dimension.
        /// </summary>
        public OrdinateDimensionAxis Axis
        {
            get { return this.axis; }
            set { this.axis = value; }
        }

        /// <summary>
        /// Actual measurement.
        /// </summary>
        public override double Measurement
        {
            get { return this.axis == OrdinateDimensionAxis.X ? this.referencePoint.X : this.referencePoint.Y; }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Gets the block that contains the entities that make up the dimension picture.
        /// </summary>
        /// <param name="name">Name to be assigned to the generated block.</param>
        /// <returns>The block that represents the actual dimension.</returns>
        internal override Block BuildBlock(string name)
        {
            return DimensionBlock.Build(this, name);
        }

        /// <summary>
        /// Creates a new OrdinateDimension that is a copy of the current instance.
        /// </summary>
        /// <returns>A new OrdinateDimension that is a copy of this instance.</returns>
        public override object Clone()
        {
            OrdinateDimension entity = new OrdinateDimension
            {
                //EntityObject properties
                Layer = (Layer) this.layer.Clone(),
                LineType = (LineType) this.lineType.Clone(),
                Color = (AciColor) this.color.Clone(),
                Lineweight = (Lineweight) this.lineweight.Clone(),
                Transparency = (Transparency) this.transparency.Clone(),
                LineTypeScale = this.lineTypeScale,
                Normal = this.normal,
                //Dimension properties
                Style = (DimensionStyle) this.style.Clone(),
                AttachmentPoint = this.attachmentPoint,
                LineSpacingStyle = this.lineSpacingStyle,
                LineSpacingFactor = this.lineSpacing,
                //OrdinateDimension properties
                Origin = this.origin,
                ReferencePoint = this.referencePoint,
                Rotation = this.rotation,
                Length = this.length,
                Axis = this.axis
            };

            foreach (XData data in this.XData.Values)
                entity.XData.Add((XData) data.Clone());

            return entity;
        }

        #endregion
    }
}