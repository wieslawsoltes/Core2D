// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Project;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Shapes.Interfaces;
using Core2D.Style;

namespace Core2D.Editor.Tools.Selection
{
    /// <summary>
    /// Helper class for <see cref="XQuadraticBezier"/> shape selection.
    /// </summary>
    public class ToolQuadraticBezierSelection
    {
        private readonly XLayer _layer;
        private readonly IQuadraticBezier _quadraticBezier;
        private readonly ShapeStyle _style;
        private readonly BaseShape _point;
        private XLine _line12;
        private XLine _line32;
        private XPoint _helperPoint1;
        private XPoint _helperPoint2;
        private XPoint _helperPoint3;

        /// <summary>
        /// Initialize new instance of <see cref="ToolQuadraticBezierSelection"/> class.
        /// </summary>
        /// <param name="layer">The selection shapes layer.</param>
        /// <param name="shape">The selected shape.</param>
        /// <param name="style">The selection shapes style.</param>
        /// <param name="point">The selection point shape.</param>
        public ToolQuadraticBezierSelection(XLayer layer, IQuadraticBezier shape, ShapeStyle style, BaseShape point)
        {
            _layer = layer;
            _quadraticBezier = shape;
            _style = style;
            _point = point;
        }

        /// <summary>
        /// Transfer selection state to <see cref="ToolState.One"/>.
        /// </summary>
        public void ToStateOne()
        {
            _helperPoint1 = XPoint.Create(0, 0, _point);
            _helperPoint3 = XPoint.Create(0, 0, _point);

            _layer.Shapes = _layer.Shapes.Add(_helperPoint1);
            _layer.Shapes = _layer.Shapes.Add(_helperPoint3);
        }

        /// <summary>
        /// Transfer selection state to <see cref="ToolState.Two"/>.
        /// </summary>
        public void ToStateTwo()
        {
            _line12 = XLine.Create(0, 0, _style, null);
            _line32 = XLine.Create(0, 0, _style, null);
            _helperPoint2 = XPoint.Create(0, 0, _point);

            _layer.Shapes = _layer.Shapes.Add(_line12);
            _layer.Shapes = _layer.Shapes.Add(_line32);
            _layer.Shapes = _layer.Shapes.Add(_helperPoint2);
        }

        /// <summary>
        /// Move selection.
        /// </summary>
        public void Move()
        {
            if (_line12 != null)
            {
                _line12.Start.X = _quadraticBezier.Point1.X;
                _line12.Start.Y = _quadraticBezier.Point1.Y;
                _line12.End.X = _quadraticBezier.Point2.X;
                _line12.End.Y = _quadraticBezier.Point2.Y;
            }

            if (_line32 != null)
            {
                _line32.Start.X = _quadraticBezier.Point3.X;
                _line32.Start.Y = _quadraticBezier.Point3.Y;
                _line32.End.X = _quadraticBezier.Point2.X;
                _line32.End.Y = _quadraticBezier.Point2.Y;
            }

            if (_helperPoint1 != null)
            {
                _helperPoint1.X = _quadraticBezier.Point1.X;
                _helperPoint1.Y = _quadraticBezier.Point1.Y;
            }

            if (_helperPoint2 != null)
            {
                _helperPoint2.X = _quadraticBezier.Point2.X;
                _helperPoint2.Y = _quadraticBezier.Point2.Y;
            }

            if (_helperPoint3 != null)
            {
                _helperPoint3.X = _quadraticBezier.Point3.X;
                _helperPoint3.Y = _quadraticBezier.Point3.Y;
            }

            _layer.Invalidate();
        }

        /// <summary>
        /// Remove selection.
        /// </summary>
        public void Remove()
        {
            if (_line12 != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_line12);
                _line12 = null;
            }

            if (_line32 != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_line32);
                _line32 = null;
            }

            if (_helperPoint1 != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_helperPoint1);
                _helperPoint1 = null;
            }

            if (_helperPoint2 != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_helperPoint2);
                _helperPoint2 = null;
            }

            if (_helperPoint3 != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_helperPoint3);
                _helperPoint3 = null;
            }

            _layer.Invalidate();
        }
    }
}
