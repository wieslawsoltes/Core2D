// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Windows;
using System.Windows.Media;
using Core2D.Path;
using Core2D.Shapes;

namespace Core2D.Renderer.Wpf
{
    /// <summary>
    /// 
    /// </summary>
    public static class PathGeometryConverter
    {
        private static ImmutableArray<XPoint> ToXPoints(this IEnumerable<Point> points, double dx, double dy)
        {
            var xpoints = ImmutableArray.CreateBuilder<XPoint>();
            foreach (var point in points)
            {
                xpoints.Add(XPoint.Create(point.X + dx, point.Y + dy));
            }
            return xpoints.ToImmutable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pg"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static XPathGeometry ToXPathGeometry(this PathGeometry pg, double dx, double dy)
        {
            var geometry = XPathGeometry.Create(
                ImmutableArray.Create<XPathFigure>(),
                pg.FillRule == FillRule.EvenOdd ? XFillRule.EvenOdd : XFillRule.Nonzero);

            var context = new XPathGeometryContext(geometry);

            foreach (var pf in pg.Figures)
            {
                context.BeginFigure(
                    XPoint.Create(pf.StartPoint.X + dx, pf.StartPoint.Y + dy),
                    pf.IsFilled,
                    pf.IsClosed);

                foreach (var segment in pf.Segments)
                {
                    if (segment is ArcSegment)
                    {
                        var arcSegment = segment as ArcSegment;
                        context.ArcTo(
                            XPoint.Create(arcSegment.Point.X + dx, arcSegment.Point.Y + dy),
                            XPathSize.Create(arcSegment.Size.Width, arcSegment.Size.Height),
                            arcSegment.RotationAngle,
                            arcSegment.IsLargeArc,
                            arcSegment.SweepDirection == SweepDirection.Clockwise ? XSweepDirection.Clockwise : XSweepDirection.Counterclockwise,
                            arcSegment.IsStroked,
                            arcSegment.IsSmoothJoin);
                    }
                    else if (segment is BezierSegment)
                    {
                        var cubicBezierSegment = segment as BezierSegment;
                        context.CubicBezierTo(
                            XPoint.Create(cubicBezierSegment.Point1.X + dx, cubicBezierSegment.Point1.Y + dy),
                            XPoint.Create(cubicBezierSegment.Point2.X + dx, cubicBezierSegment.Point2.Y + dy),
                            XPoint.Create(cubicBezierSegment.Point3.X + dx, cubicBezierSegment.Point3.Y + dy),
                            cubicBezierSegment.IsStroked,
                            cubicBezierSegment.IsSmoothJoin);
                    }
                    else if (segment is LineSegment)
                    {
                        var lineSegment = segment as LineSegment;
                        context.LineTo(
                            XPoint.Create(lineSegment.Point.X + dx, lineSegment.Point.Y + dy),
                            lineSegment.IsStroked,
                            lineSegment.IsSmoothJoin);
                    }
                    else if (segment is PolyBezierSegment)
                    {
                        var polyCubicBezierSegment = segment as PolyBezierSegment;
                        context.PolyCubicBezierTo(
                            ToXPoints(polyCubicBezierSegment.Points, dx, dy),
                            polyCubicBezierSegment.IsStroked,
                            polyCubicBezierSegment.IsSmoothJoin);
                    }
                    else if (segment is PolyLineSegment)
                    {
                        var polyLineSegment = segment as PolyLineSegment;
                        context.PolyLineTo(
                            ToXPoints(polyLineSegment.Points, dx, dy),
                            polyLineSegment.IsStroked,
                            polyLineSegment.IsSmoothJoin);
                    }
                    else if (segment is PolyQuadraticBezierSegment)
                    {
                        var polyQuadraticSegment = segment as PolyQuadraticBezierSegment;
                        context.PolyQuadraticBezierTo(
                            ToXPoints(polyQuadraticSegment.Points, dx, dy),
                            polyQuadraticSegment.IsStroked,
                            polyQuadraticSegment.IsSmoothJoin);
                    }
                    else if (segment is QuadraticBezierSegment)
                    {
                        var quadraticBezierSegment = segment as QuadraticBezierSegment;
                        context.QuadraticBezierTo(
                            XPoint.Create(quadraticBezierSegment.Point1.X + dx, quadraticBezierSegment.Point1.Y + dy),
                            XPoint.Create(quadraticBezierSegment.Point2.X + dx, quadraticBezierSegment.Point2.Y + dy),
                            quadraticBezierSegment.IsStroked,
                            quadraticBezierSegment.IsSmoothJoin);
                    }
                    else
                    {
                        throw new NotSupportedException("Not supported segment type: " + segment.GetType());
                    }
                }
            }

            return geometry;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static XPathGeometry ToXPathGeometry(this string source)
        {
            var g = Geometry.Parse(source);
            var pg = PathGeometry.CreateFromGeometry(g);
            return ToXPathGeometry(pg, 0.0, 0.0);
        }
    }
}
