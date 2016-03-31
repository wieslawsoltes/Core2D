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

using System.IO;
using netDxf.Tables;

namespace netDxf.Objects
{
    /// <summary>
    /// Represents a pdf underlay definition.
    /// </summary>
    public class UnderlayPdfDefinition :
        UnderlayDefinition
    {
        #region private fields

        private string page;

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of the <c>UnderlayPdfDefinition</c> class.
        /// </summary>
        /// <param name="fileName">Underlay file name with full or relative path.</param>
        public UnderlayPdfDefinition(string fileName)
            : this(fileName, Path.GetFileNameWithoutExtension(fileName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>UnderlayPdfDefinition</c> class.
        /// </summary>
        /// <param name="fileName">Underlay file name with full or relative path.</param>
        /// <param name="name">Underlay definition name.</param>
        public UnderlayPdfDefinition(string fileName, string name)
            : base(fileName, name, UnderlayType.PDF)
        {
            this.page = "1";
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the pdf page to show.
        /// </summary>
        public string Page
        {
            get { return this.page; }
            set { this.page = value; }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new UnderlayPdfDefinition that is a copy of the current instance.
        /// </summary>
        /// <param name="newName">UnderlayPdfDefinition name of the copy.</param>
        /// <returns>A new UnderlayPdfDefinition that is a copy of this instance.</returns>
        public override TableObject Clone(string newName)
        {
            return new UnderlayPdfDefinition(this.fileName, newName) {Page = this.page};
        }

        /// <summary>
        /// Creates a new UnderlayPdfDefinition that is a copy of the current instance.
        /// </summary>
        /// <returns>A new UnderlayPdfDefinition that is a copy of this instance.</returns>
        public override object Clone()
        {
            return this.Clone(this.name);
        }

        #endregion
    }
}
