// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Shapes;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

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
        /// <param name="xpoints"></param>
        /// <returns></returns>
        private static IList<Point> ToPoints(this IList<XPoint> xpoints)
        {
            var points = new List<Point>();
            foreach (var point in xpoints)
            {
                points.Add(new Point(point.X, point.Y));
            }
            return points;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpg"></param>
        /// <returns></returns>
        public static StreamGeometry ToStreamGeometry(this XPathGeometry xpg)
        {
            var sg = new StreamGeometry();
            var sgc = sg.Open();

            foreach (var xpf in xpg.Figures)
            {
                sgc.BeginFigure(
                    new Point(xpf.StartPoint.X, xpf.StartPoint.Y),
                    xpf.IsFilled,
                    xpf.IsClosed);

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
                            arcSegment.SweepDirection == XSweepDirection.Clockwise ? SweepDirection.Clockwise : SweepDirection.Counterclockwise,
                            arcSegment.IsStroked,
                            arcSegment.IsSmoothJoin);
                    }
                    else if (segment is XCubicBezierSegment)
                    {
                        var cubicBezierSegment = segment as XCubicBezierSegment;
                        sgc.BezierTo(
                            new Point(cubicBezierSegment.Point1.X, cubicBezierSegment.Point1.Y),
                            new Point(cubicBezierSegment.Point2.X, cubicBezierSegment.Point2.Y),
                            new Point(cubicBezierSegment.Point3.X, cubicBezierSegment.Point3.Y),
                            cubicBezierSegment.IsStroked,
                            cubicBezierSegment.IsSmoothJoin);
                    }
                    else if (segment is XLineSegment)
                    {
                        var lineSegment = segment as XLineSegment;
                        sgc.LineTo(
                            new Point(lineSegment.Point.X, lineSegment.Point.Y),
                            lineSegment.IsStroked,
                            lineSegment.IsSmoothJoin);
                    }
                    else if (segment is XPolyCubicBezierSegment)
                    {
                        var polyCubicBezierSegment = segment as XPolyCubicBezierSegment;
                        sgc.PolyBezierTo(
                            ToPoints(polyCubicBezierSegment.Points),
                            polyCubicBezierSegment.IsStroked,
                            polyCubicBezierSegment.IsSmoothJoin);
                    }
                    else if (segment is XPolyLineSegment)
                    {
                        var polyLineSegment = segment as XPolyLineSegment;
                        sgc.PolyLineTo(
                            ToPoints(polyLineSegment.Points),
                            polyLineSegment.IsStroked,
                            polyLineSegment.IsSmoothJoin);
                    }
                    else if (segment is XPolyQuadraticBezierSegment)
                    {
                        var polyQuadraticSegment = segment as XPolyQuadraticBezierSegment;
                        sgc.PolyQuadraticBezierTo(
                            ToPoints(polyQuadraticSegment.Points),
                            polyQuadraticSegment.IsStroked,
                            polyQuadraticSegment.IsSmoothJoin);
                    }
                    else if (segment is XQuadraticBezierSegment)
                    {
                        var quadraticBezierSegment = segment as XQuadraticBezierSegment;
                        sgc.QuadraticBezierTo(
                            new Point(quadraticBezierSegment.Point1.X, quadraticBezierSegment.Point1.Y),
                            new Point(quadraticBezierSegment.Point2.X, quadraticBezierSegment.Point2.Y),
                            quadraticBezierSegment.IsStroked,
                            quadraticBezierSegment.IsSmoothJoin);
                    }
                    else
                    {
                        throw new NotSupportedException("Not supported segment type: " + segment.GetType());
                    }
                }
            }

            sgc.Close();

            sg.FillRule = xpg.FillRule == XFillRule.Nonzero ? FillRule.Nonzero : FillRule.EvenOdd;
            sg.Freeze();

            return sg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpg"></param>
        /// <returns></returns>
        public static PathGeometry ToPathGeometry(this XPathGeometry xpg)
        {
            var sg = ToStreamGeometry(xpg);
            return PathGeometry.CreateFromGeometry(sg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpg"></param>
        /// <returns></returns>
        public static string ToSource(this XPathGeometry xpg)
        {
            var sg = ToStreamGeometry(xpg);
            return sg.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sg"></param>
        /// <returns></returns>
        public static string ToSource(this StreamGeometry sg)
        {
            return sg.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
