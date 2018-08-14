// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Shapes;

namespace Core2D.Renderer
{
    /// <summary>
    /// Shape renderer state.
    /// </summary>
    public class ShapeRendererState : ObservableObject
    {
        private double _panX;
        private double _panY;
        private double _zoomX;
        private double _zoomY;
        private IShapeState _drawShapeState;
        private IBaseShape _selectedShape;
        private ImmutableHashSet<IBaseShape> _selectedShapes;
        private IImageCache _imageCache;

        /// <summary>
        /// The X coordinate of current pan position.
        /// </summary>
        public double PanX
        {
            get => _panX;
            set => Update(ref _panX, value);
        }

        /// <summary>
        /// The Y coordinate of current pan position.
        /// </summary>
        public double PanY
        {
            get => _panY;
            set => Update(ref _panY, value);
        }

        /// <summary>
        /// The X component of current zoom value.
        /// </summary>
        public double ZoomX
        {
            get => _zoomX;
            set => Update(ref _zoomX, value);
        }

        /// <summary>
        /// The Y component of current zoom value.
        /// </summary>
        public double ZoomY
        {
            get => _zoomY;
            set => Update(ref _zoomY, value);
        }

        /// <summary>
        /// Flag indicating shape state to enable its drawing.
        /// </summary>
        public IShapeState DrawShapeState
        {
            get => _drawShapeState;
            set => Update(ref _drawShapeState, value);
        }

        /// <summary>
        /// Currently selected shape.
        /// </summary>
        public IBaseShape SelectedShape
        {
            get => _selectedShape;
            set => Update(ref _selectedShape, value);
        }

        /// <summary>
        /// Currently selected shapes.
        /// </summary>
        public ImmutableHashSet<IBaseShape> SelectedShapes
        {
            get => _selectedShapes;
            set => Update(ref _selectedShapes, value);
        }

        /// <summary>
        /// Image cache repository.
        /// </summary>
        public IImageCache ImageCache
        {
            get => _imageCache;
            set => Update(ref _imageCache, value);
        }

        /// <summary>
        /// Initializes a new <see cref="ShapeRendererState"/> instance.
        /// </summary>
        public ShapeRendererState()
        {
            _panX = 0.0;
            _panY = 0.0;
            _zoomX = 1.0;
            _zoomY = 1.0;
            _drawShapeState = ShapeState.Create(ShapeStateFlags.Visible);
            _selectedShape = default;
            _selectedShapes = default;
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check whether the <see cref="PanX"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializePanX() => _panX != default;

        /// <summary>
        /// Check whether the <see cref="PanY"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializePanY() => _panY != default;

        /// <summary>
        /// Check whether the <see cref="ZoomX"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeZoomX() => _zoomX != default;

        /// <summary>
        /// Check whether the <see cref="ZoomY"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeZoomY() => _zoomY != default;

        /// <summary>
        /// Check whether the <see cref="DrawShapeState"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeDrawShapeState() => _drawShapeState != null;

        /// <summary>
        /// Check whether the <see cref="SelectedShape"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeSelectedShape() => _selectedShape != null;

        /// <summary>
        /// Check whether the <see cref="SelectedShapes"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeSelectedShapes() => _selectedShapes.IsEmpty == false;

        /// <summary>
        /// Check whether the <see cref="ImageCache"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeImageCache() => _imageCache != null;
    }
}
