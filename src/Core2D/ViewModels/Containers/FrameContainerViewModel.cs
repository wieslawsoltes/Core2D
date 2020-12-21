#nullable disable
using System;
using System.Collections.Immutable;
using Core2D.Model;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Containers
{
    public partial class FrameContainerViewModel : BaseContainerViewModel, IDataObject
    {
        [AutoNotify] private ImmutableArray<LayerContainerViewModel> _layers;
        [AutoNotify] private LayerContainerViewModel _currentLayer;
        [AutoNotify] private LayerContainerViewModel _workingLayer;
        [AutoNotify] private LayerContainerViewModel _helperLayer;
        [AutoNotify] private BaseShapeViewModel _currentShape;
        [AutoNotify] private ImmutableArray<PropertyViewModel> _properties;
        [AutoNotify] private RecordViewModel _record;

        public FrameContainerViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public void SetCurrentLayer(LayerContainerViewModel layer) => CurrentLayer = layer;

        public virtual void InvalidateLayer()
        {
            if (_layers is { })
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

            foreach (var layer in _layers)
            {
                isDirty |= layer.IsDirty();
            }

            if (_workingLayer is { })
            {
                isDirty |= _workingLayer.IsDirty();
            }

            if (_helperLayer is { })
            {
                isDirty |= _helperLayer.IsDirty();
            }

            foreach (var property in _properties)
            {
                isDirty |= property.IsDirty();
            }

            if (_record is { })
            {
                isDirty |= _record.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            foreach (var layer in _layers)
            {
                layer.Invalidate();
            }

            _workingLayer?.Invalidate();
            _helperLayer?.Invalidate();
 
            foreach (var property in _properties)
            {
                property.Invalidate();
            }

            _record?.Invalidate();
        }
    }
}
