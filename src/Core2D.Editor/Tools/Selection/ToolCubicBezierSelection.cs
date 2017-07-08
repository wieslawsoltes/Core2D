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
    /// Helper class for <see cref="CubicBezierShape"/> shape selection.
    /// </summary>
    public class ToolCubicBezierSelection
    {
        private readonly LayerContainer _layer;
        private readonly ICubicBezier _cubicBezier;
        private readonly ShapeStyle _style;
        private readonly BaseShape _point;
        private LineShape _line12;
        private LineShape _line43;
        private LineShape _line23;
        private PointShape _helperPoint1;
        private PointShape _helperPoint2;
        private PointShape _helperPoint3;
        private PointShape _helperPoint4;

        /// <summary>
        /// Initialize new instance of <see cref="ToolCubicBezierSelection"/> class.
        /// </summary>
        /// <param name="layer">The selection shapes layer.</param>
        /// <param name="shape">The selected shape.</param>
        /// <param name="style">The selection shapes style.</param>
        /// <param name="point">The selection point shape.</param>
        public ToolCubicBezierSelection(LayerContainer layer, ICubicBezier shape, ShapeStyle style, BaseShape point)
        {
            _layer = layer;
            _cubicBezier = shape;
            _style = style;
            _point = point;
        }

        /// <summary>
        /// Transfer selection state to Point4.
        /// </summary>
        public void ToStatePoint4()
        {
            _helperPoint1 = PointShape.Create(0, 0, _point);
            _layer.Shapes = _layer.Shapes.Add(_helperPoint1);
            _helperPoint4 = PointShape.Create(0, 0, _point);
            _layer.Shapes = _layer.Shapes.Add(_helperPoint4);
        }

        /// <summary>
        /// Transfer selection state to Point2.
        /// </summary>
        public void ToStatePoint2()
        {
            _line12 = LineShape.Create(0, 0, _style, null);
            _helperPoint2 = PointShape.Create(0, 0, _point);

            _layer.Shapes = _layer.Shapes.Add(_line12);
            _layer.Shapes = _layer.Shapes.Add(_helperPoint2);
        }

        /// <summary>
        /// Transfer selection state to Point3.
        /// </summary>
        public void ToStatePoint3()
        {
            _line43 = LineShape.Create(0, 0, _style, null);
            _line23 = LineShape.Create(0, 0, _style, null);
            _helperPoint3 = PointShape.Create(0, 0, _point);

            _layer.Shapes = _layer.Shapes.Add(_line43);
            _layer.Shapes = _layer.Shapes.Add(_line23);
            _layer.Shapes = _layer.Shapes.Add(_helperPoint3);
        }

        /// <summary>
        /// Move selection.
        /// </summary>
        public void Move()
        {
            if (_line12 != null)
            {
                _line12.Start.X = _cubicBezier.Point1.X;
                _line12.Start.Y = _cubicBezier.Point1.Y;
                _line12.End.X = _cubicBezier.Point2.X;
                _line12.End.Y = _cubicBezier.Point2.Y;
            }

            if (_line43 != null)
            {
                _line43.Start.X = _cubicBezier.Point4.X;
                _line43.Start.Y = _cubicBezier.Point4.Y;
                _line43.End.X = _cubicBezier.Point3.X;
                _line43.End.Y = _cubicBezier.Point3.Y;
            }

            if (_line23 != null)
            {
                _line23.Start.X = _cubicBezier.Point2.X;
                _line23.Start.Y = _cubicBezier.Point2.Y;
                _line23.End.X = _cubicBezier.Point3.X;
                _line23.End.Y = _cubicBezier.Point3.Y;
            }

            if (_helperPoint1 != null)
            {
                _helperPoint1.X = _cubicBezier.Point1.X;
                _helperPoint1.Y = _cubicBezier.Point1.Y;
            }

            if (_helperPoint2 != null)
            {
                _helperPoint2.X = _cubicBezier.Point2.X;
                _helperPoint2.Y = _cubicBezier.Point2.Y;
            }

            if (_helperPoint3 != null)
            {
                _helperPoint3.X = _cubicBezier.Point3.X;
                _helperPoint3.Y = _cubicBezier.Point3.Y;
            }

            if (_helperPoint4 != null)
            {
                _helperPoint4.X = _cubicBezier.Point4.X;
                _helperPoint4.Y = _cubicBezier.Point4.Y;
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

            if (_line43 != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_line43);
                _line43 = null;
            }

            if (_line23 != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_line23);
                _line23 = null;
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

            if (_helperPoint4 != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_helperPoint4);
                _helperPoint4 = null;
            }

            _layer.Invalidate();
        }
    }
}
