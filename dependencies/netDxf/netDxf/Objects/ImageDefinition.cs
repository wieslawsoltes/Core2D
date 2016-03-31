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
using System.Drawing;
using System.IO;
using netDxf.Collections;
using netDxf.Tables;
using netDxf.Units;

namespace netDxf.Objects
{
    /// <summary>
    /// Represents an image definition.
    /// </summary>
    public class ImageDefinition :
        TableObject
    {
        #region private fields

        private readonly string fileName;
        private readonly int width;
        private readonly int height;
        private ImageResolutionUnits resolutionUnits;
        // internally we will store the resolution in PPI
        private double horizontalResolution;
        private double verticalResolution;

        // this will store the references to the images that makes use of this image definition (key: image handle, value: reactor)
        private readonly Dictionary<string, ImageDefinitionReactor> reactors;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>ImageDefinition</c> class.
        /// </summary>
        /// <param name="fileName">Image file name with full or relative path.</param>
        /// <param name="width">Image width in pixels.</param>
        /// <param name="horizontalResolution">Image horizontal resolution in pixels.</param>
        /// <param name="height">Image height in pixels.</param>
        /// <param name="verticalResolution">Image vertical resolution in pixels.</param>
        /// <param name="units">Image resolution units.</param>
        /// <remarks>
        /// <para>
        /// The name of the file without extension will be used as the name of the image definition.
        /// </para>
        /// <para>
        /// This is a generic constructor for all image formats supported by AutoCAD, note that not all AutoCAD versions support the same image formats.
        /// </para>
        /// <para>
        /// Note (this is from the ACAD docs): AutoCAD 2000, AutoCAD LT 2000, and later releases do not support LZW-compressed TIFF files,
        /// with the exception of English language versions sold in the US and Canada.<br />
        /// If you have TIFF files that were created using LZW compression and want to insert them into a drawing 
        /// you must save the TIFF files with LZW compression disabled.
        /// </para>
        /// </remarks>
        public ImageDefinition(string fileName, int width, double horizontalResolution, int height, double verticalResolution, ImageResolutionUnits units)
            : this(fileName, Path.GetFileNameWithoutExtension(fileName), width, horizontalResolution, height, verticalResolution, units)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>ImageDefinition</c> class.
        /// </summary>
        /// <param name="fileName">Image file name with full or relative path.</param>
        /// <param name="name">Image definition name.</param>
        /// <param name="width">Image width in pixels.</param>
        /// <param name="horizontalResolution">Image horizontal resolution in pixels.</param>
        /// <param name="height">Image height in pixels.</param>
        /// <param name="verticalResolution">Image vertical resolution in pixels.</param>
        /// <param name="units">Image resolution units.</param>
        /// <remarks>
        /// <para>
        /// The name assigned to the image definition must be unique.
        /// </para>
        /// <para>
        /// This is a generic constructor for all image formats supported by AutoCAD, note that not all AutoCAD versions support the same image formats.
        /// </para>
        /// <para>
        /// Note (this is from the ACAD docs): AutoCAD 2000, AutoCAD LT 2000, and later releases do not support LZW-compressed TIFF files,
        /// with the exception of English language versions sold in the US and Canada.<br />
        /// If you have TIFF files that were created using LZW compression and want to insert them into a drawing 
        /// you must save the TIFF files with LZW compression disabled.
        /// </para>
        /// </remarks>
        public ImageDefinition(string fileName, string name, int width, double horizontalResolution, int height, double verticalResolution, ImageResolutionUnits units)
            : base(name, DxfObjectCode.ImageDef, false)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName), "The image file name cannot be null or empty.");

            this.fileName = fileName;
            this.width = width;
            this.height = height;
            this.horizontalResolution = horizontalResolution;
            this.verticalResolution = verticalResolution;
            this.resolutionUnits = units;

            this.reactors = new Dictionary<string, ImageDefinitionReactor>();
        }

        ///  <summary>
        ///  Initializes a new instance of the <c>ImageDefinition</c> class.
        ///  </summary>
        ///  <param name="fileName">Image file name with full or relative path.</param>
        /// <remarks>
        ///  <para>
        ///  The name of the file without extension will be used as the name of the image definition.
        ///  </para>
        ///  <para>
        ///  Supported image formats: BMP, JPG, PNG, TIFF.<br />
        ///  Even thought AutoCAD supports more image formats, this constructor is restricted to the ones the net framework supports in common with AutoCAD.
        ///  Use the generic constructor instead.
        ///  </para>
        ///  <para>
        ///  Note (this is from the ACAD docs): AutoCAD 2000, AutoCAD LT 2000, and later releases do not support LZW-compressed TIFF files,
        ///  with the exception of English language versions sold in the US and Canada.<br />
        ///  If you have TIFF files that were created using LZW compression and want to insert them into a drawing 
        ///  you must save the TIFF files with LZW compression disabled.
        ///  </para>
        /// </remarks>
        public ImageDefinition(string fileName)
            : this(fileName, Path.GetFileNameWithoutExtension(fileName))
        {
        }

        ///  <summary>
        ///  Initializes a new instance of the <c>ImageDefinition</c> class.
        ///  </summary>
        ///  <param name="fileName">Image file name with full or relative path.</param>
        ///  <param name="name">Image definition name.</param>
        /// <remarks>
        ///  <para>
        ///  The name assigned to the image definition must be unique.
        ///  </para>
        ///  <para>
        ///  Supported image formats: BMP, JPG, PNG, TIFF.<br />
        ///  Even thought AutoCAD supports more image formats, this constructor is restricted to the ones the .net library supports in common with AutoCAD.
        ///  Use the generic constructor instead.
        ///  </para>
        ///  <para>
        ///  Note (this is from the ACAD docs): AutoCAD 2000, AutoCAD LT 2000, and later releases do not support LZW-compressed TIFF files,
        ///  with the exception of English language versions sold in the US and Canada.<br />
        ///  If you have TIFF files that were created using LZW compression and want to insert them into a drawing 
        ///  you must save the TIFF files with LZW compression disabled.
        ///  </para>
        /// </remarks>
        public ImageDefinition(string fileName, string name)
            : base(name, DxfObjectCode.ImageDef, false)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName), "The image file name cannot be null or empty.");

            FileInfo info = new FileInfo(fileName);
            if (!info.Exists)
                throw new FileNotFoundException("Image file not found", fileName);

            this.fileName = fileName;

            try
            {
                using (Image bitmap = Image.FromFile(fileName))
                {
                    this.width = bitmap.Width;
                    this.height = bitmap.Height;
                    this.horizontalResolution = bitmap.HorizontalResolution;
                    this.verticalResolution = bitmap.VerticalResolution;
                    // the System.Drawing.Image stores the image resolution in inches
                    this.resolutionUnits = ImageResolutionUnits.Inches;
                }
            }
            catch (Exception)
            {
                throw new ArgumentException("Image file not supported.", fileName);
            }

            this.reactors = new Dictionary<string, ImageDefinitionReactor>();
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the image path.
        /// </summary>
        public string FileName
        {
            get { return this.fileName; }
        }

        /// <summary>
        /// Gets the image width in pixels.
        /// </summary>
        public int Width
        {
            get { return this.width; }
        }

        /// <summary>
        /// Gets the image height in pixels.
        /// </summary>
        public int Height
        {
            get { return this.height; }
        }

        /// <summary>
        /// Gets or sets the image resolution units.
        /// </summary>
        public ImageResolutionUnits ResolutionUnits
        {
            get { return this.resolutionUnits; }
            set
            {
                if (this.resolutionUnits != value)
                {
                    switch (value)
                    {
                        case ImageResolutionUnits.Centimeters:
                            this.horizontalResolution /= 2.54;
                            this.verticalResolution /= 2.54;
                            break;
                        case ImageResolutionUnits.Inches:
                            this.horizontalResolution *= 2.54;
                            this.verticalResolution *= 2.54;
                            break;
                        case ImageResolutionUnits.Unitless:
                            break;
                    }
                }
                this.resolutionUnits = value;
            }
        }

        /// <summary>
        /// Gets the image horizontal resolution in pixels per unit.
        /// </summary>
        public double HorizontalResolution
        {
            get { return this.horizontalResolution; }
        }

        /// <summary>
        /// Gets the image vertical resolution in pixels per unit.
        /// </summary>
        public double VerticalResolution
        {
            get { return this.verticalResolution; }
        }

        /// <summary>
        /// Gets the owner of the actual dxf object.
        /// </summary>
        public new ImageDefinitions Owner
        {
            get { return (ImageDefinitions) this.owner; }
            internal set { this.owner = value; }
        }

        #endregion

        #region internal properties

        internal Dictionary<string, ImageDefinitionReactor> Reactors
        {
            get { return this.reactors; }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new ImageDefinition that is a copy of the current instance.
        /// </summary>
        /// <param name="newName">ImageDefinition name of the copy.</param>
        /// <returns>A new ImageDefinition that is a copy of this instance.</returns>
        public override TableObject Clone(string newName)
        {
            return new ImageDefinition(this.fileName, newName, this.width, this.horizontalResolution, this.height, this.verticalResolution, this.resolutionUnits);
        }

        /// <summary>
        /// Creates a new ImageDefinition that is a copy of the current instance.
        /// </summary>
        /// <returns>A new ImageDefinition that is a copy of this instance.</returns>
        public override object Clone()
        {
            return this.Clone(this.name);
        }

        #endregion
    }
}