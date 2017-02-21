// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Spatial.Arc;
using Core2D.Project;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Shapes.Interfaces;
using Core2D.Style;
using Core2D.Spatial;

namespace Core2D.Editor.Tools.Selection
{
    /// <summary>
    /// Helper class for <see cref="XArc"/> shape selection.
    /// </summary>
    public class ArcSelection
    {
        private readonly XLayer _layer;
        private readonly IArc _arc;
        private readonly ShapeStyle _style;
        private readonly BaseShape _point;
        private XLine _startLine;
        private XLine _endLine;
        private XEllipse _ellipse;
        private XPoint _p1HelperPoint;
        private XPoint _p2HelperPoint;
        private XPoint _centerHelperPoint;
        private XPoint _startHelperPoint;
        private XPoint _endHelperPoint;

        /// <summary>
        /// Initialize new instance of <see cref="ArcSelection"/> class.
        /// </summary>
        /// <param name="layer">The selection shapes layer.</param>
        /// <param name="shape">The selected shape.</param>
        /// <param name="style">The selection shapes style.</param>
        /// <param name="point">The selection point shape.</param>
        public ArcSelection(XLayer layer, IArc shape, ShapeStyle style, BaseShape point)
        {
            _layer = layer;
            _arc = shape;
            _style = style;
            _point = point;
        }

        /// <summary>
        /// Transfer selection state to <see cref="ToolState.One"/>.
        /// </summary>
        public void ToStateOne()
        {
            _ellipse = XEllipse.Create(0, 0, _style, null);
            _p1HelperPoint = XPoint.Create(0, 0, _point);
            _p2HelperPoint = XPoint.Create(0, 0, _point);
            _centerHelperPoint = XPoint.Create(0, 0, _point);

            _layer.Shapes = _layer.Shapes.Add(_ellipse);
            _layer.Shapes = _layer.Shapes.Add(_p1HelperPoint);
            _layer.Shapes = _layer.Shapes.Add(_p2HelperPoint);
            _layer.Shapes = _layer.Shapes.Add(_centerHelperPoint);
        }

        /// <summary>
        /// Transfer selection state to <see cref="ToolState.Two"/>.
        /// </summary>
        public void ToStateTwo()
        {
            if (_p1HelperPoint != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_p1HelperPoint);
                _p1HelperPoint = null;
            }

            if (_p2HelperPoint != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_p2HelperPoint);
                _p2HelperPoint = null;
            }

            _startLine = XLine.Create(0, 0, _style, null);
            _startHelperPoint = XPoint.Create(0, 0, _point);

            _layer.Shapes = _layer.Shapes.Add(_startLine);
            _layer.Shapes = _layer.Shapes.Add(_startHelperPoint);
        }

        /// <summary>
        /// Transfer selection state to <see cref="ToolState.Three"/>.
        /// </summary>
        public void ToStateThree()
        {
            if (_ellipse != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_ellipse);
                _ellipse = null;
            }

            _endLine = XLine.Create(0, 0, _style, null);
            _endHelperPoint = XPoint.Create(0, 0, _point);

            _layer.Shapes = _layer.Shapes.Add(_endLine);
            _layer.Shapes = _layer.Shapes.Add(_endHelperPoint);
        }

        /// <summary>
        /// Move selection.
        /// </summary>
        public void Move()
        {
            var a = new WpfArc(
                Point2.FromXY(_arc.Point1.X, _arc.Point1.Y),
                Point2.FromXY(_arc.Point2.X, _arc.Point2.Y),
                Point2.FromXY(_arc.Point3.X, _arc.Point3.Y),
                Point2.FromXY(_arc.Point4.X, _arc.Point4.Y));

            if (_ellipse != null)
            {
                _ellipse.TopLeft.X = a.P1.X;
                _ellipse.TopLeft.Y = a.P1.Y;
                _ellipse.BottomRight.X = a.P2.X;
                _ellipse.BottomRight.Y = a.P2.Y;
            }

            if (_startLine != null)
            {
                _startLine.Start.X = a.Center.X;
                _startLine.Start.Y = a.Center.Y;
                _startLine.End.X = a.Start.X;
                _startLine.End.Y = a.Start.Y;
            }

            if (_endLine != null)
            {
                _endLine.Start.X = a.Center.X;
                _endLine.Start.Y = a.Center.Y;
                _endLine.End.X = a.End.X;
                _endLine.End.Y = a.End.Y;
            }

            if (_p1HelperPoint != null)
            {
                _p1HelperPoint.X = a.P1.X;
                _p1HelperPoint.Y = a.P1.Y;
            }

            if (_p2HelperPoint != null)
            {
                _p2HelperPoint.X = a.P2.X;
                _p2HelperPoint.Y = a.P2.Y;
            }

            if (_centerHelperPoint != null)
            {
                _centerHelperPoint.X = a.Center.X;
                _centerHelperPoint.Y = a.Center.Y;
            }

            if (_startHelperPoint != null)
            {
                _startHelperPoint.X = a.Start.X;
                _startHelperPoint.Y = a.Start.Y;
            }

            if (_endHelperPoint != null)
            {
                _endHelperPoint.X = a.End.X;
                _endHelperPoint.Y = a.End.Y;
            }

            _layer.Invalidate();
        }

        /// <summary>
        /// Remove selection.
        /// </summary>
        public void Remove()
        {
            if (_ellipse != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_ellipse);
                _ellipse = null;
            }

            if (_startLine != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_startLine);
                _startLine = null;
            }

            if (_endLine != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_endLine);
                _endLine = null;
            }

            if (_p1HelperPoint != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_p1HelperPoint);
                _p1HelperPoint = null;
            }

            if (_p2HelperPoint != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_p2HelperPoint);
                _p2HelperPoint = null;
            }

            if (_centerHelperPoint != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_centerHelperPoint);
                _centerHelperPoint = null;
            }

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
