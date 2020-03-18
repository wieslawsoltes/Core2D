// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Style
{
    /// <summary>
    /// Base style.
    /// </summary>
    public abstract class BaseStyle : ObservableObject, IBaseStyle
    {
        private IColor _stroke;
        private IColor _fill;
        private double _thickness;
        private LineCap _lineCap;
        private string _dashes;
        private double _dashOffset;

        /// <inheritdoc/>
        public IColor Stroke
        {
            get => _stroke;
            set => Update(ref _stroke, value);
        }

        /// <inheritdoc/>
        public IColor Fill
        {
            get => _fill;
            set => Update(ref _fill, value);
        }

        /// <inheritdoc/>
        public double Thickness
        {
            get => _thickness;
            set => Update(ref _thickness, value);
        }

        /// <inheritdoc/>
        public LineCap LineCap
        {
            get => _lineCap;
            set => Update(ref _lineCap, value);
        }

        /// <inheritdoc/>
        public string Dashes
        {
            get => _dashes;
            set => Update(ref _dashes, value);
        }

        /// <inheritdoc/>
        public double DashOffset
        {
            get => _dashOffset;
            set => Update(ref _dashOffset, value);
        }

        /// <summary>
        /// Check whether the <see cref="Stroke"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeStroke() => _stroke != null;

        /// <summary>
        /// Check whether the <see cref="Fill"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeFill() => _fill != null;

        /// <summary>
        /// Check whether the <see cref="Thickness"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeThickness() => _thickness != default;

        /// <summary>
        /// Check whether the <see cref="LineCap"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeLineCap() => _lineCap != default;

        /// <summary>
        /// Check whether the <see cref="Dashes"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeDashes() => !string.IsNullOrWhiteSpace(_dashes);

        /// <summary>
        /// Check whether the <see cref="DashOffset"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeDashOffset() => _dashOffset != default;
    }
}
