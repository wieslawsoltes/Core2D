// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Containers.Interfaces;
using Core2D.Shapes.Interfaces;
using Core2D.Style;

namespace Core2D.Editor.Tools.Selection
{
    /// <summary>
    /// Helper class for <see cref="IImageShape"/> shape selection.
    /// </summary>
    public class ToolImageSelection
    {
        private readonly ILayerContainer _layer;
        private readonly IImageShape _image;
        private readonly ShapeStyle _style;
        private readonly IBaseShape _point;
        private IPointShape _topLeftHelperPoint;
        private IPointShape _bottomRightHelperPoint;

        /// <summary>
        /// Initialize new instance of <see cref="ToolImageSelection"/> class.
        /// </summary>
        /// <param name="layer">The selection shapes layer.</param>
        /// <param name="shape">The selected shape.</param>
        /// <param name="style">The selection shapes style.</param>
        /// <param name="point">The selection point shape.</param>
        public ToolImageSelection(ILayerContainer layer, IImageShape shape, ShapeStyle style, IBaseShape point)
        {
            _layer = layer;
            _image = shape;
            _style = style;
            _point = point;
        }

        /// <summary>
        /// Transfer selection state to BottomRight.
        /// </summary>
        public void ToStateBottomRight()
        {
            _topLeftHelperPoint = PointShape.Create(0, 0, _point);
            _bottomRightHelperPoint = PointShape.Create(0, 0, _point);

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

            _layer.Invalidate();
        }

        /// <summary>
        /// Remove selection.
        /// </summary>
        public void Remove()
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

            _layer.Invalidate();
        }
    }
}
