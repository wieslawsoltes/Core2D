#nullable disable
using System;
using System.Collections.Immutable;
using Core2D.Model;
using Core2D.Model.Path;
using Core2D.ViewModels.Path;
using Core2D.ViewModels.Path.Segments;
using Core2D.ViewModels.Shapes;
using Core2D.Spatial;
using Core2D.Spatial.Arc;
using A = Avalonia;
using AM = Avalonia.Media;

namespace Core2D.Modules.Renderer
{
    public static class PathGeometryConverter
    {
        public static AM.StreamGeometry ToGeometry(PathGeometryViewModel path)
        {
            var geometry = new AM.StreamGeometry();
            using var context = geometry.Open();
            context.SetFillRule(path.FillRule == FillRule.Nonzero ? AM.FillRule.NonZero : AM.FillRule.EvenOdd);

            foreach (var figure in path.Figures)
            {
                context.BeginFigure(new A.Point(figure.StartPoint.X, figure.StartPoint.Y), false);

                foreach (var segment in figure.Segments)
                {
                    if (segment is ArcSegmentViewModel arcSegment)
                    {
                        context.ArcTo(
                            new A.Point(arcSegment.Point.X, arcSegment.Point.Y),
                            new A.Size(arcSegment.Size.Width, arcSegment.Size.Height),
                            arcSegment.RotationAngle,
                            arcSegment.IsLargeArc,
                            arcSegment.SweepDirection == SweepDirection.Clockwise ? AM.SweepDirection.Clockwise : AM.SweepDirection.CounterClockwise);
                    }
                    else if (segment is CubicBezierSegmentViewModel cubicBezierSegment)
                    {
                        context.CubicBezierTo(
                            new A.Point(cubicBezierSegment.Point1.X, cubicBezierSegment.Point1.Y),
                            new A.Point(cubicBezierSegment.Point2.X, cubicBezierSegment.Point2.Y),
                            new A.Point(cubicBezierSegment.Point3.X, cubicBezierSegment.Point3.Y));
                    }
                    else if (segment is LineSegmentViewModel lineSegment)
                    {
                        context.LineTo(
                            new A.Point(lineSegment.Point.X, lineSegment.Point.Y));
                    }
                    else if (segment is QuadraticBezierSegmentViewModel quadraticBezierSegment)
                    {
                        context.QuadraticBezierTo(
                            new A.Point(
                                quadraticBezierSegment.Point1.X,
                                quadraticBezierSegment.Point1.Y),
                            new A.Point(
                                quadraticBezierSegment.Point2.X,
                                quadraticBezierSegment.Point2.Y));
                    }
                    else
                    {
                        throw new NotSupportedException("Not supported segment type: " + segment.GetType());
                    }
                }

                context.EndFigure(figure.IsClosed);
            }

            return geometry;
        }

        public static AM.Geometry ToGeometry(EllipseShapeViewModel ellipse)
        {
            var rect2 = Rect2.FromPoints(ellipse.TopLeft.X, ellipse.TopLeft.Y, ellipse.BottomRight.X, ellipse.BottomRight.Y);
            var rect = new A.Rect(rect2.X, rect2.Y, rect2.Width, rect2.Height);
            var geometry = new AM.EllipseGeometry(rect);
            return geometry;
        }

        public static AM.Geometry ToGeometry(ArcShapeViewModel arc)
        {
            var geometry = new AM.StreamGeometry();
            using var context = geometry.Open();
            var wpfArc = new WpfArc(
                Point2.FromXY(arc.Point1.X, arc.Point1.Y),
                Point2.FromXY(arc.Point2.X, arc.Point2.Y),
                Point2.FromXY(arc.Point3.X, arc.Point3.Y),
                Point2.FromXY(arc.Point4.X, arc.Point4.Y));
            context.BeginFigure(
                new A.Point(wpfArc.Start.X, wpfArc.Start.Y),
                arc.IsFilled);
            context.ArcTo(
                new A.Point(wpfArc.End.X, wpfArc.End.Y),
                new A.Size(wpfArc.Radius.Width, wpfArc.Radius.Height),
                0.0,
                wpfArc.IsLargeArc,
                AM.SweepDirection.Clockwise);
            context.EndFigure(false);
            return geometry;
        }

        public static AM.Geometry ToGeometry(CubicBezierShapeViewModel cubicBezier)
        {
            var geometry = new AM.StreamGeometry();
            using var context = geometry.Open();
            context.BeginFigure(
                new A.Point(cubicBezier.Point1.X, cubicBezier.Point1.Y),
                cubicBezier.IsFilled);
            context.CubicBezierTo(
                new A.Point(cubicBezier.Point2.X, cubicBezier.Point2.Y),
                new A.Point(cubicBezier.Point3.X, cubicBezier.Point3.Y),
                new A.Point(cubicBezier.Point4.X, cubicBezier.Point4.Y));
            context.EndFigure(false);
            return geometry;
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
    }
}
