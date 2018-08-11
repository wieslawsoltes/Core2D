// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Containers.Interfaces;
using Core2D.Shapes.Interfaces;
using Core2D.Style;

namespace Core2D.Editor.Tools.Selection
{
    /// <summary>
    /// Helper class for <see cref="ILineShape"/> shape selection.
    /// </summary>
    public class ToolLineSelection
    {
        private readonly ILayerContainer _layer;
        private readonly ILineShape _line;
        private readonly ShapeStyle _style;
        private readonly IBaseShape _point;
        private IPointShape _startHelperPoint;
        private IPointShape _endHelperPoint;

        /// <summary>
        /// Initialize new instance of <see cref="ToolLineSelection"/> class.
        /// </summary>
        /// <param name="layer">The selection shapes layer.</param>
        /// <param name="shape">The selected shape.</param>
        /// <param name="style">The selection shapes style.</param>
        /// <param name="point">The selection point shape.</param>
        public ToolLineSelection(ILayerContainer layer, ILineShape shape, ShapeStyle style, IBaseShape point)
        {
            _layer = layer;
            _line = shape;
            _style = style;
            _point = point;
        }

        /// <summary>
        /// Transfer selection state to End.
        /// </summary>
        public void ToStateEnd()
        {
            _startHelperPoint = PointShape.Create(0, 0, _point);
            _endHelperPoint = PointShape.Create(0, 0, _point);

            _layer.Shapes = _layer.Shapes.Add(_startHelperPoint);
            _layer.Shapes = _layer.Shapes.Add(_endHelperPoint);
        }

        /// <summary>
        /// Move selection.
        /// </summary>
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

            _layer.Invalidate();
        }

        /// <summary>
        /// Remove selection.
        /// </summary>
        public void Remove()
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

            _layer.Invalidate();
        }
    }
}
