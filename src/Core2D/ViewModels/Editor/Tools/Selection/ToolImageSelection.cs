using System;
using Core2D;
using Core2D.Containers;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools.Selection
{
    /// <summary>
    /// Helper class for <see cref="ImageShape"/> shape selection.
    /// </summary>
    public class ToolImageSelection
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly LayerContainer _layer;
        private readonly ImageShape _image;
        private readonly ShapeStyle _style;
        private PointShape _topLeftHelperPoint;
        private PointShape _bottomRightHelperPoint;

        /// <summary>
        /// Initialize new instance of <see cref="ToolImageSelection"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="layer">The selection shapes layer.</param>
        /// <param name="shape">The selected shape.</param>
        /// <param name="style">The selection shapes style.</param>
        public ToolImageSelection(IServiceProvider serviceProvider, LayerContainer layer, ImageShape shape, ShapeStyle style)
        {
            _serviceProvider = serviceProvider;
            _layer = layer;
            _image = shape;
            _style = style;
        }

        /// <summary>
        /// Transfer selection state to BottomRight.
        /// </summary>
        public void ToStateBottomRight()
        {
            _topLeftHelperPoint = _serviceProvider.GetService<IFactory>().CreatePointShape(0, 0);
            _bottomRightHelperPoint = _serviceProvider.GetService<IFactory>().CreatePointShape(0, 0);

            _layer.Shapes = _layer.Shapes.Add(_topLeftHelperPoint);
            _layer.Shapes = _layer.Shapes.Add(_bottomRightHelperPoint);
        }

        /// <summary>
        /// Move selection.
        /// </summary>
        public void Move()
        {
            if (_topLeftHelperPoint != null)
            {
                _topLeftHelperPoint.X = _image.TopLeft.X;
                _topLeftHelperPoint.Y = _image.TopLeft.Y;
            }

            if (_bottomRightHelperPoint != null)
            {
                _bottomRightHelperPoint.X = _image.BottomRight.X;
                _bottomRightHelperPoint.Y = _image.BottomRight.Y;
            }

            _layer.InvalidateLayer();
        }

        /// <summary>
        /// Reset selection.
        /// </summary>
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
