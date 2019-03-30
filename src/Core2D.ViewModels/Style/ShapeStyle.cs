// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Core2D.Style
{
    /// <summary>
    /// Shape style.
    /// </summary>
    public class ShapeStyle : BaseStyle, IShapeStyle
    {
        private ILineStyle _lineStyle;
        private IArrowStyle _startArrowStyle;
        private IArrowStyle _endArrowStyle;
        private ITextStyle _textStyle;

        /// <inheritdoc/>
        public ILineStyle LineStyle
        {
            get => _lineStyle;
            set => Update(ref _lineStyle, value);
        }

        /// <inheritdoc/>
        public IArrowStyle StartArrowStyle
        {
            get => _startArrowStyle;
            set => Update(ref _startArrowStyle, value);
        }

        /// <inheritdoc/>
        public IArrowStyle EndArrowStyle
        {
            get => _endArrowStyle;
            set => Update(ref _endArrowStyle, value);
        }

        /// <inheritdoc/>
        public ITextStyle TextStyle
        {
            get => _textStyle;
            set => Update(ref _textStyle, value);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            return new ShapeStyle()
            {
                Name = this.Name,
                Stroke = (IColor)this.Stroke.Copy(shared),
                Fill = (IColor)this.Fill.Copy(shared),
                Thickness = this.Thickness,
                LineCap = this.LineCap,
                Dashes = this.Dashes,
                DashOffset = 0.0,
                LineStyle = (ILineStyle)this.LineStyle.Copy(shared),
                TextStyle = (ITextStyle)this.TextStyle.Copy(shared),
                StartArrowStyle = (IArrowStyle)this.StartArrowStyle.Copy(shared),
                EndArrowStyle = (IArrowStyle)this.EndArrowStyle.Copy(shared)
            };
        }

        /// <summary>
        /// Check whether the <see cref="LineStyle"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeLineStyle() => _lineStyle != null;

        /// <summary>
        /// Check whether the <see cref="StartArrowStyle"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeStartArrowStyle() => _startArrowStyle != null;

        /// <summary>
        /// Check whether the <see cref="EndArrowStyle"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeEndArrowStyle() => _endArrowStyle != null;

        /// <summary>
        /// Check whether the <see cref="TextStyle"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeTextStyle() => _textStyle != null;
    }
}
