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

using netDxf.Tables;

namespace netDxf.Objects
{
    /// <summary>
    /// Represents an underlay definition.
    /// </summary>
    public abstract class UnderlayDefinition :
        TableObject
    {
        #region private fields

        protected readonly UnderlayType type;
        protected readonly string fileName;

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of the <c>UnderlayDefinition</c> class.
        /// </summary>
        /// <param name="fileName">Underlay file name with full or relative path.</param>
        /// <param name="name">Underlay name.</param>
        /// <param name="type">Underlay type.</param>
        protected UnderlayDefinition(string fileName, string name, UnderlayType type)
            : base(name, DxfObjectCode.UnderlayDefinition, false)
        {
            this.fileName = fileName;
            this.type = type;
            switch (type)
            {
                case UnderlayType.DGN:
                    this.codeName = DxfObjectCode.UnderlayDgnDefinition;
                    break;
                case UnderlayType.DWF:
                    this.codeName = DxfObjectCode.UnderlayDwfDefinition;
                    break;
                case UnderlayType.PDF:
                    this.codeName = DxfObjectCode.UnderlayPdfDefinition;
                    break;
            }
        }

        #endregion

        #region public properties

        /// <summary>
        /// Get the underlay type.
        /// </summary>
        public UnderlayType Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets the underlay file path.
        /// </summary>
        public string FileName
        {
            get { return this.fileName; }
        }

        #endregion
    }
}
