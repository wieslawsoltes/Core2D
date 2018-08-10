// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Path;
using Core2D.Path.Segments;
using SkiaSharp;

namespace Core2D.Renderer.SkiaSharp
{
    /// <summary>
    /// 
    /// </summary>
    public static class PathGeometryConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpg"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static SKPath ToSKPath(this PathGeometry xpg, double dx, double dy, Func<double, float> scale)
        {
            var path = new SKPath
            {
                FillType = xpg.FillRule == FillRule.EvenOdd ? SKPathFillType.EvenOdd : SKPathFillType.Winding
            };

            foreach (var xpf in xpg.Figures)
            {
                var previous = default(PointShape);

                // Begin new figure.
                path.MoveTo(
                    scale(xpf.StartPoint.X + dx),
                    scale(xpf.StartPoint.Y + dy));

                previous = xpf.StartPoint;

                foreach (var segment in xpf.Segments)
                {
                    if (segment is ArcSegment arcSegment)
                    {
                        path.ArcTo(
                            scale(arcSegment.Size.Width),
                            scale(arcSegment.Size.Height), 
                            (float)arcSegment.RotationAngle, 
                            arcSegment.IsLargeArc ? SKPathArcSize.Large : SKPathArcSize.Small,
                            arcSegment.SweepDirection == SweepDirection.Clockwise ? SKPathDirection.Clockwise : SKPathDirection.CounterClockwise,
                            scale(arcSegment.Point.X + dx),
                            scale(arcSegment.Point.Y + dy));

                        previous = arcSegment.Point;
                    }
                    else if (segment is CubicBezierSegment cubicBezierSegment)
                    {
                        path.CubicTo(
                            scale(cubicBezierSegment.Point1.X + dx),
                            scale(cubicBezierSegment.Point1.Y + dy),
                            scale(cubicBezierSegment.Point2.X + dx),
                            scale(cubicBezierSegment.Point2.Y + dy),
                            scale(cubicBezierSegment.Point3.X + dx),
                            scale(cubicBezierSegment.Point3.Y + dy));

                        previous = cubicBezierSegment.Point3;
                    }
                    else if (segment is LineSegment lineSegment)
                    {
                        path.LineTo(
                            scale(lineSegment.Point.X + dx),
                            scale(lineSegment.Point.Y + dy));

                        previous = lineSegment.Point;
                    }
                    else if (segment is PolyCubicBezierSegment polyCubicBezierSegment)
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

                            previous = polyCubicBezierSegment.Points[2];
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

                                previous = polyCubicBezierSegment.Points[i + 2];
                            }
                        }
                    }
                    else if (segment is PolyLineSegment polyLineSegment)
                    {
                        if (polyLineSegment.Points.Length >= 1)
                        {
                            path.LineTo(
                                scale(polyLineSegment.Points[0].X + dx),
                                scale(polyLineSegment.Points[0].Y + dy));

                            previous = polyLineSegment.Points[0];
                        }

                        if (polyLineSegment.Points.Length > 1)
                        {
                            for (int i = 1; i < polyLineSegment.Points.Length; i++)
                            {
                                path.LineTo(
                                    scale(polyLineSegment.Points[i].X + dx),
                                    scale(polyLineSegment.Points[i].Y + dy));

                                previous = polyLineSegment.Points[i];
                            }
                        }
                    }
                    else if (segment is PolyQuadraticBezierSegment polyQuadraticSegment)
                    {
                        if (polyQuadraticSegment.Points.Length >= 2)
                        {
                            path.QuadTo(
                                scale(polyQuadraticSegment.Points[0].X + dx),
                                scale(polyQuadraticSegment.Points[0].Y + dy),
                                scale(polyQuadraticSegment.Points[1].X + dx),
                                scale(polyQuadraticSegment.Points[1].Y + dy));

                            previous = polyQuadraticSegment.Points[1];
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

                                previous = polyQuadraticSegment.Points[i + 1];
                            }
                        }
                    }
                    else if (segment is QuadraticBezierSegment quadraticBezierSegment)
                    {
                        path.QuadTo(
                            scale(quadraticBezierSegment.Point1.X + dx),
                            scale(quadraticBezierSegment.Point1.Y + dy),
                            scale(quadraticBezierSegment.Point2.X + dx),
                            scale(quadraticBezierSegment.Point2.Y + dy));

                        previous = quadraticBezierSegment.Point2;
                    }
                    else
                    {
                        throw new NotSupportedException("Not supported segment type: " + segment.GetType());
                    }
                }

                if (xpf.IsClosed)
                {
                    path.Close();
                }
            }

            return path;
        }
    }
}
