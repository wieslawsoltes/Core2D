using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.Serialization;
using Core2D.Data;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Containers
{
    [DataContract(IsReference = true)]
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

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
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

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
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

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
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

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public ImmutableArray<LayerContainer> Layers
        {
            get => _layers;
            set => RaiseAndSetIfChanged(ref _layers, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public LayerContainer CurrentLayer
        {
            get => _currentLayer;
            set => RaiseAndSetIfChanged(ref _currentLayer, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public LayerContainer WorkingLayer
        {
            get => _workingLayer;
            set => RaiseAndSetIfChanged(ref _workingLayer, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public LayerContainer HelperLayer
        {
            get => _helperLayer;
            set => RaiseAndSetIfChanged(ref _helperLayer, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public BaseShape CurrentShape
        {
            get => _currentShape;
            set => RaiseAndSetIfChanged(ref _currentShape, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public PageContainer Template
        {
            get => _template;
            set => RaiseAndSetIfChanged(ref _template, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public Context Data
        {
            get => _data;
            set => RaiseAndSetIfChanged(ref _data, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool IsExpanded
        {
            get => _isExpanded;
            set => RaiseAndSetIfChanged(ref _isExpanded, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
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

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
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

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
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

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
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

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
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

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
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

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
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

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
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

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
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

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
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
    }
}
