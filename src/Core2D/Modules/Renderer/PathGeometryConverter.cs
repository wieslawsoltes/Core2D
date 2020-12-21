#nullable disable
using System;
using System.Collections.Immutable;
using Core2D.Model;
using Core2D.Model.Path;
using Core2D.ViewModels.Path;
using Core2D.ViewModels.Path.Segments;
using Core2D.ViewModels.Shapes;
using Spatial;
using Spatial.Arc;
using A = Avalonia;
using AM = Avalonia.Media;

namespace Core2D.Modules.Renderer
{
    public static class PathGeometryConverter
    {
        public static PathGeometryViewModel ToPathGeometry(AM.PathGeometry pg, IFactory factory)
        {
            var geometry = factory.CreatePathGeometry(
                ImmutableArray.Create<PathFigureViewModel>(),
                pg.FillRule == AM.FillRule.EvenOdd ? FillRule.EvenOdd : FillRule.Nonzero);

            var context = factory.CreateGeometryContext(geometry);

            foreach (var pf in pg.Figures)
            {
                context.BeginFigure(
                    factory.CreatePointShape(pf.StartPoint.X, pf.StartPoint.Y),
                    pf.IsClosed);

                foreach (var segment in pf.Segments)
                {
                    if (segment is AM.ArcSegment arcSegment)
                    {
                        context.ArcTo(
                            factory.CreatePointShape(arcSegment.Point.X, arcSegment.Point.Y),
                            factory.CreatePathSize(arcSegment.Size.Width, arcSegment.Size.Height),
                            arcSegment.RotationAngle,
                            arcSegment.IsLargeArc,
                            arcSegment.SweepDirection == AM.SweepDirection.Clockwise ? SweepDirection.Clockwise : SweepDirection.Counterclockwise);
                    }
                    else if (segment is AM.BezierSegment cubicBezierSegment)
                    {
                        context.CubicBezierTo(
                            factory.CreatePointShape(cubicBezierSegment.Point1.X, cubicBezierSegment.Point1.Y),
                            factory.CreatePointShape(cubicBezierSegment.Point2.X, cubicBezierSegment.Point2.Y),
                            factory.CreatePointShape(cubicBezierSegment.Point3.X, cubicBezierSegment.Point3.Y));
                    }
                    else if (segment is AM.LineSegment lineSegment)
                    {
                        context.LineTo(
                            factory.CreatePointShape(lineSegment.Point.X, lineSegment.Point.Y));
                    }
                    else if (segment is AM.QuadraticBezierSegment quadraticBezierSegment)
                    {
                        context.QuadraticBezierTo(
                            factory.CreatePointShape(quadraticBezierSegment.Point1.X, quadraticBezierSegment.Point1.Y),
                            factory.CreatePointShape(quadraticBezierSegment.Point2.X, quadraticBezierSegment.Point2.Y));
                    }
                    else
                    {
                        throw new NotSupportedException("Not supported segment type: " + segment.GetType());
                    }
                }
            }

            return geometry;
        }

        public static AM.StreamGeometry ToStreamGeometry(PathGeometryViewModel xpg)
        {
            var sg = new AM.StreamGeometry();

            using (var sgc = sg.Open())
            {
                PointShapeViewModel previous = default;

                sgc.SetFillRule(xpg.FillRule == FillRule.Nonzero ? AM.FillRule.NonZero : AM.FillRule.EvenOdd);

                foreach (var xpf in xpg.Figures)
                {
                    sgc.BeginFigure(new A.Point(xpf.StartPoint.X, xpf.StartPoint.Y), false);

                    previous = xpf.StartPoint;

                    foreach (var segment in xpf.Segments)
                    {
                        if (segment is ArcSegmentViewModel arcSegment)
                        {
                            sgc.ArcTo(
                                new A.Point(arcSegment.Point.X, arcSegment.Point.Y),
                                new A.Size(arcSegment.Size.Width, arcSegment.Size.Height),
                                arcSegment.RotationAngle,
                                arcSegment.IsLargeArc,
                                arcSegment.SweepDirection == SweepDirection.Clockwise ? AM.SweepDirection.Clockwise : AM.SweepDirection.CounterClockwise);

                            previous = arcSegment.Point;
                        }
                        else if (segment is CubicBezierSegmentViewModel cubicBezierSegment)
                        {
                            sgc.CubicBezierTo(
                                new A.Point(cubicBezierSegment.Point1.X, cubicBezierSegment.Point1.Y),
                                new A.Point(cubicBezierSegment.Point2.X, cubicBezierSegment.Point2.Y),
                                new A.Point(cubicBezierSegment.Point3.X, cubicBezierSegment.Point3.Y));

                            previous = cubicBezierSegment.Point3;
                        }
                        else if (segment is LineSegmentViewModel lineSegment)
                        {
                            sgc.LineTo(
                                new A.Point(lineSegment.Point.X, lineSegment.Point.Y));

                            previous = lineSegment.Point;
                        }
                        else if (segment is QuadraticBezierSegmentViewModel quadraticBezierSegment)
                        {
                            sgc.QuadraticBezierTo(
                                new A.Point(
                                    quadraticBezierSegment.Point1.X,
                                    quadraticBezierSegment.Point1.Y),
                                new A.Point(
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

        public static PathGeometryViewModel ToPathGeometry(string source, IFactory factory)
        {
            var pg = AM.PathGeometry.Parse(source);
            return ToPathGeometry(pg, factory);
        }

        public static AM.Geometry ToGeometry(PathGeometryViewModel xpg)
        {
            return ToStreamGeometry(xpg);
        }

        public static AM.Geometry ToGeometry(EllipseShapeViewModel ellipse)
        {
            var rect2 = Rect2.FromPoints(ellipse.TopLeft.X, ellipse.TopLeft.Y, ellipse.BottomRight.X, ellipse.BottomRight.Y);
            var rect = new A.Rect(rect2.X, rect2.Y, rect2.Width, rect2.Height);
            var g = new AM.EllipseGeometry(rect);
            return g;
        }

        public static AM.Geometry ToGeometry(ArcShapeViewModel arc)
        {
            var sg = new AM.StreamGeometry();
            using var sgc = sg.Open();
            var a = new WpfArc(
                Point2.FromXY(arc.Point1.X, arc.Point1.Y),
                Point2.FromXY(arc.Point2.X, arc.Point2.Y),
                Point2.FromXY(arc.Point3.X, arc.Point3.Y),
                Point2.FromXY(arc.Point4.X, arc.Point4.Y));
            sgc.BeginFigure(
                new A.Point(a.Start.X, a.Start.Y),
                arc.IsFilled);
            sgc.ArcTo(
                new A.Point(a.End.X, a.End.Y),
                new A.Size(a.Radius.Width, a.Radius.Height),
                0.0,
                a.IsLargeArc,
                AM.SweepDirection.Clockwise);
            sgc.EndFigure(false);
            return sg;
        }

        public static AM.Geometry ToGeometry(CubicBezierShapeViewModel cubicBezier)
        {
            var sg = new AM.StreamGeometry();
            using var sgc = sg.Open();
            sgc.BeginFigure(
                new A.Point(cubicBezier.Point1.X, cubicBezier.Point1.Y),
                cubicBezier.IsFilled);
            sgc.CubicBezierTo(
                new A.Point(cubicBezier.Point2.X, cubicBezier.Point2.Y),
                new A.Point(cubicBezier.Point3.X, cubicBezier.Point3.Y),
                new A.Point(cubicBezier.Point4.X, cubicBezier.Point4.Y));
            sgc.EndFigure(false);
            return sg;
        }

        public static AM.Geometry ToGeometry(QuadraticBezierShapeViewModel quadraticBezier)
        {
            var sg = new AM.StreamGeometry();
            using var sgc = sg.Open();
            sgc.BeginFigure(
                new A.Point(quadraticBezier.Point1.X, quadraticBezier.Point1.Y),
                quadraticBezier.IsFilled);
            sgc.QuadraticBezierTo(
                new A.Point(quadraticBezier.Point2.X, quadraticBezier.Point2.Y),
                new A.Point(quadraticBezier.Point3.X, quadraticBezier.Point3.Y));
            sgc.EndFigure(false);
            return sg;
        }

        public static string ToSource(PathGeometryViewModel xpg)
        {
            return ToStreamGeometry(xpg).ToString();
        }

        public static string ToSource(AM.StreamGeometry sg)
        {
            return sg.ToString();
        }
    }
}
