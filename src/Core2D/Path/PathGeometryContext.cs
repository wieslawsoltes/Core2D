// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using Core2D.Path.Segments;
using Core2D.Shapes;

namespace Core2D.Path
{
    /// <summary>
    /// Path geometry context.
    /// </summary>
    public class PathGeometryContext : GeometryContext
    {
        private PathGeometry _geometry;
        private PathFigure _currentFigure;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathGeometryContext"/> class.
        /// </summary>
        public PathGeometryContext(PathGeometry geometry)
        {
            if (geometry == null)
            {
                throw new ArgumentNullException(nameof(geometry));
            }

            _geometry = geometry;
        }

        /// <inheritdoc/>
        public override void BeginFigure(PointShape startPoint, bool isFilled = true, bool isClosed = true)
        {
            _currentFigure = PathFigure.Create(startPoint, isFilled, isClosed);
            _geometry.Figures = _geometry.Figures.Add(_currentFigure);
        }

        /// <inheritdoc/>
        public override void SetClosedState(bool isClosed)
        {
            _currentFigure.IsClosed = isClosed;
        }

        /// <inheritdoc/>
        public override void LineTo(PointShape point, bool isStroked = true, bool isSmoothJoin = true)
        {
            var segment = LineSegment.Create(
                point,
                isStroked,
                isSmoothJoin);
            _currentFigure.Segments = _currentFigure.Segments.Add(segment);
        }

        /// <inheritdoc/>
        public override void ArcTo(PointShape point, PathSize size, double rotationAngle = 0.0, bool isLargeArc = false, SweepDirection sweepDirection = SweepDirection.Clockwise, bool isStroked = true, bool isSmoothJoin = true)
        {
            var segment = ArcSegment.Create(
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
        public override void CubicBezierTo(PointShape point1, PointShape point2, PointShape point3, bool isStroked = true, bool isSmoothJoin = true)
        {
            var segment = CubicBezierSegment.Create(
                point1,
                point2,
                point3,
                isStroked,
                isSmoothJoin);
            _currentFigure.Segments = _currentFigure.Segments.Add(segment);
        }

        /// <inheritdoc/>
        public override void QuadraticBezierTo(PointShape point1, PointShape point2, bool isStroked = true, bool isSmoothJoin = true)
        {
            var segment = QuadraticBezierSegment.Create(
                point1,
                point2,
                isStroked,
                isSmoothJoin);
            _currentFigure.Segments = _currentFigure.Segments.Add(segment);
        }

        /// <inheritdoc/>
        public override void PolyLineTo(ImmutableArray<PointShape> points, bool isStroked = true, bool isSmoothJoin = true)
        {
            var segment = PolyLineSegment.Create(
                points,
                isStroked,
                isSmoothJoin);
            _currentFigure.Segments = _currentFigure.Segments.Add(segment);
        }

        /// <inheritdoc/>
        public override void PolyCubicBezierTo(ImmutableArray<PointShape> points, bool isStroked = true, bool isSmoothJoin = true)
        {
            var segment = PolyCubicBezierSegment.Create(
                points,
                isStroked,
                isSmoothJoin);
            _currentFigure.Segments = _currentFigure.Segments.Add(segment);
        }

        /// <inheritdoc/>
        public override void PolyQuadraticBezierTo(ImmutableArray<PointShape> points, bool isStroked = true, bool isSmoothJoin = true)
        {
            var segment = PolyQuadraticBezierSegment.Create(
                points,
                isStroked,
                isSmoothJoin);
            _currentFigure.Segments = _currentFigure.Segments.Add(segment);
        }
    }
}
