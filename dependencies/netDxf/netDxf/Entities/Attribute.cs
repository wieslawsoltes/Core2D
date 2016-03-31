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
using netDxf.Tables;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a attribute <see cref="EntityObject">entity</see>.
    /// </summary>
    /// <remarks>
    /// The attribute position, rotation, height and width factor values also includes the transformation of the <see cref="Insert">Insert</see> entity to which it belongs.<br />
    /// During the attribute initialization a copy of all attribute definition properties will be copied,
    /// so any changes made to the attribute definition will only be applied to new attribute instances and not to existing ones.
    /// This behavior is to allow imported <see cref="Insert">Insert</see> entities to have attributes without definition in the block, 
    /// although this might sound not totally correct it is allowed by AutoCad.
    /// </remarks>
    public class Attribute :
        DxfObject,
        ICloneable
    {
        #region delegates and events

        public delegate void LayerChangedEventHandler(Attribute sender, TableObjectChangedEventArgs<Layer> e);
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

        public delegate void LineTypeChangedEventHandler(Attribute sender, TableObjectChangedEventArgs<LineType> e);
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

        public delegate void TextStyleChangedEventHandler(Attribute sender, TableObjectChangedEventArgs<TextStyle> e);
        public event TextStyleChangedEventHandler TextStyleChanged;
        protected virtual TextStyle OnTextStyleChangedEvent(TextStyle oldTextStyle, TextStyle newTextStyle)
        {
            TextStyleChangedEventHandler ae = this.TextStyleChanged;
            if (ae != null)
            {
                TableObjectChangedEventArgs<TextStyle> eventArgs = new TableObjectChangedEventArgs<TextStyle>(oldTextStyle, newTextStyle);
                ae(this, eventArgs);
                return eventArgs.NewValue;
            }
            return newTextStyle;
        }

        #endregion

        #region private fields

        protected AciColor color;
        protected Layer layer;
        protected LineType lineType;
        protected Lineweight lineweight;
        protected Transparency transparency;
        protected double lineTypeScale;
        protected bool isVisible;
        protected Vector3 normal;

        private AttributeDefinition definition;
        private string tag;
        private object value;
        private TextStyle style;
        private Vector3 position;
        private AttributeFlags flags;
        private double height;
        private double widthFactor;
        private double obliqueAngle;
        private double rotation;
        private TextAlignment alignment;

        #endregion

        #region constructor

        internal Attribute()
            : base(DxfObjectCode.Attribute)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Attribute</c> class.
        /// </summary>
        /// <param name="definition"><see cref="AttributeDefinition">Attribute definition</see>.</param>
        /// <remarks>
        /// Although the attribute entity could override values defined in its definition for simplicity the implementation has restricted this possibility.
        /// </remarks>
        public Attribute(AttributeDefinition definition)
            : base(DxfObjectCode.Attribute)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            //EntityObject properties
            this.color = definition.Color;
            this.layer = definition.Layer;
            this.lineType = definition.LineType;
            this.lineweight = definition.Lineweight;
            this.lineTypeScale = definition.LineTypeScale;
            this.transparency = definition.Transparency;
            this.isVisible = definition.IsVisible;
            this.normal = definition.Normal;

            //Attribute definition properties
            this.definition = definition;
            this.tag = definition.Tag;
            this.value = definition.Value;
            this.style = definition.Style;
            this.position = definition.Position;
            this.flags = definition.Flags;
            this.height = definition.Height;
            this.widthFactor = definition.WidthFactor;
            this.obliqueAngle = definition.ObliqueAngle;
            this.rotation = definition.Rotation;
            this.alignment = definition.Alignment;
        }

        #endregion

        #region public property

        /// <summary>
        /// Gets or sets the entity <see cref="AciColor">color</see>.
        /// </summary>
        public AciColor Color
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
        public Layer Layer
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
        public LineType LineType
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
        public Lineweight Lineweight
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
        public Transparency Transparency
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
        public double LineTypeScale
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
        public bool IsVisible
        {
            get { return this.isVisible; }
            set { this.isVisible = value; }
        }

        /// <summary>
        /// Gets or sets the entity <see cref="Vector3">normal</see>.
        /// </summary>
        public Vector3 Normal
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
        public new Insert Owner
        {
            get { return (Insert)this.owner; }
            internal set { this.owner = value; }
        }

        /// <summary>
        /// Gets the attribute definition.
        /// </summary>
        /// <remarks>If the insert attribute has no definition it will return null.</remarks>
        public AttributeDefinition Definition
        {
            get { return this.definition; }
            internal set { this.definition = value; }
        }

        /// <summary>
        /// Gets the attribute tag.
        /// </summary>
        public string Tag
        {
            get { return this.tag; }
            internal set { this.tag = value;}
        }

        /// <summary>
        /// Gets or sets the attribute text height.
        /// </summary>
        public double Height
        {
            get { return this.height; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The height should be greater than zero.");
                this.height = value;
            }
        }

        /// <summary>
        /// Gets or sets the attribute text width factor.
        /// </summary>
        public double WidthFactor
        {
            get { return this.widthFactor; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The width factor should be greater than zero.");
                this.widthFactor = value;
            }
        }

        /// <summary>
        /// Gets or sets the font oblique angle.
        /// </summary>
        /// <remarks>Valid values range from -85 to 85. Default: 0.0.</remarks>
        public double ObliqueAngle
        {
            get { return this.obliqueAngle; }
            set
            {
                if (value < -85.0 || value > 85.0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The oblique angle valid values range from -85 to 85.");
                this.obliqueAngle = value;
            }
        }

        /// <summary>
        /// Gets or sets the attribute value.
        /// </summary>
        public object Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        /// <summary>
        /// Gets or sets the attribute text style.
        /// </summary>
        /// <remarks>
        /// The <see cref="TextStyle">text style</see> defines the basic properties of the information text.
        /// </remarks>
        public TextStyle Style
        {
            get { return this.style; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.style = this.OnTextStyleChangedEvent(this.style, value);
            }
        }

        /// <summary>
        /// Gets or sets the attribute <see cref="Vector3">position</see>.
        /// </summary>
        public Vector3 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        /// <summary>
        /// Gets or sets the attribute flags.
        /// </summary>
        public AttributeFlags Flags
        {
            get { return this.flags; }
            set { this.flags = value; }
        }

        /// <summary>
        /// Gets or sets the attribute text rotation in degrees.
        /// </summary>
        public double Rotation
        {
            get { return this.rotation; }
            set { this.rotation = MathHelper.NormalizeAngle(value); }
        }

        /// <summary>
        /// Gets or sets the text alignment.
        /// </summary>
        public TextAlignment Alignment
        {
            get { return this.alignment; }
            set { this.alignment = value; }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new Attribute that is a copy of the current instance.
        /// </summary>
        /// <returns>A new Attribute that is a copy of this instance.</returns>
        public object Clone()
        {
            Attribute entity = new Attribute
            {
                //EntityObject properties
                Layer = (Layer)this.layer.Clone(),
                LineType = (LineType)this.lineType.Clone(),
                Color = (AciColor)this.color.Clone(),
                Lineweight = (Lineweight)this.lineweight.Clone(),
                Transparency = (Transparency)this.transparency.Clone(),
                LineTypeScale = this.lineTypeScale,
                Normal = this.normal,
                //Attribute properties
                Definition = (AttributeDefinition)this.definition.Clone(),
                Tag = this.tag,
                Height = this.height,
                WidthFactor = this.widthFactor,
                ObliqueAngle = this.obliqueAngle,
                Value = this.value,
                Style = this.style,
                Position = this.position,
                Flags = this.flags,
                Rotation = this.rotation,
                Alignment = this.alignment
            };

            return entity;

        }

        #endregion
    }
}