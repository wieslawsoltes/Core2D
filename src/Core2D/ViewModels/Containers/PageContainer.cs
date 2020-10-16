using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Data;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Containers
{
    public class PageContainer : BaseContainer, IGrid
    {
        private double _width;
        private double _height;
        private BaseColor _background;
        private ImmutableArray<LayerContainer> _layers;
        private LayerContainer _currentLayer;
        private LayerContainer _workingLayer;
        private LayerContainer _helperLayer;
        private BaseShape _currentShape;
        private PageContainer _template;
        private Context _data;
        private bool _isExpanded = false;
        private bool _isGridEnabled;
        private bool _isBorderEnabled;
        private double _gridOffsetLeft;
        private double _gridOffsetTop;
        private double _gridOffsetRight;
        private double _gridOffsetBottom;
        private double _gridCellWidth;
        private double _gridCellHeight;
        private BaseColor _gridStrokeColor;
        private double _gridStrokeThickness;

        public double Width
        {
            get => _template != null ? _template.Width : _width;
            set
            {
                if (_template != null)
                {
                    _template.Width = value;
                    RaisePropertyChanged();
                }
                else
                {
                    RaiseAndSetIfChanged(ref _width, value);
                }
            }
        }

        public double Height
        {
            get => _template != null ? _template.Height : _height;
            set
            {
                if (_template != null)
                {
                    _template.Height = value;
                    RaisePropertyChanged();
                }
                else
                {
                    RaiseAndSetIfChanged(ref _height, value);
                }
            }
        }

        public BaseColor Background
        {
            get => _template != null ? _template.Background : _background;
            set
            {
                if (_template != null)
                {
                    _template.Background = value;
                    RaisePropertyChanged();
                }
                else
                {
                    RaiseAndSetIfChanged(ref _background, value);
                }
            }
        }

        public ImmutableArray<LayerContainer> Layers
        {
            get => _layers;
            set => RaiseAndSetIfChanged(ref _layers, value);
        }

        public LayerContainer CurrentLayer
        {
            get => _currentLayer;
            set => RaiseAndSetIfChanged(ref _currentLayer, value);
        }

        public LayerContainer WorkingLayer
        {
            get => _workingLayer;
            set => RaiseAndSetIfChanged(ref _workingLayer, value);
        }

        public LayerContainer HelperLayer
        {
            get => _helperLayer;
            set => RaiseAndSetIfChanged(ref _helperLayer, value);
        }

        public BaseShape CurrentShape
        {
            get => _currentShape;
            set => RaiseAndSetIfChanged(ref _currentShape, value);
        }

        public PageContainer Template
        {
            get => _template;
            set => RaiseAndSetIfChanged(ref _template, value);
        }

        public Context Data
        {
            get => _data;
            set => RaiseAndSetIfChanged(ref _data, value);
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set => RaiseAndSetIfChanged(ref _isExpanded, value);
        }

        public bool IsGridEnabled
        {
            get => _template != null ? _template.IsGridEnabled : _isGridEnabled;
            set
            {
                if (_template != null)
                {
                    _template.IsGridEnabled = value;
                    RaisePropertyChanged();
                }
                else
                {
                    RaiseAndSetIfChanged(ref _isGridEnabled, value);
                }
            }
        }

        public bool IsBorderEnabled
        {
            get => _template != null ? _template.IsBorderEnabled : _isBorderEnabled;
            set
            {
                if (_template != null)
                {
                    _template.IsBorderEnabled = value;
                    RaisePropertyChanged();
                }
                else
                {
                    RaiseAndSetIfChanged(ref _isBorderEnabled, value);
                }
            }
        }

        public double GridOffsetLeft
        {
            get => _template != null ? _template.GridOffsetLeft : _gridOffsetLeft;
            set
            {
                if (_template != null)
                {
                    _template.GridOffsetLeft = value;
                    RaisePropertyChanged();
                }
                else
                {
                    RaiseAndSetIfChanged(ref _gridOffsetLeft, value);
                }
            }
        }

        public double GridOffsetTop
        {
            get => _template != null ? _template.GridOffsetTop : _gridOffsetTop;
            set
            {
                if (_template != null)
                {
                    _template.GridOffsetTop = value;
                    RaisePropertyChanged();
                }
                else
                {
                    RaiseAndSetIfChanged(ref _gridOffsetTop, value);
                }
            }
        }

        public double GridOffsetRight
        {
            get => _template != null ? _template.GridOffsetRight : _gridOffsetRight;
            set
            {
                if (_template != null)
                {
                    _template.GridOffsetRight = value;
                    RaisePropertyChanged();
                }
                else
                {
                    RaiseAndSetIfChanged(ref _gridOffsetRight, value);
                }
            }
        }

        public double GridOffsetBottom
        {
            get => _template != null ? _template.GridOffsetBottom : _gridOffsetBottom;
            set
            {
                if (_template != null)
                {
                    _template.GridOffsetBottom = value;
                    RaisePropertyChanged();
                }
                else
                {
                    RaiseAndSetIfChanged(ref _gridOffsetBottom, value);
                }
            }
        }

        public double GridCellWidth
        {
            get => _template != null ? _template.GridCellWidth : _gridCellWidth;
            set
            {
                if (_template != null)
                {
                    _template.GridCellWidth = value;
                    RaisePropertyChanged();
                }
                else
                {
                    RaiseAndSetIfChanged(ref _gridCellWidth, value);
                }
            }
        }

        public double GridCellHeight
        {
            get => _template != null ? _template.GridCellHeight : _gridCellHeight;
            set
            {
                if (_template != null)
                {
                    _template.GridCellHeight = value;
                    RaisePropertyChanged();
                }
                else
                {
                    RaiseAndSetIfChanged(ref _gridCellHeight, value);
                }
            }
        }

        public BaseColor GridStrokeColor
        {
            get => _template != null ? _template.GridStrokeColor : _gridStrokeColor;
            set
            {
                if (_template != null)
                {
                    _template.GridStrokeColor = value;
                    RaisePropertyChanged();
                }
                else
                {
                    RaiseAndSetIfChanged(ref _gridStrokeColor, value);
                }
            }
        }

        public double GridStrokeThickness
        {
            get => _template != null ? _template.GridStrokeThickness : _gridStrokeThickness;
            set
            {
                if (_template != null)
                {
                    _template.GridStrokeThickness = value;
                    RaisePropertyChanged();
                }
                else
                {
                    RaiseAndSetIfChanged(ref _gridStrokeThickness, value);
                }
            }
        }

        public void SetCurrentLayer(LayerContainer layer) => CurrentLayer = layer;

        public virtual void InvalidateLayer()
        {
            if (Template != null)
            {
                Template.InvalidateLayer();
            }

            if (Layers != null)
            {
                foreach (var layer in Layers)
                {
                    layer.InvalidateLayer();
                }
            }

            if (WorkingLayer != null)
            {
                WorkingLayer.InvalidateLayer();
            }

            if (HelperLayer != null)
            {
                HelperLayer.InvalidateLayer();
            }
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            if (Background != null)
            {
                isDirty |= Background.IsDirty();
            }

            foreach (var layer in Layers)
            {
                isDirty |= layer.IsDirty();
            }

            if (WorkingLayer != null)
            {
                isDirty |= WorkingLayer.IsDirty();
            }

            if (HelperLayer != null)
            {
                isDirty |= HelperLayer.IsDirty();
            }

            if (Template != null)
            {
                isDirty |= Template.IsDirty();
            }

            if (Data != null)
            {
                isDirty |= Data.IsDirty();
            }

            if (GridStrokeColor != null)
            {
                isDirty |= GridStrokeColor.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            Background?.Invalidate();

            foreach (var layer in Layers)
            {
                layer.Invalidate();
            }

            WorkingLayer?.Invalidate();
            HelperLayer?.Invalidate();
            Template?.Invalidate();
            Data?.Invalidate();
        }

        public virtual bool ShouldSerializeWidth() => _width != default;

        public virtual bool ShouldSerializeHeight() => _height != default;

        public virtual bool ShouldSerializeBackground() => _background != null;

        public virtual bool ShouldSerializeLayers() => true;

        public virtual bool ShouldSerializeCurrentLayer() => _currentLayer != null;

        public virtual bool ShouldSerializeWorkingLayer() => _workingLayer != null;

        public virtual bool ShouldSerializeHelperLayer() => _helperLayer != null;

        public virtual bool ShouldSerializeCurrentShape() => _currentShape != null;

        public virtual bool ShouldSerializeTemplate() => _template != null;

        public virtual bool ShouldSerializeData() => _data != null;

        public virtual bool ShouldSerializeIsExpanded() => _isExpanded != default;

        public virtual bool ShouldSerializeIsGridEnabled() => _isGridEnabled != default;

        public virtual bool ShouldSerializeIsBorderEnabled() => _isBorderEnabled != default;

        public virtual bool ShouldSerializeGridOffsetLeft() => _gridOffsetLeft != default;

        public virtual bool ShouldSerializeGridOffsetTop() => _gridOffsetTop != default;

        public virtual bool ShouldSerializeGridOffsetRight() => _gridOffsetRight != default;

        public virtual bool ShouldSerializeGridOffsetBottom() => _gridOffsetBottom != default;

        public virtual bool ShouldSerializeGridCellWidth() => _gridCellWidth != default;

        public virtual bool ShouldSerializeGridCellHeight() => _gridCellHeight != default;

        public virtual bool ShouldSerializeGridStrokeColor() => _gridStrokeColor != null;

        public virtual bool ShouldSerializeGridStrokeThickness() => _gridStrokeThickness != default;
    }
}
