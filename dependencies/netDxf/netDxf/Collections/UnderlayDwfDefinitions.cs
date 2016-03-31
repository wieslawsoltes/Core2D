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
using netDxf.Objects;
using netDxf.Tables;

namespace netDxf.Collections
{
    /// <summary>
    /// Represents a collection of pdf underlay definitions.
    /// </summary>
    public sealed class UnderlayDwfDefinitions :
        TableObjects<UnderlayDwfDefinition>
    {
        #region constructor

        internal UnderlayDwfDefinitions(DxfDocument document, string handle = null)
            : this(document, 0, handle)
        {
        }

        internal UnderlayDwfDefinitions(DxfDocument document, int capacity, string handle = null)
            : base(document,
            new Dictionary<string, UnderlayDwfDefinition>(capacity, StringComparer.OrdinalIgnoreCase),
            new Dictionary<string, List<DxfObject>>(capacity, StringComparer.OrdinalIgnoreCase),
            DxfObjectCode.UnderlayDwfDefinitionDictionary,
            handle)
        {
            this.maxCapacity = int.MaxValue;
        }

        #endregion

        #region override methods

        /// <summary>
        /// Adds a dwf underlay definition to the list.
        /// </summary>
        /// <param name="underlayDwfDefinition"><see cref="UnderlayDwfDefinition">UnderlayDwfDefinition</see> to add to the list.</param>
        /// <param name="assignHandle">Specifies if a handle needs to be generated for the underlay definition parameter.</param>
        /// <returns>
        /// If an underlay definition already exists with the same name as the instance that is being added the method returns the existing underlay definition,
        /// if not it will return the new underlay definition.
        /// </returns>
        internal override UnderlayDwfDefinition Add(UnderlayDwfDefinition underlayDwfDefinition, bool assignHandle)
        {
            if (this.list.Count >= this.maxCapacity)
                throw new OverflowException(string.Format("Table overflow. The maximum number of elements the table {0} can have is {1}", this.codeName, this.maxCapacity));

            UnderlayDwfDefinition add;
            if (this.list.TryGetValue(underlayDwfDefinition.Name, out add))
                return add;

            if (assignHandle || string.IsNullOrEmpty(underlayDwfDefinition.Handle))
                this.document.NumHandles = underlayDwfDefinition.AsignHandle(this.document.NumHandles);

            this.document.AddedObjects.Add(underlayDwfDefinition.Handle, underlayDwfDefinition);
            this.list.Add(underlayDwfDefinition.Name, underlayDwfDefinition);
            this.references.Add(underlayDwfDefinition.Name, new List<DxfObject>());

            underlayDwfDefinition.Owner = this;

            underlayDwfDefinition.NameChanged += this.Item_NameChanged;

            return underlayDwfDefinition;
        }

        /// <summary>
        /// Removes a dwf underlay definition.
        /// </summary>
        /// <param name="name"><see cref="UnderlayDwfDefinition">UnderlayDwfDefinition</see> name to remove from the document.</param>
        /// <returns>True if the underlay definition has been successfully removed, or false otherwise.</returns>
        /// <remarks>Any underlay definition referenced by objects cannot be removed.</remarks>
        public override bool Remove(string name)
        {
            return this.Remove(this[name]);
        }

        /// <summary>
        /// Removes a dwf underlay definition.
        /// </summary>
        /// <param name="underlayDwfDefinition"><see cref="UnderlayDwfDefinition">UnderlayDwfDefinition</see> to remove from the document.</param>
        /// <returns>True if the underlay definition has been successfully removed, or false otherwise.</returns>
        /// <remarks>Any underlay definition referenced by objects cannot be removed.</remarks>
        public override bool Remove(UnderlayDwfDefinition underlayDwfDefinition)
        {
            if (underlayDwfDefinition == null)
                return false;

            if (!this.Contains(underlayDwfDefinition))
                return false;

            if (underlayDwfDefinition.IsReserved)
                return false;

            if (this.references[underlayDwfDefinition.Name].Count != 0)
                return false;

            this.document.AddedObjects.Remove(underlayDwfDefinition.Handle);
            this.references.Remove(underlayDwfDefinition.Name);
            this.list.Remove(underlayDwfDefinition.Name);

            underlayDwfDefinition.Handle = null;
            underlayDwfDefinition.Owner = null;

            underlayDwfDefinition.NameChanged -= this.Item_NameChanged;

            return true;
        }

        #endregion

        #region TableObject events

        private void Item_NameChanged(TableObject sender, TableObjectChangedEventArgs<string> e)
        {
            if (this.Contains(e.NewValue))
                throw new ArgumentException("There is already another dwf underlay definition with the same name.");

            this.list.Remove(sender.Name);
            this.list.Add(e.NewValue, (UnderlayDwfDefinition)sender);

            List<DxfObject> refs = this.references[sender.Name];
            this.references.Remove(sender.Name);
            this.references.Add(e.NewValue, refs);
        }

        #endregion
    }
}
