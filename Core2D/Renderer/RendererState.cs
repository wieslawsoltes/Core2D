// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;

namespace Core2D
{
    /// <summary>
    /// Renderer state.
    /// </summary>
    public class RendererState : ObservableObject
    {
        private double _panX;
        private double _panY;
        private double _zoom;
        private bool _enableAutofit;
        private ShapeState _drawShapeState;
        private BaseShape _selectedShape;
        private ImmutableHashSet<BaseShape> _selectedShapes;
        private IImageCache _imageCache;

        /// <summary>
        /// The X coordinate of current pan position.
        /// </summary>
        public double PanX
        {
            get { return _panX; }
            set { Update(ref _panX, value); }
        }

        /// <summary>
        /// The Y coordinate of current pan position.
        /// </summary>
        public double PanY
        {
            get { return _panY; }
            set { Update(ref _panY, value); }
        }

        /// <summary>
        /// The current zoom value.
        /// </summary>
        public double Zoom
        {
            get { return _zoom; }
            set { Update(ref _zoom, value); }
        }

        /// <summary>
        /// Flag indicating whether auto-fit is enabled.
        /// </summary>
        public bool EnableAutofit
        {
            get { return _enableAutofit; }
            set { Update(ref _enableAutofit, value); }
        }

        /// <summary>
        /// Flag indicating shape state to enable its drawing.
        /// </summary>
        public ShapeState DrawShapeState
        {
            get { return _drawShapeState; }
            set { Update(ref _drawShapeState, value); }
        }

        /// <summary>
        /// Currently selected shape.
        /// </summary>
        public BaseShape SelectedShape
        {
            get { return _selectedShape; }
            set { Update(ref _selectedShape, value); }
        }

        /// <summary>
        /// Currently selected shapes.
        /// </summary>
        public ImmutableHashSet<BaseShape> SelectedShapes
        {
            get { return _selectedShapes; }
            set { Update(ref _selectedShapes, value); }
        }

        /// <summary>
        /// Image cache repository.
        /// </summary>
        public IImageCache ImageCache
        {
            get { return _imageCache; }
            set { Update(ref _imageCache, value); }
        }

        /// <summary>
        /// Initializes a new <see cref="RendererState"/> instance.
        /// </summary>
        public RendererState()
        {
            _panX = 0.0;
            _panY = 0.0;
            _zoom = 1.0;
            _enableAutofit = true;
            _drawShapeState = ShapeState.Create(ShapeStateFlags.Visible | ShapeStateFlags.Printable);
            _selectedShape = default(BaseShape);
            _selectedShapes = default(ImmutableHashSet<BaseShape>);
        }
    }
}
