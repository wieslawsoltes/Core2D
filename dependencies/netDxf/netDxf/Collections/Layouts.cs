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
using netDxf.Blocks;
using netDxf.Entities;
using netDxf.Objects;
using netDxf.Tables;

namespace netDxf.Collections
{
    /// <summary>
    /// Represents a collection of layouts.
    /// </summary>
    /// <remarks>
    /// AutoCad limits the number of layouts to 256, but at the same time it allows to import dxf files with more than that,
    /// for this reason the max capacity has been set to short.MaxValue.
    /// The maximum number of layouts is also limited by the number of blocks, due to that for each layout a block record must exist in the blocks collection.
    /// </remarks>
    public sealed class Layouts :
        TableObjects<Layout>
    {
        #region constructor

        internal Layouts(DxfDocument document, string handle = null)
            : this(document, 0, handle)
        {
        }

        internal Layouts(DxfDocument document, int capacity, string handle = null)
            : base(document,
                new Dictionary<string, Layout>(capacity, StringComparer.OrdinalIgnoreCase),
                new Dictionary<string, List<DxfObject>>(capacity, StringComparer.OrdinalIgnoreCase),
                DxfObjectCode.LayoutDictionary,
                handle)
        {
            this.maxCapacity = short.MaxValue;
        }

        #endregion

        #region override methods

        /// <summary>
        /// Adds a layout to the list.
        /// </summary>
        /// <param name="layout"><see cref="Layout">Layout</see> to add to the list.</param>
        /// <param name="assignHandle">Specifies if a handle needs to be generated for the layout parameter.</param>
        /// <returns>
        /// If a layout already exists with the same name as the instance that is being added the method returns the existing layout.
        /// </returns>
        internal override Layout Add(Layout layout, bool assignHandle)
        {
            if (this.list.Count >= this.maxCapacity)
                throw new OverflowException(string.Format("Table overflow. The maximum number of elements the table {0} can have is {1}", this.codeName, this.maxCapacity));

            Layout add;

            if (this.list.TryGetValue(layout.Name, out add))
                return add;

            layout.Owner = this;

            Block associatadBlock = layout.AssociatedBlock;

            // create and add the corresponding PaperSpace block
            if (layout.IsPaperSpace && associatadBlock == null)
            {
                // the PaperSpace block names follow the naming Paper_Space, Paper_Space0, Paper_Space1, ...
                string spaceName = this.list.Count == 1 ? Block.DefaultPaperSpaceName : string.Concat(Block.DefaultPaperSpaceName, this.list.Count - 2);
                associatadBlock = new Block(spaceName, false, null, null);
                if (layout.TabOrder == 0) layout.TabOrder = (short)this.list.Count;
            }

            associatadBlock = this.document.Blocks.Add(associatadBlock);

            layout.AssociatedBlock = associatadBlock;
            associatadBlock.Record.Layout = layout;
            this.document.Blocks.References[associatadBlock.Name].Add(layout);

            if (layout.Viewport != null)
                layout.Viewport.Owner = associatadBlock;

            if (assignHandle || string.IsNullOrEmpty(layout.Handle))
                this.document.NumHandles = layout.AsignHandle(this.document.NumHandles);

            this.document.AddedObjects.Add(layout.Handle, layout);

            this.list.Add(layout.Name, layout);
            this.references.Add(layout.Name, new List<DxfObject>());

            layout.NameChanged += this.Item_NameChanged;

            return layout;
        }

        /// <summary>
        /// Deletes a layout and removes the layout entities from the document.
        /// </summary>
        /// <param name="name"><see cref="Layout">Layout</see> name to remove from the document.</param>
        /// <returns>True if the layout has been successfully removed, or false otherwise.</returns>
        /// <remarks>
        /// The ModelSpace layout cannot be remove. If all PaperSpace layouts have been removed a default PaperSpace will be created since it is required by the dxf implementation.<br />
        /// When a Layout is deleted all entities that has been added to it will also be removed.<br />
        /// Removing a Layout will rebuild the PaperSpace block names, to follow the naming rule: Paper_Space, Paper_Space0, Paper_Space1, ...
        /// </remarks>
        public override bool Remove(string name)
        {
            return this.Remove(this[name]);
        }

        /// <summary>
        /// Deletes a layout and removes the layout entities from the document.
        /// </summary>
        /// <param name="layout"><see cref="Layout">Layout</see> to remove from the document.</param>
        /// <returns>True if the layout has been successfully removed, or false otherwise.</returns>
        /// <remarks>Reserved layouts or any other referenced by objects cannot be removed.</remarks>
        public override bool Remove(Layout layout)
        {
            if (layout == null)
                return false;

            if (!this.Contains(layout))
                return false;

            if (layout.IsReserved)
                return false;

            // remove the entities of the layout
            List<DxfObject> refObjects = this.references[layout.Name];
            if (refObjects.Count != 0)
            {
                DxfObject[] entities = new DxfObject[refObjects.Count];
                refObjects.CopyTo(entities);
                foreach (DxfObject e in entities)
                    this.document.RemoveEntity(e as EntityObject);
            }

            // When a layout is removed we need to rebuild the PaperSpace block names, to follow the naming Paper_Space, Paper_Space0, Paper_Space1, ...
            foreach (Layout l in this.list.Values)
            {
                // The ModelSpace block cannot be removed. 
                if (l.IsPaperSpace)
                {
                    this.document.Blocks.References[l.AssociatedBlock.Name].Remove(l);
                    this.document.Blocks.Remove(l.AssociatedBlock);
                    l.AssociatedBlock = null;
                }
            }

            // remove the layout
            this.document.AddedObjects.Remove(layout.Handle);
            this.references.Remove(layout.Name);
            this.list.Remove(layout.Name);

            layout.Handle = null;
            layout.Owner = null;
            layout.Viewport.Owner = null;

            layout.NameChanged -= this.Item_NameChanged;

            // When a layout is removed we need to rebuild the PaperSpace block names, to follow the naming Paper_Space, Paper_Space0, Paper_Space1, ...
            int index = 0;
            foreach (Layout l in this.list.Values)
            {
                // Create and add the corresponding PaperSpace block
                if (l.IsPaperSpace)
                {
                    string spaceName = index == 0 ? Block.PaperSpace.Name : string.Concat(Block.PaperSpace.Name, index - 1);
                    Block associatadBlock = this.document.Blocks.Add(new Block(spaceName, false, null, null));
                    this.document.Blocks.References[associatadBlock.Name].Add(l);
                    l.AssociatedBlock = associatadBlock;
                    associatadBlock.Record.Layout = l;
                    index += 1;

                    // we need to redefine the owner of the layout entities
                    l.Viewport.Owner = l.AssociatedBlock;
                    foreach (DxfObject o in this.references[l.Name])
                        o.Owner = l.AssociatedBlock;
                }
            }
            return true;
        }

        #endregion

        #region LineType events

        private void Item_NameChanged(TableObject sender, TableObjectChangedEventArgs<string> e)
        {
            if (this.Contains(e.NewValue))
                throw new ArgumentException("There is already another layout with the same name.");

            this.list.Remove(sender.Name);
            this.list.Add(e.NewValue, (Layout)sender);

            List<DxfObject> refs = this.references[sender.Name];
            this.references.Remove(sender.Name);
            this.references.Add(e.NewValue, refs);
        }

        #endregion
    }
}