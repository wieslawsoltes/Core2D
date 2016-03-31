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
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using Microsoft.Win32;
using netDxf.Collections;

namespace netDxf.Tables
{
    /// <summary>
    /// Represents a text style.
    /// </summary>
    public class TextStyle :
        TableObject
    {
        #region private fields

        private readonly string font;
        private readonly string fontFamilyName;
        private readonly bool registered;
        private double height;
        private bool isBackward;
        private bool isUpsideDown;
        private bool isVertical;
        private double obliqueAngle;
        private double widthFactor;

        #endregion

        #region constants

        /// <summary>
        /// Default text style name.
        /// </summary>
        public const string DefaultName = "Standard";

        /// <summary>
        /// Gets the default text style.
        /// </summary>
        public static TextStyle Default
        {
            get { return new TextStyle(DefaultName, "simplex.shx"); }
        }

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>TextStyle</c> class. The font file name, without the extension, will be used as the TextStyle name.
        /// </summary>
        /// <param name="font">Text style font file name.</param>
        public TextStyle(string font)
            : this(Path.GetFileNameWithoutExtension(font), font)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>TextStyle</c> class.
        /// </summary>
        /// <param name="name">Text style name.</param>
        /// <param name="font">Text style font file name.</param>
        public TextStyle(string name, string font)
            : this(name, font, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>TextStyle</c> class.
        /// </summary>
        /// <param name="name">Text style name.</param>
        /// <param name="font">Text style font file name.</param>
        /// <param name="checkName">Specifies if the style name has to be checked.</param>
        internal TextStyle(string name, string font, bool checkName)
            : base(name, DxfObjectCode.TextStyle, checkName)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name), "The text style name should be at least one character long.");

            if (string.IsNullOrEmpty(font))
                throw new ArgumentNullException(nameof(font));

            this.reserved = name.Equals(DefaultName, StringComparison.OrdinalIgnoreCase);
            this.font = font;
            this.fontFamilyName = string.Empty;
            this.widthFactor = 1.0;
            this.obliqueAngle = 0.0;
            this.height = 0.0;
            this.isVertical = false;
            this.isBackward = false;
            this.isUpsideDown = false;
            
            if (Path.GetExtension(font).Equals(".ttf", StringComparison.OrdinalIgnoreCase))
            {
                PrivateFontCollection privateFontCollection = new PrivateFontCollection();
                privateFontCollection.AddFontFile(font);
                FontFamily[] fontFamilies = privateFontCollection.Families;
                string familyName = fontFamilies[0].Name;
                string registeredFileName = FontFileFromFamilyName(familyName);
                this.registered = !string.IsNullOrEmpty(registeredFileName);
                this.fontFamilyName = familyName;
            }
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the text style font file name.
        /// </summary>
        public string FontName
        {
            get { return this.font; }
        }

        /// <summary>
        /// Gets the style font file name without the extension.
        /// </summary>
        public string FontNameWithoutExtension
        {
            get { return Path.GetFileNameWithoutExtension(this.font); }
        }

        /// <summary>
        /// Gets or sets the text height.
        /// </summary>
        /// <remarks>Fixed text height; 0 if not fixed.</remarks>
        public double Height
        {
            get { return this.height; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The TextStyle height must be equals or greater than zero.");
                this.height = value;
            }
        }

        /// <summary>
        /// Gets or sets the width factor.
        /// </summary>
        /// <remarks>Valid values range from 0.01 to 100. Default: 1.0.</remarks>
        public double WidthFactor
        {
            get { return this.widthFactor; }
            set
            {
                if (value < 0.01 || value > 100.0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The TextStyle width factor valid values range from 0.01 to 100.");
                this.widthFactor = value;
            }
        }

        /// <summary>
        /// Gets or sets the font oblique angle in degrees.
        /// </summary>
        /// <remarks>Valid values range from -85 to 85. Default: 0.0.</remarks>
        public double ObliqueAngle
        {
            get { return this.obliqueAngle; }
            set
            {
                if (value < -85.0 || value > 85.0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The TextStyle oblique angle valid values range from -85 to 85.");
                this.obliqueAngle = value;
            }
        }

        /// <summary>
        /// Gets or sets the text is vertical.
        /// </summary>
        public bool IsVertical
        {
            get { return this.isVertical; }
            set { this.isVertical = value; }
        }

        /// <summary>
        /// Gets or sets if the text is backward (mirrored in X).
        /// </summary>
        public bool IsBackward
        {
            get { return this.isBackward; }
            set { this.isBackward = value; }
        }

        /// <summary>
        /// Gets or sets if the text is upside down (mirrored in Y).
        /// </summary>
        public bool IsUpsideDown
        {
            get { return this.isUpsideDown; }
            set { this.isUpsideDown = value; }
        }

        /// <summary>
        /// Gets the owner of the actual dxf object.
        /// </summary>
        public new TextStyles Owner
        {
            get { return (TextStyles)this.owner; }
            internal set { this.owner = value; }
        }

        #endregion

        #region internal properties

        internal string FontFamilyName
        {
            get { return this.fontFamilyName; }
        }

        internal bool Registered
        {
            get { return this.registered; }
        }

        #endregion

        #region internal methods

        internal static string FontFileFromFamilyName(string familyName)
        {
            RegistryKey fonts;

            fonts = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Fonts", false);
            if (fonts != null) return fonts.GetValue(familyName) as string;

            fonts = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Fonts", false);
            if (fonts != null) return fonts.GetValue(familyName) as string;

            return null;
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new TextStyle that is a copy of the current instance.
        /// </summary>
        /// <param name="newName">TextStyle name of the copy.</param>
        /// <returns>A new TextStyle that is a copy of this instance.</returns>
        public override TableObject Clone(string newName)
        {
            return new TextStyle(newName, this.font)
            {
                Height = this.height,
                IsBackward = this.isBackward,
                IsUpsideDown = this.isUpsideDown,
                IsVertical = this.isVertical,
                ObliqueAngle = this.obliqueAngle,
                WidthFactor = this.widthFactor
            };

        }

        /// <summary>
        /// Creates a new TextStyle that is a copy of the current instance.
        /// </summary>
        /// <returns>A new TextStyle that is a copy of this instance.</returns>
        public override object Clone()
        {
            return this.Clone(this.name);
        }

        #endregion
    }
}