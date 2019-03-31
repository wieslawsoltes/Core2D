// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using Core2D.Interfaces;
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
    public class PathGeometryConverter
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathGeometryConverter"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public PathGeometryConverter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private ImmutableArray<IPointShape> ToPointShapes(IEnumerable<W.Point> points, double dx, double dy)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var pointShapes = ImmutableArray.CreateBuilder<IPointShape>();
            foreach (var point in points)
            {
                pointShapes.Add(factory.CreatePointShape(point.X + dx, point.Y + dy));
            }
            return pointShapes.ToImmutable();
        }

        private IList<W.Point> ToPoints(IList<IPointShape> pointShapes, double dx, double dy)
        {
            var points = new List<W.Point>();
            foreach (var point in pointShapes)
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
        public IPathGeometry ToPathGeometry(WM.PathGeometry pg, double dx, double dy)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var geometry = factory.CreatePathGeometry(
                ImmutableArray.Create<IPathFigure>(),
                pg.FillRule == WM.FillRule.EvenOdd ? FillRule.EvenOdd : FillRule.Nonzero);

            var context = new PathGeometryContext(factory, geometry);

            foreach (var pf in pg.Figures)
            {
                context.BeginFigure(
                    factory.CreatePointShape(pf.StartPoint.X + dx, pf.StartPoint.Y + dy),
                    pf.IsFilled,
                    pf.IsClosed);

                foreach (var segment in pf.Segments)
                {
                    if (segment is WM.ArcSegment arcSegment)
                    {
                        context.ArcTo(
                            factory.CreatePointShape(arcSegment.Point.X + dx, arcSegment.Point.Y + dy),
                            factory.CreatePathSize(arcSegment.Size.Width, arcSegment.Size.Height),
                            arcSegment.RotationAngle,
                            arcSegment.IsLargeArc,
                            arcSegment.SweepDirection == WM.SweepDirection.Clockwise ? SweepDirection.Clockwise : SweepDirection.Counterclockwise,
                            arcSegment.IsStroked,
                            arcSegment.IsSmoothJoin);
                    }
                    else if (segment is WM.BezierSegment cubicBezierSegment)
                    {
                        context.CubicBezierTo(
                            factory.CreatePointShape(cubicBezierSegment.Point1.X + dx, cubicBezierSegment.Point1.Y + dy),
                            factory.CreatePointShape(cubicBezierSegment.Point2.X + dx, cubicBezierSegment.Point2.Y + dy),
                            factory.CreatePointShape(cubicBezierSegment.Point3.X + dx, cubicBezierSegment.Point3.Y + dy),
                            cubicBezierSegment.IsStroked,
                            cubicBezierSegment.IsSmoothJoin);
                    }
                    else if (segment is WM.LineSegment lineSegment)
                    {
                        context.LineTo(
                            factory.CreatePointShape(lineSegment.Point.X + dx, lineSegment.Point.Y + dy),
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
                            factory.CreatePointShape(quadraticBezierSegment.Point1.X + dx, quadraticBezierSegment.Point1.Y + dy),
                            factory.CreatePointShape(quadraticBezierSegment.Point2.X + dx, quadraticBezierSegment.Point2.Y + dy),
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
        public WM.StreamGeometry ToStreamGeometry(IPathGeometry xpg, double dx, double dy)
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
                        if (segment is IArcSegment arcSegment)
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
                        else if (segment is ICubicBezierSegment cubicBezierSegment)
                        {
                            sgc.BezierTo(
                                new W.Point(cubicBezierSegment.Point1.X + dx, cubicBezierSegment.Point1.Y + dy),
                                new W.Point(cubicBezierSegment.Point2.X + dx, cubicBezierSegment.Point2.Y + dy),
                                new W.Point(cubicBezierSegment.Point3.X + dx, cubicBezierSegment.Point3.Y + dy),
                                cubicBezierSegment.IsStroked,
                                cubicBezierSegment.IsSmoothJoin);
                        }
                        else if (segment is ILineSegment lineSegment)
                        {
                            sgc.LineTo(
                                new W.Point(lineSegment.Point.X + dx, lineSegment.Point.Y + dy),
                                lineSegment.IsStroked,
                                lineSegment.IsSmoothJoin);
                        }
                        else if (segment is IPolyCubicBezierSegment polyCubicBezierSegment)
                        {
                            sgc.PolyBezierTo(
                                ToPoints(polyCubicBezierSegment.Points, dx, dy),
                                polyCubicBezierSegment.IsStroked,
                                polyCubicBezierSegment.IsSmoothJoin);
                        }
                        else if (segment is IPolyLineSegment polyLineSegment)
                        {
                            sgc.PolyLineTo(
                                ToPoints(polyLineSegment.Points, dx, dy),
                                polyLineSegment.IsStroked,
                                polyLineSegment.IsSmoothJoin);
                        }
                        else if (segment is IPolyQuadraticBezierSegment polyQuadraticSegment)
                        {
                            sgc.PolyQuadraticBezierTo(
                                ToPoints(polyQuadraticSegment.Points, dx, dy),
                                polyQuadraticSegment.IsStroked,
                                polyQuadraticSegment.IsSmoothJoin);
                        }
                        else if (segment is IQuadraticBezierSegment quadraticBezierSegment)
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
        public IPathGeometry ToPathGeometry(string source)
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
        public WM.PathGeometry ToPathGeometry(IPathGeometry xpg, double dx, double dy)
        {
            return WM.PathGeometry.CreateFromGeometry(ToStreamGeometry(xpg, dx, dy));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpg"></param>
        /// <returns></returns>
        public string ToSource(IPathGeometry xpg)
        {
            return ToStreamGeometry(xpg, 0.0, 0.0).ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sg"></param>
        /// <returns></returns>
        public string ToSource(WM.StreamGeometry sg)
        {
            return sg.ToString(CultureInfo.InvariantCulture);
        }
    }
}
