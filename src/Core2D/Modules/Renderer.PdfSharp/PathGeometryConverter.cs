using System;
using System.Linq;
using Core2D.Path;
using Core2D.Path.Segments;
using PDF = PdfSharp.Drawing;

namespace Core2D.Renderer.PdfSharp
{
    public static class PathGeometryConverter
    {
        public static PDF.XGraphicsPath ToXGraphicsPath(this PathGeometryViewModel pg, Func<double, double> scale)
        {
            var gp = new PDF.XGraphicsPath()
            {
                FillMode = pg.FillRule == FillRule.EvenOdd ? PDF.XFillMode.Alternate : PDF.XFillMode.Winding
            };

            foreach (var pf in pg.Figures)
            {
                var startPoint = pf.StartPoint;

                foreach (var segment in pf.Segments)
                {
                    if (segment is ArcSegmentViewModel arcSegment)
                    {
#if WPF
                        var point1 = new PDF.XPoint(
                            scale(startPoint.X),
                            scale(startPoint.Y));
                        var point2 = new PDF.XPoint(
                            scale(arcSegment.Point.X),
                            scale(arcSegment.Point.Y));
                        var size = new PDF.XSize(
                            scale(arcSegment.Size.Width),
                            scale(arcSegment.Size.Height));
                        gp.AddArc(
                            point1,
                            point2,
                            size, arcSegment.RotationAngle, arcSegment.IsLargeArc,
                            arcSegment.SweepDirection == SweepDirection.Clockwise ? PDF.XSweepDirection.Clockwise : PDF.XSweepDirection.Counterclockwise);
                        startPoint = arcSegment.Point;
#else
                        // TODO: Convert WPF/SVG elliptical arc segment format to GDI+ bezier curves.
                        startPoint = arcSegment.Point;
#endif
                    }
                    else if (segment is CubicBezierSegmentViewModel cubicBezierSegment)
                    {
                        gp.AddBezier(
                            scale(startPoint.X),
                            scale(startPoint.Y),
                            scale(cubicBezierSegment.Point1.X),
                            scale(cubicBezierSegment.Point1.Y),
                            scale(cubicBezierSegment.Point2.X),
                            scale(cubicBezierSegment.Point2.Y),
                            scale(cubicBezierSegment.Point3.X),
                            scale(cubicBezierSegment.Point3.Y));
                        startPoint = cubicBezierSegment.Point3;
                    }
                    else if (segment is LineSegmentViewModel)
                    {
                        var lineSegment = segment as LineSegmentViewModel;
                        gp.AddLine(
                            scale(startPoint.X),
                            scale(startPoint.Y),
                            scale(lineSegment.Point.X),
                            scale(lineSegment.Point.Y));
                        startPoint = lineSegment.Point;
                    }
                    else if (segment is QuadraticBezierSegmentViewModel quadraticBezierSegment)
                    {
                        var p1 = startPoint;
                        var p2 = quadraticBezierSegment.Point1;
                        var p3 = quadraticBezierSegment.Point2;
                        double x1 = p1.X;
                        double y1 = p1.Y;
                        double x2 = p1.X + (2.0 * (p2.X - p1.X)) / 3.0;
                        double y2 = p1.Y + (2.0 * (p2.Y - p1.Y)) / 3.0;
                        double x3 = x2 + (p3.X - p1.X) / 3.0;
                        double y3 = y2 + (p3.Y - p1.Y) / 3.0;
                        double x4 = p3.X;
                        double y4 = p3.Y;
                        gp.AddBezier(
                            scale(x1),
                            scale(y1),
                            scale(x2),
                            scale(y2),
                            scale(x3),
                            scale(y3),
                            scale(x4),
                            scale(y4));
                        startPoint = quadraticBezierSegment.Point2;
                    }
                    else
                    {
                        throw new NotSupportedException("Not supported segment type: " + segment.GetType());
                    }
                }

                if (pf.IsClosed)
                {
                    gp.CloseFigure();
                }
                else
                {
                    gp.StartFigure();
                }
            }

            return gp;
        }
    }
}
