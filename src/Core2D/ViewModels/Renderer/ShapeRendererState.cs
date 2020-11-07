using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Renderer
{
    [DataContract(IsReference = true)]
    public class ShapeRendererState : ViewModelBase
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

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public double PanX
        {
            get => _panX;
            set => RaiseAndSetIfChanged(ref _panX, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public double PanY
        {
            get => _panY;
            set => RaiseAndSetIfChanged(ref _panY, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public double ZoomX
        {
            get => _zoomX;
            set => RaiseAndSetIfChanged(ref _zoomX, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public double ZoomY
        {
            get => _zoomY;
            set => RaiseAndSetIfChanged(ref _zoomY, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public ShapeState DrawShapeState
        {
            get => _drawShapeState;
            set => RaiseAndSetIfChanged(ref _drawShapeState, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public ISet<BaseShape> SelectedShapes
        {
            get => _selectedShapes;
            set => RaiseAndSetIfChanged(ref _selectedShapes, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public IImageCache ImageCache
        {
            get => _imageCache;
            set => RaiseAndSetIfChanged(ref _imageCache, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool DrawDecorators
        {
            get => _drawDecorators;
            set => RaiseAndSetIfChanged(ref _drawDecorators, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool DrawPoints
        {
            get => _drawPoints;
            set => RaiseAndSetIfChanged(ref _drawPoints, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public ShapeStyle PointStyle
        {
            get => _pointStyle;
            set => RaiseAndSetIfChanged(ref _pointStyle, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public ShapeStyle SelectedPointStyle
        {
            get => _selectedPointStyle;
            set => RaiseAndSetIfChanged(ref _selectedPointStyle, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public double PointSize
        {
            get => _pointSize;
            set => RaiseAndSetIfChanged(ref _pointSize, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public ShapeStyle SelectionStyle
        {
            get => _selectionStyle;
            set => RaiseAndSetIfChanged(ref _selectionStyle, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public ShapeStyle HelperStyle
        {
            get => _helperStyle;
            set => RaiseAndSetIfChanged(ref _helperStyle, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
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
    }
}
