// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Containers;
using Core2D.Shapes;
using Core2D.Style;
using Spatial;
using Spatial.Arc;

namespace Core2D.Editor.Tools.Selection
{
    /// <summary>
    /// Helper class for <see cref="IArcShape"/> shape selection.
    /// </summary>
    public class ToolArcSelection
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILayerContainer _layer;
        private readonly IArcShape _arc;
        private readonly IShapeStyle _style;
        private readonly IBaseShape _point;
        private ILineShape _startLine;
        private ILineShape _endLine;
        private IEllipseShape _ellipse;
        private IPointShape _p1HelperPoint;
        private IPointShape _p2HelperPoint;
        private IPointShape _centerHelperPoint;
        private IPointShape _startHelperPoint;
        private IPointShape _endHelperPoint;

        /// <summary>
        /// Initialize new instance of <see cref="ToolArcSelection"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="layer">The selection shapes layer.</param>
        /// <param name="shape">The selected shape.</param>
        /// <param name="style">The selection shapes style.</param>
        /// <param name="point">The selection point shape.</param>
        public ToolArcSelection(IServiceProvider serviceProvider, ILayerContainer layer, IArcShape shape, IShapeStyle style, IBaseShape point)
        {
            _serviceProvider = serviceProvider;
            _layer = layer;
            _arc = shape;
            _style = style;
            _point = point;
        }

        /// <summary>
        /// Transfer selection state to Point2.
        /// </summary>
        public void ToStatePoint2()
        {
            _ellipse = _serviceProvider.GetService<IFactory>().CreateEllipseShape(0, 0, _style, null);
            _p1HelperPoint = _serviceProvider.GetService<IFactory>().CreatePointShape(0, 0, _point);
            _p2HelperPoint = _serviceProvider.GetService<IFactory>().CreatePointShape(0, 0, _point);
            _centerHelperPoint = _serviceProvider.GetService<IFactory>().CreatePointShape(0, 0, _point);

            _layer.Shapes = _layer.Shapes.Add(_ellipse);
            _layer.Shapes = _layer.Shapes.Add(_p1HelperPoint);
            _layer.Shapes = _layer.Shapes.Add(_p2HelperPoint);
            _layer.Shapes = _layer.Shapes.Add(_centerHelperPoint);
        }

        /// <summary>
        /// Transfer selection state to Point3.
        /// </summary>
        public void ToStatePoint3()
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

            _startLine = _serviceProvider.GetService<IFactory>().CreateLineShape(0, 0, _style, null);
            _startHelperPoint = _serviceProvider.GetService<IFactory>().CreatePointShape(0, 0, _point);

            _layer.Shapes = _layer.Shapes.Add(_startLine);
            _layer.Shapes = _layer.Shapes.Add(_startHelperPoint);
        }

        /// <summary>
        /// Transfer selection state to Point4.
        /// </summary>
        public void ToStatePoint4()
        {
            if (_ellipse != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_ellipse);
                _ellipse = null;
            }

            _endLine = _serviceProvider.GetService<IFactory>().CreateLineShape(0, 0, _style, null);
            _endHelperPoint = _serviceProvider.GetService<IFactory>().CreatePointShape(0, 0, _point);

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
