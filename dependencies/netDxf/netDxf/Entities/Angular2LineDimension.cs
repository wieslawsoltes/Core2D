#region netDxf, Copyright(C) 2014 Daniel Carvajal, Licensed under LGPL.

//                        netDxf library
// Copyright (C) 2014 Daniel Carvajal (haplokuon@gmail.com)
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 

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
    public class Angular2LineDimension :
        Dimension
    {
        #region private fields

        private double offset;
        private Vector3 startFirstLine;
        private Vector3 endFirstLine;
        private Vector3 startSecondLine;
        private Vector3 endSecondLine;
        private Vector3 arcDefinitionPoint;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>Angular2LineDimension</c> class.
        /// </summary>
        public Angular2LineDimension()
            : this(Vector3.Zero, Vector3.UnitX, Vector3.Zero, Vector3.UnitY, 0.1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular2LineDimension</c> class.
        /// </summary>
        /// <param name="firstLine">First <see cref="Line">line</see> that defines the angle to measure.</param>
        /// <param name="secondLine">Second <see cref="Line">line</see> that defines the angle to measure.</param>
        /// <param name="offset">Distance between the center point and the dimension line.</param>
        public Angular2LineDimension(Line firstLine, Line secondLine, double offset)
            : this(firstLine, secondLine, offset, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular2LineDimension</c> class.
        /// </summary>
        /// <param name="firstLine">First <see cref="Line">line</see> that defines the angle to measure.</param>
        /// <param name="secondLine">Second <see cref="Line">line</see> that defines the angle to measure.</param>
        /// <param name="offset">Distance between the center point and the dimension line.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        public Angular2LineDimension(Line firstLine, Line secondLine, double offset, DimensionStyle style)
            : this(firstLine.StartPoint, firstLine.EndPoint, secondLine.StartPoint, secondLine.EndPoint, offset, style)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular2LineDimension</c> class.
        /// </summary>
        /// <param name="startFirstLine">Start <see cref="Vector2">point</see> of the first line that defines the angle to measure.</param>
        /// <param name="endFirstLine">End <see cref="Vector2">point</see> of the first line that defines the angle to measure.</param>
        /// <param name="startSecondLine">Start <see cref="Vector2">point</see> of the second line that defines the angle to measure.</param>
        /// <param name="endSecondLine">End <see cref="Vector2">point</see> of the second line that defines the angle to measure.</param>
        /// <param name="offset">Distance between the center point and the dimension line.</param>
        public Angular2LineDimension(Vector2 startFirstLine, Vector2 endFirstLine, Vector2 startSecondLine, Vector2 endSecondLine, double offset)
            : this(new Vector3(startFirstLine.X, startFirstLine.Y, 0.0), new Vector3(endFirstLine.X, endFirstLine.Y, 0.0), new Vector3(startSecondLine.X, startSecondLine.Y, 0.0), new Vector3(endSecondLine.X, endSecondLine.Y, 0.0), offset, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular2LineDimension</c> class.
        /// </summary>
        /// <param name="startFirstLine">Start <see cref="Vector3">point</see> of the first line that defines the angle to measure.</param>
        /// <param name="endFirstLine">End <see cref="Vector3">point</see> of the first line that defines the angle to measure.</param>
        /// <param name="startSecondLine">Start <see cref="Vector3">point</see> of the second line that defines the angle to measure.</param>
        /// <param name="endSecondLine">End <see cref="Vector3">point</see> of the second line that defines the angle to measure.</param>
        /// <param name="offset">Distance between the center point and the dimension line.</param>
        public Angular2LineDimension(Vector3 startFirstLine, Vector3 endFirstLine, Vector3 startSecondLine, Vector3 endSecondLine, double offset)
            : this(startFirstLine, endFirstLine, startSecondLine, endSecondLine, offset, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular2LineDimension</c> class.
        /// </summary>
        /// <param name="startFirstLine">Start <see cref="Vector2">point</see> of the first line that defines the angle to measure.</param>
        /// <param name="endFirstLine">End <see cref="Vector2">point</see> of the first line that defines the angle to measure.</param>
        /// <param name="startSecondLine">Start <see cref="Vector2">point</see> of the second line that defines the angle to measure.</param>
        /// <param name="endSecondLine">End <see cref="Vector2">point</see> of the second line that defines the angle to measure.</param>
        /// <param name="offset">Distance between the center point and the dimension line.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        public Angular2LineDimension(Vector2 startFirstLine, Vector2 endFirstLine, Vector2 startSecondLine, Vector2 endSecondLine, double offset, DimensionStyle style)
            : this(new Vector3(startFirstLine.X, startFirstLine.Y, 0.0), new Vector3(endFirstLine.X, endFirstLine.Y, 0.0), new Vector3(startSecondLine.X, startSecondLine.Y, 0.0), new Vector3(endSecondLine.X, endSecondLine.Y, 0.0), offset, style)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular2LineDimension</c> class.
        /// </summary>
        /// <param name="startFirstLine">Start <see cref="Vector3">point</see> of the first line that defines the angle to measure.</param>
        /// <param name="endFirstLine">End <see cref="Vector3">point</see> of the first line that defines the angle to measure.</param>
        /// <param name="startSecondLine">Start <see cref="Vector3">point</see> of the second line that defines the angle to measure.</param>
        /// <param name="endSecondLine">End <see cref="Vector3">point</see> of the second line that defines the angle to measure.</param>
        /// <param name="offset">Distance between the center point and the dimension line.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        public Angular2LineDimension(Vector3 startFirstLine, Vector3 endFirstLine, Vector3 startSecondLine, Vector3 endSecondLine, double offset, DimensionStyle style)
            : base(DimensionType.Angular)
        {
            Vector3 dir1 = endFirstLine - startFirstLine;
            Vector3 dir2 = endSecondLine - startSecondLine;
            if (Vector3.AreParallel(dir1, dir2))
                throw new ArgumentException("The two lines that define the dimension are parallel.");

            this.startFirstLine = startFirstLine;
            this.endFirstLine = endFirstLine;
            this.startSecondLine = startSecondLine;
            this.endSecondLine = endSecondLine;

            if (MathHelper.IsZero(offset))
                throw new ArgumentOutOfRangeException(nameof(offset), "The offset value cannot be zero.");
            this.offset = offset;

            if (style == null)
                throw new ArgumentNullException(nameof(style), "The Dimension style cannot be null.");
            this.style = style;
        }

        #endregion

        #region internal properties

        internal Vector3 ArcDefinitionPoint
        {
            get { return this.arcDefinitionPoint; }
            set { this.arcDefinitionPoint = value; }
        }

        #endregion

        #region public properties

        /// <summary>
        /// Start <see cref="Vector3">point</see> of the first line that defines the angle to measure.
        /// </summary>
        public Vector3 StartFirstLine
        {
            get { return this.startFirstLine; }
            set { this.startFirstLine = value; }
        }

        /// <summary>
        /// End <see cref="Vector3">point</see> of the first line that defines the angle to measure.
        /// </summary>
        public Vector3 EndFirstLine
        {
            get { return this.endFirstLine; }
            set { this.endFirstLine = value; }
        }

        /// <summary>
        /// Start <see cref="Vector3">point</see> of the second line that defines the angle to measure.
        /// </summary>
        public Vector3 StartSecondLine
        {
            get { return this.startSecondLine; }
            set { this.startSecondLine = value; }
        }

        /// <summary>
        /// End <see cref="Vector3">point</see> of the second line that defines the angle to measure.
        /// </summary>
        public Vector3 EndSecondLine
        {
            get { return this.endSecondLine; }
            set { this.endSecondLine = value; }
        }

        /// <summary>
        /// Gets or sets the distance between the center point and the dimension line.
        /// </summary>
        /// <remarks>Zero values are not allowed, and even if negative values are permitted they are not recommended.</remarks>
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
                IList<Vector3> ocsPoints = MathHelper.Transform(new[] { this.startFirstLine, this.endFirstLine, this.startSecondLine, this.endSecondLine }, this.normal, CoordinateSystem.World, CoordinateSystem.Object);
                Vector3 refPoint;

                refPoint = ocsPoints[0];
                Vector2 ref1Start = new Vector2(refPoint.X, refPoint.Y);

                refPoint = ocsPoints[1];
                Vector2 ref1End = new Vector2(refPoint.X, refPoint.Y);

                refPoint = ocsPoints[2];
                Vector2 ref2Start = new Vector2(refPoint.X, refPoint.Y);

                refPoint = ocsPoints[3];
                Vector2 ref2End = new Vector2(refPoint.X, refPoint.Y);

                Vector2 dirRef1 = ref1End - ref1Start;
                Vector2 dirRef2 = ref2End - ref2Start;
                if (Vector2.AreParallel(dirRef1, dirRef2))
                    throw new ArgumentException("The two lines that define the dimension are parallel.");

                return Vector2.AngleBetween(dirRef1, dirRef2) * MathHelper.RadToDeg;
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
        /// Creates a new Angular2LineDimension that is a copy of the current instance.
        /// </summary>
        /// <returns>A new Angular2LineDimension that is a copy of this instance.</returns>
        public override object Clone()
        {
            Angular2LineDimension entity = new Angular2LineDimension
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
                    //Angular2LineDimension properties
                    StartFirstLine = this.startFirstLine,
                    EndFirstLine = this.endFirstLine,
                    StartSecondLine = this.startSecondLine,
                    EndSecondLine = this.endSecondLine,
                    Offset = this.offset
                };

            foreach (XData data in this.XData.Values)
                entity.XData.Add((XData)data.Clone());

            return entity;

        }

        #endregion
    }
}