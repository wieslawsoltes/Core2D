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
using netDxf.Tables;

namespace netDxf.Collections
{
    /// <summary>
    /// Represents a collection of application registries.
    /// </summary>
    public sealed class ApplicationRegistries :
        TableObjects<ApplicationRegistry>
    {
        #region constructor

        internal ApplicationRegistries(DxfDocument document, string handle = null)
            : this(document, 0, handle)
        {         
        }

        internal ApplicationRegistries(DxfDocument document, int capacity, string handle = null)
            : base(document,
            new Dictionary<string, ApplicationRegistry>(capacity, StringComparer.OrdinalIgnoreCase),
            new Dictionary<string, List<DxfObject>>(capacity, StringComparer.OrdinalIgnoreCase),
            DxfObjectCode.ApplicationIDTable,
            handle)
        {
            this.maxCapacity = short.MaxValue;
        }

        #endregion

        #region override methods

        /// <summary>
        /// Adds an application registry to the list.
        /// </summary>
        /// <param name="appReg"><see cref="ApplicationRegistry">ApplicationRegistry</see> to add to the list.</param>
        /// <param name="assignHandle">Checks if the appReg parameter requires a handle.</param>
        /// <returns>
        /// If a an application registry already exists with the same name as the instance that is being added the method returns the existing application registry,
        /// if not it will return the new application registry.
        /// </returns>
        internal override ApplicationRegistry Add(ApplicationRegistry appReg, bool assignHandle)
        {
            if (this.list.Count >= this.maxCapacity)
                throw new OverflowException(string.Format("Table overflow. The maximum number of elements the table {0} can have is {1}", this.codeName, this.maxCapacity));

            ApplicationRegistry add;
            if (this.list.TryGetValue(appReg.Name, out add))
                return add;

            if(assignHandle || string.IsNullOrEmpty(appReg.Handle))
                this.document.NumHandles = appReg.AsignHandle(this.document.NumHandles);

            this.document.AddedObjects.Add(appReg.Handle, appReg);
            this.list.Add(appReg.Name, appReg);
            this.references.Add(appReg.Name, new List<DxfObject>());
            appReg.Owner = this;

            appReg.NameChanged += this.Item_NameChanged;

            return appReg;
        }

        /// <summary>
        /// Removes an application registry.
        /// </summary>
        /// <param name="name"><see cref="ApplicationRegistry">ApplicationRegistry</see> name to remove from the document.</param>
        /// <returns>True if the application registry has been successfully removed, or false otherwise.</returns>
        /// <remarks>Reserved application registries or any other referenced by objects cannot be removed.</remarks>
        public override bool Remove(string name)
        {
            return this.Remove(this[name]);
        }

        /// <summary>
        /// Removes an application registry.
        /// </summary>
        /// <param name="appReg"><see cref="ApplicationRegistry">ApplicationRegistry</see> to remove from the document.</param>
        /// <returns>True if the application registry has been successfully removed, or false otherwise.</returns>
        /// <remarks>Reserved application registries or any other referenced by objects cannot be removed.</remarks>
        public override bool Remove(ApplicationRegistry appReg)
        {
            if (appReg == null)
                return false;

            if (!this.Contains(appReg))
                return false;

            if (appReg.IsReserved)
                return false;

            if (this.references[appReg.Name].Count != 0)
                return false;

            
            this.document.AddedObjects.Remove(appReg.Handle);
            this.references.Remove(appReg.Name);
            this.list.Remove(appReg.Name);

            appReg.Handle = null;
            appReg.Owner = null;

            appReg.NameChanged -= this.Item_NameChanged;

            return true;
        }

        #endregion

        #region TableObject events

        private void Item_NameChanged(TableObject sender, TableObjectChangedEventArgs<string> e)
        {
            if (this.Contains(e.NewValue))
                throw new ArgumentException("There is already another application registry with the same name.");

            this.list.Remove(sender.Name);
            this.list.Add(e.NewValue, (ApplicationRegistry) sender);

            List<DxfObject> refs = this.references[sender.Name];
            this.references.Remove(sender.Name);
            this.references.Add(e.NewValue, refs);
        }

        #endregion
    }
}