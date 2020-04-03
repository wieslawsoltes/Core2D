using System;
using System.Linq;
using Core2D.Path;
using Core2D.Path.Segments;
using PDF = PdfSharp.Drawing;

namespace Core2D.Renderer.PdfSharp
{
    public static class PathGeometryConverter
    {
        public static PDF.XGraphicsPath ToXGraphicsPath(this IPathGeometry pg, double dx, double dy, Func<double, double> scale)
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
                    if (segment is IArcSegment arcSegment)
                    {
#if WPF
                        var point1 = new PDF.XPoint(
                            scale(startPoint.X + dx),
                            scale(startPoint.Y + dy));
                        var point2 = new PDF.XPoint(
                            scale(arcSegment.Point.X + dx),
                            scale(arcSegment.Point.Y + dy));
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
                    else if (segment is ICubicBezierSegment cubicBezierSegment)
                    {
                        gp.AddBezier(
                            scale(startPoint.X + dx),
                            scale(startPoint.Y + dy),
                            scale(cubicBezierSegment.Point1.X + dx),
                            scale(cubicBezierSegment.Point1.Y + dy),
                            scale(cubicBezierSegment.Point2.X + dx),
                            scale(cubicBezierSegment.Point2.Y + dy),
                            scale(cubicBezierSegment.Point3.X + dx),
                            scale(cubicBezierSegment.Point3.Y + dy));
                        startPoint = cubicBezierSegment.Point3;
                    }
                    else if (segment is ILineSegment)
                    {
                        var lineSegment = segment as ILineSegment;
                        gp.AddLine(
                            scale(startPoint.X + dx),
                            scale(startPoint.Y + dy),
                            scale(lineSegment.Point.X + dx),
                            scale(lineSegment.Point.Y + dy));
                        startPoint = lineSegment.Point;
                    }
                    else if (segment is IQuadraticBezierSegment quadraticBezierSegment)
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
                            scale(x1 + dx),
                            scale(y1 + dy),
                            scale(x2 + dx),
                            scale(y2 + dy),
                            scale(x3 + dx),
                            scale(y3 + dy),
                            scale(x4 + dx),
                            scale(y4 + dy));
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
