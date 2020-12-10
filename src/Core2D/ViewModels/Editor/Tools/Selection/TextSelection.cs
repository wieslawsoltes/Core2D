using System;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Editor.Tools.Selection
{
    public class TextSelection
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly LayerContainerViewModel _layer;
        private readonly TextShapeViewModel _text;
        private readonly ShapeStyleViewModel _styleViewModel;
        private PointShapeViewModel _topLeftHelperPoint;
        private PointShapeViewModel _bottomRightHelperPoint;
        private RectangleShapeViewModel _helperRectangle;

        public TextSelection(IServiceProvider serviceProvider, LayerContainerViewModel layer, TextShapeViewModel shape, ShapeStyleViewModel style)
        {
            _serviceProvider = serviceProvider;
            _layer = layer;
            _text = shape;
            _styleViewModel = style;
        }

        public void ToStateBottomRight()
        {
            _helperRectangle = _serviceProvider.GetService<IFactory>().CreateRectangleShape(0, 0, _styleViewModel);
            _helperRectangle.State |= ShapeStateFlags.Thickness;

            _topLeftHelperPoint = _serviceProvider.GetService<IFactory>().CreatePointShape(0, 0);
            _bottomRightHelperPoint = _serviceProvider.GetService<IFactory>().CreatePointShape(0, 0);

            _layer.Shapes = _layer.Shapes.Add(_helperRectangle);
            _layer.Shapes = _layer.Shapes.Add(_topLeftHelperPoint);
            _layer.Shapes = _layer.Shapes.Add(_bottomRightHelperPoint);
        }

        public void Move()
        {
            if (_helperRectangle != null)
            {
                _helperRectangle.TopLeft.X = _text.TopLeft.X;
                _helperRectangle.TopLeft.Y = _text.TopLeft.Y;
                _helperRectangle.BottomRight.X = _text.BottomRight.X;
                _helperRectangle.BottomRight.Y = _text.BottomRight.Y;
            }

            if (_topLeftHelperPoint != null)
            {
                _topLeftHelperPoint.X = _text.TopLeft.X;
                _topLeftHelperPoint.Y = _text.TopLeft.Y;
            }

            if (_bottomRightHelperPoint != null)
            {
                _bottomRightHelperPoint.X = _text.BottomRight.X;
                _bottomRightHelperPoint.Y = _text.BottomRight.Y;
            }

            _layer.InvalidateLayer();
        }

        public void Reset()
        {
            if (_helperRectangle != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_helperRectangle);
                _helperRectangle = null;
            }

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
