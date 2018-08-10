// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Path;
using Core2D.Shape;
using Core2D.Style;

namespace Core2D.Containers
{
    /// <summary>
    /// Project options.
    /// </summary>
    public class Options : ObservableObject, ICopyable
    {
        private bool _snapToGrid = true;
        private double _snapX = 15.0;
        private double _snapY = 15.0;
        private double _hitThreshold = 7.0;
        private MoveMode _moveMode = MoveMode.Point;
        private bool _defaultIsStroked = true;
        private bool _defaultIsFilled = false;
        private bool _defaultIsClosed = true;
        private bool _defaultIsSmoothJoin = true;
        private FillRule _defaultFillRule = FillRule.EvenOdd;
        private bool _tryToConnect = false;
        private BaseShape _pointShape;
        private ShapeStyle _pointStyle;
        private ShapeStyle _selectionStyle;
        private ShapeStyle _helperStyle;
        private bool _cloneStyle = false;

        /// <summary>
        /// Gets or sets how grid snapping is handled. 
        /// </summary>
        public bool SnapToGrid
        {
            get => _snapToGrid;
            set => Update(ref _snapToGrid, value);
        }

        /// <summary>
        /// Gets or sets how much grid X axis is snapped.
        /// </summary>
        public double SnapX
        {
            get => _snapX;
            set => Update(ref _snapX, value);
        }

        /// <summary>
        /// Gets or sets how much grid Y axis is snapped.
        /// </summary>
        public double SnapY
        {
            get => _snapY;
            set => Update(ref _snapY, value);
        }

        /// <summary>
        /// Gets or sets hit test threshold radius.
        /// </summary>
        public double HitThreshold
        {
            get => _hitThreshold;
            set => Update(ref _hitThreshold, value);
        }

        /// <summary>
        /// Gets or sets how selected shapes are moved.
        /// </summary>
        public MoveMode MoveMode
        {
            get => _moveMode;
            set => Update(ref _moveMode, value);
        }

        /// <summary>
        /// Gets or sets value indicating whether path/shape is stroked during creation.
        /// </summary>
        public bool DefaultIsStroked
        {
            get => _defaultIsStroked;
            set => Update(ref _defaultIsStroked, value);
        }

        /// <summary>
        /// Gets or sets value indicating whether path/shape is filled during creation.
        /// </summary>
        public bool DefaultIsFilled
        {
            get => _defaultIsFilled;
            set => Update(ref _defaultIsFilled, value);
        }

        /// <summary>
        /// Gets or sets value indicating whether path is closed during creation.
        /// </summary>
        public bool DefaultIsClosed
        {
            get => _defaultIsClosed;
            set => Update(ref _defaultIsClosed, value);
        }

        /// <summary>
        /// Gets or sets value indicating whether path segment is smooth join during creation.
        /// </summary>
        public bool DefaultIsSmoothJoin
        {
            get => _defaultIsSmoothJoin;
            set => Update(ref _defaultIsSmoothJoin, value);
        }

        /// <summary>
        /// Gets or sets value indicating path fill rule during creation.
        /// </summary>
        public FillRule DefaultFillRule
        {
            get => _defaultFillRule;
            set => Update(ref _defaultFillRule, value);
        }

        /// <summary>
        /// Gets or sets how point connection is handled.
        /// </summary>
        public bool TryToConnect
        {
            get => _tryToConnect;
            set => Update(ref _tryToConnect, value);
        }

        /// <summary>
        /// Gets or sets shape used to draw points.
        /// </summary>
        public BaseShape PointShape
        {
            get => _pointShape;
            set => Update(ref _pointShape, value);
        }

        /// <summary>
        /// Gets or sets point shape style.
        /// </summary>
        public ShapeStyle PointStyle
        {
            get => _pointStyle;
            set => Update(ref _pointStyle, value);
        }

        /// <summary>
        /// Gets or sets selection rectangle style.
        /// </summary>
        public ShapeStyle SelectionStyle
        {
            get => _selectionStyle;
            set => Update(ref _selectionStyle, value);
        }

        /// <summary>
        /// Gets or sets editor helper shapes style.
        /// </summary>
        public ShapeStyle HelperStyle
        {
            get => _helperStyle;
            set => Update(ref _helperStyle, value);
        }

        /// <summary>
        /// Gets or sets value indicating whether style is cloned during creation.
        /// </summary>
        public bool CloneStyle
        {
            get => _cloneStyle;
            set => Update(ref _cloneStyle, value);
        }

        /// <inheritdoc/>
        public virtual object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="Options"/> instance.
        /// </summary>
        /// <returns>The new instance of the <see cref="Options"/> class.</returns>
        public static Options Create()
        {
            var options = new Options()
            {
                SnapToGrid = true,
                SnapX = 15.0,
                SnapY = 15.0,
                HitThreshold = 7.0,
                MoveMode = MoveMode.Point,
                DefaultIsStroked = true,
                DefaultIsFilled = false,
                DefaultIsClosed = true,
                DefaultIsSmoothJoin = true,
                DefaultFillRule = FillRule.EvenOdd,
                TryToConnect = false,
                CloneStyle = false
            };

            options.SelectionStyle =
                ShapeStyle.Create(
                    "Selection",
                    0x7F, 0x33, 0x33, 0xFF,
                    0x4F, 0x33, 0x33, 0xFF,
                    1.0);

            options.HelperStyle =
                ShapeStyle.Create(
                    "Helper",
                    0xFF, 0x00, 0x00, 0x00,
                    0xFF, 0x00, 0x00, 0x00,
                    1.0);

            options.PointStyle =
                ShapeStyle.Create(
                    "Point",
                    0xFF, 0x00, 0x00, 0x00,
                    0xFF, 0x00, 0x00, 0x00,
                    1.0);

            options.PointShape = RectanglePointShape(options.PointStyle);

            return options;
        }

        /// <summary>
        /// Creates a new <see cref="BaseShape"/> instance.
        /// </summary>
        /// <param name="pss">The point shape <see cref="ShapeStyle"/>.</param>
        /// <returns>The new instance of the <see cref="BaseShape"/> class.</returns>
        public static BaseShape EllipsePointShape(ShapeStyle pss)
        {
            var ellipse = EllipseShape.Create(-4, -4, 4, 4, pss, null, true, false);
            ellipse.Name = "EllipsePoint";
            return ellipse;
        }

        /// <summary>
        /// Creates a new <see cref="BaseShape"/> instance.
        /// </summary>
        /// <param name="pss">The point shape <see cref="ShapeStyle"/>.</param>
        /// <returns>The new instance of the <see cref="BaseShape"/> class.</returns>
        public static BaseShape FilledEllipsePointShape(ShapeStyle pss)
        {
            var ellipse = EllipseShape.Create(-3, -3, 3, 3, pss, null, true, true);
            ellipse.Name = "FilledEllipsePoint";
            return ellipse;
        }

        /// <summary>
        /// Creates a new <see cref="BaseShape"/> instance.
        /// </summary>
        /// <param name="pss">The point shape <see cref="ShapeStyle"/>.</param>
        /// <returns>The new instance of the <see cref="BaseShape"/> class.</returns>
        public static BaseShape RectanglePointShape(ShapeStyle pss)
        {
            var rectangle = RectangleShape.Create(-4, -4, 4, 4, pss, null, true, false);
            rectangle.Name = "RectanglePoint";
            return rectangle;
        }

        /// <summary>
        /// Creates a new <see cref="BaseShape"/> instance.
        /// </summary>
        /// <param name="pss">The point shape <see cref="ShapeStyle"/>.</param>
        /// <returns>The new instance of the <see cref="BaseShape"/> class.</returns>
        public static BaseShape FilledRectanglePointShape(ShapeStyle pss)
        {
            var rectangle = RectangleShape.Create(-3, -3, 3, 3, pss, null, true, true);
            rectangle.Name = "FilledRectanglePoint";
            return rectangle;
        }

        /// <summary>
        /// Creates a new <see cref="BaseShape"/> instance.
        /// </summary>
        /// <param name="pss">The point shape <see cref="ShapeStyle"/>.</param>
        /// <returns>The new instance of the <see cref="BaseShape"/> class.</returns>
        public static BaseShape CrossPointShape(ShapeStyle pss)
        {
            var group = GroupShape.Create("CrossPoint");
            var builder = group.Shapes.ToBuilder();
            builder.Add(LineShape.Create(-4, 0, 4, 0, pss, null));
            builder.Add(LineShape.Create(0, -4, 0, 4, pss, null));
            group.Shapes = builder.ToImmutable();
            return group;
        }

        /// <summary>
        /// Check whether the <see cref="SnapToGrid"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeSnapToGrid() => _snapToGrid != default;

        /// <summary>
        /// Check whether the <see cref="SnapX"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeSnapX() => _snapX != default;

        /// <summary>
        /// Check whether the <see cref="SnapY"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeSnapY() => _snapY != default;

        /// <summary>
        /// Check whether the <see cref="HitThreshold"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeHitThreshold() => _hitThreshold != default;

        /// <summary>
        /// Check whether the <see cref="MoveMode"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeMoveMode() => _moveMode != default;

        /// <summary>
        /// Check whether the <see cref="DefaultIsStroked"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeDefaultIsStroked() => _defaultIsStroked != default;

        /// <summary>
        /// Check whether the <see cref="DefaultIsFilled"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeDefaultIsFilled() => _defaultIsFilled != default;

        /// <summary>
        /// Check whether the <see cref="DefaultIsClosed"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeDefaultIsClosed() => _defaultIsClosed != default;

        /// <summary>
        /// Check whether the <see cref="DefaultIsSmoothJoin"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeDefaultIsSmoothJoin() => _defaultIsSmoothJoin != default;

        /// <summary>
        /// Check whether the <see cref="DefaultFillRule"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeDefaultFillRule() => _defaultFillRule != default;

        /// <summary>
        /// Check whether the <see cref="TryToConnect"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeTryToConnect() => _tryToConnect != default;

        /// <summary>
        /// Check whether the <see cref="PointShape"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializePointShape() => _pointShape != null;

        /// <summary>
        /// Check whether the <see cref="PointStyle"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializePointStyle() => _pointStyle != null;

        /// <summary>
        /// Check whether the <see cref="SelectionStyle"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeSelectionStyle() => _selectionStyle != null;

        /// <summary>
        /// Check whether the <see cref="HelperStyle"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeHelperStyle() => _helperStyle != null;

        /// <summary>
        /// Check whether the <see cref="CloneStyle"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeCloneStyle() => _cloneStyle != default;
    }
}
