// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Test2d;

namespace Test
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
                    else if (segment is XBezierSegment)
                    {
                        var bezierSegment = segment as XBezierSegment;
                        sgc.BezierTo(
                            new Point(bezierSegment.Point1.X, bezierSegment.Point1.Y),
                            new Point(bezierSegment.Point2.X, bezierSegment.Point2.Y),
                            new Point(bezierSegment.Point3.X, bezierSegment.Point3.Y),
                            bezierSegment.IsStroked,
                            bezierSegment.IsSmoothJoin);
                    }
                    else if (segment is XLineSegment)
                    {
                        var lineSegment = segment as XLineSegment;
                        sgc.LineTo(
                            new Point(lineSegment.Point.X, lineSegment.Point.Y),
                            lineSegment.IsStroked,
                            lineSegment.IsSmoothJoin);
                    }
                    else if (segment is XPolyBezierSegment)
                    {
                        var polyBezierSegment = segment as XPolyBezierSegment;
                        sgc.PolyBezierTo(
                            ToPoints(polyBezierSegment.Points),
                            polyBezierSegment.IsStroked,
                            polyBezierSegment.IsSmoothJoin);
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
                        var qbezierSegment = segment as XQuadraticBezierSegment;
                        sgc.QuadraticBezierTo(
                            new Point(qbezierSegment.Point1.X, qbezierSegment.Point1.Y),
                            new Point(qbezierSegment.Point2.X, qbezierSegment.Point2.Y),
                            qbezierSegment.IsStroked,
                            qbezierSegment.IsSmoothJoin);
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
