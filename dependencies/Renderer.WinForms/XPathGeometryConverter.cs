// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Drawing.Drawing2D;
using System.Linq;

namespace Renderer.WinForms
{
    /// <summary>
    /// 
    /// </summary>
    public static class XPathGeometryConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pg"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static GraphicsPath ToGraphicsPath(this Core2D.Path.XPathGeometry pg, double dx, double dy, Func<double, float> scale)
        {
            var gp = new GraphicsPath();
            gp.FillMode = pg.FillRule == Core2D.Path.XFillRule.EvenOdd ? FillMode.Alternate : FillMode.Winding;

            foreach (var pf in pg.Figures)
            {
                var startPoint = pf.StartPoint;

                foreach (var segment in pf.Segments)
                {
                    if (segment is Core2D.Path.Segments.XArcSegment)
                    {
                        throw new NotSupportedException("Not supported segment type: " + segment.GetType());
                        //var arcSegment = segment as Core2D.XArcSegment;
                        // TODO: Convert WPF/SVG elliptical arc segment format to GDI+ bezier curves.
                        //startPoint = arcSegment.Point;
                    }
                    else if (segment is Core2D.Path.Segments.XCubicBezierSegment)
                    {
                        var cubicBezierSegment = segment as Core2D.Path.Segments.XCubicBezierSegment;
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
                    else if (segment is Core2D.Path.Segments.XLineSegment)
                    {
                        var lineSegment = segment as Core2D.Path.Segments.XLineSegment;
                        gp.AddLine(
                            scale(startPoint.X + dx),
                            scale(startPoint.Y + dy),
                            scale(lineSegment.Point.X + dx),
                            scale(lineSegment.Point.Y + dy));
                        startPoint = lineSegment.Point;
                    }
                    else if (segment is Core2D.Path.Segments.XPolyCubicBezierSegment)
                    {
                        var polyCubicBezierSegment = segment as Core2D.Path.Segments.XPolyCubicBezierSegment;
                        if (polyCubicBezierSegment.Points.Length >= 3)
                        {
                            gp.AddBezier(
                                scale(startPoint.X + dx),
                                scale(startPoint.Y + dy),
                                scale(polyCubicBezierSegment.Points[0].X + dx),
                                scale(polyCubicBezierSegment.Points[0].Y + dy),
                                scale(polyCubicBezierSegment.Points[1].X + dx),
                                scale(polyCubicBezierSegment.Points[1].Y + dy),
                                scale(polyCubicBezierSegment.Points[2].X + dx),
                                scale(polyCubicBezierSegment.Points[2].Y + dy));
                        }

                        if (polyCubicBezierSegment.Points.Length > 3
                            && polyCubicBezierSegment.Points.Length % 3 == 0)
                        {
                            for (int i = 3; i < polyCubicBezierSegment.Points.Length; i += 3)
                            {
                                gp.AddBezier(
                                    scale(polyCubicBezierSegment.Points[i - 1].X + dx),
                                    scale(polyCubicBezierSegment.Points[i - 1].Y + dy),
                                    scale(polyCubicBezierSegment.Points[i].X + dx),
                                    scale(polyCubicBezierSegment.Points[i].Y + dy),
                                    scale(polyCubicBezierSegment.Points[i + 1].X + dx),
                                    scale(polyCubicBezierSegment.Points[i + 1].Y + dy),
                                    scale(polyCubicBezierSegment.Points[i + 2].X + dx),
                                    scale(polyCubicBezierSegment.Points[i + 2].Y + dy));
                            }
                        }

                        startPoint = polyCubicBezierSegment.Points.Last();
                    }
                    else if (segment is Core2D.Path.Segments.XPolyLineSegment)
                    {
                        var polyLineSegment = segment as Core2D.Path.Segments.XPolyLineSegment;
                        if (polyLineSegment.Points.Length >= 1)
                        {
                            gp.AddLine(
                                scale(startPoint.X + dx),
                                scale(startPoint.Y + dy),
                                scale(polyLineSegment.Points[0].X + dx),
                                scale(polyLineSegment.Points[0].Y + dy));
                        }

                        if (polyLineSegment.Points.Length > 1)
                        {
                            for (int i = 1; i < polyLineSegment.Points.Length; i++)
                            {
                                gp.AddLine(
                                    scale(polyLineSegment.Points[i - 1].X + dx),
                                    scale(polyLineSegment.Points[i - 1].Y + dy),
                                    scale(polyLineSegment.Points[i].X + dx),
                                    scale(polyLineSegment.Points[i].Y + dy));
                            }
                        }

                        startPoint = polyLineSegment.Points.Last();
                    }
                    else if (segment is Core2D.Path.Segments.XPolyQuadraticBezierSegment)
                    {
                        var polyQuadraticSegment = segment as Core2D.Path.Segments.XPolyQuadraticBezierSegment;
                        if (polyQuadraticSegment.Points.Length >= 2)
                        {
                            var p1 = startPoint;
                            var p2 = polyQuadraticSegment.Points[0];
                            var p3 = polyQuadraticSegment.Points[1];
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
                        }

                        if (polyQuadraticSegment.Points.Length > 2
                            && polyQuadraticSegment.Points.Length % 2 == 0)
                        {
                            for (int i = 3; i < polyQuadraticSegment.Points.Length; i += 3)
                            {
                                var p1 = polyQuadraticSegment.Points[i - 1];
                                var p2 = polyQuadraticSegment.Points[i];
                                var p3 = polyQuadraticSegment.Points[i + 1];
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
                            }
                        }

                        startPoint = polyQuadraticSegment.Points.Last();
                    }
                    else if (segment is Core2D.Path.Segments.XQuadraticBezierSegment)
                    {
                        var quadraticBezierSegment = segment as Core2D.Path.Segments.XQuadraticBezierSegment;
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
