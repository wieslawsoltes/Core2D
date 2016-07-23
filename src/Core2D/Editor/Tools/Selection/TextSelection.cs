// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Project;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools.Selection
{
    /// <summary>
    /// Helper class for <see cref="XText"/> shape selection.
    /// </summary>
    public class TextSelection
    {
        private XLayer _layer;
        private XText _shape;
        private ShapeStyle _style;
        private BaseShape _point;
        private XPoint _topLeftHelperPoint;
        private XPoint _bottomRightHelperPoint;
        private XRectangle _helperRectangle;

        /// <summary>
        /// Initialize new instance of <see cref="TextSelection"/> class.
        /// </summary>
        /// <param name="layer">The selection shapes layer.</param>
        /// <param name="shape">The selected shape.</param>
        /// <param name="style">The selection shapes style.</param>
        /// <param name="point">The selection point shape.</param>
        public TextSelection(XLayer layer, XText shape, ShapeStyle style, BaseShape point)
        {
            _layer = layer;
            _shape = shape;
            _style = style;
            _point = point;
        }

        /// <summary>
        /// Transfer selection state to <see cref="ToolState.One"/>.
        /// </summary>
        public void ToStateOne()
        {
            _helperRectangle = XRectangle.Create(0, 0, _style, null);
            _topLeftHelperPoint = XPoint.Create(0, 0, _point);
            _bottomRightHelperPoint = XPoint.Create(0, 0, _point);

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
                _helperRectangle.TopLeft.X = _shape.TopLeft.X;
                _helperRectangle.TopLeft.Y = _shape.TopLeft.Y;
                _helperRectangle.BottomRight.X = _shape.BottomRight.X;
                _helperRectangle.BottomRight.Y = _shape.BottomRight.Y;
            }

            if (_topLeftHelperPoint != null)
            {
                _topLeftHelperPoint.X = _shape.TopLeft.X;
                _topLeftHelperPoint.Y = _shape.TopLeft.Y;
            }

            if (_bottomRightHelperPoint != null)
            {
                _bottomRightHelperPoint.X = _shape.BottomRight.X;
                _bottomRightHelperPoint.Y = _shape.BottomRight.Y;
            }
        }

        /// <summary>
        /// Remove selection.
        /// </summary>
        public void Remove()
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
        }
    }
}
