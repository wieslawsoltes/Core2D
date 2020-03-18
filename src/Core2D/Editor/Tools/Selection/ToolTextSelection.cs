using System;
using Core2D.Containers;
using Core2D.Interfaces;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools.Selection
{
    /// <summary>
    /// Helper class for <see cref="ITextShape"/> shape selection.
    /// </summary>
    public class ToolTextSelection
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILayerContainer _layer;
        private readonly ITextShape _text;
        private readonly IShapeStyle _style;
        private readonly IBaseShape _point;
        private IPointShape _topLeftHelperPoint;
        private IPointShape _bottomRightHelperPoint;
        private IRectangleShape _helperRectangle;

        /// <summary>
        /// Initialize new instance of <see cref="ToolTextSelection"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="layer">The selection shapes layer.</param>
        /// <param name="shape">The selected shape.</param>
        /// <param name="style">The selection shapes style.</param>
        /// <param name="point">The selection point shape.</param>
        public ToolTextSelection(IServiceProvider serviceProvider, ILayerContainer layer, ITextShape shape, IShapeStyle style, IBaseShape point)
        {
            _serviceProvider = serviceProvider;
            _layer = layer;
            _text = shape;
            _style = style;
            _point = point;
        }

        /// <summary>
        /// Transfer selection state to BottomRight.
        /// </summary>
        public void ToStateBottomRight()
        {
            _helperRectangle = _serviceProvider.GetService<IFactory>().CreateRectangleShape(0, 0, _style, null);
            _topLeftHelperPoint = _serviceProvider.GetService<IFactory>().CreatePointShape(0, 0, _point);
            _bottomRightHelperPoint = _serviceProvider.GetService<IFactory>().CreatePointShape(0, 0, _point);

            _layer.Shapes = _layer.Shapes.Add(_helperRectangle);
            _layer.Shapes = _layer.Shapes.Add(_topLeftHelperPoint);
            _layer.Shapes = _layer.Shapes.Add(_bottomRightHelperPoint);
        }

        /// <summary>
        /// Move selection.
        /// </summary>
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

            _layer.Invalidate();
        }

        /// <summary>
        /// Reset selection.
        /// </summary>
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

            _layer.Invalidate();
        }
    }
}
