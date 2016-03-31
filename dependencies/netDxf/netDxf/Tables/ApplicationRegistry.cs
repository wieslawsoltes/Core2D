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
using netDxf.Collections;

namespace netDxf.Tables
{
    /// <summary>
    /// Represents a registered application name to which the <see cref="XData">extended data</see> is associated.
    /// </summary>
    public class ApplicationRegistry :
        TableObject
    {
        #region constants

        /// <summary>
        /// Default application registry name.
        /// </summary>
        public const string DefaultName = "ACAD";

        /// <summary>
        /// Gets the default application registry.
        /// </summary>
        public static ApplicationRegistry Default
        {
            get { return new ApplicationRegistry(DefaultName); }
        }

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>ApplicationRegistry</c> class.
        /// </summary>
        /// <param name="name">Layer name.</param>
        public ApplicationRegistry(string name)
            : this(name, true)
        {
        }

        internal ApplicationRegistry(string name, bool checkName)
            : base(name, DxfObjectCode.AppId, checkName)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name), "The application registry name should be at least one character long.");

            this.reserved = name.Equals(DefaultName, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the owner of the actual dxf object.
        /// </summary>
        public new ApplicationRegistries Owner
        {
            get { return (ApplicationRegistries)this.owner; }
            internal set { this.owner = value; }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new ApplicationRegistry that is a copy of the current instance.
        /// </summary>
        /// <param name="newName">ApplicationRegistry name of the copy.</param>
        /// <returns>A new ApplicationRegistry that is a copy of this instance.</returns>
        public override TableObject Clone(string newName)
        {
            return new ApplicationRegistry(newName);
        }

        /// <summary>
        /// Creates a new ApplicationRegistry that is a copy of the current instance.
        /// </summary>
        /// <returns>A new ApplicationRegistry that is a copy of this instance.</returns>
        public override object Clone()
        {
            return this.Clone(this.name);
        }

        #endregion
    }
}