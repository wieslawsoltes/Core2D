using System;
using netDxf.Objects;
using netDxf.Tables;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents an underlay <see cref="EntityObject">entity</see>.
    /// </summary>
    public class Underlay :
        EntityObject
    {
        #region private fields

        private UnderlayDefinition definition;
        private Vector3 position;
        private Vector3 scale;
        private double rotation;
        private short contrast;
        private short fade;
        private UnderlayDisplayFlags displayOptions;
        private ClippingBoundary clippingBoundary;

        #endregion

        #region constructor

        internal Underlay()
            : base(EntityType.Underlay, DxfObjectCode.Underlay)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Underlay</c> class.
        /// </summary>
        /// <param name="definition">Underlay definition.</param>
        public Underlay(UnderlayDefinition definition)
            : base(EntityType.Underlay, DxfObjectCode.Underlay)
        {
            this.definition = definition;
            this.position = Vector3.Zero;
            this.scale = new Vector3(1.0);
            this.rotation = 0.0;
            this.contrast = 100;
            this.fade = 0;
            this.displayOptions = UnderlayDisplayFlags.ShowUnderlay;
            this.clippingBoundary = null;
            switch (this.definition.Type)
            {
                case UnderlayType.DGN:
                    this.codeName = DxfObjectCode.UnderlayDgn;
                    break;
                case UnderlayType.DWF:
                    this.codeName = DxfObjectCode.UnderlayDwf;
                    break;
                case UnderlayType.PDF:
                    this.codeName = DxfObjectCode.UnderlayPdf;
                    break;
            }
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the underlay definition.
        /// </summary>
        public UnderlayDefinition Definition
        {
            get { return this.definition; }
            internal set
            {
                switch (value.Type)
                {
                    case UnderlayType.DGN:
                        this.codeName = DxfObjectCode.UnderlayDgn;
                        break;
                    case UnderlayType.DWF:
                        this.codeName = DxfObjectCode.UnderlayDwf;
                        break;
                    case UnderlayType.PDF:
                        this.codeName = DxfObjectCode.UnderlayPdf;
                        break;
                }
                this.definition = value;
            }
        }

        /// <summary>
        /// Gets or sets the underlay position in world coordinates.
        /// </summary>
        public Vector3 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        /// <summary>
        /// Gets or sets the underlay scale.
        /// </summary>
        public Vector3 Scale
        {
            get { return this.scale; }
            set { this.scale = value; }
        }

        /// <summary>
        /// Gets or sets the underlay rotation around its normal.
        /// </summary>
        public double Rotation
        {
            get { return this.rotation; }
            set { this.rotation = MathHelper.NormalizeAngle(value); }
        }

        /// <summary>
        /// Gets or sets the underlay contrast.
        /// </summary>
        /// <remarks>Valid values range from 20 to 100.</remarks>
        public short Contrast
        {
            get { return this.contrast; }
            set
            {
                if (value < 20 || value > 100)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Accepted contrast values range from 20 to 100.");
                this.contrast = value;
            }
        }

        /// <summary>
        /// Gets or sets the underlay fade.
        /// </summary>
        /// <remarks>Valid values range from 0 to 80.</remarks>
        public short Fade
        {
            get { return this.fade; }
            set
            {
                if (value < 0 || value > 80)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Accepted fade values range from 0 to 80.");
                this.fade = value;
            }
        }

        /// <summary>
        /// Gets or sets the underlay display options.
        /// </summary>
        public UnderlayDisplayFlags DisplayOptions
        {
            get { return this.displayOptions; }
            set { this.displayOptions = value; }
        }

        /// <summary>
        /// Gets or sets the underlay clipping boundary.
        /// </summary>
        /// <remarks>
        /// Set as null to restore the default clipping boundary, show the full underlay without clipping.
        /// </remarks>
        public ClippingBoundary ClippingBoundary
        {
            get { return this.clippingBoundary; }
            set { this.clippingBoundary = value; }
        }

        #endregion

        #region overrides
        
        /// <summary>
        /// Creates a new Underlay that is a copy of the current instance.
        /// </summary>
        /// <returns>A new Underlay that is a copy of this instance.</returns>
        public override object Clone()
        {
            Underlay entity = new Underlay
            {
                //EntityObject properties
                Layer = (Layer) this.layer.Clone(),
                LineType = (LineType) this.lineType.Clone(),
                Color = (AciColor) this.color.Clone(),
                Lineweight = (Lineweight) this.lineweight.Clone(),
                Transparency = (Transparency) this.transparency.Clone(),
                LineTypeScale = this.lineTypeScale,
                Normal = this.normal,
                //Underlay properties
                Definition = (UnderlayDefinition) this.definition.Clone(),
                Position = this.position,
                Scale = this.scale,
                Rotation = this.rotation,
                Contrast = this.contrast,
                Fade = this.fade,
                DisplayOptions = this.displayOptions,
                ClippingBoundary = this.clippingBoundary != null ? (ClippingBoundary) this.clippingBoundary.Clone() : null
            };

            foreach (XData data in this.xData.Values)
                entity.XData.Add((XData)data.Clone());

            return entity;
        }

        #endregion
    }
}
