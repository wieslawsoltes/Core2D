// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;

namespace Test2d
{
    /// <summary>
    /// 
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

        /// <summary>
        /// 
        /// </summary>
        public double PanX
        {
            get { return _panX; }
            set { Update(ref _panX, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double PanY
        {
            get { return _panY; }
            set { Update(ref _panY, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double Zoom
        {
            get { return _zoom; }
            set { Update(ref _zoom, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool EnableAutofit
        {
            get { return _enableAutofit; }
            set { Update(ref _enableAutofit, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ShapeState DrawShapeState
        {
            get { return _drawShapeState; }
            set { Update(ref _drawShapeState, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public BaseShape SelectedShape
        {
            get { return _selectedShape; }
            set { Update(ref _selectedShape, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ImmutableHashSet<BaseShape> SelectedShapes
        {
            get { return _selectedShapes; }
            set { Update(ref _selectedShapes, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public RendererState()
        {
            _panX = 0.0;
            _panY = 0.0;
            _zoom = 1.0;
            _enableAutofit = true;
            _drawShapeState = ShapeState.Visible | ShapeState.Printable;
            _selectedShape = default(BaseShape);
            _selectedShapes = default(ImmutableHashSet<BaseShape>);
        }
    }
}
