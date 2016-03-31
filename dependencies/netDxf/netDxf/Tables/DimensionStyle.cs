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
using netDxf.Collections;
using netDxf.Units;

namespace netDxf.Tables
{
    /// <summary>
    /// Represents a dimension style.
    /// </summary>
    public class DimensionStyle :
        TableObject
    {
        #region delegates and events

        public delegate void LineTypeChangedEventHandler(TableObject sender, TableObjectChangedEventArgs<LineType> e);
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

        public delegate void TextStyleChangedEventHandler(TableObject sender, TableObjectChangedEventArgs<TextStyle> e);
        public event TextStyleChangedEventHandler TextStyleChanged;
        protected virtual TextStyle OnTextStyleChangedEvent(TextStyle oldTextStyle, TextStyle newTextStyle)
        {
            TextStyleChangedEventHandler ae = this.TextStyleChanged;
            if (ae != null)
            {
                TableObjectChangedEventArgs<TextStyle> eventArgs = new TableObjectChangedEventArgs<TextStyle>(oldTextStyle, newTextStyle);
                ae(this, eventArgs);
                return eventArgs.NewValue;
            }
            return newTextStyle;
        }

        public delegate void BlockChangedEventHandler(TableObject sender, TableObjectChangedEventArgs<Block> e);
        public event BlockChangedEventHandler BlockChanged;
        protected virtual Block OnBlockChangedEvent(Block oldBlock, Block newBlock)
        {
            BlockChangedEventHandler ae = this.BlockChanged;
            if (ae != null)
            {
                TableObjectChangedEventArgs<Block> eventArgs = new TableObjectChangedEventArgs<Block>(oldBlock, newBlock);
                ae(this, eventArgs);
                return eventArgs.NewValue;
            }
            return newBlock;
        }

        #endregion

        #region private fields

        // dimension lines
        private AciColor dimclrd;
        private LineType dimltype;
        private Lineweight dimlwd;
        private double dimdle;
        private double dimdli;

        // extension lines
        private AciColor dimclre;
        private LineType dimltex1;
        private LineType dimltex2;
        private Lineweight dimlwe;
        private bool dimse1;
        private bool dimse2;
        private double dimexo;
        private double dimexe;

        // symbols and arrows
        private double dimasz;
        private double dimcen;
        private bool dimsah;
        private Block dimldrblk;
        private Block dimblk;
        private Block dimblk1;
        private Block dimblk2;

        // text
        private TextStyle dimtxsty;
        private AciColor dimclrt;
        private double dimtxt;
        private short dimjust;
        private short dimtad;
        private double dimgap;

        //fit
        private double dimscale;
        private short dimtih;
        private short dimtoh;

        // primary units
        private short dimadec;
        private short dimdec;
        private string dimpost;
        private char dimdsep;
        private double dimlfac;
        private LinearUnitType dimlunit;
        private AngleUnitType dimaunit;
        private FractionFormatType dimfrac;
        private bool suppressLinearLeadingZeros;
        private bool suppressLinearTrailingZeros;
        private bool suppressAngularLeadingZeros;
        private bool suppressAngularTrailingZeros;
        private bool suppressZeroFeet;
        private bool suppressZeroInches;
        private double dimrnd;

        #endregion

        #region constants

        /// <summary>
        /// Default dimension style name.
        /// </summary>
        public const string DefaultName = "Standard";

        /// <summary>
        /// Gets the default dimension style.
        /// </summary>
        public static DimensionStyle Default
        {
            get { return new DimensionStyle(DefaultName); }
        }

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>DimensionStyle</c> class.
        /// </summary>
        /// <param name="name">The dimension style name.</param>
        public DimensionStyle(string name)
            : this(name, true)
        {
        }

        internal DimensionStyle(string name, bool checkName)
            : base(name, DxfObjectCode.DimStyle, checkName)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name), "The dimension style name should be at least one character long.");

            this.reserved = name.Equals(DefaultName, StringComparison.OrdinalIgnoreCase);

            // dimension lines
            this.dimclrd = AciColor.ByBlock;
            this.dimltype = LineType.ByBlock;
            this.dimlwd = Lineweight.ByBlock;
            this.dimdli = 0.38;
            this.dimdle = 0.0;

            // extension lines
            this.dimclre = AciColor.ByBlock;
            this.dimltex1 = LineType.ByBlock;
            this.dimltex2 = LineType.ByBlock;
            this.dimlwe = Lineweight.ByBlock;
            this.dimse1 = false;
            this.dimse2 = false;
            this.dimexo = 0.0625;
            this.dimexe = 0.18;

            // symbols and arrows
            this.dimasz = 0.18;
            this.dimcen = 0.09;
            this.dimsah = false;
            this.dimldrblk = null;
            this.dimblk = null;
            this.dimblk1 = null;
            this.dimblk2 = null;

            // text
            this.dimtxsty = TextStyle.Default;
            this.dimclrt = AciColor.ByBlock;
            this.dimtxt = 0.18;
            this.dimtad = 1;
            this.dimjust = 0;
            this.dimgap = 0.09;

            // fit
            this.dimscale = 1.0;

            // primary units
            this.dimdec = 2;
            this.dimadec = 0;
            this.dimpost = "<>";
            this.dimtih = 0;
            this.dimtoh = 0;
            this.dimdsep = '.';
            this.dimlfac = 1.0;
            this.dimaunit = AngleUnitType.DecimalDegrees;
            this.dimlunit = LinearUnitType.Decimal;
            this.dimfrac = FractionFormatType.Horizontal;
            this.suppressLinearLeadingZeros = false;
            this.suppressLinearTrailingZeros = false;
            this.suppressAngularLeadingZeros = false;
            this.suppressAngularTrailingZeros = false;
            this.suppressZeroFeet = true;
            this.suppressZeroInches = true;
            this.dimrnd = 0.0;
        }

        #endregion

        #region public properties

        #region dimension lines

        /// <summary>
        /// Assigns colors to dimension lines, arrowheads, and dimension leader lines.
        /// </summary>
        /// <remarks>
        /// Default: ByBlock<br />
        /// Only indexed AciColors are supported.
        /// </remarks>
        public AciColor DIMCLRD
        {
            get { return this.dimclrd; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.dimclrd = value;
            }
        }

        /// <summary>
        /// Sets the line type of the dimension line.
        /// </summary>
        /// <remarks>Default: ByBlock</remarks>
        public LineType DIMLTYPE
        {
            get { return this.dimltype; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.dimltype = this.OnLineTypeChangedEvent(this.dimltype, value);
            }
        }

        /// <summary>
        /// Assigns line weight to dimension lines.
        /// </summary>
        /// <remarks>Default: ByBlock</remarks>
        public Lineweight DIMLWD
        {
            get { return this.dimlwd; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.dimlwd = value;
            }
        }

        /// <summary>
        /// Sets the distance the dimension line extends beyond the extension line when oblique, architectural tick, integral, or no marks are drawn for arrowheads.
        /// </summary>
        /// <remarks>Default: 0.0</remarks>
        public double DIMDLE
        {
            get { return this.dimdle; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The DIMDLE must be equals or greater than zero.");
                this.dimdle = value;
            }
        }

        /// <summary>
        /// Controls the spacing of the dimension lines in baseline dimensions.
        /// </summary>
        /// <remarks>
        /// Default: 0.38<br />
        /// This value is stored only for information purposes.
        /// Base dimensions are a compound entity made of several dimensions, there is no actual dxf entity that represents that.
        /// </remarks>
        public double DIMDLI
        {
            get { return this.dimdli; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The DIMDLI must be equals or greater than zero.");
                this.dimdli = value;
            }
        }

        #endregion

        #region extension lines

        /// <summary>
        /// Assigns colors to extension lines, center marks, and centerlines.
        /// </summary>
        /// <remarks>
        /// Default: ByBlock<br />
        /// Only indexed AciColors are supported.
        /// </remarks>
        public AciColor DIMCLRE
        {
            get { return this.dimclre; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.dimclre = value;
            }
        }

        /// <summary>
        /// Sets the line type of the first extension line.
        /// </summary>
        /// <remarks>Default: ByBlock</remarks>
        public LineType DIMLTEX1
        {
            get { return this.dimltex1; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.dimltex1 = this.OnLineTypeChangedEvent(this.dimltex1, value);
            }
        }

        /// <summary>
        /// Sets the line type of the second extension line.
        /// </summary>
        /// <remarks>Default: ByBlock</remarks>
        public LineType DIMLTEX2
        {
            get { return this.dimltex2; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.dimltex2 = this.OnLineTypeChangedEvent(this.dimltex2, value);
            }
        }

        /// <summary>
        /// Assigns line weight to extension lines.
        /// </summary>
        /// <remarks>Default: ByBlock</remarks>
        public Lineweight DIMLWE
        {
            get { return this.dimlwe; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.dimlwe = value;
            }
        }

        /// <summary>
        /// Suppresses display of the first extension line.
        /// </summary>
        /// <remarks>Default: false</remarks>
        public bool DIMSE1
        {
            get { return this.dimse1; }
            set { this.dimse1 = value; }
        }

        /// <summary>
        /// Suppresses display of the second extension line.
        /// </summary>
        /// <remarks>Default: false</remarks>
        public bool DIMSE2
        {
            get { return this.dimse2; }
            set { this.dimse2 = value; }
        }

        /// <summary>
        /// Specifies how far extension lines are offset from origin points.
        /// </summary>
        /// <remarks>Default: 0.0625</remarks>
        public double DIMEXO
        {
            get { return this.dimexo; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The DIMEXO must be equals or greater than zero.");
                this.dimexo = value;
            }
        }

        /// <summary>
        /// Specifies how far to extend the extension line beyond the dimension line.
        /// </summary>
        /// <remarks>Default: 0.18</remarks>
        public double DIMEXE
        {
            get { return this.dimexe; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The DIMEXE must be equals or greater than zero.");
                this.dimexe = value;
            }
        }

        #endregion

        #region symbols and arrows

        /// <summary>
        /// Controls the size of dimension line and leader line arrowheads. Also controls the size of hook lines.
        /// </summary>
        /// <remarks>Default: 0.18</remarks>
        public double DIMASZ
        {
            get { return this.dimasz; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The DIMASZ must be equals or greater than zero.");
                this.dimasz = value;
            }
        }

        /// <summary>
        /// Controls drawing of circle or arc center marks and centerlines.
        /// </summary>
        /// <remarks>
        /// 0 - No center marks or lines are drawn.<br />
        /// greater than 0 - Center marks are drawn.<br />
        /// lower than 0 - Center marks and centerlines are drawn.<br />
        /// The absolute value specifies the size of the center mark or centerline. 
        /// The size of the centerline is the length of the centerline segment that extends outside the circle or arc.
        /// It is also the size of the gap between the center mark and the start of the centerline. 
        /// The size of the center mark is the distance from the center of the circle or arc to the end of the center mark.<br/>
        /// Default: 0.09
        /// </remarks>
        public double DIMCEN
        {
            get { return this.dimcen; }
            set { this.dimcen = value; }
        }

        /// <summary>
        /// Controls the display of dimension line arrowhead blocks.
        /// </summary>
        /// <remarks>
        /// Default value: false.<br />
        /// false = Use arrowhead blocks set by DIMBLK.<br />
        /// true = Use arrowhead blocks set by DIMBLK1 and DIMBLK2.
        /// </remarks>
        public bool DIMSAH
        {
            get { return this.dimsah; }
            set { this.dimsah = value; }
        }

        /// <summary>
        /// Gets or sets the arrowhead block for leaders.
        /// </summary>
        /// <remarks>Default: null. Closed filled.</remarks>
        public Block DIMLDRBLK
        {
            get { return this.dimldrblk; }
            set { this.dimldrblk = value == null ? null : this.OnBlockChangedEvent(this.dimldrblk, value); }
        }

        /// <summary>
        /// Gets or sets the arrowhead block displayed at the ends of dimension lines.
        /// </summary>
        /// <remarks>Default: null. Closed filled.</remarks>
        public Block DIMBLK
        {
            get { return this.dimblk; }
            set { this.dimblk = value == null ? null : this.OnBlockChangedEvent(this.dimblk, value); }
        }

        /// <summary>
        /// Gets or sets the arrowhead block for the first end of the dimension line when the drawing variable DIMSAH is true.
        /// </summary>
        /// <remarks>Default: null. Closed filled.</remarks>
        public Block DIMBLK1
        {
            get { return this.dimblk1; }
            set { this.dimblk1 = value == null ? null : this.OnBlockChangedEvent(this.dimblk1, value); }
        }

        /// <summary>
        /// Gets or sets the arrowhead block for the second end of the dimension line when the drawing variable DIMSAH is true.
        /// </summary>
        /// <remarks>Default: null. Closed filled.</remarks>
        public Block DIMBLK2
        {
            get { return this.dimblk2; }
            set{ this.dimblk2 = value == null ? null : this.OnBlockChangedEvent(this.dimblk2, value); }
        }

        #endregion

        #region text appearance

        /// <summary>
        /// Gets or sets the text style of the dimension.
        /// </summary>
        /// <remarks>Default: Standard</remarks>
        public TextStyle DIMTXSTY
        {
            get { return this.dimtxsty; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.dimtxsty = this.OnTextStyleChangedEvent(this.dimtxsty, value);
            }
        }

        /// <summary>
        /// Gets or set the color of dimension text.
        /// </summary>
        /// <remarks>
        /// Default: ByBlock<br />
        /// Only indexed AciColors are supported.
        /// </remarks>
        public AciColor DIMCLRT
        {
            get { return this.dimclrt; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.dimclrt = value;
            }
        }

        /// <summary>
        /// Specifies the height of dimension text, unless the current text style has a fixed height.
        /// </summary>
        /// <remarks>Default: 0.18</remarks>
        public double DIMTXT
        {
            get { return this.dimtxt; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The DIMTXT must be greater than zero.");
                this.dimtxt = value;
            }
        }

        /// <summary>
        /// Sets the distance around the dimension text when the dimension line breaks to accommodate dimension text.
        /// </summary>
        /// <remarks>Default: 0.09</remarks>
        public double DIMGAP
        {
            get { return this.dimgap; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The DIMGAP must be equals or greater than zero.");
                this.dimgap = value;
            }
        }

        /// <summary>
        /// Controls the horizontal positioning of dimension text.
        /// </summary>
        /// <remarks>
        /// Default: Centered
        /// <br/>
        /// Not implemented in the dimension drawing.
        /// </remarks>
        internal short DIMJUST
        {
            get { return this.dimjust; }
            set { this.dimjust = value; }
        }

        /// <summary>
        /// Controls the vertical position of text in relation to the dimension line.
        /// </summary>
        /// <remarks>
        /// Default: Above<br/>
        /// Not implemented in the dimension drawing.
        /// </remarks>
        internal short DIMTAD
        {
            get { return this.dimtad; }
            set { this.dimtad = value; }
        }

        /// <summary>
        /// Gets or sets the fraction format when DIMLUNIT is set to Architectural or Fractional.
        /// </summary>
        /// <remarks>
        /// Horizontal stacking<br/>
        /// Diagonal stacking<br/>
        /// Not stacked (for example, 1/2)
        /// </remarks>
        public FractionFormatType DIMFRAC
        {
            get { return this.dimfrac; }
            set { this.dimfrac = value; }
        }

        #endregion

        #region fit

        /// <summary>
        /// Get or set the overall scale factor applied to dimensioning variables that specify sizes, distances, or offsets.
        /// </summary>
        /// <remarks>
        /// DIMSCALE does not affect measured lengths, coordinates, or angles.<br/>
        /// DIMSCALE values of zero are not supported, any imported drawing with a zero value will set the scale to the default 1.0.<br/>
        /// Default: 1.0
        /// </remarks>
        public double DIMSCALE
        {
            get { return this.dimscale; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The DIMSCALE must be greater than zero.");
                this.dimscale = value;
            }
        }

        /// <summary>
        /// Controls the position of dimension text inside the extension lines for all dimension types except Ordinate.
        /// </summary>
        /// <remarks>
        /// Default: 0<br/>
        /// Not implemented in the dimension drawing.
        /// </remarks>
        internal short DIMTIH
        {
            get { return this.dimtih; }
            set { this.dimtih = value; }
        }

        /// <summary>
        /// Controls the position of dimension text outside the extension lines.
        /// </summary>
        /// <remarks>
        /// Default: 0<br/>
        /// Not implemented in the dimension drawing.
        /// </remarks>
        internal short DIMTOH
        {
            get { return this.dimtoh; }
            set { this.dimtoh = value; }
        }

        #endregion

        #region primary units

        /// <summary>
        /// Sets the number of decimal places displayed for the primary units of a dimension.
        /// </summary>
        /// <remarks>
        /// Default: 2<br/>
        /// It is recommended to use values in the range 0 to 8.<br/>
        /// For architectural and fractional the precision used for the minimum fraction is 1/2^LinearDecimalPlaces.
        /// </remarks>
        public short DIMDEC
        {
            get { return this.dimdec; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The DIMDEC must be equals or greater than zero.");
                this.dimdec = value;
            }
        }
 
        /// <summary>
        /// Controls the number of precision places displayed in angular dimensions.
        /// </summary>
        /// <remarks>
        /// Default: 0<br/>
        /// If set to -1 angular dimensions display the number of decimal places specified by DIMDEC.
        /// It is recommended to use values in the range 0 to 8.
        /// </remarks>
        public short DIMADEC
        {
            get { return this.dimadec; }
            set
            {
                if (value < -1)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The DIMADEC must be equals or greater than zero.");
                this.dimadec = value;
            }
        }

        /// <summary>
        /// Specifies a text prefix or suffix (or both) to the dimension measurement.
        /// </summary>
        /// <remarks>
        /// Use "&lt;&gt;" to indicate placement of the text in relation to the dimension value. 
        /// For example, enter "&lt;&gt;mm" to display a 5.0 millimeter radial dimension as "5.0mm". 
        /// If you entered "mm &lt;&gt;", the dimension would be displayed as "mm 5.0".
        /// Use the "&lt;&gt;" mechanism for angular dimensions.<br/>
        /// Default: "&lt;&gt;"
        /// </remarks>
        public string DIMPOST
        {
            get { return this.dimpost; }
            set { this.dimpost = value; }
        }

        /// <summary>
        /// Specifies a single-character decimal separator to use when creating dimensions whose unit format is decimal.
        /// </summary>
        /// <remarks>Default: "."</remarks>
        public char DIMDSEP
        {
            get { return this.dimdsep; }
            set { this.dimdsep = value; }
        }

        /// <summary>
        /// Gets or sets a scale factor for linear dimension measurements
        /// </summary>
        /// <remarks>
        /// All linear dimension distances, including radii, diameters, and coordinates, are multiplied by DIMLFAC before being converted to dimension text.<br />
        /// Positive values of DIMLFAC are applied to dimensions in both model space and paper space; negative values are applied to paper space only.<br />
        /// DIMLFAC has no effect on angular dimensions.
        /// </remarks>
        public double DIMLFAC
        {
            get { return this.dimlfac; }
            set
            {
                if (MathHelper.IsZero(value)) throw new ArgumentOutOfRangeException(nameof(value), value, "The scale factor cannot be zero.");
                this.dimlfac = value;
            }
        }

        /// <summary>
        /// Gets or sets the units for all dimension types except angular.
        /// </summary>
        /// <remarks>
        /// Scientific<br/>
        /// Decimal<br/>
        /// Engineering<br/>
        /// Architectural<br/>
        /// Fractional
        /// </remarks>
        public LinearUnitType DIMLUNIT
        {
            get { return this.dimlunit; }
            set { this.dimlunit = value; }
        }

        /// <summary>
        /// Gets or sets the units format for angular dimensions.
        /// </summary>
        /// <remarks>
        /// Decimal degrees<br/>
        /// Degrees/minutes/seconds<br/>
        /// Gradians<br/>
        /// Radians
        /// </remarks>
        public AngleUnitType DIMAUNIT
        {
            get { return this.dimaunit; }
            set
            {
                if (value == AngleUnitType.SurveyorUnits)
                    throw new ArgumentException("Surveyor's units are not applicable in angular dimensions.");
                this.dimaunit = value;
            }
        }

        /// <summary>
        /// Suppresses leading zeros in linear decimal dimensions (for example, 0.5000 becomes .5000).
        /// </summary>
        /// <remarks>This value is part of the DIMDEC variable.</remarks>
        public bool SuppressLinearLeadingZeros
        {
            get { return this.suppressLinearLeadingZeros; }
            set { this.suppressLinearLeadingZeros = value; }
        }

        /// <summary>
        /// Suppresses trailing zeros in linear decimal dimensions (for example, 12.5000 becomes 12.5).
        /// </summary>
        /// <remarks>This value is part of the DIMDEC variable.</remarks>
        public bool SuppressLinearTrailingZeros
        {
            get { return this.suppressLinearTrailingZeros; }
            set { this.suppressLinearTrailingZeros = value; }
        }

        /// <summary>
        /// Suppresses leading zeros in angular decimal dimensions (for example, 0.5000 becomes .5000).
        /// </summary>
        /// <remarks>This value is part of the DIMADEC variable.</remarks>
        public bool SuppressAngularLeadingZeros
        {
            get { return this.suppressAngularLeadingZeros; }
            set { this.suppressAngularLeadingZeros = value; }
        }

        /// <summary>
        /// Suppresses trailing zeros in angular decimal dimensions (for example, 12.5000 becomes 12.5).
        /// </summary>
        /// <remarks>This value is part of the DIMADEC variable.</remarks>
        public bool SuppressAngularTrailingZeros
        {
            get { return this.suppressAngularTrailingZeros; }
            set { this.suppressAngularTrailingZeros = value; }
        }

        /// <summary>
        /// Suppresses zero feet in architectural dimensions.
        /// </summary>
        /// <remarks>This value is part of the DIMDEC variable.</remarks>
        public bool SuppressZeroFeet
        {
            get { return this.suppressZeroFeet; }
            set { this.suppressZeroFeet = value; }
        }

        /// <summary>
        /// Suppresses zero inches in architectural dimensions.
        /// </summary>
        /// <remarks>This value is part of the DIMDEC variable.</remarks>
        public bool SuppressZeroInches
        {
            get { return this.suppressZeroInches; }
            set { this.suppressZeroInches = value; }
        }

        /// <summary>
        /// Gets or sets the value to round all dimensioning distances.
        /// </summary>
        /// <remarks>
        /// Default: 0 (no rounding off).<br/>
        /// If DIMRND is set to 0.25, all distances round to the nearest 0.25 unit.
        /// If you set DIMRND to 1.0, all distances round to the nearest integer.
        /// Note that the number of digits edited after the decimal point depends on the precision set by DIMDEC.
        /// DIMRND does not apply to angular dimensions.
        /// </remarks>
        public double DIMRND
        {
            get { return this.dimrnd; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The nearest value to round all distances must be equals or greater than zero.");
                this.dimrnd = value;
            }
        }

        #endregion
 
        /// <summary>
        /// Gets the owner of the actual dxf object.
        /// </summary>
        public new DimensionStyles Owner
        {
            get { return (DimensionStyles) this.owner; }
            internal set { this.owner = value; }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new DimensionStyle that is a copy of the current instance.
        /// </summary>
        /// <param name="newName">DimensionStyle name of the copy.</param>
        /// <returns>A new DimensionStyle that is a copy of this instance.</returns>
        public override TableObject Clone(string newName)
        {
            DimensionStyle copy = new DimensionStyle(newName)
            {
                // dimension lines
                DIMCLRD = (AciColor) this.dimclrd.Clone(),
                DIMLTYPE = (LineType) this.dimltype.Clone(),
                DIMLWD = (Lineweight) this.dimlwd.Clone(),
                DIMDLI = this.dimdli,
                DIMDLE = this.dimdle,

                // extension lines
                DIMCLRE = (AciColor) this.dimclre.Clone(),
                DIMLTEX1 = (LineType) this.dimltex1.Clone(),
                DIMLTEX2 = (LineType) this.dimltex2.Clone(),
                DIMLWE = (Lineweight) this.dimlwe.Clone(),
                DIMSE1 = this.dimse1,
                DIMSE2 = this.dimse2,
                DIMEXO = this.dimexo,
                DIMEXE = this.dimexe,

                // symbols and arrows
                DIMASZ = this.dimasz,
                DIMCEN = this.dimcen,
                DIMSAH = this.dimsah,            

                // fit
                DIMSCALE = this.dimscale,
                DIMTIH = this.dimtih,
                DIMTOH = this.dimtoh,

                // text appearance
                DIMTXSTY = (TextStyle)this.dimtxsty.Clone(),
                DIMCLRT = (AciColor)this.dimclrt.Clone(),
                DIMTXT = this.dimtxt,
                DIMJUST = this.dimjust,
                DIMTAD = this.dimtad,
                DIMGAP = this.dimgap,

                // primary units
                DIMADEC = this.dimadec,
                DIMDEC = this.dimdec,
                DIMPOST = this.dimpost,
                DIMDSEP = this.dimdsep,
                DIMAUNIT = this.dimaunit
            };

            if (this.dimldrblk != null) copy.DIMLDRBLK = (Block)this.dimldrblk.Clone();
            if (this.dimblk != null) copy.DIMBLK = (Block) this.dimblk.Clone();
            if (this.dimblk1 != null) copy.DIMBLK1 = (Block) this.dimblk1.Clone();
            if (this.dimblk2 != null) copy.DIMBLK2 = (Block) this.dimblk2.Clone();

            return copy;
        }

        /// <summary>
        /// Creates a new DimensionStyle that is a copy of the current instance.
        /// </summary>
        /// <returns>A new DimensionStyle that is a copy of this instance.</returns>
        public override object Clone()
        {
            return this.Clone(this.name);
        }

        #endregion
    }
}