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
        private static ImmutableArray<PointShape> ToPointShapes(this IEnumerable<Point> points, double dx, double dy)
        {
            var PointShapes = ImmutableArray.CreateBuilder<PointShape>();
            foreach (var point in points)
            {
                PointShapes.Add(PointShape.Create(point.X + dx, point.Y + dy));
            }
            return PointShapes.ToImmutable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pg"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static Path.PathGeometry ToPathGeometry(this System.Windows.Media.PathGeometry pg, double dx, double dy)
        {
            var geometry = Path.PathGeometry.Create(
                ImmutableArray.Create<Path.PathFigure>(),
                pg.FillRule == System.Windows.Media.FillRule.EvenOdd ? Path.FillRule.EvenOdd : Path.FillRule.Nonzero);

            var context = new PathGeometryContext(geometry);

            foreach (var pf in pg.Figures)
            {
                context.BeginFigure(
                    PointShape.Create(pf.StartPoint.X + dx, pf.StartPoint.Y + dy),
                    pf.IsFilled,
                    pf.IsClosed);

                foreach (var segment in pf.Segments)
                {
                    if (segment is ArcSegment)
                    {
                        var arcSegment = segment as ArcSegment;
                        context.ArcTo(
                            PointShape.Create(arcSegment.Point.X + dx, arcSegment.Point.Y + dy),
                            PathSize.Create(arcSegment.Size.Width, arcSegment.Size.Height),
                            arcSegment.RotationAngle,
                            arcSegment.IsLargeArc,
                            arcSegment.SweepDirection == System.Windows.Media.SweepDirection.Clockwise ? Path.SweepDirection.Clockwise : Path.SweepDirection.Counterclockwise,
                            arcSegment.IsStroked,
                            arcSegment.IsSmoothJoin);
                    }
                    else if (segment is BezierSegment)
                    {
                        var cubicBezierSegment = segment as BezierSegment;
                        context.CubicBezierTo(
                            PointShape.Create(cubicBezierSegment.Point1.X + dx, cubicBezierSegment.Point1.Y + dy),
                            PointShape.Create(cubicBezierSegment.Point2.X + dx, cubicBezierSegment.Point2.Y + dy),
                            PointShape.Create(cubicBezierSegment.Point3.X + dx, cubicBezierSegment.Point3.Y + dy),
                            cubicBezierSegment.IsStroked,
                            cubicBezierSegment.IsSmoothJoin);
                    }
                    else if (segment is LineSegment)
                    {
                        var lineSegment = segment as LineSegment;
                        context.LineTo(
                            PointShape.Create(lineSegment.Point.X + dx, lineSegment.Point.Y + dy),
                            lineSegment.IsStroked,
                            lineSegment.IsSmoothJoin);
                    }
                    else if (segment is PolyBezierSegment)
                    {
                        var polyCubicBezierSegment = segment as PolyBezierSegment;
                        context.PolyCubicBezierTo(
                            ToPointShapes(polyCubicBezierSegment.Points, dx, dy),
                            polyCubicBezierSegment.IsStroked,
                            polyCubicBezierSegment.IsSmoothJoin);
                    }
                    else if (segment is PolyLineSegment)
                    {
                        var polyLineSegment = segment as PolyLineSegment;
                        context.PolyLineTo(
                            ToPointShapes(polyLineSegment.Points, dx, dy),
                            polyLineSegment.IsStroked,
                            polyLineSegment.IsSmoothJoin);
                    }
                    else if (segment is PolyQuadraticBezierSegment)
                    {
                        var polyQuadraticSegment = segment as PolyQuadraticBezierSegment;
                        context.PolyQuadraticBezierTo(
                            ToPointShapes(polyQuadraticSegment.Points, dx, dy),
                            polyQuadraticSegment.IsStroked,
                            polyQuadraticSegment.IsSmoothJoin);
                    }
                    else if (segment is QuadraticBezierSegment)
                    {
                        var quadraticBezierSegment = segment as QuadraticBezierSegment;
                        context.QuadraticBezierTo(
                            PointShape.Create(quadraticBezierSegment.Point1.X + dx, quadraticBezierSegment.Point1.Y + dy),
                            PointShape.Create(quadraticBezierSegment.Point2.X + dx, quadraticBezierSegment.Point2.Y + dy),
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
        public static Path.PathGeometry ToPathGeometry(this string source)
        {
            var g = Geometry.Parse(source);
            var pg = System.Windows.Media.PathGeometry.CreateFromGeometry(g);
            return ToPathGeometry(pg, 0.0, 0.0);
        }
    }
}
