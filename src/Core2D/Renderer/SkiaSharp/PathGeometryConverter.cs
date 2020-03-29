using System;
using Core2D.Path;
using Core2D.Path.Segments;
using SkiaSharp;

namespace Core2D.Renderer.SkiaSharp
{
    public static class PathGeometryConverter
    {
        public static SKPath ToSKPath(this IPathGeometry xpg, double dx, double dy, Func<double, float> scale)
        {
            var path = new SKPath
            {
                FillType = pathGeometry.FillRule == FillRule.EvenOdd ? SKPathFillType.EvenOdd : SKPathFillType.Winding
            };

            foreach (var pathFigure in pathGeometry.Figures)
            {
                path.MoveTo(
                    scale(pathFigure.StartPoint.X + dx),
                    scale(pathFigure.StartPoint.Y + dy));

                foreach (var segment in pathFigure.Segments)
                {
                    if (segment is IArcSegment arcSegment)
                    {
                        path.ArcTo(
                            scale(arcSegment.Size.Width),
                            scale(arcSegment.Size.Height),
                            (float)arcSegment.RotationAngle,
                            arcSegment.IsLargeArc ? SKPathArcSize.Large : SKPathArcSize.Small,
                            arcSegment.SweepDirection == SweepDirection.Clockwise ? SKPathDirection.Clockwise : SKPathDirection.CounterClockwise,
                            scale(arcSegment.Point.X + dx),
                            scale(arcSegment.Point.Y + dy));
                    }
                    else if (segment is ICubicBezierSegment cubicBezierSegment)
                    {
                        path.CubicTo(
                            scale(cubicBezierSegment.Point1.X + dx),
                            scale(cubicBezierSegment.Point1.Y + dy),
                            scale(cubicBezierSegment.Point2.X + dx),
                            scale(cubicBezierSegment.Point2.Y + dy),
                            scale(cubicBezierSegment.Point3.X + dx),
                            scale(cubicBezierSegment.Point3.Y + dy));
                    }
                    else if (segment is ILineSegment lineSegment)
                    {
                        path.LineTo(
                            scale(lineSegment.Point.X + dx),
                            scale(lineSegment.Point.Y + dy));
                    }
                    else if (segment is IPolyCubicBezierSegment polyCubicBezierSegment)
                    {
                        if (polyCubicBezierSegment.Points.Length >= 3)
                        {
                            path.CubicTo(
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
                                path.CubicTo(
                                    scale(polyCubicBezierSegment.Points[i].X + dx),
                                    scale(polyCubicBezierSegment.Points[i].Y + dy),
                                    scale(polyCubicBezierSegment.Points[i + 1].X + dx),
                                    scale(polyCubicBezierSegment.Points[i + 1].Y + dy),
                                    scale(polyCubicBezierSegment.Points[i + 2].X + dx),
                                    scale(polyCubicBezierSegment.Points[i + 2].Y + dy));
                            }
                        }
                    }
                    else if (segment is IPolyLineSegment polyLineSegment)
                    {
                        if (polyLineSegment.Points.Length >= 1)
                        {
                            path.LineTo(
                                scale(polyLineSegment.Points[0].X + dx),
                                scale(polyLineSegment.Points[0].Y + dy));
                        }

                        if (polyLineSegment.Points.Length > 1)
                        {
                            for (int i = 1; i < polyLineSegment.Points.Length; i++)
                            {
                                path.LineTo(
                                    scale(polyLineSegment.Points[i].X + dx),
                                    scale(polyLineSegment.Points[i].Y + dy));
                            }
                        }
                    }
                    else if (segment is IPolyQuadraticBezierSegment polyQuadraticSegment)
                    {
                        if (polyQuadraticSegment.Points.Length >= 2)
                        {
                            path.QuadTo(
                                scale(polyQuadraticSegment.Points[0].X + dx),
                                scale(polyQuadraticSegment.Points[0].Y + dy),
                                scale(polyQuadraticSegment.Points[1].X + dx),
                                scale(polyQuadraticSegment.Points[1].Y + dy));
                        }

                        if (polyQuadraticSegment.Points.Length > 2
                            && polyQuadraticSegment.Points.Length % 2 == 0)
                        {
                            for (int i = 3; i < polyQuadraticSegment.Points.Length; i += 3)
                            {
                                path.QuadTo(
                                    scale(polyQuadraticSegment.Points[i].X + dx),
                                    scale(polyQuadraticSegment.Points[i].Y + dy),
                                    scale(polyQuadraticSegment.Points[i + 1].X + dx),
                                    scale(polyQuadraticSegment.Points[i + 1].Y + dy));
                            }
                        }
                    }
                    else if (segment is IQuadraticBezierSegment quadraticBezierSegment)
                    {
                        path.QuadTo(
                            scale(quadraticBezierSegment.Point1.X + dx),
                            scale(quadraticBezierSegment.Point1.Y + dy),
                            scale(quadraticBezierSegment.Point2.X + dx),
                            scale(quadraticBezierSegment.Point2.Y + dy));
                    }
                    else
                    {
                        throw new NotSupportedException("Not supported segment type: " + segment.GetType());
                    }
                }

                if (pathFigure.IsClosed)
                {
                    path.Close();
                }
            }

            return path;
        }
    }
}
