using System;
using System.Collections.Immutable;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Containers
{
    public partial class PageContainerViewModel : ViewModelBase, IDataObject, IGrid
    {
        [AutoNotify] private double _width;
        [AutoNotify] private double _height;
        [AutoNotify] private BaseColorViewModel _background;
        [AutoNotify] private ImmutableArray<LayerContainerViewModel> _layers;
        [AutoNotify] private LayerContainerViewModel _currentLayer;
        [AutoNotify] private LayerContainerViewModel _workingLayer;
        [AutoNotify] private LayerContainerViewModel _helperLayer;
        [AutoNotify] private BaseShapeViewModel _currentShape;
        [AutoNotify] private PageContainerViewModel _template;
        [AutoNotify] private ImmutableArray<PropertyViewModel> _properties;
        [AutoNotify] private RecordViewModel _record;
        [AutoNotify] private bool _isExpanded;
        [AutoNotify] private bool _isGridEnabled;
        [AutoNotify] private bool _isBorderEnabled;
        [AutoNotify] private double _gridOffsetLeft;
        [AutoNotify] private double _gridOffsetTop;
        [AutoNotify] private double _gridOffsetRight;
        [AutoNotify] private double _gridOffsetBottom;
        [AutoNotify] private double _gridCellWidth;
        [AutoNotify] private double _gridCellHeight;
        [AutoNotify] private BaseColorViewModel _gridStrokeColor;
        [AutoNotify] private double _gridStrokeThickness;

        public PageContainerViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public void SetCurrentLayer(LayerContainerViewModel layer) => CurrentLayer = layer;

        public void InvalidateLayer()
        {
            _template?.InvalidateLayer();

            if (_layers != null)
            {
                foreach (var layer in _layers)
                {
                    layer.InvalidateLayer();
                }
            }

            _workingLayer?.InvalidateLayer();

            _helperLayer?.InvalidateLayer();
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            if (_template?._background != null)
            {
                isDirty |= _background.IsDirty();
            }

            foreach (var layer in _layers)
            {
                isDirty |= layer.IsDirty();
            }

            if (_workingLayer != null)
            {
                isDirty |= _workingLayer.IsDirty();
            }

            if (_helperLayer != null)
            {
                isDirty |= _helperLayer.IsDirty();
            }

            if (_template != null)
            {
                isDirty |= _template.IsDirty();
            }

            foreach (var property in _properties)
            {
                isDirty |= property.IsDirty();
            }

            if (_record != null)
            {
                isDirty |= _record.IsDirty();
            }

            if (_gridStrokeColor != null)
            {
                isDirty |= _gridStrokeColor.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            _background?.Invalidate();

            foreach (var layer in _layers)
            {
                layer.Invalidate();
            }

            _workingLayer?.Invalidate();
            _helperLayer?.Invalidate();
            _template?.Invalidate();
 
            foreach (var property in _properties)
            {
                property.Invalidate();
            }

            _record?.Invalidate();
        }
    }
}
