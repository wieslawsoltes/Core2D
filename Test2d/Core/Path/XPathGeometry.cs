// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class XPathGeometry
    {
        /// <summary>
        /// 
        /// </summary>
        public IList<XPathFigure> Figures { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public XFillRule FillRule { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public XPathRect Bounds { get; set; }

        private XPathFigure _figure;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="figures"></param>
        /// <param name="fillRule"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static XPathGeometry Create(
            IList<XPathFigure> figures,
            XFillRule fillRule,
            XPathRect bounds)
        {
            return new XPathGeometry()
            {
                Figures = figures,
                FillRule = fillRule,
                Bounds = bounds
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="isFilled"></param>
        /// <param name="isClosed"></param>
        public void BeginFigure(
            XPathPoint startPoint,
            bool isFilled,
            bool isClosed)
        {
            _figure = XPathFigure.Create(
                startPoint,
                new List<XPathSegment>(),
                isFilled,
                isClosed);
            Figures.Add(_figure);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="size"></param>
        /// <param name="rotationAngle"></param>
        /// <param name="isLargeArc"></param>
        /// <param name="sweepDirection"></param>
        /// <param name="isStroked"></param>
        /// <param name="isSmoothJoin"></param>
        public void ArcTo(
            XPathPoint point,
            XPathSize size,
            double rotationAngle,
            bool isLargeArc,
            XSweepDirection sweepDirection,
            bool isStroked,
            bool isSmoothJoin)
        {
            var segment = XArcSegment.Create(
                point,
                size,
                rotationAngle,
                isLargeArc,
                sweepDirection,
                isStroked,
                isSmoothJoin);
            _figure.Segments.Add(segment);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="isStroked"></param>
        /// <param name="isSmoothJoin"></param>
        public void BezierTo(
            XPathPoint point1,
            XPathPoint point2,
            XPathPoint point3,
            bool isStroked,
            bool isSmoothJoin)
        {
            var segment = XBezierSegment.Create(
                point1,
                point2,
                point3,
                isStroked,
                isSmoothJoin);
            _figure.Segments.Add(segment);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="isStroked"></param>
        /// <param name="isSmoothJoin"></param>
        public void LineTo(
            XPathPoint point,
            bool isStroked,
            bool isSmoothJoin)
        {
            var segment = XLineSegment.Create(
                point,
                isStroked,
                isSmoothJoin);
            _figure.Segments.Add(segment);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="isStroked"></param>
        /// <param name="isSmoothJoin"></param>
        public void PolyBezierTo(
            IList<XPathPoint> points,
            bool isStroked,
            bool isSmoothJoin)
        {
            var segment = XPolyBezierSegment.Create(
                points,
                isStroked,
                isSmoothJoin);
            _figure.Segments.Add(segment);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="isStroked"></param>
        /// <param name="isSmoothJoin"></param>
        public void PolyLineTo(
            IList<XPathPoint> points,
            bool isStroked,
            bool isSmoothJoin)
        {
            var segment = XPolyLineSegment.Create(
                points,
                isStroked,
                isSmoothJoin);
            _figure.Segments.Add(segment);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="isStroked"></param>
        /// <param name="isSmoothJoin"></param>
        public void PolyQuadraticBezierTo(
            IList<XPathPoint> points,
            bool isStroked,
            bool isSmoothJoin)
        {
            var segment = XPolyQuadraticBezierSegment.Create(
                points,
                isStroked,
                isSmoothJoin);
            _figure.Segments.Add(segment);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="isStroked"></param>
        /// <param name="isSmoothJoin"></param>
        public void QuadraticBezierTo(
            XPathPoint point1,
            XPathPoint point2,
            bool isStroked,
            bool isSmoothJoin)
        {
            var segment = XQuadraticBezierSegment.Create(
                point1,
                point2,
                isStroked,
                isSmoothJoin);
            _figure.Segments.Add(segment);
        }
    }
}
