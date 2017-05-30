// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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

        /// <summary>
        /// Gets or sets arrow type.
        /// </summary>
        public ArrowType ArrowType
        {
            get => _arrowType;
            set => Update(ref _arrowType, value);
        }

        /// <summary>
        /// Gets or sets value indicating whether arrow shape is stroked.
        /// </summary>
        public bool IsStroked
        {
            get => _isStroked;
            set => Update(ref _isStroked, value);
        }

        /// <summary>
        /// Gets or sets value indicating whether arrow shape is filled.
        /// </summary>
        public bool IsFilled
        {
            get => _isFilled;
            set => Update(ref _isFilled, value);
        }

        /// <summary>
        /// Gets or sets arrow X axis radius.
        /// </summary>
        public double RadiusX
        {
            get => _radiusX;
            set => Update(ref _radiusX, value);
        }

        /// <summary>
        /// Gets or sets arrow Y axis radius.
        /// </summary>
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
            Stroke = ArgbColor.Create
                (source.Stroke.A,
                 source.Stroke.R,
                 source.Stroke.G,
                 source.Stroke.B);
            Fill = ArgbColor.Create
                (source.Fill.A,
                 source.Fill.R,
                 source.Fill.G,
                 source.Fill.B);
            Thickness = source.Thickness;
            LineCap = source.LineCap;
            Dashes = source.Dashes != null ? source.Dashes : default(string);
            DashOffset = source.DashOffset;
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
        public static ArrowStyle Create(ArrowType arrowType = ArrowType.None, bool isStroked = true, bool isFilled = false, double radiusX = 5.0, double radiusY = 3.0)
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
        public static ArrowStyle Create(BaseStyle source, ArrowType arrowType = ArrowType.None, bool isStroked = true, bool isFilled = false, double radiusX = 5.0, double radiusY = 5.0)
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
        public static ArrowStyle Create(string name, BaseStyle source, ArrowType arrowType = ArrowType.None, bool isStroked = true, bool isFilled = false, double radiusX = 5.0, double radiusY = 5.0)
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
        /// Clones arrow style.
        /// </summary>
        /// <returns>The new instance of the <see cref="ArrowStyle"/> class.</returns>
        public ArrowStyle Clone()
        {
            return new ArrowStyle()
            {
                Name = Name,
                Stroke = Stroke.Clone(),
                Fill = Fill.Clone(),
                Thickness = Thickness,
                LineCap = LineCap,
                Dashes = Dashes,
                DashOffset = DashOffset,
                ArrowType = _arrowType,
                IsStroked = _isStroked,
                IsFilled = _isFilled,
                RadiusX = _radiusX,
                RadiusY = _radiusY
            };
        }

        /// <summary>
        /// Check whether the <see cref="ArrowType"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeArrowType() => _arrowType != default(ArrowType);

        /// <summary>
        /// Check whether the <see cref="IsStroked"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeIsStroked() => _isStroked != default(bool);

        /// <summary>
        /// Check whether the <see cref="IsFilled"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeIsFilled() => _isFilled != default(bool);

        /// <summary>
        /// Check whether the <see cref="RadiusX"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeRadiusX() => _radiusX != default(double);

        /// <summary>
        /// Check whether the <see cref="RadiusY"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeRadiusY() => _radiusY != default(double);
    }
}
