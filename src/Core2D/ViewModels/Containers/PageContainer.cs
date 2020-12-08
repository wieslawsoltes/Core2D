using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Data;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Containers
{
    public partial class PageContainer : BaseContainer, IDataObject, IGrid
    {
        [AutoNotify] private double _width;
        [AutoNotify] private double _height;
        [AutoNotify] private BaseColor _background;
        [AutoNotify] private ImmutableArray<LayerContainer> _layers;
        [AutoNotify] private LayerContainer _currentLayer;
        [AutoNotify] private LayerContainer _workingLayer;
        [AutoNotify] private LayerContainer _helperLayer;
        [AutoNotify] private BaseShape _currentShape;
        [AutoNotify] private PageContainer _template;
        [AutoNotify] private ImmutableArray<Property> _properties;
        [AutoNotify] private Record _record;
        [AutoNotify] private bool _isExpanded = false;
        [AutoNotify] private bool _isGridEnabled;
        [AutoNotify] private bool _isBorderEnabled;
        [AutoNotify] private double _gridOffsetLeft;
        [AutoNotify] private double _gridOffsetTop;
        [AutoNotify] private double _gridOffsetRight;
        [AutoNotify] private double _gridOffsetBottom;
        [AutoNotify] private double _gridCellWidth;
        [AutoNotify] private double _gridCellHeight;
        [AutoNotify] private BaseColor _gridStrokeColor;
        [AutoNotify] private double _gridStrokeThickness;

        public void SetCurrentLayer(LayerContainer layer) => CurrentLayer = layer;

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

            if (Record != null)
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
