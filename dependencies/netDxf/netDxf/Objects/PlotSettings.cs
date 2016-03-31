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

namespace netDxf.Objects
{
    /// <summary>
    /// Represents the plot settings of a layout.
    /// </summary>
    public class PlotSettings :
        ICloneable
    {
        #region private fields

        private string pageSetupName;
        private string plotterName;
        private string paperSizeName;
        private string viewName;
        private string currentStyleSheet;

        private double left;
        private double bottom;
        private double right;
        private double top;

        private Vector2 paperSize;
        private Vector2 origin;
        private Vector2 windowUpRight;
        private Vector2 windowBottomLeft;

        private double numeratorScale;
        private double denominatorScale;
        private PlotFlags flags;
        private PlotPaperUnits paperUnits;
        private PlotRotation rotation;
        private Vector2 paperImageOrigin;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new plot settings.
        /// </summary>
        public PlotSettings()
        {
            this.pageSetupName = string.Empty;
            this.plotterName = "none_device";
            this.paperSizeName = "ISO_A4_(210.00_x_297.00_MM)";
            this.viewName = string.Empty;
            this.currentStyleSheet = string.Empty;

            this.left = 7.5;
            this.bottom = 20.0;
            this.right = 7.5;
            this.top = 20.0;

            this.paperSize = new Vector2(210.0, 297.0);
            this.origin = Vector2.Zero;
            this.windowUpRight = Vector2.Zero;
            this.windowBottomLeft = Vector2.Zero;

            this.numeratorScale = 1.0;
            this.denominatorScale = 1.0;
            this.flags = PlotFlags.DrawViewportsFirst | PlotFlags.PrintLineweights | PlotFlags.PlotPlotStyles | PlotFlags.UseStandardScale;

            this.paperUnits = PlotPaperUnits.Milimeters;
            this.rotation = PlotRotation.Degrees90;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the page setup name.
        /// </summary>
        public string PageSetupName
        {
            get { return this.pageSetupName; }
            set { this.pageSetupName = value; }
        }

        /// <summary>
        /// Gets or sets the name of system printer or plot configuration file.
        /// </summary>
        public string PlotterName
        {
            get { return this.plotterName; }
            set { this.plotterName = value; }
        }

        /// <summary>
        /// Gets or set the paper size name.
        /// </summary>
        public string PaperSizeName
        {
            get { return this.paperSizeName; }
            set { this.paperSizeName = value; }
        }

        /// <summary>
        /// Gets or sets the plot view name.
        /// </summary>
        public string ViewName
        {
            get { return this.viewName; }
            set { this.viewName = value; }
        }

        /// <summary>
        /// Gets or sets the current style sheet name.
        /// </summary>
        public string CurrentStyleSheet
        {
            get { return this.currentStyleSheet; }
            set { this.currentStyleSheet = value; }
        }

        /// <summary>
        /// Gets or set the size, in millimeters, of unprintable margin on left side of paper.
        /// </summary>
        public double LeftMargin
        {
            get { return this.left; }
            set { this.left = value; }
        }

        /// <summary>
        /// Gets or set the size, in millimeters, of unprintable margin on bottom side of paper.
        /// </summary>
        public double BottomMargin
        {
            get { return this.bottom; }
            set { this.bottom = value; }
        }

        /// <summary>
        /// Gets or set the size, in millimeters, of unprintable margin on right side of paper.
        /// </summary>
        public double RightMargin
        {
            get { return this.right; }
            set { this.right = value; }
        }

        /// <summary>
        /// Gets or set the size, in millimeters, of unprintable margin on top side of paper.
        /// </summary>
        public double TopMargin
        {
            get { return this.top; }
            set { this.top = value; }
        }

        /// <summary>
        /// Gets or sets the plot paper size: physical paper width and height in millimeters.
        /// </summary>
        public Vector2 PaperSize
        {
            get { return this.paperSize; }
            set { this.paperSize = value; }
        }

        /// <summary>
        /// Gets or sets the plot origin in millimeters.
        /// </summary>
        public Vector2 Origin
        {
            get { return this.origin; }
            set { this.origin = value; }
        }

        /// <summary>
        /// Gets or sets the plot upper-right window corner.
        /// </summary>
        public Vector2 WindowUpRight
        {
            get { return this.windowUpRight; }
            set { this.windowUpRight = value; }
        }

        /// <summary>
        /// Gets or sets the plot lower-left window corner.
        /// </summary>
        public Vector2 WindowBottomLeft
        {
            get { return this.windowBottomLeft; }
            set { this.windowBottomLeft = value; }
        }

        /// <summary>
        /// Gets or sets the numerator of custom print scale: real world (paper) units
        /// </summary>
        public double PrintScaleNumerator
        {
            get { return this.numeratorScale; }
            set { this.numeratorScale = value; }
        }

        /// <summary>
        /// Gets or sets the denominator of custom print scale: real world (paper) units
        /// </summary>
        public double PrintScaleDenominator
        {
            get { return this.denominatorScale; }
            set { this.denominatorScale = value; }
        }

        /// <summary>
        /// Gets or sets the plot layout flags.
        /// </summary>
        public PlotFlags Flags
        {
            get { return this.flags; }
            set { this.flags = value; }
        }

        /// <summary>
        /// Gets or sets the paper units.
        /// </summary>
        public PlotPaperUnits PaperUnits
        {
            get { return this.paperUnits; }
            set { this.paperUnits = value; }
        }

        /// <summary>
        /// Gets or sets the paper rotation.
        /// </summary>
        public PlotRotation PaperRotation
        {
            get { return this.rotation; }
            set { this.rotation = value; }
        }

        /// <summary>
        /// Gets the scale factor.
        /// </summary>
        public double PrintScale
        {
            get { return this.numeratorScale/this.denominatorScale; }
        }

        /// <summary>
        /// Gets or sets the paper image origin.
        /// </summary>
        public Vector2 PaperImageOrigin
        {
            get { return this.paperImageOrigin; }
            set { this.paperImageOrigin = value; }
        }

        #endregion

        #region implements ICloneable

        /// <summary>
        /// Creates a new plot settings that is a copy of the current instance.
        /// </summary>
        /// <returns>A new plot settings that is a copy of this instance.</returns>
        public object Clone()
        {
            return new PlotSettings
            {
                PageSetupName = this.pageSetupName,
                PlotterName = this.plotterName,
                PaperSizeName = this.paperSizeName,
                ViewName = this.viewName,
                CurrentStyleSheet = this.currentStyleSheet,
                LeftMargin = this.left,
                BottomMargin = this.bottom,
                RightMargin = this.right,
                TopMargin = this.top,
                PaperSize = this.paperSize,
                Origin = this.origin,
                WindowUpRight = this.windowUpRight,
                WindowBottomLeft = this.windowBottomLeft,
                PrintScaleNumerator = this.numeratorScale,
                PrintScaleDenominator = this.denominatorScale,
                Flags = this.flags,
                PaperUnits = this.paperUnits,
                PaperRotation = this.rotation
            };
        }

        #endregion
    }
}