// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Avalonia;
using Avalonia.Media;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Shapes;

namespace Core2D.Renderer.Avalonia
{
    /// <summary>
    /// 
    /// </summary>
    public static class PathGeometryConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpg"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static StreamGeometry ToStreamGeometry(this Path.PathGeometry xpg, double dx, double dy)
        {
            var sg = new StreamGeometry();

            using (var sgc = sg.Open())
            {
                var previous = default(PointShape);

                sgc.SetFillRule(xpg.FillRule == Path.FillRule.Nonzero ? global::Avalonia.Media.FillRule.NonZero : global::Avalonia.Media.FillRule.EvenOdd);

                foreach (var xpf in xpg.Figures)
                {
                    sgc.BeginFigure(new Point(xpf.StartPoint.X + dx, xpf.StartPoint.Y + dy), xpf.IsFilled);

                    previous = xpf.StartPoint;

                    foreach (var segment in xpf.Segments)
                    {
                        if (segment is Path.Segments.ArcSegment)
                        {
                            var arcSegment = segment as Path.Segments.ArcSegment;
                            sgc.ArcTo(
                                new Point(arcSegment.Point.X + dx, arcSegment.Point.Y + dy),
                                new Size(arcSegment.Size.Width,  arcSegment.Size.Height),
                                arcSegment.RotationAngle,
                                arcSegment.IsLargeArc,
                                arcSegment.SweepDirection == Path.SweepDirection.Clockwise ? global::Avalonia.Media.SweepDirection.Clockwise : global::Avalonia.Media.SweepDirection.CounterClockwise);

                            previous = arcSegment.Point;
                        }
                        else if (segment is CubicBezierSegment)
                        {
                            var cubicBezierSegment = segment as CubicBezierSegment;
                            sgc.CubicBezierTo(
                                new Point(cubicBezierSegment.Point1.X + dx, cubicBezierSegment.Point1.Y + dy),
                                new Point(cubicBezierSegment.Point2.X + dx, cubicBezierSegment.Point2.Y + dy),
                                new Point(cubicBezierSegment.Point3.X + dx, cubicBezierSegment.Point3.Y + dy));

                            previous = cubicBezierSegment.Point3;
                        }
                        else if (segment is Path.Segments.LineSegment)
                        {
                            var lineSegment = segment as Path.Segments.LineSegment;
                            sgc.LineTo(
                                new Point(lineSegment.Point.X + dx, lineSegment.Point.Y + dy));

                            previous = lineSegment.Point;
                        }
                        else if (segment is PolyCubicBezierSegment)
                        {
                            var polyCubicBezierSegment = segment as PolyCubicBezierSegment;
                            if (polyCubicBezierSegment.Points.Length >= 3)
                            {
                                sgc.CubicBezierTo(
                                    new Point(
                                        polyCubicBezierSegment.Points[0].X + dx,
                                        polyCubicBezierSegment.Points[0].Y + dy),
                                    new Point(
                                        polyCubicBezierSegment.Points[1].X + dx,
                                        polyCubicBezierSegment.Points[1].Y + dy),
                                    new Point(
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
                                        new Point(
                                            polyCubicBezierSegment.Points[i].X + dx,
                                            polyCubicBezierSegment.Points[i].Y + dy),
                                        new Point(
                                            polyCubicBezierSegment.Points[i + 1].X + dx,
                                            polyCubicBezierSegment.Points[i + 1].Y + dy),
                                        new Point(
                                            polyCubicBezierSegment.Points[i + 2].X + dx,
                                            polyCubicBezierSegment.Points[i + 2].Y + dy));

                                    previous = polyCubicBezierSegment.Points[i + 2];
                                }
                            }
                        }
                        else if (segment is PolyLineSegment)
                        {
                            var polyLineSegment = segment as PolyLineSegment;
                            if (polyLineSegment.Points.Length >= 1)
                            {
                                sgc.LineTo(
                                    new Point(
                                        polyLineSegment.Points[0].X + dx,
                                        polyLineSegment.Points[0].Y + dy));

                                previous = polyLineSegment.Points[0];
                            }

                            if (polyLineSegment.Points.Length > 1)
                            {
                                for (int i = 1; i < polyLineSegment.Points.Length; i++)
                                {
                                    sgc.LineTo(
                                        new Point(
                                            polyLineSegment.Points[i].X + dx,
                                            polyLineSegment.Points[i].Y + dy));

                                    previous = polyLineSegment.Points[i];
                                }
                            }
                        }
                        else if (segment is PolyQuadraticBezierSegment)
                        {
                            var polyQuadraticSegment = segment as PolyQuadraticBezierSegment;
                            if (polyQuadraticSegment.Points.Length >= 2)
                            {
                                sgc.QuadraticBezierTo(
                                    new Point(
                                        polyQuadraticSegment.Points[0].X + dx,
                                        polyQuadraticSegment.Points[0].Y + dy),
                                    new Point(
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
                                        new Point(
                                            polyQuadraticSegment.Points[i].X + dx,
                                            polyQuadraticSegment.Points[i].Y + dy),
                                        new Point(
                                            polyQuadraticSegment.Points[i + 1].X + dx,
                                            polyQuadraticSegment.Points[i + 1].Y + dy));

                                    previous = polyQuadraticSegment.Points[i + 1];
                                }
                            }
                        }
                        else if (segment is Path.Segments.QuadraticBezierSegment)
                        {
                            var quadraticBezierSegment = segment as Path.Segments.QuadraticBezierSegment;
                            sgc.QuadraticBezierTo(
                                new Point(
                                    quadraticBezierSegment.Point1.X + dx,
                                    quadraticBezierSegment.Point1.Y + dy),
                                new Point(
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
        /// <param name="xpg"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static Geometry ToGeometry(this Path.PathGeometry xpg, double dx, double dy)
        {
            return ToStreamGeometry(xpg, dx, dy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpg"></param>
        /// <returns></returns>
        public static string ToSource(this Path.PathGeometry xpg)
        {
            return ToStreamGeometry(xpg, 0.0, 0.0).ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sg"></param>
        /// <returns></returns>
        public static string ToSource(this StreamGeometry sg)
        {
            return sg.ToString();
        }
    }
}
