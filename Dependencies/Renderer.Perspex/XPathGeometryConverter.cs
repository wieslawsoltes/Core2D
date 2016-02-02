// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Perspex;
using Perspex.Media;
using Core2D;

namespace Dependencies
{
    /// <summary>
    /// 
    /// </summary>
    public static class XPathGeometryConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpg"></param>
        /// <returns></returns>
        public static StreamGeometry ToStreamGeometry(this XPathGeometry xpg)
        {
            var sg = new StreamGeometry();

            using (var sgc = sg.Open())
            {
                var previous = default(XPoint);

                sgc.SetFillRule(xpg.FillRule == XFillRule.Nonzero ? FillRule.NonZero : FillRule.EvenOdd);

                foreach (var xpf in xpg.Figures)
                {
                    sgc.BeginFigure(
                        new Point(xpf.StartPoint.X, xpf.StartPoint.Y),
                        xpf.IsFilled);

                    previous = xpf.StartPoint;

                    foreach (var segment in xpf.Segments)
                    {
                        if (segment is XArcSegment)
                        {
                            var arcSegment = segment as XArcSegment;
                            sgc.ArcTo(
                                new Point(arcSegment.Point.X, arcSegment.Point.Y),
                                new Size(arcSegment.Size.Width, arcSegment.Size.Height),
                                arcSegment.RotationAngle,
                                arcSegment.IsLargeArc,
                                arcSegment.SweepDirection == XSweepDirection.Clockwise ? SweepDirection.Clockwise : SweepDirection.CounterClockwise);

                            previous = arcSegment.Point;
                        }
                        else if (segment is XCubicBezierSegment)
                        {
                            var cubicBezierSegment = segment as XCubicBezierSegment;
                            sgc.CubicBezierTo(
                                new Point(cubicBezierSegment.Point1.X, cubicBezierSegment.Point1.Y),
                                new Point(cubicBezierSegment.Point2.X, cubicBezierSegment.Point2.Y),
                                new Point(cubicBezierSegment.Point3.X, cubicBezierSegment.Point3.Y));

                            previous = cubicBezierSegment.Point3;
                        }
                        else if (segment is XLineSegment)
                        {
                            var lineSegment = segment as XLineSegment;
                            sgc.LineTo(
                                new Point(lineSegment.Point.X, lineSegment.Point.Y));

                            previous = lineSegment.Point;
                        }
                        else if (segment is XPolyCubicBezierSegment)
                        {
                            var polyCubicBezierSegment = segment as XPolyCubicBezierSegment;
                            if (polyCubicBezierSegment.Points.Count >= 3)
                            {
                                sgc.CubicBezierTo(
                                    new Point(
                                        polyCubicBezierSegment.Points[0].X,
                                        polyCubicBezierSegment.Points[0].Y),
                                    new Point(
                                        polyCubicBezierSegment.Points[1].X,
                                        polyCubicBezierSegment.Points[1].Y),
                                    new Point(
                                        polyCubicBezierSegment.Points[2].X,
                                        polyCubicBezierSegment.Points[2].Y));

                                previous = polyCubicBezierSegment.Points[2];
                            }

                            if (polyCubicBezierSegment.Points.Count > 3
                                && polyCubicBezierSegment.Points.Count % 3 == 0)
                            {
                                for (int i = 3; i < polyCubicBezierSegment.Points.Count; i += 3)
                                {
                                    sgc.CubicBezierTo(
                                        new Point(
                                            polyCubicBezierSegment.Points[i].X,
                                            polyCubicBezierSegment.Points[i].Y),
                                        new Point(
                                            polyCubicBezierSegment.Points[i + 1].X,
                                            polyCubicBezierSegment.Points[i + 1].Y),
                                        new Point(
                                            polyCubicBezierSegment.Points[i + 2].X,
                                            polyCubicBezierSegment.Points[i + 2].Y));

                                    previous = polyCubicBezierSegment.Points[i + 2];
                                }
                            }
                        }
                        else if (segment is XPolyLineSegment)
                        {
                            var polyLineSegment = segment as XPolyLineSegment;
                            if (polyLineSegment.Points.Count >= 1)
                            {
                                sgc.LineTo(
                                    new Point(
                                        polyLineSegment.Points[0].X,
                                        polyLineSegment.Points[0].Y));

                                previous = polyLineSegment.Points[0];
                            }

                            if (polyLineSegment.Points.Count > 1)
                            {
                                for (int i = 1; i < polyLineSegment.Points.Count; i++)
                                {
                                    sgc.LineTo(
                                        new Point(
                                            polyLineSegment.Points[i].X,
                                            polyLineSegment.Points[i].Y));

                                    previous = polyLineSegment.Points[i];
                                }
                            }
                        }
                        else if (segment is XPolyQuadraticBezierSegment)
                        {
                            var polyQuadraticSegment = segment as XPolyQuadraticBezierSegment;
                            if (polyQuadraticSegment.Points.Count >= 2)
                            {
                                sgc.QuadraticBezierTo(
                                    new Point(
                                        polyQuadraticSegment.Points[0].X,
                                        polyQuadraticSegment.Points[0].Y),
                                    new Point(
                                        polyQuadraticSegment.Points[1].X,
                                        polyQuadraticSegment.Points[1].Y));

                                previous = polyQuadraticSegment.Points[1];
                            }

                            if (polyQuadraticSegment.Points.Count > 2
                                && polyQuadraticSegment.Points.Count % 2 == 0)
                            {
                                for (int i = 3; i < polyQuadraticSegment.Points.Count; i += 3)
                                {
                                    sgc.QuadraticBezierTo(
                                        new Point(
                                            polyQuadraticSegment.Points[i].X,
                                            polyQuadraticSegment.Points[i].Y),
                                        new Point(
                                            polyQuadraticSegment.Points[i + 1].X,
                                            polyQuadraticSegment.Points[i + 1].Y));

                                    previous = polyQuadraticSegment.Points[i + 1];
                                }
                            }
                        }
                        else if (segment is XQuadraticBezierSegment)
                        {
                            var quadraticBezierSegment = segment as XQuadraticBezierSegment;
                            sgc.QuadraticBezierTo(
                                new Point(
                                    quadraticBezierSegment.Point1.X,
                                    quadraticBezierSegment.Point1.Y),
                                new Point(
                                    quadraticBezierSegment.Point2.X,
                                    quadraticBezierSegment.Point2.Y));

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
        /// <returns></returns>
        public static Geometry ToGeometry(this XPathGeometry xpg)
        {
            return ToStreamGeometry(xpg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpg"></param>
        /// <returns></returns>
        public static string ToSource(this XPathGeometry xpg)
        {
            return ToStreamGeometry(xpg).ToString();
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
