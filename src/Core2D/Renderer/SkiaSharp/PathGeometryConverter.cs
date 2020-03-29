using System;
using System.Collections.Immutable;
using Core2D.Interfaces;
using Core2D.Path;
using Core2D.Path.Segments;
using SkiaSharp;

namespace Core2D.Renderer.SkiaSharp
{
    public static class PathGeometryConverter
    {
        public static IPathGeometry ToPathGeometry(SKPath path, double dx, double dy, IFactory factory)
        {
            var geometry = factory.CreatePathGeometry(
                ImmutableArray.Create<IPathFigure>(),
                path.FillType == SKPathFillType.EvenOdd ? FillRule.EvenOdd : FillRule.Nonzero);

            var context = factory.CreateGeometryContext(geometry);

            using (var iterator = path.CreateRawIterator())
            {
                var points = new SKPoint[4];
                var pathVerb = SKPathVerb.Move;

                while ((pathVerb = iterator.Next(points)) != SKPathVerb.Done)
                {
                    switch (pathVerb)
                    {
                        case SKPathVerb.Move:
                            {
                                context.BeginFigure(
                                    factory.CreatePointShape(points[0].X + dx, points[0].Y + dy),
                                    false,
                                    false);
                            }
                            break;
                        case SKPathVerb.Line:
                            {
                                context.LineTo(
                                    factory.CreatePointShape(points[1].X + dx, points[1].Y + dy));
                            }
                            break;
                        case SKPathVerb.Cubic:
                            {
                                context.CubicBezierTo(
                                    factory.CreatePointShape(points[1].X + dx, points[1].Y + dy),
                                    factory.CreatePointShape(points[2].X + dx, points[2].Y + dy),
                                    factory.CreatePointShape(points[3].X + dx, points[3].Y + dy));
                            }
                            break;
                        case SKPathVerb.Quad:
                            {
                                context.QuadraticBezierTo(
                                    factory.CreatePointShape(points[1].X + dx, points[1].Y + dy),
                                    factory.CreatePointShape(points[2].X + dx, points[2].Y + dy));
                            }
                            break;
                        case SKPathVerb.Conic:
                            {
                                var quads = SKPath.ConvertConicToQuads(points[0], points[1], points[2], iterator.ConicWeight(), 1);
                                context.QuadraticBezierTo(
                                    factory.CreatePointShape(quads[1].X + dx, quads[1].Y + dy),
                                    factory.CreatePointShape(quads[2].X + dx, quads[2].Y + dy));
                                context.QuadraticBezierTo(
                                    factory.CreatePointShape(quads[3].X + dx, quads[3].Y + dy),
                                    factory.CreatePointShape(quads[4].X + dx, quads[4].Y + dy));
                            }
                            break;
                        case SKPathVerb.Close:
                            {
                                context.SetClosedState(true);
                            }
                            break;
                    }
                }
            }

            return geometry;
        }

        public static SKPath ToSKPath(this IPathGeometry pathGeometry, double dx, double dy, Func<double, float> scale)
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
