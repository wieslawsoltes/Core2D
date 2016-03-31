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
    /// Represents a linear or rotated dimension <see cref="EntityObject">entity</see>.
    /// </summary>
    public class LinearDimension :
        Dimension
    {
        #region private fields

        private Vector3 start;
        private Vector3 end;
        private double offset;
        private double rotation;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>LinearDimension</c> class.
        /// </summary>
        public LinearDimension()
            : this(Vector3.Zero, Vector3.UnitX, 0.1, 0.0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>LinearDimension</c> class.
        /// </summary>
        /// <param name="referenceLine">Reference <see cref="Line">line</see> of the dimension.</param>
        /// <param name="offset">Distance between the reference line and the dimension line.</param>
        /// <param name="rotation">Rotation in degrees of the dimension line.</param>
        /// <remarks>The reference points define the distance to be measure.</remarks>
        public LinearDimension(Line referenceLine, double offset, double rotation)
            : this(referenceLine, offset, rotation, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>LinearDimension</c> class.
        /// </summary>
        /// <param name="referenceLine">Reference <see cref="Line">line</see> of the dimension.</param>
        /// <param name="offset">Distance between the reference line and the dimension line.</param>
        /// <param name="rotation">Rotation in degrees of the dimension line.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        /// <remarks>The reference line define the distance to be measure.</remarks>
        public LinearDimension(Line referenceLine, double offset, double rotation, DimensionStyle style)
            : this(referenceLine.StartPoint, referenceLine.EndPoint, offset, rotation, style)
        {
            this.normal = referenceLine.Normal;
        }

        /// <summary>
        /// Initializes a new instance of the <c>LinearDimension</c> class.
        /// </summary>
        /// <param name="firstPoint">First reference <see cref="Vector2">point</see> of the dimension.</param>
        /// <param name="secondPoint">Second reference <see cref="Vector2">point</see> of the dimension.</param>
        /// <param name="offset">Distance between the mid point reference line and the dimension line.</param>
        /// <param name="rotation">Rotation in degrees of the dimension line.</param>
        /// <remarks>The reference points define the distance to be measure.</remarks>
        public LinearDimension(Vector2 firstPoint, Vector2 secondPoint, double offset, double rotation)
            : this(new Vector3(firstPoint.X, firstPoint.Y, 0.0), new Vector3(secondPoint.X, secondPoint.Y, 0.0), offset, rotation, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>LinearDimension</c> class.
        /// </summary>
        /// <param name="firstPoint">First reference <see cref="Vector3">point</see> of the dimension.</param>
        /// <param name="secondPoint">Second reference <see cref="Vector3">point</see> of the dimension.</param>
        /// <param name="offset">Distance between the mid point reference line and the dimension line.</param>
        /// <param name="rotation">Rotation in degrees of the dimension line.</param>
        /// <remarks>The reference points define the distance to be measure.</remarks>
        public LinearDimension(Vector3 firstPoint, Vector3 secondPoint, double offset, double rotation)
            : this(firstPoint, secondPoint, offset, rotation, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>LinearDimension</c> class.
        /// </summary>
        /// <param name="firstPoint">First reference <see cref="Vector2">point</see> of the dimension.</param>
        /// <param name="secondPoint">Second reference <see cref="Vector2">point</see> of the dimension.</param>
        /// <param name="offset">Distance between the mid point reference line and the dimension line.</param>
        /// <param name="rotation">Rotation in degrees of the dimension line.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        /// <remarks>The reference points define the distance to be measure.</remarks>
        public LinearDimension(Vector2 firstPoint, Vector2 secondPoint, double offset, double rotation, DimensionStyle style)
            : this(new Vector3(firstPoint.X, firstPoint.Y, 0.0), new Vector3(secondPoint.X, secondPoint.Y, 0.0), offset, rotation, style)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>LinearDimension</c> class.
        /// </summary>
        /// <param name="firstPoint">First reference <see cref="Vector3">point</see> of the dimension.</param>
        /// <param name="secondPoint">Second reference <see cref="Vector3">point</see> of the dimension.</param>
        /// <param name="offset">Distance between the mid point reference line and the dimension line.</param>
        /// <param name="rotation">Rotation in degrees of the dimension line.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        /// <remarks>The reference points define the distance to be measure.</remarks>
        public LinearDimension(Vector3 firstPoint, Vector3 secondPoint, double offset, double rotation, DimensionStyle style)
            : base(DimensionType.Linear)
        {
            this.start = firstPoint;
            this.end = secondPoint;
            this.offset = offset;
            this.style = style;
            this.rotation = MathHelper.NormalizeAngle(rotation);
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the first definition point of the dimension.
        /// </summary>
        public Vector3 FirstReferencePoint
        {
            get { return this.start; }
            set { this.start = value; }
        }

        /// <summary>
        /// Gets or sets the second definition point of the dimension.
        /// </summary>
        public Vector3 SecondReferencePoint
        {
            get { return this.end; }
            set { this.end = value; }
        }

        /// <summary>
        /// Gets or sets the rotation of the dimension line.
        /// </summary>
        public double Rotation
        {
            get { return this.rotation; }
            set { this.rotation = MathHelper.NormalizeAngle(value); }
        }

        /// <summary>
        /// Gets or sets the distance between the mid point of the reference line and the dimension line.
        /// </summary>
        public double Offset
        {
            get { return this.offset; }
            set { this.offset = value; }
        }

        /// <summary>
        /// Gets the actual measurement.
        /// </summary>
        /// <remarks>The dimension is always measured in the plane defined by the normal.</remarks>
        public override double Measurement
        {
            get
            {
                Vector3 refPoint;
                
                refPoint = MathHelper.Transform(this.start, this.normal, CoordinateSystem.World, CoordinateSystem.Object);
                Vector2 ref1 = new Vector2(refPoint.X, refPoint.Y);
                
                refPoint = MathHelper.Transform(this.end, this.normal, CoordinateSystem.World, CoordinateSystem.Object);
                Vector2 ref2 = new Vector2(refPoint.X, refPoint.Y);

                double refRot = Vector2.Angle(ref1, ref2);
                return Math.Abs(Vector2.Distance(ref1, ref2) * Math.Cos(this.rotation * MathHelper.DegToRad - refRot));
            }
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
        /// Creates a new LinearDimension that is a copy of the current instance.
        /// </summary>
        /// <returns>A new LinearDimension that is a copy of this instance.</returns>
        public override object Clone()
        {

            LinearDimension entity = new LinearDimension
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
                //LinearDimension properties
                FirstReferencePoint = this.start,
                SecondReferencePoint = this.end,
                Rotation = this.rotation,
                Offset = this.offset
            };

            foreach (XData data in this.XData.Values)
                entity.XData.Add((XData)data.Clone());

            return entity;

        }

        #endregion
    }
}