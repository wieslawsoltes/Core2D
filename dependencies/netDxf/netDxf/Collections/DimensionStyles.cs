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
using netDxf.Tables;

namespace netDxf.Collections
{
    /// <summary>
    /// Represents a collection of dimension styles.
    /// </summary>
    public sealed class DimensionStyles :
        TableObjects<DimensionStyle>
    {
        #region constructor

        internal DimensionStyles(DxfDocument document, string handle = null)
            : this(document,0,handle)
        {
        }

        internal DimensionStyles(DxfDocument document, int capacity, string handle = null)
            : base(document,
            new Dictionary<string, DimensionStyle>(capacity, StringComparer.OrdinalIgnoreCase),
            new Dictionary<string, List<DxfObject>>(capacity, StringComparer.OrdinalIgnoreCase),
            DxfObjectCode.DimensionStyleTable,
            handle)
        {
            this.maxCapacity = short.MaxValue;
        }

        #endregion

        #region override methods

        /// <summary>
        /// Adds a dimension style to the list.
        /// </summary>
        /// <param name="style"><see cref="DimensionStyle">DimensionStyle</see> to add to the list.</param>
        /// <param name="assignHandle">Specifies if a handle needs to be generated for the dimension style parameter.</param>
        /// <returns>
        /// If a dimension style already exists with the same name as the instance that is being added the method returns the existing dimension style,
        /// if not it will return the new dimension style.
        /// </returns>
        internal override DimensionStyle Add(DimensionStyle style, bool assignHandle)
        {
            if (this.list.Count >= this.maxCapacity)
                throw new OverflowException(string.Format("Table overflow. The maximum number of elements the table {0} can have is {1}", this.codeName, this.maxCapacity));

            DimensionStyle add;
            if (this.list.TryGetValue(style.Name, out add))
                return add;

            if (assignHandle || string.IsNullOrEmpty(style.Handle))
                this.document.NumHandles = style.AsignHandle(this.document.NumHandles);

            this.document.AddedObjects.Add(style.Handle, style);
            this.list.Add(style.Name, style);
            this.references.Add(style.Name, new List<DxfObject>());
            
            // add referenced text style
            style.DIMTXSTY = this.document.TextStyles.Add(style.DIMTXSTY, assignHandle);
            this.document.TextStyles.References[style.DIMTXSTY.Name].Add(style);

            // add referenced blocks
            if (style.DIMLDRBLK != null)
            {
                style.DIMLDRBLK = this.document.Blocks.Add(style.DIMLDRBLK, assignHandle);
                this.document.Blocks.References[style.DIMLDRBLK.Name].Add(style);
            }

            if (style.DIMBLK != null)
            {
                style.DIMBLK = this.document.Blocks.Add(style.DIMBLK, assignHandle);
                this.document.Blocks.References[style.DIMBLK.Name].Add(style);
            }
            if (style.DIMBLK1 != null)
            {
                style.DIMBLK1 = this.document.Blocks.Add(style.DIMBLK1, assignHandle);
                this.document.Blocks.References[style.DIMBLK1.Name].Add(style);
            }
            if (style.DIMBLK2 != null)
            {
                style.DIMBLK2 = this.document.Blocks.Add(style.DIMBLK2, assignHandle);
                this.document.Blocks.References[style.DIMBLK2.Name].Add(style);
            }

            // add referenced line types
            style.DIMLTYPE = this.document.LineTypes.Add(style.DIMLTYPE, assignHandle);
            this.document.LineTypes.References[style.DIMLTYPE.Name].Add(style);

            style.DIMLTEX1 = this.document.LineTypes.Add(style.DIMLTEX1, assignHandle);
            this.document.LineTypes.References[style.DIMLTEX1.Name].Add(style);

            style.DIMLTEX2 = this.document.LineTypes.Add(style.DIMLTEX2, assignHandle);
            this.document.LineTypes.References[style.DIMLTEX2.Name].Add(style);

            style.Owner = this;

            style.NameChanged += this.Item_NameChanged;
            style.LineTypeChanged += this.DimensionStyleLineTypeChanged;
            style.TextStyleChanged += this.DimensionStyleTextStyleChanged;
            style.BlockChanged += this.DimensionStyleBlockChanged;

            return style;
        }

        /// <summary>
        /// Removes a dimension style.
        /// </summary>
        /// <param name="name"><see cref="DimensionStyle">DimensionStyle</see> name to remove from the document.</param>
        /// <returns>True if the dimension style has been successfully removed, or false otherwise.</returns>
        /// <remarks>Reserved dimension styles or any other referenced by objects cannot be removed.</remarks>
        public override bool Remove(string name)
        {
            return this.Remove(this[name]);
        }

        /// <summary>
        /// Removes a dimension style.
        /// </summary>
        /// <param name="style"><see cref="DimensionStyle">DimensionStyle</see> to remove from the document.</param>
        /// <returns>True if the dimension style has been successfully removed, or false otherwise.</returns>
        /// <remarks>Reserved dimension styles or any other referenced by objects cannot be removed.</remarks>
        public override bool Remove(DimensionStyle style)
        {
            if (style == null)
                return false;

            if (!this.Contains(style))
                return false;

            if (style.IsReserved)
                return false;

            if (this.references[style.Name].Count != 0)
                return false;

            this.document.AddedObjects.Remove(style.Handle);


            // remove referenced text style
            this.document.TextStyles.References[style.DIMTXSTY.Name].Remove(style);

            // remove referenced blocks
            if (style.DIMBLK != null) this.document.Blocks.References[style.DIMBLK.Name].Remove(style);
            if (style.DIMBLK1 != null) this.document.Blocks.References[style.DIMBLK1.Name].Remove(style);
            if (style.DIMBLK2 != null) this.document.Blocks.References[style.DIMBLK2.Name].Remove(style);

            // remove referenced line types
            this.document.LineTypes.References[style.DIMLTYPE.Name].Remove(style);
            this.document.LineTypes.References[style.DIMLTEX1.Name].Remove(style);
            this.document.LineTypes.References[style.DIMLTEX2.Name].Remove(style);

            this.references.Remove(style.Name);
            this.list.Remove(style.Name);

            style.Handle = null;
            style.Owner = null;

            style.NameChanged -= this.Item_NameChanged;
            style.LineTypeChanged -= this.DimensionStyleLineTypeChanged;
            style.TextStyleChanged -= this.DimensionStyleTextStyleChanged;
            style.BlockChanged -= this.DimensionStyleBlockChanged;

            return true;
        }

        #endregion

        #region TableObject events

        private void Item_NameChanged(TableObject sender, TableObjectChangedEventArgs<string> e)
        {
            if (this.Contains(e.NewValue))
                throw new ArgumentException("There is already another dimension style with the same name.");

            this.list.Remove(sender.Name);
            this.list.Add(e.NewValue, (DimensionStyle)sender);

            List<DxfObject> refs = this.references[sender.Name];
            this.references.Remove(sender.Name);
            this.references.Add(e.NewValue, refs);
        }

        private void DimensionStyleLineTypeChanged(TableObject sender, TableObjectChangedEventArgs<LineType> e)
        {
            this.document.LineTypes.References[e.OldValue.Name].Remove(sender);

            e.NewValue = this.document.LineTypes.Add(e.NewValue);
            this.document.LineTypes.References[e.NewValue.Name].Add(sender);
        }

        private void DimensionStyleTextStyleChanged(TableObject sender, TableObjectChangedEventArgs<TextStyle> e)
        {
            this.document.TextStyles.References[e.OldValue.Name].Remove(sender);

            e.NewValue = this.document.TextStyles.Add(e.NewValue);
            this.document.TextStyles.References[e.NewValue.Name].Add(sender);
        }

        private void DimensionStyleBlockChanged(TableObject sender, TableObjectChangedEventArgs<Block> e)
        {
            if (e.OldValue != null) this.document.Blocks.References[e.OldValue.Name].Remove(sender);

            e.NewValue = this.document.Blocks.Add(e.NewValue);
            if (e.NewValue != null) this.document.Blocks.References[e.NewValue.Name].Add(sender);
        }

        #endregion
    }
}