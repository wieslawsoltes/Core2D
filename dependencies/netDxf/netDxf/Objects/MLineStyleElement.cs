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

namespace netDxf.Objects
{
    /// <summary>
    /// Represent each of the elements that make up a MLineStyle.
    /// </summary>
    public class MLineStyleElement :
        IComparable<MLineStyleElement>,
        ICloneable
    {
        #region delegates and events

        public delegate void LineTypeChangedEventHandler(MLineStyleElement sender, TableObjectChangedEventArgs<LineType> e);
        public event LineTypeChangedEventHandler LineTypeChanged;
        protected virtual LineType OnLineTypeChangedEvent(LineType oldLineType, LineType newLineType)
        {
            LineTypeChangedEventHandler ae = this.LineTypeChanged;
            if (ae != null)
            {
                TableObjectChangedEventArgs<LineType> eventArgs = new TableObjectChangedEventArgs<LineType>(oldLineType, newLineType);
                ae(this, eventArgs);
                return eventArgs.NewValue;
            }
            return newLineType;
        }

        #endregion

        #region private fields

        private double offset;
        private AciColor color;
        private LineType lineType;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>MLineStyleElement</c> class.
        /// </summary>
        /// <param name="offset">Element offset.</param>
        public MLineStyleElement(double offset)
            : this(offset, AciColor.ByLayer, LineType.ByLayer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>MLineStyleElement</c> class.
        /// </summary>
        /// <param name="offset">Element offset.</param>
        /// <param name="color">Element color.</param>
        /// <param name="lineType">Element line type.</param>
        public MLineStyleElement(double offset, AciColor color, LineType lineType)
        {
            this.offset = offset;
            this.color = color;
            this.lineType = lineType;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the element offset.
        /// </summary>
        public double Offset
        {
            get { return this.offset; }
            set { this.offset = value; }
        }

        /// <summary>
        /// Gets or sets the element color.
        /// </summary>
        /// <remarks>
        /// AutoCad2000 dxf version does not support true colors for MLineStyleElement color.
        /// </remarks>
        public AciColor Color
        {
            get { return this.color; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.color = value;
            }
        }

        /// <summary>
        /// Gets or sets the element line type.
        /// </summary>
        public LineType LineType
        {
            get { return this.lineType; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.lineType = this.OnLineTypeChangedEvent(this.lineType, value);
            }
        }

        #endregion

        #region implements IComparable

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared.
        /// The return value has the following meanings: Value Meaning Less than zero This object is less than the other parameter.
        /// Zero This object is equal to other. Greater than zero This object is greater than other.
        /// </returns>
        public int CompareTo(MLineStyleElement other)
        {
            return this.offset.CompareTo(other.offset);
        }

        #endregion

        #region implements ICloneable

        /// <summary>
        /// Creates a MLineStyleElement that is a copy of the current instance.
        /// </summary>
        /// <returns>A new MLineStyleElement is a copy of this instance.</returns>
        public object Clone()
        {
            return new MLineStyleElement(this.offset)
            {
                Color = (AciColor)this.color.Clone(),
                LineType = (LineType) this.lineType.Clone()
            };
        }

        #endregion

        #region overrides

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return string.Format("{0}, color:{1}, line type:{2}", this.offset, this.color, this.lineType);
        }

        #endregion
    }
}