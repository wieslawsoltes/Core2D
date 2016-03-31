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
using netDxf.Blocks;
using netDxf.Tables;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a 3 point angular dimension <see cref="EntityObject">entity</see>.
    /// </summary>
    public class Angular3PointDimension :
        Dimension
    {
        #region private fields

        private double offset;
        private Vector3 center;
        private Vector3 start;
        private Vector3 end;
        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>Angular3PointDimension</c> class.
        /// </summary>
        public Angular3PointDimension()
            : this(Vector3.Zero, Vector3.UnitX, Vector3.UnitY, 0.1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular3PointDimension</c> class.
        /// </summary>
        /// <param name="arc"><see cref="Arc">Arc</see> to measure.</param>
        /// <param name="offset">Distance between the center of the arc and the dimension line.</param>
        public Angular3PointDimension(Arc arc, double offset)
            : this(arc, offset, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular3PointDimension</c> class.
        /// </summary>
        /// <param name="arc">Angle <see cref="Arc">arc</see> to measure.</param>
        /// <param name="offset">Distance between the center of the arc and the dimension line.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        public Angular3PointDimension(Arc arc, double offset, DimensionStyle style)
            : base(DimensionType.Angular3Point)
        {
            this.center = arc.Center;

            Vector3 refPoint = MathHelper.Transform(arc.Center, arc.Normal, CoordinateSystem.World, CoordinateSystem.Object);

            Vector2 centerRef = new Vector2(refPoint.X, refPoint.Y);
            double elev = refPoint.Z;

            Vector2 ref1 = Vector2.Polar(centerRef, arc.Radius, arc.StartAngle * MathHelper.DegToRad);
            this.start = MathHelper.Transform(new Vector3(ref1.X, ref1.Y, elev), arc.Normal, CoordinateSystem.Object, CoordinateSystem.World);

            Vector2 ref2 = Vector2.Polar(centerRef, arc.Radius, arc.EndAngle * MathHelper.DegToRad);
            this.end = MathHelper.Transform(new Vector3(ref2.X, ref2.Y, elev), arc.Normal, CoordinateSystem.Object, CoordinateSystem.World);

            if (MathHelper.IsZero(offset))
                throw new ArgumentOutOfRangeException(nameof(offset), "The offset value cannot be zero.");
            this.offset = offset;

            if (style == null)
                throw new ArgumentNullException(nameof(style), "The Dimension style cannot be null.");
            this.style = style;
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular3PointDimension</c> class.
        /// </summary>
        /// <param name="centerPoint">Center of the angle arc to measure.</param>
        /// <param name="startPoint">Angle start point.</param>
        /// <param name="endPoint">Angle end point.</param>
        /// <param name="offset">Distance between the center point and the dimension line.</param>
        public Angular3PointDimension(Vector2 centerPoint, Vector2 startPoint, Vector2 endPoint, double offset)
            : this(new Vector3(centerPoint.X, centerPoint.Y, 0.0), new Vector3(startPoint.X, startPoint.Y, 0.0), new Vector3(endPoint.X, endPoint.Y, 0.0), offset, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular3PointDimension</c> class.
        /// </summary>
        /// <param name="centerPoint">Center of the angle arc to measure.</param>
        /// <param name="startPoint">Angle start point.</param>
        /// <param name="endPoint">Angle end point.</param>
        /// <param name="offset">Distance between the center point and the dimension line.</param>
        public Angular3PointDimension(Vector3 centerPoint, Vector3 startPoint, Vector3 endPoint, double offset)
            : this(centerPoint, startPoint, endPoint, offset, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular3PointDimension</c> class.
        /// </summary>
        /// <param name="centerPoint">Center of the angle arc to measure.</param>
        /// <param name="startPoint">Angle start point.</param>
        /// <param name="endPoint">Angle end point.</param>
        /// <param name="offset">Distance between the center point and the dimension line.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        public Angular3PointDimension(Vector2 centerPoint, Vector2 startPoint, Vector2 endPoint, double offset, DimensionStyle style)
            : this(new Vector3(centerPoint.X, centerPoint.Y, 0.0), new Vector3(startPoint.X, startPoint.Y, 0.0), new Vector3(endPoint.X, endPoint.Y, 0.0), offset, style)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular3PointDimension</c> class.
        /// </summary>
        /// <param name="centerPoint">Center of the angle arc to measure.</param>
        /// <param name="startPoint">Angle start point.</param>
        /// <param name="endPoint">Angle end point.</param>
        /// <param name="offset">Distance between the center point and the dimension line.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        public Angular3PointDimension(Vector3 centerPoint, Vector3 startPoint, Vector3 endPoint, double offset, DimensionStyle style)
            : base(DimensionType.Angular3Point)
        {
            this.center = centerPoint;
            this.start = startPoint;
            this.end = endPoint;
            if (MathHelper.IsZero(offset))
                throw new ArgumentOutOfRangeException(nameof(offset), "The offset value cannot be zero.");
            this.offset = offset;
            this.style = style;
        }
        #endregion

        #region internal properties

        /// <summary>
        /// Definition point for linear and angular dimensions (in WCS).
        /// </summary>
        internal Vector3 FirstPoint
        {
            get { return this.start; }
            set { this.start = value; }
        }

        /// <summary>
        /// Definition point for linear and angular dimensions (in WCS).
        /// </summary>
        internal Vector3 SecondPoint
        {
            get { return this.end; }
            set { this.end = value; }
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the center <see cref="Vector3">point</see> of the arc.
        /// </summary>
        public Vector3 CenterPoint
        {
            get { return this.center; }
            set { this.center = value; }
        }

        /// <summary>
        /// Gets or sets the angle start <see cref="Vector3">point</see> of the dimension.
        /// </summary>
        public Vector3 StartPoint
        {
            get { return this.start; }
            set { this.start = value; }
        }

        /// <summary>
        /// Gets or sets the angle end <see cref="Vector3">point</see> of the dimension.
        /// </summary>
        public Vector3 EndPoint
        {
            get { return this.end; }
            set { this.end = value; }
        }

        /// <summary>
        /// Gets or sets the distance between the center <see cref="Vector3">point</see> and the dimension line.
        /// </summary>
        /// <remarks>
        /// Zero values are not allowed, and even if negative values are permitted they are not recommended.<br />
        /// Setting a negative value is equivalent as switching the start and end points.
        /// </remarks>
        public double Offset
        {
            get { return this.offset; }
            set
            {
                if (MathHelper.IsZero(value))
                    throw new ArgumentOutOfRangeException(nameof(value), "The offset value cannot be zero.");
                this.offset = value;
            }
        }

        /// <summary>
        /// Actual measurement.
        /// </summary>
        /// <remarks>The dimension is always measured in the plane defined by the normal.</remarks>
        public override double Measurement
        {
            get
            {
                IList<Vector3> ocsPoints = MathHelper.Transform(new[] { this.center, this.start, this.end }, this.normal, CoordinateSystem.World, CoordinateSystem.Object);
                Vector3 refPoint;

                refPoint = ocsPoints[0];
                Vector2 refCenter = new Vector2(refPoint.X, refPoint.Y);

                refPoint = ocsPoints[1];
                Vector2 ref1 = new Vector2(refPoint.X, refPoint.Y);

                refPoint = ocsPoints[2];
                Vector2 ref2 = new Vector2(refPoint.X, refPoint.Y);

                if (this.offset < 0)
                {
                    Vector2 tmp = ref1;
                    ref1 = ref2;
                    ref2 = tmp;
                }

                double angle = (Vector2.Angle(refCenter, ref2) - Vector2.Angle(refCenter, ref1)) * MathHelper.RadToDeg;
                return MathHelper.NormalizeAngle(angle);
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
        /// Creates a new Angular3PointDimension that is a copy of the current instance.
        /// </summary>
        /// <returns>A new Angular3PointDimension that is a copy of this instance.</returns>
        public override object Clone()
        {
            Angular3PointDimension entity = new Angular3PointDimension
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
                //Angular3PointDimension properties
                CenterPoint = this.center,
                StartPoint = this.start,
                EndPoint = this.end,
                Offset = this.offset
            };

            foreach (XData data in this.XData.Values)
                entity.XData.Add((XData)data.Clone());

            return entity;

        }

        #endregion
    }
}
