using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Data;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Containers
{
    public partial class PageContainerViewModel : BaseContainerViewModel, IDataObject, IGrid
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
        [AutoNotify] private bool _isExpanded = false;
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

        public void SetCurrentLayer(LayerContainerViewModel layer) => CurrentLayer = layer;

        public virtual void InvalidateLayer()
        {
            Template?.InvalidateLayer();

            if (Layers != null)
            {
                foreach (var layer in Layers)
                {
                    layer.InvalidateLayer();
                }
            }

            WorkingLayer?.InvalidateLayer();

            HelperLayer?.InvalidateLayer();
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            if (_template?.Background != null)
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

            foreach (var property in Properties)
            {
                isDirty |= property.IsDirty();
            }

            if (RecordViewModel != null)
            {
                isDirty |= Record.IsDirty();
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
 
            foreach (var property in Properties)
            {
                property.Invalidate();
            }

            Record?.Invalidate();
        }
    }
}
