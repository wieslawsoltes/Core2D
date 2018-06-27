// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Path.Segments;
using Core2D.Shapes;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Core2D.Renderer.Wpf
{
    /// <summary>
    /// 
    /// </summary>
    public static class WpfPathGeometryConverter
    {
        private static IList<Point> ToPoints(this IList<PointShape> PointShapes, double dx, double dy)
        {
            var points = new List<Point>();
            foreach (var point in PointShapes)
            {
                points.Add(new Point(point.X + dx, point.Y + dy));
            }
            return points;
        }

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
            var sgc = sg.Open();

            foreach (var xpf in xpg.Figures)
            {
                sgc.BeginFigure(
                    new Point(xpf.StartPoint.X, xpf.StartPoint.Y),
                    xpf.IsFilled,
                    xpf.IsClosed);

                foreach (var segment in xpf.Segments)
                {
                    if (segment is Path.Segments.ArcSegment)
                    {
                        var arcSegment = segment as Path.Segments.ArcSegment;
                        sgc.ArcTo(
                            new Point(arcSegment.Point.X + dx, arcSegment.Point.Y + dy),
                            new Size(arcSegment.Size.Width, arcSegment.Size.Height),
                            arcSegment.RotationAngle,
                            arcSegment.IsLargeArc,
                            arcSegment.SweepDirection == Path.SweepDirection.Clockwise ? SweepDirection.Clockwise : SweepDirection.Counterclockwise,
                            arcSegment.IsStroked,
                            arcSegment.IsSmoothJoin);
                    }
                    else if (segment is CubicBezierSegment)
                    {
                        var cubicBezierSegment = segment as CubicBezierSegment;
                        sgc.BezierTo(
                            new Point(cubicBezierSegment.Point1.X + dx, cubicBezierSegment.Point1.Y + dy),
                            new Point(cubicBezierSegment.Point2.X + dx, cubicBezierSegment.Point2.Y + dy),
                            new Point(cubicBezierSegment.Point3.X + dx, cubicBezierSegment.Point3.Y + dy),
                            cubicBezierSegment.IsStroked,
                            cubicBezierSegment.IsSmoothJoin);
                    }
                    else if (segment is Path.Segments.LineSegment)
                    {
                        var lineSegment = segment as Path.Segments.LineSegment;
                        sgc.LineTo(
                            new Point(lineSegment.Point.X + dx, lineSegment.Point.Y + dy),
                            lineSegment.IsStroked,
                            lineSegment.IsSmoothJoin);
                    }
                    else if (segment is PolyCubicBezierSegment)
                    {
                        var polyCubicBezierSegment = segment as PolyCubicBezierSegment;
                        sgc.PolyBezierTo(
                            ToPoints(polyCubicBezierSegment.Points, dx, dy),
                            polyCubicBezierSegment.IsStroked,
                            polyCubicBezierSegment.IsSmoothJoin);
                    }
                    else if (segment is Path.Segments.PolyLineSegment)
                    {
                        var polyLineSegment = segment as Path.Segments.PolyLineSegment;
                        sgc.PolyLineTo(
                            ToPoints(polyLineSegment.Points, dx, dy),
                            polyLineSegment.IsStroked,
                            polyLineSegment.IsSmoothJoin);
                    }
                    else if (segment is Path.Segments.PolyQuadraticBezierSegment)
                    {
                        var polyQuadraticSegment = segment as Path.Segments.PolyQuadraticBezierSegment;
                        sgc.PolyQuadraticBezierTo(
                            ToPoints(polyQuadraticSegment.Points, dx, dy),
                            polyQuadraticSegment.IsStroked,
                            polyQuadraticSegment.IsSmoothJoin);
                    }
                    else if (segment is Path.Segments.QuadraticBezierSegment)
                    {
                        var quadraticBezierSegment = segment as Path.Segments.QuadraticBezierSegment;
                        sgc.QuadraticBezierTo(
                            new Point(quadraticBezierSegment.Point1.X + dx, quadraticBezierSegment.Point1.Y + dy),
                            new Point(quadraticBezierSegment.Point2.X + dx, quadraticBezierSegment.Point2.Y + dy),
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
            sg.FillRule = xpg.FillRule == Path.FillRule.Nonzero ? FillRule.Nonzero : FillRule.EvenOdd;
            sg.Freeze();

            return sg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpg"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static PathGeometry ToPathGeometry(this Path.PathGeometry xpg, double dx, double dy)
        {
            var sg = ToStreamGeometry(xpg, dx, dy);
            return PathGeometry.CreateFromGeometry(sg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpg"></param>
        /// <returns></returns>
        public static string ToSource(this Path.PathGeometry xpg)
        {
            var sg = ToStreamGeometry(xpg, 0.0, 0.0);
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
