using System;
using System.Linq;
using Core2D.Path;
using Core2D.Path.Segments;
using D2D = System.Drawing.Drawing2D;

namespace Core2D.Renderer.WinForms
{
    public static class PathGeometryConverter
    {
        public static D2D.GraphicsPath ToGraphicsPath(this IPathGeometry pg, Func<double, float> scale)
        {
            var gp = new D2D.GraphicsPath
            {
                FillMode = pg.FillRule == FillRule.EvenOdd ? D2D.FillMode.Alternate : D2D.FillMode.Winding
            };

            foreach (var pf in pg.Figures)
            {
                var startPoint = pf.StartPoint;

                foreach (var segment in pf.Segments)
                {
                    if (segment is IArcSegment arcSegment)
                    {
                        // TODO: Convert WPF/SVG elliptical arc segment format to GDI+ bezier curves.
                        startPoint = arcSegment.Point;
                    }
                    else if (segment is ICubicBezierSegment cubicBezierSegment)
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
                    else if (segment is ILineSegment lineSegment)
                    {
                        gp.AddLine(
                            scale(startPoint.X),
                            scale(startPoint.Y),
                            scale(lineSegment.Point.X),
                            scale(lineSegment.Point.Y));
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
