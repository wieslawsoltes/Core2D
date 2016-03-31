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
    /// Represents a collection of image definitions.
    /// </summary>
    public sealed class ImageDefinitions :
        TableObjects<ImageDefinition>
    {
        #region constructor

        internal ImageDefinitions(DxfDocument document, string handle = null)
            : this(document,0,handle)
        {
        }

        internal ImageDefinitions(DxfDocument document, int capacity, string handle = null)
            : base(document,
            new Dictionary<string, ImageDefinition>(capacity, StringComparer.OrdinalIgnoreCase),
            new Dictionary<string, List<DxfObject>>(capacity, StringComparer.OrdinalIgnoreCase),
            DxfObjectCode.ImageDefDictionary,
            handle)
        {
            this.maxCapacity = int.MaxValue;
        }

        #endregion

        #region override methods

        /// <summary>
        /// Adds an image definition to the list.
        /// </summary>
        /// <param name="imageDefinition"><see cref="ImageDefinition">ImageDefinition</see> to add to the list.</param>
        /// <param name="assignHandle">Specifies if a handle needs to be generated for the image definition parameter.</param>
        /// <returns>
        /// If an image definition already exists with the same name as the instance that is being added the method returns the existing image definition,
        /// if not it will return the new image definition.
        /// </returns>
        internal override ImageDefinition Add(ImageDefinition imageDefinition, bool assignHandle)
        {
            if (this.list.Count >= this.maxCapacity)
                throw new OverflowException(string.Format("Table overflow. The maximum number of elements the table {0} can have is {1}", this.codeName, this.maxCapacity));

            ImageDefinition add;
            if (this.list.TryGetValue(imageDefinition.Name, out add))
                return add;

            if (assignHandle || string.IsNullOrEmpty(imageDefinition.Handle))
                this.document.NumHandles = imageDefinition.AsignHandle(this.document.NumHandles);

            this.document.AddedObjects.Add(imageDefinition.Handle, imageDefinition);
            this.list.Add(imageDefinition.Name, imageDefinition);
            this.references.Add(imageDefinition.Name, new List<DxfObject>());

            imageDefinition.Owner = this;

            imageDefinition.NameChanged += this.Item_NameChanged;

            return imageDefinition;
        }

        /// <summary>
        /// Removes an image definition.
        /// </summary>
        /// <param name="name"><see cref="ImageDefinition">ImageDefinition</see> name to remove from the document.</param>
        /// <returns>True if the image definition has been successfully removed, or false otherwise.</returns>
        /// <remarks>Any image definition referenced by objects cannot be removed.</remarks>
        public override bool Remove(string name)
        {
            return this.Remove(this[name]);
        }

        /// <summary>
        /// Removes an image definition.
        /// </summary>
        /// <param name="imageDefinition"><see cref="ImageDefinition">ImageDefinition</see> to remove from the document.</param>
        /// <returns>True if the image definition has been successfully removed, or false otherwise.</returns>
        /// <remarks>Any image definition referenced by objects cannot be removed.</remarks>
        public override bool Remove(ImageDefinition imageDefinition)
        {
            if (imageDefinition == null)
                return false;

            if (!this.Contains(imageDefinition))
                return false;

            if (imageDefinition.IsReserved)
                return false;

            if (this.references[imageDefinition.Name].Count != 0)
                return false;

            this.document.AddedObjects.Remove(imageDefinition.Handle);
            this.references.Remove(imageDefinition.Name);
            this.list.Remove(imageDefinition.Name);

            imageDefinition.Handle = null;
            imageDefinition.Owner = null;

            imageDefinition.NameChanged -= this.Item_NameChanged;

            return true;
        }

        #endregion

        #region TableObject events

        private void Item_NameChanged(TableObject sender, TableObjectChangedEventArgs<string> e)
        {
            if (this.Contains(e.NewValue))
                throw new ArgumentException("There is already another image definition with the same name.");

            this.list.Remove(sender.Name);
            this.list.Add(e.NewValue, (ImageDefinition)sender);

            List<DxfObject> refs = this.references[sender.Name];
            this.references.Remove(sender.Name);
            this.references.Add(e.NewValue, refs);
        }

        #endregion
    }
}