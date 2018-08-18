// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Core2D.Style
{
    /// <summary>
    /// Line ending arrow style.
    /// </summary>
    public class ArrowStyle : BaseStyle, IArrowStyle
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
        public ArrowStyle(IBaseStyle source) : this()
        {
            Stroke = source.Stroke.Clone();
            Fill = source.Fill.Clone();
            Thickness = source.Thickness;
            LineCap = source.LineCap;
            Dashes = source.Dashes ?? (default);
            DashOffset = source.DashOffset;
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="ArrowStyle"/> instance.
        /// </summary>
        /// <param name="arrowType">The arrow type.</param>
        /// <param name="isStroked">The arrow shape stroke flag.</param>
        /// <param name="isFilled">The arrow shape fill flag.</param>
        /// <param name="radiusX">The arrow X axis radius.</param>
        /// <param name="radiusY">The arrow Y axis radius.</param>
        /// <returns>The new instance of the <see cref="ArrowStyle"/> class.</returns>
        public static IArrowStyle Create(ArrowType arrowType = ArrowType.None, bool isStroked = true, bool isFilled = false, double radiusX = 5.0, double radiusY = 3.0)
        {
            return new ArrowStyle()
            {
                ArrowType = arrowType,
                IsFilled = isFilled,
                RadiusX = radiusX,
                RadiusY = radiusY
            };
        }

        /// <summary>
        /// Creates a new <see cref="ArrowStyle"/> instance.
        /// </summary>
        /// <param name="source">The source style.</param>
        /// <param name="arrowType">The arrow type.</param>
        /// <param name="isStroked">The arrow shape stroke flag.</param>
        /// <param name="isFilled">The arrow shape fill flag.</param>
        /// <param name="radiusX">The arrow X axis radius.</param>
        /// <param name="radiusY">The arrow Y axis radius.</param>
        /// <returns>The new instance of the <see cref="ArrowStyle"/> class.</returns>
        public static IArrowStyle Create(IBaseStyle source, ArrowType arrowType = ArrowType.None, bool isStroked = true, bool isFilled = false, double radiusX = 5.0, double radiusY = 5.0)
        {
            return new ArrowStyle(source)
            {
                ArrowType = arrowType,
                IsStroked = isStroked,
                IsFilled = isFilled,
                RadiusX = radiusX,
                RadiusY = radiusY
            };
        }

        /// <summary>
        /// Creates a new <see cref="ArrowStyle"/> instance.
        /// </summary>
        /// <param name="name">The arrow style name.</param>
        /// <param name="source">The source style.</param>
        /// <param name="arrowType">The arrow type.</param>
        /// <param name="isStroked">The arrow shape stroke flag.</param>
        /// <param name="isFilled">The arrow shape fill flag.</param>
        /// <param name="radiusX">The arrow X axis radius.</param>
        /// <param name="radiusY">The arrow Y axis radius.</param>
        /// <returns>The new instance of the <see cref="ArrowStyle"/> class.</returns>
        public static IArrowStyle Create(string name, IBaseStyle source, ArrowType arrowType = ArrowType.None, bool isStroked = true, bool isFilled = false, double radiusX = 5.0, double radiusY = 5.0)
        {
            return new ArrowStyle(source)
            {
                Name = name,
                ArrowType = arrowType,
                IsStroked = isStroked,
                IsFilled = isFilled,
                RadiusX = radiusX,
                RadiusY = radiusY
            };
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
