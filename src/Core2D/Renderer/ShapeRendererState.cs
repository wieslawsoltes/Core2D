// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using Core2D.Shape;

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
        private ShapeState _drawShapeState;
        private BaseShape _selectedShape;
        private ImmutableHashSet<BaseShape> _selectedShapes;
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
        public ShapeState DrawShapeState
        {
            get => _drawShapeState;
            set => Update(ref _drawShapeState, value);
        }

        /// <summary>
        /// Currently selected shape.
        /// </summary>
        public BaseShape SelectedShape
        {
            get => _selectedShape;
            set => Update(ref _selectedShape, value);
        }

        /// <summary>
        /// Currently selected shapes.
        /// </summary>
        public ImmutableHashSet<BaseShape> SelectedShapes
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
            _selectedShape = default(BaseShape);
            _selectedShapes = default(ImmutableHashSet<BaseShape>);
        }

        /// <summary>
        /// Check whether the <see cref="PanX"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializePanX() => _panX != default(double);

        /// <summary>
        /// Check whether the <see cref="PanY"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializePanY() => _panY != default(double);

        /// <summary>
        /// Check whether the <see cref="ZoomX"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeZoomX() => _zoomX != default(double);

        /// <summary>
        /// Check whether the <see cref="ZoomY"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeZoomY() => _zoomY != default(double);

        /// <summary>
        /// Check whether the <see cref="DrawShapeState"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeDrawShapeState() => _drawShapeState != null;

        /// <summary>
        /// Check whether the <see cref="SelectedShape"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeSelectedShape() => _selectedShape != null;

        /// <summary>
        /// Check whether the <see cref="SelectedShapes"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeSelectedShapes() => _selectedShapes.IsEmpty == false;

        /// <summary>
        /// Check whether the <see cref="ImageCache"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeImageCache() => _imageCache != null;
    }
}
