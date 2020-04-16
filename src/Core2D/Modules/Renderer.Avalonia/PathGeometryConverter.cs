using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Shapes;
using A = Avalonia;
using AM = Avalonia.Media;

namespace Core2D.Renderer.Avalonia
{
    public static class PathGeometryConverter
    {
        public static IPathGeometry ToPathGeometry(AM.PathGeometry pg, double dx, double dy, IFactory factory)
        {
            var geometry = factory.CreatePathGeometry(
                ImmutableArray.Create<IPathFigure>(),
                pg.FillRule == AM.FillRule.EvenOdd ? FillRule.EvenOdd : FillRule.Nonzero);

            var context = factory.CreateGeometryContext(geometry);

            foreach (var pf in pg.Figures)
            {
                context.BeginFigure(
                    factory.CreatePointShape(pf.StartPoint.X + dx, pf.StartPoint.Y + dy),
                    pf.IsFilled,
                    pf.IsClosed);

                foreach (var segment in pf.Segments)
                {
                    if (segment is AM.ArcSegment arcSegment)
                    {
                        context.ArcTo(
                            factory.CreatePointShape(arcSegment.Point.X + dx, arcSegment.Point.Y + dy),
                            factory.CreatePathSize(arcSegment.Size.Width, arcSegment.Size.Height),
                            arcSegment.RotationAngle,
                            arcSegment.IsLargeArc,
                            arcSegment.SweepDirection == AM.SweepDirection.Clockwise ? SweepDirection.Clockwise : SweepDirection.Counterclockwise);
                    }
                    else if (segment is AM.BezierSegment cubicBezierSegment)
                    {
                        context.CubicBezierTo(
                            factory.CreatePointShape(cubicBezierSegment.Point1.X + dx, cubicBezierSegment.Point1.Y + dy),
                            factory.CreatePointShape(cubicBezierSegment.Point2.X + dx, cubicBezierSegment.Point2.Y + dy),
                            factory.CreatePointShape(cubicBezierSegment.Point3.X + dx, cubicBezierSegment.Point3.Y + dy));
                    }
                    else if (segment is AM.LineSegment lineSegment)
                    {
                        context.LineTo(
                            factory.CreatePointShape(lineSegment.Point.X + dx, lineSegment.Point.Y + dy));
                    }
                    else if (segment is AM.QuadraticBezierSegment quadraticBezierSegment)
                    {
                        context.QuadraticBezierTo(
                            factory.CreatePointShape(quadraticBezierSegment.Point1.X + dx, quadraticBezierSegment.Point1.Y + dy),
                            factory.CreatePointShape(quadraticBezierSegment.Point2.X + dx, quadraticBezierSegment.Point2.Y + dy));
                    }
                    else
                    {
                        throw new NotSupportedException("Not supported segment type: " + segment.GetType());
                    }
                }
            }

            return geometry;
        }

        public static AM.StreamGeometry ToStreamGeometry(IPathGeometry xpg, double dx, double dy)
        {
            var sg = new AM.StreamGeometry();

            using (var sgc = sg.Open())
            {
                IPointShape previous = default;

                sgc.SetFillRule(xpg.FillRule == FillRule.Nonzero ? AM.FillRule.NonZero : AM.FillRule.EvenOdd);

                foreach (var xpf in xpg.Figures)
                {
                    sgc.BeginFigure(new A.Point(xpf.StartPoint.X + dx, xpf.StartPoint.Y + dy), xpf.IsFilled);

                    previous = xpf.StartPoint;

                    foreach (var segment in xpf.Segments)
                    {
                        if (segment is IArcSegment arcSegment)
                        {
                            sgc.ArcTo(
                                new A.Point(arcSegment.Point.X + dx, arcSegment.Point.Y + dy),
                                new A.Size(arcSegment.Size.Width, arcSegment.Size.Height),
                                arcSegment.RotationAngle,
                                arcSegment.IsLargeArc,
                                arcSegment.SweepDirection == SweepDirection.Clockwise ? AM.SweepDirection.Clockwise : AM.SweepDirection.CounterClockwise);

                            previous = arcSegment.Point;
                        }
                        else if (segment is ICubicBezierSegment cubicBezierSegment)
                        {
                            sgc.CubicBezierTo(
                                new A.Point(cubicBezierSegment.Point1.X + dx, cubicBezierSegment.Point1.Y + dy),
                                new A.Point(cubicBezierSegment.Point2.X + dx, cubicBezierSegment.Point2.Y + dy),
                                new A.Point(cubicBezierSegment.Point3.X + dx, cubicBezierSegment.Point3.Y + dy));

                            previous = cubicBezierSegment.Point3;
                        }
                        else if (segment is ILineSegment lineSegment)
                        {
                            sgc.LineTo(
                                new A.Point(lineSegment.Point.X + dx, lineSegment.Point.Y + dy));

                            previous = lineSegment.Point;
                        }
                        else if (segment is IQuadraticBezierSegment quadraticBezierSegment)
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

        public static IPathGeometry ToPathGeometry(string source, IFactory factory)
        {
            var pg = AM.PathGeometry.Parse(source);
            return ToPathGeometry(pg, 0.0, 0.0, factory);
        }

        public static AM.Geometry ToGeometry(IPathGeometry xpg, double dx, double dy)
        {
            return ToStreamGeometry(xpg, dx, dy);
        }

        public static string ToSource(IPathGeometry xpg)
        {
            return ToStreamGeometry(xpg, 0.0, 0.0).ToString();
        }

        public static string ToSource(AM.StreamGeometry sg)
        {
            return sg.ToString();
        }
    }
}
