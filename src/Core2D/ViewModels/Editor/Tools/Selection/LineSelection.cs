using System;
using Core2D.Model;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Editor.Tools.Selection
{
    public partial class LineSelection
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly LayerContainerViewModel _layer;
        private readonly LineShapeViewModel _line;
        private readonly ShapeStyleViewModel _styleViewModel;
        private PointShapeViewModel _startHelperPoint;
        private PointShapeViewModel _endHelperPoint;

        public LineSelection(IServiceProvider serviceProvider, LayerContainerViewModel layer, LineShapeViewModel shape, ShapeStyleViewModel style)
        {
            _serviceProvider = serviceProvider;
            _layer = layer;
            _line = shape;
            _styleViewModel = style;
        }

        public void ToStateEnd()
        {
            _startHelperPoint = _serviceProvider.GetService<IFactory>().CreatePointShape(0, 0);
            _endHelperPoint = _serviceProvider.GetService<IFactory>().CreatePointShape(0, 0);

            _layer.Shapes = _layer.Shapes.Add(_startHelperPoint);
            _layer.Shapes = _layer.Shapes.Add(_endHelperPoint);
        }

        public void Move()
        {
            if (_startHelperPoint != null)
            {
                _startHelperPoint.X = _line.Start.X;
                _startHelperPoint.Y = _line.Start.Y;
            }

            if (_endHelperPoint != null)
            {
                _endHelperPoint.X = _line.End.X;
                _endHelperPoint.Y = _line.End.Y;
            }

            _layer.InvalidateLayer();
        }

        public void Reset()
        {
            if (_startHelperPoint != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_startHelperPoint);
                _startHelperPoint = null;
            }

            if (_endHelperPoint != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_endHelperPoint);
                _endHelperPoint = null;
            }

            _layer.InvalidateLayer();
        }
    }
}
