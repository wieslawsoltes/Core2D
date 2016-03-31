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

namespace netDxf
{
    /// <summary>
    /// Represents an entry in the extended data of an entity.
    /// </summary>
    public struct XDataRecord
    {
        #region private fields

        private object value;
        private short code;

        #endregion

        #region constants

        /// <summary>
        /// An extended data control string can be either "{"or "}".
        /// These braces enable applications to organize their data by subdividing the data into lists.
        /// The left brace begins a list, and the right brace terminates the most recent list. Lists can be nested.
        /// </summary>
        public static XDataRecord OpenControlString
        {
            get { return new XDataRecord(XDataCode.ControlString, "{"); }
        }

        /// <summary>
        /// An extended data control string can be either "{" or "}".
        /// These braces enable applications to organize their data by subdividing the data into lists.
        /// The left brace begins a list, and the right brace terminates the most recent list. Lists can be nested.
        /// </summary>
        public static XDataRecord CloseControlString
        {
            get { return new XDataRecord(XDataCode.ControlString, "}"); }
        }

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new XDataRecord.
        /// </summary>
        /// <param name="code">XData code.</param>
        /// <param name="value">XData value.</param>
        public XDataRecord(short code, object value)
        {
            this.code = code;
            this.value = value;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or set the XData code.
        /// </summary>
        /// <remarks>The only valid values are the ones defined in the <see cref="XDataCode">XDataCode</see> class.</remarks>
        public short Code
        {
            get { return this.code; }
            set { this.code = value; }
        }

        /// <summary>
        /// Gets or sets the XData value.
        /// </summary>
        public object Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Obtains a string that represents the XDataRecord.
        /// </summary>
        /// <returns>A string text.</returns>
        public override string ToString()
        {
            return string.Format("{0} - {1}", this.code, this.value);
        }

        #endregion
    }
}