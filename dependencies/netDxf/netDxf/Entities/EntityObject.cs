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
using System.Collections.ObjectModel;
using netDxf.Blocks;
using netDxf.Collections;
using netDxf.Tables;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a generic entity.
    /// </summary>
    public abstract class EntityObject :
        DxfObject,
        ICloneable
    {
        #region delegates and events

        public delegate void LayerChangedEventHandler(EntityObject sender, TableObjectChangedEventArgs<Layer> e);
        public event LayerChangedEventHandler LayerChanged;
        protected virtual Layer OnLayerChangedEvent(Layer oldLayer, Layer newLayer)
        {
            LayerChangedEventHandler ae = this.LayerChanged;
            if (ae != null)
            {
                TableObjectChangedEventArgs<Layer> eventArgs = new TableObjectChangedEventArgs<Layer>(oldLayer, newLayer);
                ae(this, eventArgs);
                return eventArgs.NewValue;
            }
            return newLayer;
        }

        public delegate void LineTypeChangedEventHandler(EntityObject sender, TableObjectChangedEventArgs<LineType> e);
        public event LineTypeChangedEventHandler LineTypeChanged;
        protected virtual LineType OnLineTypeChangedEvent(LineType oldLineType, LineType newLineType)
        {
            LineTypeChangedEventHandler ae = this.LineTypeChanged;
            if (ae != null)
            {
                TableObjectChangedEventArgs<LineType> eventArgs = new TableObjectChangedEventArgs<LineType>(oldLineType, newLineType);
                ae(this, eventArgs);
                return eventArgs.NewValue;
            }
            return newLineType;
        }

        public delegate void XDataAddAppRegEventHandler(EntityObject sender, ObservableCollectionEventArgs<ApplicationRegistry> e);
        public delegate void XDataRemoveAppRegEventHandler(EntityObject sender, ObservableCollectionEventArgs<ApplicationRegistry> e);

        public event XDataAddAppRegEventHandler XDataAddAppReg;
        public event XDataRemoveAppRegEventHandler XDataRemoveAppReg;

        protected virtual void OnXDataAddAppRegEvent(ApplicationRegistry item)
        {
            XDataAddAppRegEventHandler ae = this.XDataAddAppReg;
            if (ae != null)
                ae(this, new ObservableCollectionEventArgs<ApplicationRegistry>(item));
        }

        protected virtual void OnXDataRemoveAppRegEvent(ApplicationRegistry item)
        {
            XDataRemoveAppRegEventHandler ae = this.XDataRemoveAppReg;
            if (ae != null)
                ae(this, new ObservableCollectionEventArgs<ApplicationRegistry>(item));
        }
        #endregion

        #region private fields

        private readonly EntityType type;
        protected AciColor color;
        protected Layer layer;
        protected LineType lineType;
        protected Lineweight lineweight;
        protected Transparency transparency;
        protected double lineTypeScale;
        protected bool isVisible;
        protected Vector3 normal;
        protected readonly XDataDictionary xData;
        protected readonly List<DxfObject> reactors;

        #endregion

        #region constructors

        protected EntityObject(EntityType type, string dxfCode)
            : base(dxfCode)
        {
            this.type = type;
            this.color = AciColor.ByLayer;
            this.layer = Layer.Default;
            this.lineType = LineType.ByLayer;
            this.lineweight = Lineweight.ByLayer;
            this.transparency = Transparency.ByLayer;
            this.lineTypeScale = 1.0;
            this.isVisible = true;
            this.normal = Vector3.UnitZ;
            this.reactors = new List<DxfObject>();
            this.xData = new XDataDictionary();
            this.xData.AddAppReg += this.XData_AddAppReg;
            this.xData.RemoveAppReg += this.XData_RemoveAppReg;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the list of dxf objects that has been attached to this entity.
        /// </summary>
        public ReadOnlyCollection<DxfObject> Reactors
        {
            get { return this.reactors.AsReadOnly(); }
        }

        /// <summary>
        /// Gets the entity <see cref="EntityType">type</see>.
        /// </summary>
        public EntityType Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets or sets the entity <see cref="AciColor">color</see>.
        /// </summary>
        public virtual AciColor Color
        {
            get { return this.color; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.color = value;
            }
        }

        /// <summary>
        /// Gets or sets the entity <see cref="Layer">layer</see>.
        /// </summary>
        public virtual Layer Layer
        {
            get { return this.layer; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.layer = this.OnLayerChangedEvent(this.layer, value);
            }
        }

        /// <summary>
        /// Gets or sets the entity <see cref="LineType">line type</see>.
        /// </summary>
        public virtual LineType LineType
        {
            get { return this.lineType; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.lineType = this.OnLineTypeChangedEvent(this.lineType, value);
            }
        }

        /// <summary>
        /// Gets or sets the entity line weight, one unit is always 1/100 mm (default = ByLayer).
        /// </summary>
        public virtual Lineweight Lineweight
        {
            get { return this.lineweight; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.lineweight = value;
            }
        }

        /// <summary>
        /// Gets or sets layer transparency (default: ByLayer).
        /// </summary>
        public virtual Transparency Transparency
        {
            get { return this.transparency; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.transparency = value;
            }
        }

        /// <summary>
        /// Gets or sets the entity line type scale.
        /// </summary>
        public virtual double LineTypeScale
        {
            get { return this.lineTypeScale; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The line type scale must be greater than zero.");
                this.lineTypeScale = value;
            }
        }

        /// <summary>
        /// Gets or set the entity visibility.
        /// </summary>
        public virtual bool IsVisible
        {
            get { return this.isVisible; }
            set { this.isVisible = value; }
        }

        /// <summary>
        /// Gets or sets the entity <see cref="Vector3">normal</see>.
        /// </summary>
        public virtual Vector3 Normal
        {
            get { return this.normal; }
            set
            {
                if (value == Vector3.Zero)
                    throw new ArgumentNullException(nameof(value), "The normal can not be the zero vector.");
                this.normal = Vector3.Normalize(value);
            }
        }

        /// <summary>
        /// Gets the owner of the actual dxf object.
        /// </summary>
        public new Block Owner
        {
            get { return (Block) this.owner; }
            internal set { this.owner = value; }
        }

        /// <summary>
        /// Gets the entity <see cref="XDataDictionary">extended data</see>.
        /// </summary>
        public XDataDictionary XData
        {
            get { return this.xData; }
        }

        #endregion

        #region internal methods

        internal void AddReactor(DxfObject o)
        {
            this.reactors.Add(o);
        }

        internal bool RemoveReactor(DxfObject o)
        {
            return this.reactors.Remove(o);
        }

        #endregion

        #region overrides

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return this.type.ToString();
        }

        #endregion

        #region ICloneable

        /// <summary>
        /// Creates a new entity that is a copy of the current instance.
        /// </summary>
        /// <returns>A new entity that is a copy of this instance.</returns>
        public abstract object Clone();

        #endregion

        #region XData events

        private void XData_AddAppReg(XDataDictionary sender, ObservableCollectionEventArgs<ApplicationRegistry> e)
        {           
            this.OnXDataAddAppRegEvent(e.Item);
        }

        private void XData_RemoveAppReg(XDataDictionary sender, ObservableCollectionEventArgs<ApplicationRegistry> e)
        {
            this.OnXDataRemoveAppRegEvent(e.Item);
        }

        #endregion
    }
}