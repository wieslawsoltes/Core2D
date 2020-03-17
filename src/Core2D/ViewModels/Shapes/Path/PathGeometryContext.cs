// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using Core2D.Interfaces;
using Core2D.Shapes;

namespace Core2D.Path
{
    /// <summary>
    /// Path geometry context.
    /// </summary>
    public class PathGeometryContext : IGeometryContext
    {
        private readonly IFactory _factory;
        private IPathGeometry _geometry;
        private IPathFigure _currentFigure;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathGeometryContext"/> class.
        /// </summary>
        /// <param name="factory">The factory instance.</param>
        /// <param name="geometry">The path geometry.</param>
        public PathGeometryContext(IFactory factory, IPathGeometry geometry)
        {
            _factory = factory;
            _geometry = geometry ?? throw new ArgumentNullException(nameof(geometry));
        }

        /// <inheritdoc/>
        public void BeginFigure(IPointShape startPoint, bool isFilled = true, bool isClosed = true)
        {
            _currentFigure = _factory.CreatePathFigure(startPoint, isFilled, isClosed);
            _geometry.Figures = _geometry.Figures.Add(_currentFigure);
        }

        /// <inheritdoc/>
        public void SetClosedState(bool isClosed)
        {
            _currentFigure.IsClosed = isClosed;
        }

        /// <inheritdoc/>
        public void LineTo(IPointShape point, bool isStroked = true, bool isSmoothJoin = true)
        {
            var segment = _factory.CreateLineSegment(
                point,
                isStroked,
                isSmoothJoin);
            _currentFigure.Segments = _currentFigure.Segments.Add(segment);
        }

        /// <inheritdoc/>
        public void ArcTo(IPointShape point, IPathSize size, double rotationAngle = 0.0, bool isLargeArc = false, SweepDirection sweepDirection = SweepDirection.Clockwise, bool isStroked = true, bool isSmoothJoin = true)
        {
            var segment = _factory.CreateArcSegment(
                point,
                size,
                rotationAngle,
                isLargeArc,
                sweepDirection,
                isStroked,
                isSmoothJoin);
            _currentFigure.Segments = _currentFigure.Segments.Add(segment);
        }

        /// <inheritdoc/>
        public void CubicBezierTo(IPointShape point1, IPointShape point2, IPointShape point3, bool isStroked = true, bool isSmoothJoin = true)
        {
            var segment = _factory.CreateCubicBezierSegment(
                point1,
                point2,
                point3,
                isStroked,
                isSmoothJoin);
            _currentFigure.Segments = _currentFigure.Segments.Add(segment);
        }

        /// <inheritdoc/>
        public void QuadraticBezierTo(IPointShape point1, IPointShape point2, bool isStroked = true, bool isSmoothJoin = true)
        {
            var segment = _factory.CreateQuadraticBezierSegment(
                point1,
                point2,
                isStroked,
                isSmoothJoin);
            _currentFigure.Segments = _currentFigure.Segments.Add(segment);
        }

        /// <inheritdoc/>
        public void PolyLineTo(ImmutableArray<IPointShape> points, bool isStroked = true, bool isSmoothJoin = true)
        {
            var segment = _factory.CreatePolyLineSegment(
                points,
                isStroked,
                isSmoothJoin);
            _currentFigure.Segments = _currentFigure.Segments.Add(segment);
        }

        /// <inheritdoc/>
        public void PolyCubicBezierTo(ImmutableArray<IPointShape> points, bool isStroked = true, bool isSmoothJoin = true)
        {
            var segment = _factory.CreatePolyCubicBezierSegment(
                points,
                isStroked,
                isSmoothJoin);
            _currentFigure.Segments = _currentFigure.Segments.Add(segment);
        }

        /// <inheritdoc/>
        public void PolyQuadraticBezierTo(ImmutableArray<IPointShape> points, bool isStroked = true, bool isSmoothJoin = true)
        {
            var segment = _factory.CreatePolyQuadraticBezierSegment(
                points,
                isStroked,
                isSmoothJoin);
            _currentFigure.Segments = _currentFigure.Segments.Add(segment);
        }
    }
}
