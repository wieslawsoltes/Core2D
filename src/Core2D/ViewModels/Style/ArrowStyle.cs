using System.Collections.Generic;

namespace Core2D.Style
{
    /// <summary>
    /// Line ending arrow style.
    /// </summary>
    public class ArrowStyle : BaseStyle
    {
        private ArrowType _arrowType;
        private bool _isStroked;
        private bool _isFilled;
        private double _radiusX;
        private double _radiusY;

        /// <inheritdoc/>
        public ArrowType ArrowType
        {
            get => _arrowType;
            set => Update(ref _arrowType, value);
        }

        /// <inheritdoc/>
        public bool IsStroked
        {
            get => _isStroked;
            set => Update(ref _isStroked, value);
        }

        /// <inheritdoc/>
        public bool IsFilled
        {
            get => _isFilled;
            set => Update(ref _isFilled, value);
        }

        /// <inheritdoc/>
        public double RadiusX
        {
            get => _radiusX;
            set => Update(ref _radiusX, value);
        }

        /// <inheritdoc/>
        public double RadiusY
        {
            get => _radiusY;
            set => Update(ref _radiusY, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrowStyle"/> class.
        /// </summary>
        public ArrowStyle() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrowStyle"/> class.
        /// </summary>
        /// <param name="source">The source style.</param>
        public ArrowStyle(BaseStyle source) : this()
        {
            Stroke = (Color)source.Stroke.Copy(null);
            Fill = (Color)source.Fill.Copy(null);
            Thickness = source.Thickness;
            LineCap = source.LineCap;
            Dashes = source.Dashes ?? (default);
            DashOffset = source.DashOffset;
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            return new ArrowStyle()
            {
                Name = this.Name,
                Stroke = (Color)this.Stroke.Copy(shared),
                Fill = (Color)this.Fill.Copy(shared),
                Thickness = this.Thickness,
                LineCap = this.LineCap,
                Dashes = this.Dashes,
                DashOffset = this.DashOffset,
                ArrowType = this.ArrowType,
                IsStroked = this.IsStroked,
                IsFilled = this.IsFilled,
                RadiusX = this.RadiusX,
                RadiusY = this.RadiusY
            };
        }

        /// <inheritdoc/>
        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();
            return isDirty;
        }

        /// <inheritdoc/>
        public override void Invalidate()
        {
            base.Invalidate();
        }

        /// <summary>
        /// Check whether the <see cref="ArrowType"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeArrowType() => _arrowType != default;

        /// <summary>
        /// Check whether the <see cref="IsStroked"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeIsStroked() => _isStroked != default;

        /// <summary>
        /// Check whether the <see cref="IsFilled"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeIsFilled() => _isFilled != default;

        /// <summary>
        /// Check whether the <see cref="RadiusX"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeRadiusX() => _radiusX != default;

        /// <summary>
        /// Check whether the <see cref="RadiusY"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeRadiusY() => _radiusY != default;
    }
}
