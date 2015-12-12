// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace Core2D
{
    /// <summary>
    /// Path geometry.
    /// </summary>
    public class XPathGeometry
    {
        private XPathFigure _currentFigure;
        
        /// <summary>
        /// Gets or sets figures collection.
        /// </summary>
        public IList<XPathFigure> Figures { get; set; }

        /// <summary>
        /// Gets or sets fill rule.
        /// </summary>
        public XFillRule FillRule { get; set; }

        /// <summary>
        /// Creates a new <see cref="XPathGeometry"/> instance.
        /// </summary>
        /// <param name="figures">The figures collection.</param>
        /// <param name="fillRule">The fill rule.</param>
        /// <returns>The new instance of the <see cref="XPathGeometry"/> class.</returns>
        public static XPathGeometry Create(
            IList<XPathFigure> figures,
            XFillRule fillRule)
        {
            return new XPathGeometry()
            {
                Figures = figures,
                FillRule = fillRule
            };
        }

        /// <summary>
        /// Begins path figure.
        /// </summary>
        /// <param name="startPoint">The start point.</param>
        /// <param name="isFilled">The flag indicating whether figure is filled.</param>
        /// <param name="isClosed">The flag indicating whether figure is closed.</param>
        public void BeginFigure(
            XPoint startPoint,
            bool isFilled = true,
            bool isClosed = true)
        {
            _currentFigure = XPathFigure.Create(
                startPoint,
                new List<XPathSegment>(),
                isFilled,
                isClosed);
            Figures.Add(_currentFigure);
        }

        /// <summary>
        /// Sets the current closed state of the figure. 
        /// </summary>
        /// <param name="isClosed">The flag indicating whether figure is closed.</param>
        public void SetClosedState(bool isClosed)
        {
            _currentFigure.IsClosed = isClosed;
        }

        /// <summary>
        /// Adds line segment.
        /// </summary>
        /// <param name="point">The end point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        public void LineTo(
            XPoint point,
            bool isStroked = true,
            bool isSmoothJoin = true)
        {
            var segment = XLineSegment.Create(
                point,
                isStroked,
                isSmoothJoin);
            _currentFigure.Segments.Add(segment);
        }

        /// <summary>
        /// Adds arc segment.
        /// </summary>
        /// <param name="point">The end point.</param>
        /// <param name="size">The arc size.</param>
        /// <param name="rotationAngle">The rotation angle.</param>
        /// <param name="isLargeArc">The is large flag.</param>
        /// <param name="sweepDirection">The sweep direction flag.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        public void ArcTo(
            XPoint point,
            XPathSize size,
            double rotationAngle,
            bool isLargeArc = false,
            XSweepDirection sweepDirection = XSweepDirection.Clockwise,
            bool isStroked = true,
            bool isSmoothJoin = true)
        {
            var segment = XArcSegment.Create(
                point,
                size,
                rotationAngle,
                isLargeArc,
                sweepDirection,
                isStroked,
                isSmoothJoin);
            _currentFigure.Segments.Add(segment);
        }

        /// <summary>
        /// Adds cubic bezier segment.
        /// </summary>
        /// <param name="point1">The first control point.</param>
        /// <param name="point2">The second control point.</param>
        /// <param name="point3">The end point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        public void BezierTo(
            XPoint point1,
            XPoint point2,
            XPoint point3,
            bool isStroked = true,
            bool isSmoothJoin = true)
        {
            var segment = XBezierSegment.Create(
                point1,
                point2,
                point3,
                isStroked,
                isSmoothJoin);
            _currentFigure.Segments.Add(segment);
        }

        /// <summary>
        /// Adds quadratic bezier segment.
        /// </summary>
        /// <param name="point1">The control point.</param>
        /// <param name="point2">The end point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        public void QuadraticBezierTo(
            XPoint point1,
            XPoint point2,
            bool isStroked = true,
            bool isSmoothJoin = true)
        {
            var segment = XQuadraticBezierSegment.Create(
                point1,
                point2,
                isStroked,
                isSmoothJoin);
            _currentFigure.Segments.Add(segment);
        }

        /// <summary>
        /// Adds poly line segment.
        /// </summary>
        /// <param name="points">The points array.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        public void PolyLineTo(
            IList<XPoint> points,
            bool isStroked = true,
            bool isSmoothJoin = true)
        {
            var segment = XPolyLineSegment.Create(
                points,
                isStroked,
                isSmoothJoin);
            _currentFigure.Segments.Add(segment);
        }

        /// <summary>
        /// Adds poly cubic bezier segment.
        /// </summary>
        /// <param name="points">The points array.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        public void PolyBezierTo(
            IList<XPoint> points,
            bool isStroked = true,
            bool isSmoothJoin = true)
        {
            var segment = XPolyBezierSegment.Create(
                points,
                isStroked,
                isSmoothJoin);
            _currentFigure.Segments.Add(segment);
        }

        /// <summary>
        /// Adds poly quadratic bezier segment.
        /// </summary>
        /// <param name="points">The points array.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        public void PolyQuadraticBezierTo(
            IList<XPoint> points,
            bool isStroked = true,
            bool isSmoothJoin = true)
        {
            var segment = XPolyQuadraticBezierSegment.Create(
                points,
                isStroked,
                isSmoothJoin);
            _currentFigure.Segments.Add(segment);
        }
    }
}
