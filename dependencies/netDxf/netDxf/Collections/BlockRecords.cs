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
using netDxf.Tables;

namespace netDxf.Collections
{
    /// <summary>
    /// Represents a collection of blocks.
    /// </summary>
    public sealed class BlockRecords :
        TableObjects<Block>
    {
        #region constructor

        internal BlockRecords(DxfDocument document, string handle = null)
            : this(document,0,handle)
        {
        }

        internal BlockRecords(DxfDocument document, int capacity, string handle = null)
            : base(document,
            new Dictionary<string, Block>(capacity, StringComparer.OrdinalIgnoreCase),
            new Dictionary<string, List<DxfObject>>(capacity, StringComparer.OrdinalIgnoreCase),
            DxfObjectCode.BlockRecordTable,
            handle)
        {
            this.maxCapacity = short.MaxValue;
        }

        #endregion

        #region override methods

        /// <summary>
        /// Adds a block to the list.
        /// </summary>
        /// <param name="block"><see cref="Block">Block</see> to add to the list.</param>
        /// <param name="assignHandle">Specifies if a handle needs to be generated for the block parameter.</param>
        /// <returns>
        /// If a block already exists with the same name as the instance that is being added the method returns the existing block,
        /// if not it will return the new block.
        /// </returns>
        internal override Block Add(Block block, bool assignHandle)
        {
            if (this.list.Count >= this.maxCapacity)
                throw new OverflowException(string.Format("Table overflow. The maximum number of elements the table {0} can have is {1}", this.codeName, this.maxCapacity));

            Block add;
            if (this.list.TryGetValue(block.Name, out add))
                return add;

            if(assignHandle || string.IsNullOrEmpty(block.Handle))
                this.document.NumHandles = block.AsignHandle(this.document.NumHandles);

            this.document.AddedObjects.Add(block.Handle, block);
            this.document.AddedObjects.Add(block.Owner.Handle, block.Owner);

            this.list.Add(block.Name, block);
            this.references.Add(block.Name, new List<DxfObject>());

            block.Layer = this.document.Layers.Add(block.Layer);
            this.document.Layers.References[block.Layer.Name].Add(block);

            //for new block definitions configure its entities
            foreach (EntityObject entity in block.Entities)
            {
                this.document.AddEntity(entity, true, assignHandle);
            }

            //for new block definitions configure its attributes
            foreach (AttributeDefinition attDef in block.AttributeDefinitions.Values)
            {
                this.document.AddEntity(attDef, true, assignHandle);
            }

            block.Record.Owner = this;

            block.NameChanged += this.Item_NameChanged;
            block.LayerChanged += this.Block_LayerChanged;
            block.EntityAdded += this.Block_EntityAdded;
            block.EntityRemoved += this.Block_EntityRemoved;
            block.AttributeDefinitionAdded += this.Block_EntityAdded;
            block.AttributeDefinitionRemoved += this.Block_EntityRemoved;

            return block;
        }

        /// <summary>
        /// Removes a block.
        /// </summary>
        /// <param name="name"><see cref="Block">Block</see> name to remove from the document.</param>
        /// <returns>True if the block has been successfully removed, or false otherwise.</returns>
        /// <remarks>Reserved blocks or any other referenced by objects cannot be removed.</remarks>
        public override bool Remove(string name)
        {
            return this.Remove(this[name]);
        }

        /// <summary>
        /// Removes a block.
        /// </summary>
        /// <param name="block"><see cref="Block">Block</see> to remove from the document.</param>
        /// <returns>True if the block has been successfully removed, or false otherwise.</returns>
        /// <remarks>Reserved blocks or any other referenced by objects cannot be removed.</remarks>
        public override bool Remove(Block block)
        {
            if (block == null)
                return false;

            if (!this.Contains(block))
                return false;

            if (block.IsReserved)
                return false;

            if (this.references[block.Name].Count != 0)
                return false;

            // remove the block from the associated layer
            this.document.Layers.References[block.Layer.Name].Remove(block);

            // we will remove all entities from the block definition
            foreach (EntityObject entity in block.Entities)
            {
                this.document.RemoveEntity(entity, true);
            }

            // remove all attribute definitions from the associated layers
            foreach (AttributeDefinition attDef in block.AttributeDefinitions.Values)
            {
                this.document.RemoveEntity(attDef, true);
            }

            this.document.AddedObjects.Remove(block.Handle);
            this.references.Remove(block.Name);
            this.list.Remove(block.Name);

            block.Record.Handle = null;
            block.Record.Owner = null;

            block.Handle = null;
            block.Owner = null;

            block.NameChanged -= this.Item_NameChanged;
            block.LayerChanged -= this.Block_LayerChanged;
            block.EntityAdded -= this.Block_EntityAdded;
            block.EntityRemoved -= this.Block_EntityRemoved;
            block.AttributeDefinitionAdded -= this.Block_EntityAdded;
            block.AttributeDefinitionRemoved -= this.Block_EntityRemoved;

            return true;
        }

        #endregion

        #region Block events

        private void Item_NameChanged(TableObject sender, TableObjectChangedEventArgs<string> e)
        {
            if (this.Contains(e.NewValue))
                throw new ArgumentException("There is already another block with the same name.");

            this.list.Remove(sender.Name);
            this.list.Add(e.NewValue, (Block)sender);

            List<DxfObject> refs = this.references[sender.Name];
            this.references.Remove(sender.Name);
            this.references.Add(e.NewValue, refs);
        }

        private void Block_LayerChanged(Block sender, TableObjectChangedEventArgs<Layer> e)
        {
            this.document.Layers.References[e.OldValue.Name].Remove(sender);

            e.NewValue = this.document.Layers.Add(e.NewValue);
            this.document.Layers.References[e.NewValue.Name].Add(sender);
        }

        void Block_EntityAdded(TableObject sender, BlockEntityChangeEventArgs e)
        {
            if (e.Item.Owner != null)
            {
                // the block and its entities must belong to the same document
                if (!ReferenceEquals(e.Item.Owner.Record.Owner.Owner, this.owner))
                    throw new ArgumentException("The block and the entity must belong to the same document. Clone it instead.");

                // the entity cannot belong to another block
                if(e.Item.Owner.Record.Layout == null)
                    throw new ArgumentException("The entity cannot belong to another block. Clone it instead.");

                // we will exchange the owner of the entity
                this.document.RemoveEntity(e.Item);
            }
            this.document.AddEntity(e.Item, true, string.IsNullOrEmpty(e.Item.Handle));
        }

        private void Block_EntityRemoved(TableObject sender, BlockEntityChangeEventArgs e)
        {
            this.document.RemoveEntity(e.Item, true);
        }

        #endregion
    }
}