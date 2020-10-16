using System;
using System.Collections.Generic;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Renderer
{
    public class ShapeRendererState : ObservableObject
    {
        private double _panX;
        private double _panY;
        private double _zoomX;
        private double _zoomY;
        private ShapeState _drawShapeState;
        private ISet<BaseShape> _selectedShapes;
        private IImageCache _imageCache;
        private bool _drawDecorators;
        private bool _drawPoints;
        private ShapeStyle _pointStyle;
        private ShapeStyle _selectedPointStyle;
        private double _pointSize;
        private ShapeStyle _selectionStyle;
        private ShapeStyle _helperStyle;
        private IDecorator _decorator;

        public double PanX
        {
            get => _panX;
            set => RaiseAndSetIfChanged(ref _panX, value);
        }

        public double PanY
        {
            get => _panY;
            set => RaiseAndSetIfChanged(ref _panY, value);
        }

        public double ZoomX
        {
            get => _zoomX;
            set => RaiseAndSetIfChanged(ref _zoomX, value);
        }

        public double ZoomY
        {
            get => _zoomY;
            set => RaiseAndSetIfChanged(ref _zoomY, value);
        }

        public ShapeState DrawShapeState
        {
            get => _drawShapeState;
            set => RaiseAndSetIfChanged(ref _drawShapeState, value);
        }

        public ISet<BaseShape> SelectedShapes
        {
            get => _selectedShapes;
            set => RaiseAndSetIfChanged(ref _selectedShapes, value);
        }

        public IImageCache ImageCache
        {
            get => _imageCache;
            set => RaiseAndSetIfChanged(ref _imageCache, value);
        }

        public bool DrawDecorators
        {
            get => _drawDecorators;
            set => RaiseAndSetIfChanged(ref _drawDecorators, value);
        }

        public bool DrawPoints
        {
            get => _drawPoints;
            set => RaiseAndSetIfChanged(ref _drawPoints, value);
        }

        public ShapeStyle PointStyle
        {
            get => _pointStyle;
            set => RaiseAndSetIfChanged(ref _pointStyle, value);
        }

        public ShapeStyle SelectedPointStyle
        {
            get => _selectedPointStyle;
            set => RaiseAndSetIfChanged(ref _selectedPointStyle, value);
        }

        public double PointSize
        {
            get => _pointSize;
            set => RaiseAndSetIfChanged(ref _pointSize, value);
        }

        public ShapeStyle SelectionStyle
        {
            get => _selectionStyle;
            set => RaiseAndSetIfChanged(ref _selectionStyle, value);
        }

        public ShapeStyle HelperStyle
        {
            get => _helperStyle;
            set => RaiseAndSetIfChanged(ref _helperStyle, value);
        }

        public IDecorator Decorator
        {
            get => _decorator;
            set => RaiseAndSetIfChanged(ref _decorator, value);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= DrawShapeState.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            DrawShapeState.Invalidate();
        }

        public virtual bool ShouldSerializePanX() => _panX != default;

        public virtual bool ShouldSerializePanY() => _panY != default;

        public virtual bool ShouldSerializeZoomX() => _zoomX != default;

        public virtual bool ShouldSerializeZoomY() => _zoomY != default;

        public virtual bool ShouldSerializeDrawShapeState() => _drawShapeState != null;

        public virtual bool ShouldSerializeSelectedShapes() => true;

        public virtual bool ShouldSerializeImageCache() => _imageCache != null;

        public virtual bool ShouldSerializeDrawDecorators() => _drawDecorators != default;

        public virtual bool ShouldSerializeDrawPoints() => _drawPoints != default;

        public virtual bool ShouldSerializePointStyle() => _pointStyle != null;

        public virtual bool ShouldSerializeSelectedPointStyle() => _selectedPointStyle != null;

        public virtual bool ShouldSerializePointSize() => _pointSize != default;

        public virtual bool ShouldSerializeSelectionStyle() => _selectionStyle != null;

        public virtual bool ShouldSerializeHelperStyle() => _helperStyle != null;

        public virtual bool ShouldSerializeDecorator() => _decorator != null;
    }
}
