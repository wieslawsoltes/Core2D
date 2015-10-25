// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                        else if (segment is XBezierSegment)
                        {
                            var bezierSegment = segment as XBezierSegment;
                            sgc.BezierTo(
                                new Point(bezierSegment.Point1.X, bezierSegment.Point1.Y),
                                new Point(bezierSegment.Point2.X, bezierSegment.Point2.Y),
                                new Point(bezierSegment.Point3.X, bezierSegment.Point3.Y));

                            previous = bezierSegment.Point3;
                        }
                        else if (segment is XLineSegment)
                        {
                            var lineSegment = segment as XLineSegment;
                            sgc.LineTo(
                                new Point(lineSegment.Point.X, lineSegment.Point.Y));

                            previous = lineSegment.Point;
                        }
                        else if (segment is XPolyBezierSegment)
                        {
                            var polyBezierSegment = segment as XPolyBezierSegment;
                            if (polyBezierSegment.Points.Count >= 3)
                            {
                                sgc.BezierTo(
                                    new Point(
                                        polyBezierSegment.Points[0].X,
                                        polyBezierSegment.Points[0].Y),
                                    new Point(
                                        polyBezierSegment.Points[1].X,
                                        polyBezierSegment.Points[1].Y),
                                    new Point(
                                        polyBezierSegment.Points[2].X,
                                        polyBezierSegment.Points[2].Y));

                                previous = polyBezierSegment.Points[2];
                            }

                            if (polyBezierSegment.Points.Count > 3
                                && polyBezierSegment.Points.Count % 3 == 0)
                            {
                                for (int i = 3; i < polyBezierSegment.Points.Count; i += 3)
                                {
                                    sgc.BezierTo(
                                        new Point(
                                            polyBezierSegment.Points[i].X,
                                            polyBezierSegment.Points[i].Y),
                                        new Point(
                                            polyBezierSegment.Points[i + 1].X,
                                            polyBezierSegment.Points[i + 1].Y),
                                        new Point(
                                            polyBezierSegment.Points[i + 2].X,
                                            polyBezierSegment.Points[i + 2].Y));

                                    previous = polyBezierSegment.Points[i + 2];
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
                                var p1 = previous;
                                var p2 = polyQuadraticSegment.Points[0];
                                var p3 = polyQuadraticSegment.Points[1];
                                double x1 = p1.X;
                                double y1 = p1.Y;
                                double x2 = p1.X + (2.0 * (p2.X - p1.X)) / 3.0;
                                double y2 = p1.Y + (2.0 * (p2.Y - p1.Y)) / 3.0;
                                double x3 = x2 + (p3.X - p1.X) / 3.0;
                                double y3 = y2 + (p3.Y - p1.Y) / 3.0;
                                double x4 = p3.X;
                                double y4 = p3.Y;
                                sgc.BezierTo(
                                    new Point(x2, y2),
                                    new Point(x3, y3),
                                    new Point(x4, y4));

                                previous = polyQuadraticSegment.Points[1];
                            }

                            if (polyQuadraticSegment.Points.Count > 2
                                && polyQuadraticSegment.Points.Count % 2 == 0)
                            {
                                for (int i = 3; i < polyQuadraticSegment.Points.Count; i += 3)
                                {
                                    var p1 = polyQuadraticSegment.Points[i - 1];
                                    var p2 = polyQuadraticSegment.Points[i];
                                    var p3 = polyQuadraticSegment.Points[i + 1];
                                    double x1 = p1.X;
                                    double y1 = p1.Y;
                                    double x2 = p1.X + (2.0 * (p2.X - p1.X)) / 3.0;
                                    double y2 = p1.Y + (2.0 * (p2.Y - p1.Y)) / 3.0;
                                    double x3 = x2 + (p3.X - p1.X) / 3.0;
                                    double y3 = y2 + (p3.Y - p1.Y) / 3.0;
                                    double x4 = p3.X;
                                    double y4 = p3.Y;
                                    sgc.BezierTo(
                                        new Point(x2, y2),
                                        new Point(x3, y3),
                                        new Point(x4, y4));

                                    previous = polyQuadraticSegment.Points[i + 1];
                                }
                            }
                        }
                        else if (segment is XQuadraticBezierSegment)
                        {
                            var qbezierSegment = segment as XQuadraticBezierSegment;
                            var p1 = previous;
                            var p2 = qbezierSegment.Point1;
                            var p3 = qbezierSegment.Point2;
                            double x1 = p1.X;
                            double y1 = p1.Y;
                            double x2 = p1.X + (2.0 * (p2.X - p1.X)) / 3.0;
                            double y2 = p1.Y + (2.0 * (p2.Y - p1.Y)) / 3.0;
                            double x3 = x2 + (p3.X - p1.X) / 3.0;
                            double y3 = y2 + (p3.Y - p1.Y) / 3.0;
                            double x4 = p3.X;
                            double y4 = p3.Y;
                            sgc.BezierTo(
                                new Point(x2, y2),
                                new Point(x3, y3),
                                new Point(x4, y4));

                            previous = qbezierSegment.Point2;
                        }
                        else
                        {
                            throw new NotSupportedException("Not supported segment type: " + segment.GetType());
                        }
                    }

                    sgc.EndFigure(xpf.IsClosed);
                }
            }

            // TODO: Perspex has not yet implemented FillRule.
            //sg.FillRule = xpg.FillRule == XFillRule.Nonzero ? FillRule.Nonzero : FillRule.EvenOdd;

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
