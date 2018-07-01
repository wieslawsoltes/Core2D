// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Shapes;
using A=Avalonia;
using AM=Avalonia.Media;

namespace Core2D.Renderer.Avalonia
{
    /// <summary>
    /// 
    /// </summary>
    public static class PathGeometryConverter
    {
        private static ImmutableArray<PointShape> ToPointShapes(this IEnumerable<A.Point> points, double dx, double dy)
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
        public static PathGeometry ToPathGeometry(this AM.PathGeometry pg, double dx, double dy)
        {
            var geometry = PathGeometry.Create(
                ImmutableArray.Create<PathFigure>(),
                pg.FillRule == AM.FillRule.EvenOdd ? FillRule.EvenOdd : FillRule.Nonzero);

            var context = new PathGeometryContext(geometry);

            foreach (var pf in pg.Figures)
            {
                context.BeginFigure(
                    PointShape.Create(pf.StartPoint.X + dx, pf.StartPoint.Y + dy),
                    pf.IsFilled,
                    pf.IsClosed);

                foreach (var segment in pf.Segments)
                {
                    if (segment is AM.ArcSegment arcSegment)
                    {
                        context.ArcTo(
                            PointShape.Create(arcSegment.Point.X + dx, arcSegment.Point.Y + dy),
                            PathSize.Create(arcSegment.Size.Width, arcSegment.Size.Height),
                            arcSegment.RotationAngle,
                            arcSegment.IsLargeArc,
                            arcSegment.SweepDirection == AM.SweepDirection.Clockwise ? SweepDirection.Clockwise : SweepDirection.Counterclockwise);
                    }
                    else if (segment is AM.BezierSegment cubicBezierSegment)
                    {
                        context.CubicBezierTo(
                            PointShape.Create(cubicBezierSegment.Point1.X + dx, cubicBezierSegment.Point1.Y + dy),
                            PointShape.Create(cubicBezierSegment.Point2.X + dx, cubicBezierSegment.Point2.Y + dy),
                            PointShape.Create(cubicBezierSegment.Point3.X + dx, cubicBezierSegment.Point3.Y + dy));
                    }
                    else if (segment is AM.LineSegment lineSegment)
                    {
                        context.LineTo(
                            PointShape.Create(lineSegment.Point.X + dx, lineSegment.Point.Y + dy));
                    }
                    else if (segment is AM.QuadraticBezierSegment quadraticBezierSegment)
                    {
                        context.QuadraticBezierTo(
                            PointShape.Create(quadraticBezierSegment.Point1.X + dx, quadraticBezierSegment.Point1.Y + dy),
                            PointShape.Create(quadraticBezierSegment.Point2.X + dx, quadraticBezierSegment.Point2.Y + dy));
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
        public static AM.StreamGeometry ToStreamGeometry(this PathGeometry xpg, double dx, double dy)
        {
            var sg = new AM.StreamGeometry();

            using (var sgc = sg.Open())
            {
                var previous = default(PointShape);

                sgc.SetFillRule(xpg.FillRule == FillRule.Nonzero ? AM.FillRule.NonZero : AM.FillRule.EvenOdd);

                foreach (var xpf in xpg.Figures)
                {
                    sgc.BeginFigure(new A.Point(xpf.StartPoint.X + dx, xpf.StartPoint.Y + dy), xpf.IsFilled);

                    previous = xpf.StartPoint;

                    foreach (var segment in xpf.Segments)
                    {
                        if (segment is ArcSegment arcSegment)
                        {
                            sgc.ArcTo(
                                new A.Point(arcSegment.Point.X + dx, arcSegment.Point.Y + dy),
                                new A.Size(arcSegment.Size.Width,  arcSegment.Size.Height),
                                arcSegment.RotationAngle,
                                arcSegment.IsLargeArc,
                                arcSegment.SweepDirection == SweepDirection.Clockwise ? AM.SweepDirection.Clockwise : AM.SweepDirection.CounterClockwise);

                            previous = arcSegment.Point;
                        }
                        else if (segment is CubicBezierSegment cubicBezierSegment)
                        {
                            sgc.CubicBezierTo(
                                new A.Point(cubicBezierSegment.Point1.X + dx, cubicBezierSegment.Point1.Y + dy),
                                new A.Point(cubicBezierSegment.Point2.X + dx, cubicBezierSegment.Point2.Y + dy),
                                new A.Point(cubicBezierSegment.Point3.X + dx, cubicBezierSegment.Point3.Y + dy));

                            previous = cubicBezierSegment.Point3;
                        }
                        else if (segment is LineSegment lineSegment)
                        {
                            sgc.LineTo(
                                new A.Point(lineSegment.Point.X + dx, lineSegment.Point.Y + dy));

                            previous = lineSegment.Point;
                        }
                        else if (segment is PolyCubicBezierSegment polyCubicBezierSegment)
                        {
                            if (polyCubicBezierSegment.Points.Length >= 3)
                            {
                                sgc.CubicBezierTo(
                                    new A.Point(
                                        polyCubicBezierSegment.Points[0].X + dx,
                                        polyCubicBezierSegment.Points[0].Y + dy),
                                    new A.Point(
                                        polyCubicBezierSegment.Points[1].X + dx,
                                        polyCubicBezierSegment.Points[1].Y + dy),
                                    new A.Point(
                                        polyCubicBezierSegment.Points[2].X + dx,
                                        polyCubicBezierSegment.Points[2].Y + dy));

                                previous = polyCubicBezierSegment.Points[2];
                            }

                            if (polyCubicBezierSegment.Points.Length > 3
                                && polyCubicBezierSegment.Points.Length % 3 == 0)
                            {
                                for (int i = 3; i < polyCubicBezierSegment.Points.Length; i += 3)
                                {
                                    sgc.CubicBezierTo(
                                        new A.Point(
                                            polyCubicBezierSegment.Points[i].X + dx,
                                            polyCubicBezierSegment.Points[i].Y + dy),
                                        new A.Point(
                                            polyCubicBezierSegment.Points[i + 1].X + dx,
                                            polyCubicBezierSegment.Points[i + 1].Y + dy),
                                        new A.Point(
                                            polyCubicBezierSegment.Points[i + 2].X + dx,
                                            polyCubicBezierSegment.Points[i + 2].Y + dy));

                                    previous = polyCubicBezierSegment.Points[i + 2];
                                }
                            }
                        }
                        else if (segment is PolyLineSegment polyLineSegment)
                        {
                            if (polyLineSegment.Points.Length >= 1)
                            {
                                sgc.LineTo(
                                    new A.Point(
                                        polyLineSegment.Points[0].X + dx,
                                        polyLineSegment.Points[0].Y + dy));

                                previous = polyLineSegment.Points[0];
                            }

                            if (polyLineSegment.Points.Length > 1)
                            {
                                for (int i = 1; i < polyLineSegment.Points.Length; i++)
                                {
                                    sgc.LineTo(
                                        new A.Point(
                                            polyLineSegment.Points[i].X + dx,
                                            polyLineSegment.Points[i].Y + dy));

                                    previous = polyLineSegment.Points[i];
                                }
                            }
                        }
                        else if (segment is PolyQuadraticBezierSegment polyQuadraticSegment)
                        {
                            if (polyQuadraticSegment.Points.Length >= 2)
                            {
                                sgc.QuadraticBezierTo(
                                    new A.Point(
                                        polyQuadraticSegment.Points[0].X + dx,
                                        polyQuadraticSegment.Points[0].Y + dy),
                                    new A.Point(
                                        polyQuadraticSegment.Points[1].X + dx,
                                        polyQuadraticSegment.Points[1].Y + dy));

                                previous = polyQuadraticSegment.Points[1];
                            }

                            if (polyQuadraticSegment.Points.Length > 2
                                && polyQuadraticSegment.Points.Length % 2 == 0)
                            {
                                for (int i = 3; i < polyQuadraticSegment.Points.Length; i += 3)
                                {
                                    sgc.QuadraticBezierTo(
                                        new A.Point(
                                            polyQuadraticSegment.Points[i].X + dx,
                                            polyQuadraticSegment.Points[i].Y + dy),
                                        new A.Point(
                                            polyQuadraticSegment.Points[i + 1].X + dx,
                                            polyQuadraticSegment.Points[i + 1].Y + dy));

                                    previous = polyQuadraticSegment.Points[i + 1];
                                }
                            }
                        }
                        else if (segment is QuadraticBezierSegment quadraticBezierSegment)
                        {
                            sgc.QuadraticBezierTo(
                                new A.Point(
                                    quadraticBezierSegment.Point1.X + dx,
                                    quadraticBezierSegment.Point1.Y + dy),
                                new A.Point(
                                    quadraticBezierSegment.Point2.X + dx,
                                    quadraticBezierSegment.Point2.Y + dy));

                            previous = quadraticBezierSegment.Point2;
                        }
                        else
                        {
                            throw new NotSupportedException("Not supported segment type: " + segment.GetType());
                        }
                    }

                    sgc.EndFigure(xpf.IsClosed);
                }
            }

            return sg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static PathGeometry ToPathGeometry(this string source)
        {
            var pg = AM.PathGeometry.Parse(source);
            return ToPathGeometry(pg, 0.0, 0.0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpg"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static AM.Geometry ToGeometry(this PathGeometry xpg, double dx, double dy)
        {
            return ToStreamGeometry(xpg, dx, dy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpg"></param>
        /// <returns></returns>
        public static string ToSource(this PathGeometry xpg)
        {
            return ToStreamGeometry(xpg, 0.0, 0.0).ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sg"></param>
        /// <returns></returns>
        public static string ToSource(this AM.StreamGeometry sg)
        {
            return sg.ToString();
        }
    }
}
