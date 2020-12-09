using System;
using Core2D;
using Core2D.Containers;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools.Selection
{
    public class RectangleSelection
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly LayerContainerViewModel _layer;
        private readonly RectangleShapeViewModel _rectangle;
        private readonly ShapeStyleViewModel _styleViewModel;
        private PointShapeViewModel _topLeftHelperPoint;
        private PointShapeViewModel _bottomRightHelperPoint;

        public RectangleSelection(IServiceProvider serviceProvider, LayerContainerViewModel layer, RectangleShapeViewModel shapeViewModel, ShapeStyleViewModel styleViewModel)
        {
            _serviceProvider = serviceProvider;
            _layer = layer;
            _rectangle = shapeViewModel;
            _styleViewModel = styleViewModel;
        }

        public void ToStateBottomRight()
        {
            _topLeftHelperPoint = _serviceProvider.GetService<IFactory>().CreatePointShape(0, 0);
            _bottomRightHelperPoint = _serviceProvider.GetService<IFactory>().CreatePointShape(0, 0);

            _layer.Shapes = _layer.Shapes.Add(_topLeftHelperPoint);
            _layer.Shapes = _layer.Shapes.Add(_bottomRightHelperPoint);
        }

        public void Move()
        {
            if (_topLeftHelperPoint != null)
            {
                _topLeftHelperPoint.X = _rectangle.TopLeft.X;
                _topLeftHelperPoint.Y = _rectangle.TopLeft.Y;
            }

            if (_bottomRightHelperPoint != null)
            {
                _bottomRightHelperPoint.X = _rectangle.BottomRight.X;
                _bottomRightHelperPoint.Y = _rectangle.BottomRight.Y;
            }

            _layer.InvalidateLayer();
        }

        public void Reset()
        {
            if (_topLeftHelperPoint != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_topLeftHelperPoint);
                _topLeftHelperPoint = null;
            }

            if (_bottomRightHelperPoint != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_bottomRightHelperPoint);
                _bottomRightHelperPoint = null;
            }

            _layer.InvalidateLayer();
        }
    }
}
