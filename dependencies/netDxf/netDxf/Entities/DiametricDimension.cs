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
using netDxf.Blocks;
using netDxf.Tables;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a diametric dimension <see cref="EntityObject">entity</see>.
    /// </summary>
    public class DiametricDimension :
        Dimension
    {
        #region private fields

        private Vector3 center;
        private Vector3 refPoint;
        private double offset;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>DiametricDimension</c> class.
        /// </summary>
        public DiametricDimension()
            : this(Vector3.Zero, Vector3.UnitX, 0.0, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>DiametricDimension</c> class.
        /// </summary>
        /// <param name="arc"><see cref="Arc">Arc</see> to measure.</param>
        /// <param name="rotation">Rotation in degrees of the dimension line.</param>
        /// <param name="offset">Distance between the reference point and the dimension text</param>
        /// <remarks>The center point and the definition point define the distance to be measure.</remarks>
        public DiametricDimension(Arc arc, double rotation, double offset)
            : this(arc, rotation, offset, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>DiametricDimension</c> class.
        /// </summary>
        /// <param name="arc"><see cref="Arc">Arc</see> to measure.</param>
        /// <param name="rotation">Rotation in degrees of the dimension line.</param>
        /// <param name="offset">Distance between the reference point and the dimension text</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        /// <remarks>The center point and the definition point define the distance to be measure.</remarks>
        public DiametricDimension(Arc arc, double rotation, double offset, DimensionStyle style)
            : base(DimensionType.Diameter)
        {
            double angle = rotation * MathHelper.DegToRad;
            Vector3 point = MathHelper.Transform(new Vector3(arc.Radius * Math.Sin(angle), arc.Radius * Math.Cos(angle), 0.0), arc.Normal, CoordinateSystem.Object, CoordinateSystem.World);
            this.center = arc.Center;
            this.refPoint = arc.Center + point;
            this.offset = offset;
            if (style == null)
                throw new ArgumentNullException(nameof(style), "The Dimension style cannot be null.");
            this.style = style;
        }

        /// <summary>
        /// Initializes a new instance of the <c>DiametricDimension</c> class.
        /// </summary>
        /// <param name="circle"><see cref="Circle">Circle</see> to measure.</param>
        /// <param name="rotation">Rotation in degrees of the dimension line.</param>
        /// <param name="offset">Distance between the reference point and the dimension text</param>
        /// <remarks>The center point and the definition point define the distance to be measure.</remarks>
        public DiametricDimension(Circle circle, double rotation, double offset)
            : this(circle, rotation, offset, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>DiametricDimension</c> class.
        /// </summary>
        /// <param name="circle"><see cref="Circle">Circle</see> to measure.</param>
        /// <param name="rotation">Rotation in degrees of the dimension line.</param>
        /// <param name="offset">Distance between the reference point and the dimension text</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        /// <remarks>The center point and the definition point define the distance to be measure.</remarks>
        public DiametricDimension(Circle circle, double rotation, double offset, DimensionStyle style)
            : base(DimensionType.Diameter)
        {
            double angle = rotation*MathHelper.DegToRad;
            Vector3 point = MathHelper.Transform(new Vector3(circle.Radius * Math.Cos(angle), circle.Radius * Math.Sin(angle), 0.0), circle.Normal, CoordinateSystem.Object, CoordinateSystem.World);
            this.center = circle.Center;
            this.refPoint = circle.Center + point;

            if (offset < 0.0)
                throw new ArgumentOutOfRangeException(nameof(offset), "The offset value cannot be negative.");
            this.offset = offset;

            if (style == null)
                throw new ArgumentNullException(nameof(style), "The Dimension style cannot be null.");
            this.style = style;
        }

        /// <summary>
        /// Initializes a new instance of the <c>DiametricDimension</c> class.
        /// </summary>
        /// <param name="centerPoint">Center <see cref="Vector2">point</see> of the circumference.</param>
        /// <param name="referencePoint"><see cref="Vector2">Point</see> on circle or arc.</param>
        /// <param name="offset">Distance between the reference point and the dimension text</param>
        /// <remarks>The center point and the definition point define the distance to be measure.</remarks>
        public DiametricDimension(Vector2 centerPoint, Vector2 referencePoint, double offset)
            : this(new Vector3(centerPoint.X, centerPoint.Y, 0.0), new Vector3(referencePoint.X, referencePoint.Y, 0.0), offset, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>DiametricDimension</c> class.
        /// </summary>
        /// <param name="centerPoint">Center <see cref="Vector2">point</see> of the circumference.</param>
        /// <param name="referencePoint"><see cref="Vector2">Point</see> on circle or arc.</param>
        /// <param name="offset">Distance between the reference point and the dimension text</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        /// <remarks>The center point and the definition point define the distance to be measure.</remarks>
        public DiametricDimension(Vector2 centerPoint, Vector2 referencePoint, double offset, DimensionStyle style)
            : this(new Vector3(centerPoint.X, centerPoint.Y, 0.0), new Vector3(referencePoint.X, referencePoint.Y, 0.0), offset, style)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>DiametricDimension</c> class.
        /// </summary>
        /// <param name="centerPoint">Center <see cref="Vector3">point</see> of the circumference.</param>
        /// <param name="referencePoint"><see cref="Vector3">Point</see> on circle or arc.</param>
        /// <param name="offset">Distance between the reference point and the dimension text</param>
        /// <remarks>The center point and the definition point define the distance to be measure.</remarks>
        public DiametricDimension(Vector3 centerPoint, Vector3 referencePoint, double offset)
            : this(centerPoint, referencePoint, offset, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>DiametricDimension</c> class.
        /// </summary>
        /// <param name="centerPoint">Center <see cref="Vector3">point</see> of the circumference.</param>
        /// <param name="referencePoint"><see cref="Vector3">Point</see> on circle or arc.</param>
        /// <param name="offset">Distance between the reference point and the dimension text</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        /// <remarks>The center point and the definition point define the distance to be measure.</remarks>
        public DiametricDimension(Vector3 centerPoint, Vector3 referencePoint, double offset, DimensionStyle style)
            : base(DimensionType.Diameter)
        {
            this.center = centerPoint;
            this.refPoint = referencePoint;

            if (offset < 0.0)
                throw new ArgumentOutOfRangeException(nameof(offset), "The offset value cannot be negative.");
            this.offset = offset;

            if (style == null)
                throw new ArgumentNullException(nameof(style), "The Dimension style cannot be null.");
            this.style = style;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the center <see cref="Vector3">point</see> of the circumference (in WCS).
        /// </summary>
        public Vector3 CenterPoint
        {
            get { return this.center; }
            set { this.center = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="Vector3">point</see> on circumference or arc (in WCS).
        /// </summary>
        public Vector3 ReferencePoint
        {
            get { return this.refPoint; }
            set { this.refPoint = value; }
        }

        /// <summary>
        /// Gets or sets the distance between the center point and the dimension text.
        /// </summary>
        /// <remarks>
        /// If the value is close to half of the diameter, the offset will be 2*DIMASZ+DIMGAP*DIMSCALE.
        /// If the value is set to zero the dimension will be shown at the center, both arrowheads will be shown and no center mark will be drawn.
        /// </remarks>
        public double Offset
        {
            get { return this.offset; }
            set
            {
                if (value<0.0)
                    throw new ArgumentOutOfRangeException(nameof(value), "The offset value cannot be negative.");
                this.offset = value;
            }
        }

        /// <summary>
        /// Actual measurement.
        /// </summary>
        public override double Measurement
        {
            get { return 2 * Vector3.Distance(this.center, this.refPoint); }
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
        /// Creates a new DiametricDimension that is a copy of the current instance.
        /// </summary>
        /// <returns>A new DiametricDimension that is a copy of this instance.</returns>
        public override object Clone()
        {
            DiametricDimension entity = new DiametricDimension
            {
                //EntityObject properties
                Layer = (Layer)this.layer.Clone(),
                LineType = (LineType)this.lineType.Clone(),
                Color = (AciColor)this.color.Clone(),
                Lineweight = (Lineweight)this.lineweight.Clone(),
                Transparency = (Transparency)this.transparency.Clone(),
                LineTypeScale = this.lineTypeScale,
                Normal = this.normal,
                //Dimension properties
                Style = (DimensionStyle)this.style.Clone(),
                AttachmentPoint = this.attachmentPoint,
                LineSpacingStyle = this.lineSpacingStyle,
                LineSpacingFactor = this.lineSpacing,
                //DiametricDimension properties
                CenterPoint = this.center,
                ReferencePoint = this.refPoint,
                Offset = this.offset
            };

            foreach (XData data in this.xData.Values)
                entity.XData.Add((XData)data.Clone());

            return entity;

        }

        #endregion
    }
}