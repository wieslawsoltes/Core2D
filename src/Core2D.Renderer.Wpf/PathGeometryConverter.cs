// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Shapes;
using W = System.Windows;
using WM = System.Windows.Media;

namespace Core2D.Renderer.Wpf
{
    /// <summary>
    /// 
    /// </summary>
    public static class PathGeometryConverter
    {
        private static ImmutableArray<PointShape> ToPointShapes(this IEnumerable<W.Point> points, double dx, double dy)
        {
            var PointShapes = ImmutableArray.CreateBuilder<PointShape>();
            foreach (var point in points)
            {
                PointShapes.Add(PointShape.Create(point.X + dx, point.Y + dy));
            }
            return PointShapes.ToImmutable();
        }

        private static IList<W.Point> ToPoints(this IList<PointShape> PointShapes, double dx, double dy)
        {
            var points = new List<W.Point>();
            foreach (var point in PointShapes)
            {
                points.Add(new W.Point(point.X + dx, point.Y + dy));
            }
            return points;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pg"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static PathGeometry ToPathGeometry(this WM.PathGeometry pg, double dx, double dy)
        {
            var geometry = PathGeometry.Create(
                ImmutableArray.Create<PathFigure>(),
                pg.FillRule == WM.FillRule.EvenOdd ? FillRule.EvenOdd : FillRule.Nonzero);

            var context = new PathGeometryContext(geometry);

            foreach (var pf in pg.Figures)
            {
                context.BeginFigure(
                    PointShape.Create(pf.StartPoint.X + dx, pf.StartPoint.Y + dy),
                    pf.IsFilled,
                    pf.IsClosed);

                foreach (var segment in pf.Segments)
                {
                    if (segment is WM.ArcSegment arcSegment)
                    {
                        context.ArcTo(
                            PointShape.Create(arcSegment.Point.X + dx, arcSegment.Point.Y + dy),
                            PathSize.Create(arcSegment.Size.Width, arcSegment.Size.Height),
                            arcSegment.RotationAngle,
                            arcSegment.IsLargeArc,
                            arcSegment.SweepDirection == WM.SweepDirection.Clockwise ? SweepDirection.Clockwise : SweepDirection.Counterclockwise,
                            arcSegment.IsStroked,
                            arcSegment.IsSmoothJoin);
                    }
                    else if (segment is WM.BezierSegment cubicBezierSegment)
                    {
                        context.CubicBezierTo(
                            PointShape.Create(cubicBezierSegment.Point1.X + dx, cubicBezierSegment.Point1.Y + dy),
                            PointShape.Create(cubicBezierSegment.Point2.X + dx, cubicBezierSegment.Point2.Y + dy),
                            PointShape.Create(cubicBezierSegment.Point3.X + dx, cubicBezierSegment.Point3.Y + dy),
                            cubicBezierSegment.IsStroked,
                            cubicBezierSegment.IsSmoothJoin);
                    }
                    else if (segment is WM.LineSegment lineSegment)
                    {
                        context.LineTo(
                            PointShape.Create(lineSegment.Point.X + dx, lineSegment.Point.Y + dy),
                            lineSegment.IsStroked,
                            lineSegment.IsSmoothJoin);
                    }
                    else if (segment is WM.PolyBezierSegment polyCubicBezierSegment)
                    {
                        context.PolyCubicBezierTo(
                            ToPointShapes(polyCubicBezierSegment.Points, dx, dy),
                            polyCubicBezierSegment.IsStroked,
                            polyCubicBezierSegment.IsSmoothJoin);
                    }
                    else if (segment is WM.PolyLineSegment polyLineSegment)
                    {
                        context.PolyLineTo(
                            ToPointShapes(polyLineSegment.Points, dx, dy),
                            polyLineSegment.IsStroked,
                            polyLineSegment.IsSmoothJoin);
                    }
                    else if (segment is WM.PolyQuadraticBezierSegment polyQuadraticSegment)
                    {
                        context.PolyQuadraticBezierTo(
                            ToPointShapes(polyQuadraticSegment.Points, dx, dy),
                            polyQuadraticSegment.IsStroked,
                            polyQuadraticSegment.IsSmoothJoin);
                    }
                    else if (segment is WM.QuadraticBezierSegment quadraticBezierSegment)
                    {
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
        /// <param name="xpg"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static WM.StreamGeometry ToStreamGeometry(this PathGeometry xpg, double dx, double dy)
        {
            var sg = new WM.StreamGeometry();

            using (var sgc = sg.Open())
            {
                foreach (var xpf in xpg.Figures)
                {
                    sgc.BeginFigure(
                        new W.Point(xpf.StartPoint.X, xpf.StartPoint.Y),
                        xpf.IsFilled,
                        xpf.IsClosed);

                    foreach (var segment in xpf.Segments)
                    {
                        if (segment is ArcSegment arcSegment)
                        {
                            sgc.ArcTo(
                                new W.Point(arcSegment.Point.X + dx, arcSegment.Point.Y + dy),
                                new W.Size(arcSegment.Size.Width, arcSegment.Size.Height),
                                arcSegment.RotationAngle,
                                arcSegment.IsLargeArc,
                                arcSegment.SweepDirection == SweepDirection.Clockwise ? WM.SweepDirection.Clockwise : WM.SweepDirection.Counterclockwise,
                                arcSegment.IsStroked,
                                arcSegment.IsSmoothJoin);
                        }
                        else if (segment is CubicBezierSegment cubicBezierSegment)
                        {
                            sgc.BezierTo(
                                new W.Point(cubicBezierSegment.Point1.X + dx, cubicBezierSegment.Point1.Y + dy),
                                new W.Point(cubicBezierSegment.Point2.X + dx, cubicBezierSegment.Point2.Y + dy),
                                new W.Point(cubicBezierSegment.Point3.X + dx, cubicBezierSegment.Point3.Y + dy),
                                cubicBezierSegment.IsStroked,
                                cubicBezierSegment.IsSmoothJoin);
                        }
                        else if (segment is LineSegment lineSegment)
                        {
                            sgc.LineTo(
                                new W.Point(lineSegment.Point.X + dx, lineSegment.Point.Y + dy),
                                lineSegment.IsStroked,
                                lineSegment.IsSmoothJoin);
                        }
                        else if (segment is PolyCubicBezierSegment polyCubicBezierSegment)
                        {
                            sgc.PolyBezierTo(
                                ToPoints(polyCubicBezierSegment.Points, dx, dy),
                                polyCubicBezierSegment.IsStroked,
                                polyCubicBezierSegment.IsSmoothJoin);
                        }
                        else if (segment is PolyLineSegment polyLineSegment)
                        {
                            sgc.PolyLineTo(
                                ToPoints(polyLineSegment.Points, dx, dy),
                                polyLineSegment.IsStroked,
                                polyLineSegment.IsSmoothJoin);
                        }
                        else if (segment is PolyQuadraticBezierSegment polyQuadraticSegment)
                        {
                            sgc.PolyQuadraticBezierTo(
                                ToPoints(polyQuadraticSegment.Points, dx, dy),
                                polyQuadraticSegment.IsStroked,
                                polyQuadraticSegment.IsSmoothJoin);
                        }
                        else if (segment is QuadraticBezierSegment quadraticBezierSegment)
                        {
                            sgc.QuadraticBezierTo(
                                new W.Point(quadraticBezierSegment.Point1.X + dx, quadraticBezierSegment.Point1.Y + dy),
                                new W.Point(quadraticBezierSegment.Point2.X + dx, quadraticBezierSegment.Point2.Y + dy),
                                quadraticBezierSegment.IsStroked,
                                quadraticBezierSegment.IsSmoothJoin);
                        }
                        else
                        {
                            throw new NotSupportedException("Not supported segment type: " + segment.GetType());
                        }
                    }
                }
            }

            sg.FillRule = xpg.FillRule == FillRule.Nonzero ? WM.FillRule.Nonzero : WM.FillRule.EvenOdd;
            sg.Freeze();

            return sg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static PathGeometry ToPathGeometry(this string source)
        {
            var g = WM.Geometry.Parse(source);
            var pg = WM.PathGeometry.CreateFromGeometry(g);
            return ToPathGeometry(pg, 0.0, 0.0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpg"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static WM.PathGeometry ToPathGeometry(this PathGeometry xpg, double dx, double dy)
        {
            return WM.PathGeometry.CreateFromGeometry(ToStreamGeometry(xpg, dx, dy));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpg"></param>
        /// <returns></returns>
        public static string ToSource(this PathGeometry xpg)
        {
            return ToStreamGeometry(xpg, 0.0, 0.0).ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sg"></param>
        /// <returns></returns>
        public static string ToSource(this WM.StreamGeometry sg)
        {
            return sg.ToString(CultureInfo.InvariantCulture);
        }
    }
}
