using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Model;
using Core2D.Model.Path;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Path;
using Core2D.ViewModels.Path.Segments;
using Core2D.ViewModels.Shapes;
using SkiaSharp;
using Spatial;
using Spatial.Arc;

namespace Core2D.Modules.Renderer.SkiaSharp
{
    public static class PathGeometryConverter
    {
        public static void CreateFigure(this PathFigureViewModel pathFigure, SKPath path)
        {
            path.MoveTo(
                (float)(pathFigure.StartPoint.X),
                (float)(pathFigure.StartPoint.Y));

            foreach (var segment in pathFigure.Segments)
            {
                if (segment is LineSegmentViewModel lineSegment)
                {
                    path.LineTo(
                        (float)(lineSegment.Point.X),
                        (float)(lineSegment.Point.Y));
                }
                else if (segment is QuadraticBezierSegmentViewModel quadraticBezierSegment)
                {
                    path.QuadTo(
                        (float)(quadraticBezierSegment.Point1.X),
                        (float)(quadraticBezierSegment.Point1.Y),
                        (float)(quadraticBezierSegment.Point2.X),
                        (float)(quadraticBezierSegment.Point2.Y));
                }
                else if (segment is CubicBezierSegmentViewModel cubicBezierSegment)
                {
                    path.CubicTo(
                        (float)(cubicBezierSegment.Point1.X),
                        (float)(cubicBezierSegment.Point1.Y),
                        (float)(cubicBezierSegment.Point2.X),
                        (float)(cubicBezierSegment.Point2.Y),
                        (float)(cubicBezierSegment.Point3.X),
                        (float)(cubicBezierSegment.Point3.Y));
                }
                else if (segment is ArcSegmentViewModel arcSegment)
                {
                    path.ArcTo(
                        (float)(arcSegment.Size.Width),
                        (float)(arcSegment.Size.Height),
                        (float)arcSegment.RotationAngle,
                        arcSegment.IsLargeArc ? SKPathArcSize.Large : SKPathArcSize.Small,
                        arcSegment.SweepDirection == SweepDirection.Clockwise ? SKPathDirection.Clockwise : SKPathDirection.CounterClockwise,
                        (float)(arcSegment.Point.X),
                        (float)(arcSegment.Point.Y));
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

        public static PathGeometryViewModel ToPathGeometry(SKPath path, IFactory factory)
        {
            var geometry = factory.CreatePathGeometry(
                ImmutableArray.Create<PathFigureViewModel>(),
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
                                    factory.CreatePointShape(points[0].X, points[0].Y),
                                    false);
                            }
                            break;

                        case SKPathVerb.Line:
                            {
                                context.LineTo(
                                    factory.CreatePointShape(points[1].X, points[1].Y));
                            }
                            break;

                        case SKPathVerb.Cubic:
                            {
                                context.CubicBezierTo(
                                    factory.CreatePointShape(points[1].X, points[1].Y),
                                    factory.CreatePointShape(points[2].X, points[2].Y),
                                    factory.CreatePointShape(points[3].X, points[3].Y));
                            }
                            break;

                        case SKPathVerb.Quad:
                            {
                                context.QuadraticBezierTo(
                                    factory.CreatePointShape(points[1].X, points[1].Y),
                                    factory.CreatePointShape(points[2].X, points[2].Y));
                            }
                            break;

                        case SKPathVerb.Conic:
                            {
                                var quads = SKPath.ConvertConicToQuads(points[0], points[1], points[2], iterator.ConicWeight(), 1);
                                context.QuadraticBezierTo(
                                    factory.CreatePointShape(quads[1].X, quads[1].Y),
                                    factory.CreatePointShape(quads[2].X, quads[2].Y));
                                context.QuadraticBezierTo(
                                    factory.CreatePointShape(quads[3].X, quads[3].Y),
                                    factory.CreatePointShape(quads[4].X, quads[4].Y));
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

        public static SKPath ToSKPath(this IEnumerable<BaseShapeViewModel> shapes)
        {
            var path = new SKPath
            {
                FillType = SKPathFillType.Winding
            };
            var previous = default(PointShapeViewModel);
            foreach (var shape in shapes)
            {
                switch (shape)
                {
                    case LineShapeViewModel lineShape:
                        {
                            if (previous == null || previous != lineShape.Start)
                            {
                                path.MoveTo(
                                    (float)(lineShape.Start.X),
                                    (float)(lineShape.Start.Y));
                            }
                            path.LineTo(
                                (float)(lineShape.End.X),
                                (float)(lineShape.End.Y));
                            previous = lineShape.End;
                        }
                        break;

                    case RectangleShapeViewModel rectangleShape:
                        {
                            path.AddRect(
                                SkiaSharpDrawUtil.CreateRect(rectangleShape.TopLeft, rectangleShape.BottomRight),
                                SKPathDirection.Clockwise);
                        }
                        break;

                    case EllipseShapeViewModel ellipseShape:
                        {
                            path.AddOval(
                                SkiaSharpDrawUtil.CreateRect(ellipseShape.TopLeft, ellipseShape.BottomRight),
                                SKPathDirection.Clockwise);
                        }
                        break;

                    case ArcShapeViewModelViewModel arcShape:
                        {
                            var a = new GdiArc(
                                Point2.FromXY(arcShape.Point1.X, arcShape.Point1.Y),
                                Point2.FromXY(arcShape.Point2.X, arcShape.Point2.Y),
                                Point2.FromXY(arcShape.Point3.X, arcShape.Point3.Y),
                                Point2.FromXY(arcShape.Point4.X, arcShape.Point4.Y));
                            var rect = new SKRect(
                                (float)(a.X),
                                (float)(a.Y),
                                (float)(a.X + a.Width),
                                (float)(a.Y + a.Height));
                            path.AddArc(rect, (float)a.StartAngle, (float)a.SweepAngle);
                        }
                        break;

                    case CubicBezierShapeViewModel cubicBezierShape:
                        {
                            if (previous == null || previous != cubicBezierShape.Point1)
                            {
                                path.MoveTo(
                                    (float)(cubicBezierShape.Point1.X),
                                    (float)(cubicBezierShape.Point1.Y));
                            }
                            path.CubicTo(
                                (float)(cubicBezierShape.Point2.X),
                                (float)(cubicBezierShape.Point2.Y),
                                (float)(cubicBezierShape.Point3.X),
                                (float)(cubicBezierShape.Point3.Y),
                                (float)(cubicBezierShape.Point4.X),
                                (float)(cubicBezierShape.Point4.Y));
                            previous = cubicBezierShape.Point4;
                        }
                        break;

                    case QuadraticBezierShapeViewModel quadraticBezierShape:
                        {
                            if (previous == null || previous != quadraticBezierShape.Point1)
                            {
                                path.MoveTo(
                                    (float)(quadraticBezierShape.Point1.X),
                                    (float)(quadraticBezierShape.Point1.Y));
                            }
                            path.QuadTo(
                                (float)(quadraticBezierShape.Point2.X),
                                (float)(quadraticBezierShape.Point2.Y),
                                (float)(quadraticBezierShape.Point3.X),
                                (float)(quadraticBezierShape.Point3.Y));
                            previous = quadraticBezierShape.Point3;
                        }
                        break;

                    case TextShapeViewModel textShape:
                        {
                            var resultPath = ToSKPath(textShape);
                            if (resultPath != null && !resultPath.IsEmpty)
                            {
                                path.AddPath(resultPath, SKPathAddMode.Append);
                            }
                        }
                        break;

                    case PathShapeViewModel pathShape:
                        {
                            var resultPath = ToSKPath(pathShape);
                            if (resultPath != null && !resultPath.IsEmpty)
                            {
                                path.AddPath(resultPath, SKPathAddMode.Append);
                            }
                        }
                        break;

                    case GroupShapeViewModel groupShape:
                        {
                            var resultPath = ToSKPath(groupShape.Shapes);
                            if (resultPath != null && !resultPath.IsEmpty)
                            {
                                path.AddPath(resultPath, SKPathAddMode.Append);
                            }
                        }
                        break;
                }
            }
            return path;
        }

        public static SKPath ToSKPath(this BaseShapeViewModel shape)
        {
            return shape switch
            {
                LineShapeViewModel lineShape => ToSKPath(lineShape),
                RectangleShapeViewModel rectangleShape => ToSKPath(rectangleShape),
                EllipseShapeViewModel ellipseShape => ToSKPath(ellipseShape),
                ImageShapeViewModel imageShape => ToSKPath(imageShape),
                ArcShapeViewModelViewModel arcShape => ToSKPath(arcShape),
                CubicBezierShapeViewModel cubicBezierShape => ToSKPath(cubicBezierShape),
                QuadraticBezierShapeViewModel quadraticBezierShape => ToSKPath(quadraticBezierShape),
                TextShapeViewModel textShape => ToSKPath(textShape),
                PathShapeViewModel pathShape => ToSKPath(pathShape),
                GroupShapeViewModel groupShape => ToSKPath(groupShape.Shapes),
                _ => null,
            };
        }

        public static SKPath ToSKPath(this LineShapeViewModel line)
        {
            var path = new SKPath
            {
                FillType = SKPathFillType.Winding
            };
            path.MoveTo(
                (float)(line.Start.X),
                (float)(line.Start.Y));
            path.LineTo(
                (float)(line.End.X),
                (float)(line.End.Y));
            return path;
        }

        public static SKPath ToSKPath(this RectangleShapeViewModel rectangle)
        {
            var path = new SKPath
            {
                FillType = SKPathFillType.Winding
            };
            path.AddRect(
                SkiaSharpDrawUtil.CreateRect(rectangle.TopLeft, rectangle.BottomRight),
                SKPathDirection.Clockwise);
            return path;
        }

        public static SKPath ToSKPath(this EllipseShapeViewModel ellipse)
        {
            var path = new SKPath
            {
                FillType = SKPathFillType.Winding
            };
            path.AddOval(
                SkiaSharpDrawUtil.CreateRect(ellipse.TopLeft, ellipse.BottomRight),
                SKPathDirection.Clockwise);
            return path;
        }

        public static SKPath ToSKPath(this ImageShapeViewModel image)
        {
            var path = new SKPath
            {
                FillType = SKPathFillType.Winding
            };
            path.AddRect(
                SkiaSharpDrawUtil.CreateRect(image.TopLeft, image.BottomRight),
                SKPathDirection.Clockwise);
            return path;
        }

        public static SKPath ToSKPath(this ArcShapeViewModelViewModel arc)
        {
            var path = new SKPath
            {
                FillType = SKPathFillType.Winding
            };
            var a = new GdiArc(
                Point2.FromXY(arc.Point1.X, arc.Point1.Y),
                Point2.FromXY(arc.Point2.X, arc.Point2.Y),
                Point2.FromXY(arc.Point3.X, arc.Point3.Y),
                Point2.FromXY(arc.Point4.X, arc.Point4.Y));
            var rect = new SKRect(
                (float)(a.X),
                (float)(a.Y),
                (float)(a.X + a.Width),
                (float)(a.Y + a.Height));
            path.AddArc(rect, (float)a.StartAngle, (float)a.SweepAngle);
            return path;
        }

        public static SKPath ToSKPath(this CubicBezierShapeViewModel cubicBezier)
        {
            var path = new SKPath
            {
                FillType = SKPathFillType.Winding
            };
            path.MoveTo(
                (float)(cubicBezier.Point1.X),
                (float)(cubicBezier.Point1.Y));
            path.CubicTo(
                (float)(cubicBezier.Point2.X),
                (float)(cubicBezier.Point2.Y),
                (float)(cubicBezier.Point3.X),
                (float)(cubicBezier.Point3.Y),
                (float)(cubicBezier.Point4.X),
                (float)(cubicBezier.Point4.Y));
            return path;
        }

        public static SKPath ToSKPath(this QuadraticBezierShapeViewModel quadraticBezier)
        {
            var path = new SKPath
            {
                FillType = SKPathFillType.Winding
            };
            path.MoveTo(
                (float)(quadraticBezier.Point1.X),
                (float)(quadraticBezier.Point1.Y));
            path.QuadTo(
                (float)(quadraticBezier.Point2.X),
                (float)(quadraticBezier.Point2.Y),
                (float)(quadraticBezier.Point3.X),
                (float)(quadraticBezier.Point3.Y));
            return path;
        }

        public static SKPath ToSKPath(this TextShapeViewModel text)
        {
            var path = new SKPath
            {
                FillType = SKPathFillType.Winding
            };

            if (!(text.GetProperty(nameof(TextShapeViewModel.Text)) is string tbind))
            {
                tbind = text.Text;
            }

            if (tbind == null)
            {
                return path;
            }

            using var pen = SkiaSharpDrawUtil.GetSKPaint(tbind, text.Style, text.TopLeft, text.BottomRight, out var origin);
            using var outlinePath = pen.GetTextPath(tbind, origin.X, origin.Y);
            using var fillPath = pen.GetFillPath(outlinePath);

            path.AddPath(fillPath, SKPathAddMode.Append);

            return path;
        }

        public static SKPath ToSKPath(this PathGeometryViewModel pathGeometryViewModel)
        {
            var fillType = pathGeometryViewModel.FillRule == FillRule.EvenOdd ? SKPathFillType.EvenOdd : SKPathFillType.Winding;
            var path = new SKPath
            {
                FillType = fillType
            };

            foreach (var pathFigure in pathGeometryViewModel.Figures)
            {
                CreateFigure(pathFigure, path);
            }

            return path;
        }

        public static SKPath ToSKPath(this PathShapeViewModel path)
        {
            return ToSKPath(path.Geometry);
        }

        public static SKPathOp ToSKPathOp(PathOp op)
        {
            return op switch
            {
                PathOp.Intersect => SKPathOp.Intersect,
                PathOp.Union => SKPathOp.Union,
                PathOp.Xor => SKPathOp.Xor,
                PathOp.ReverseDifference => SKPathOp.ReverseDifference,
                _ => SKPathOp.Difference,
            };
        }

        public static void Op(SKPath first, SKPath second, SKPathOp op, out SKPath result, out bool haveResult)
        {
            haveResult = false;
            result = new SKPath(first) { FillType = first.FillType };

            var next = result.Op(second, op);
            if (next != null)
            {
                result.Dispose();
                result = next;
                haveResult = true;
            }
        }

        public static void Op(IList<SKPath> paths, IList<SKPathOp> ops, out SKPath result, out bool haveResult)
        {
            using var builder = new SKPath.OpBuilder();

            for (int i = 0; i < paths.Count; i++)
            {
                builder.Add(paths[i], ops[i]);
            }

            result = new SKPath(paths[0]) { FillType = paths[0].FillType };
            haveResult = builder.Resolve(result);
        }

        public static void Op(IList<SKPath> paths, SKPathOp op, out SKPath result, out bool haveResult)
        {
            haveResult = false;
            result = new SKPath(paths[0]) { FillType = paths[0].FillType };

            if (paths.Count == 1)
            {
                using var empty = new SKPath() { FillType = paths[0].FillType };
                result = empty.Op(paths[0], op);
                haveResult = true;
            }
            else
            {
                for (int i = 1; i < paths.Count; i++)
                {
                    var next = result.Op(paths[i], op);
                    if (next != null)
                    {
                        result.Dispose();
                        result = next;
                        haveResult = true;
                    }
                }
            }
        }
    }
}
