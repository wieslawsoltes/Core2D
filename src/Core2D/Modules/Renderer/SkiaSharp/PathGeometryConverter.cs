#nullable enable
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
using Core2D.Spatial;
using Core2D.Spatial.Arc;

namespace Core2D.Modules.Renderer.SkiaSharp;

public static class PathGeometryConverter
{
    private static bool CreateFigure(this PathFigureViewModel figure, SKPath path)
    {
        if (figure.StartPoint is null)
        {
            return false;
        }

        path.MoveTo(
            (float)figure.StartPoint.X,
            (float)figure.StartPoint.Y);

        foreach (var segment in figure.Segments)
        {
            switch (segment)
            {
                case LineSegmentViewModel lineSegment:
                {
                    if (lineSegment.Point is null)
                    {
                        return false;
                    }
                    path.LineTo(
                        (float)lineSegment.Point.X,
                        (float)lineSegment.Point.Y);
                    break;
                }
                case CubicBezierSegmentViewModel cubicBezierSegment:
                {
                    if (cubicBezierSegment.Point1 is null || cubicBezierSegment.Point2 is null || cubicBezierSegment.Point3 is null)
                    {
                        return false;
                    }
                    path.CubicTo(
                        (float)cubicBezierSegment.Point1.X,
                        (float)cubicBezierSegment.Point1.Y,
                        (float)cubicBezierSegment.Point2.X,
                        (float)cubicBezierSegment.Point2.Y,
                        (float)cubicBezierSegment.Point3.X,
                        (float)cubicBezierSegment.Point3.Y);
                    break;
                }
                case QuadraticBezierSegmentViewModel quadraticBezierSegment:
                {
                    if (quadraticBezierSegment.Point1 is null || quadraticBezierSegment.Point2 is null)
                    {
                        return false;
                    }
                    path.QuadTo(
                        (float)quadraticBezierSegment.Point1.X,
                        (float)quadraticBezierSegment.Point1.Y,
                        (float)quadraticBezierSegment.Point2.X,
                        (float)quadraticBezierSegment.Point2.Y);
                    break;
                }
                case ArcSegmentViewModel arcSegment:
                {
                    if (arcSegment.Point is null || arcSegment.Size is null)
                    {
                        return false;
                    }
                    path.ArcTo(
                        (float)arcSegment.Size.Width,
                        (float)arcSegment.Size.Height,
                        (float)arcSegment.RotationAngle,
                        arcSegment.IsLargeArc ? SKPathArcSize.Large : SKPathArcSize.Small,
                        arcSegment.SweepDirection == SweepDirection.Clockwise
                            ? SKPathDirection.Clockwise
                            : SKPathDirection.CounterClockwise,
                        (float)arcSegment.Point.X,
                        (float)arcSegment.Point.Y);
                    break;
                }
                default:
                    throw new NotSupportedException("Not supported segment type: " + segment.GetType());
            }
        }

        if (figure.IsClosed)
        {
            path.Close();
        }

        return true;
    }

    public static PathShapeViewModel ToPathGeometry(SKPath path, IViewModelFactory viewModelFactory)
    {
        var geometry = viewModelFactory.CreatePathShape(
            null,
            ImmutableArray.Create<PathFigureViewModel>(),
            path.FillType == SKPathFillType.EvenOdd ? FillRule.EvenOdd : FillRule.Nonzero);

        var context = viewModelFactory.CreateGeometryContext(geometry);

        using var iterator = path.CreateRawIterator();
        var points = new SKPoint[4];
        SKPathVerb pathVerb;

        while ((pathVerb = iterator.Next(points)) != SKPathVerb.Done)
        {
            switch (pathVerb)
            {
                case SKPathVerb.Move:
                {
                    context.BeginFigure(
                        viewModelFactory.CreatePointShape(points[0].X, points[0].Y),
                        false);
                    break;
                }
                case SKPathVerb.Line:
                {
                    context.LineTo(
                        viewModelFactory.CreatePointShape(points[1].X, points[1].Y));
                    break;
                }
                case SKPathVerb.Cubic:
                {
                    context.CubicBezierTo(
                        viewModelFactory.CreatePointShape(points[1].X, points[1].Y),
                        viewModelFactory.CreatePointShape(points[2].X, points[2].Y),
                        viewModelFactory.CreatePointShape(points[3].X, points[3].Y));
                    break;
                }
                case SKPathVerb.Quad:
                {
                    context.QuadraticBezierTo(
                        viewModelFactory.CreatePointShape(points[1].X, points[1].Y),
                        viewModelFactory.CreatePointShape(points[2].X, points[2].Y));
                    break;
                }
                case SKPathVerb.Conic:
                {
                    var quads = SKPath.ConvertConicToQuads(points[0], points[1], points[2], iterator.ConicWeight(), 1);
                    context.QuadraticBezierTo(
                        viewModelFactory.CreatePointShape(quads[1].X, quads[1].Y),
                        viewModelFactory.CreatePointShape(quads[2].X, quads[2].Y));
                    context.QuadraticBezierTo(
                        viewModelFactory.CreatePointShape(quads[3].X, quads[3].Y),
                        viewModelFactory.CreatePointShape(quads[4].X, quads[4].Y));
                    break;
                }
                case SKPathVerb.Close:
                {
                    context.SetClosedState(true);
                    break;
                }
            }
        }

        return geometry;
    }

    public static SKPath? ToSKPath(this IEnumerable<BaseShapeViewModel> shapes)
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
                    if (lineShape.Start is null || lineShape.End is null)
                    {
                        return null;
                    }
                    if (previous is null || previous != lineShape.Start)
                    {
                        path.MoveTo(
                            (float)lineShape.Start.X,
                            (float)lineShape.Start.Y);
                    }
                    path.LineTo(
                        (float)lineShape.End.X,
                        (float)lineShape.End.Y);
                    previous = lineShape.End;
                    break;
                }
                case RectangleShapeViewModel rectangleShape:
                {
                    if (rectangleShape.TopLeft is null || rectangleShape.BottomRight is null)
                    {
                        return null;
                    }
                    path.AddRect(
                        SkiaSharpDrawUtil.CreateRect(rectangleShape.TopLeft, rectangleShape.BottomRight));
                    break;
                }
                case EllipseShapeViewModel ellipseShape:
                {
                    if (ellipseShape.TopLeft is null || ellipseShape.BottomRight is null)
                    {
                        return null;
                    }
                    path.AddOval(
                        SkiaSharpDrawUtil.CreateRect(ellipseShape.TopLeft, ellipseShape.BottomRight));
                    break;
                }
                case ArcShapeViewModel arcShape:
                {
                    if (arcShape.Point1 is null 
                        || arcShape.Point2 is null 
                        || arcShape.Point3 is null 
                        || arcShape.Point4 is null)
                    {
                        return null;
                    }
                    var a = new GdiArc(
                        Point2.FromXY(arcShape.Point1.X, arcShape.Point1.Y),
                        Point2.FromXY(arcShape.Point2.X, arcShape.Point2.Y),
                        Point2.FromXY(arcShape.Point3.X, arcShape.Point3.Y),
                        Point2.FromXY(arcShape.Point4.X, arcShape.Point4.Y));
                    var rect = new SKRect(
                        (float)a.X,
                        (float)a.Y,
                        (float)(a.X + a.Width),
                        (float)(a.Y + a.Height));
                    path.AddArc(rect, (float)a.StartAngle, (float)a.SweepAngle);
                    break;
                }
                case CubicBezierShapeViewModel cubicBezierShape:
                {
                    if (cubicBezierShape.Point1 is null 
                        || cubicBezierShape.Point2 is null 
                        || cubicBezierShape.Point3 is null 
                        || cubicBezierShape.Point4 is null)
                    {
                        return null;
                    }
                    if (previous is null || previous != cubicBezierShape.Point1)
                    {
                        path.MoveTo(
                            (float)cubicBezierShape.Point1.X,
                            (float)cubicBezierShape.Point1.Y);
                    }
                    path.CubicTo(
                        (float)cubicBezierShape.Point2.X,
                        (float)cubicBezierShape.Point2.Y,
                        (float)cubicBezierShape.Point3.X,
                        (float)cubicBezierShape.Point3.Y,
                        (float)cubicBezierShape.Point4.X,
                        (float)cubicBezierShape.Point4.Y);
                    previous = cubicBezierShape.Point4;
                    break;
                }
                case QuadraticBezierShapeViewModel quadraticBezierShape:
                {
                    if (quadraticBezierShape.Point1 is null 
                        || quadraticBezierShape.Point2 is null 
                        || quadraticBezierShape.Point3 is null)
                    {
                        return null;
                    }
                    if (previous is null || previous != quadraticBezierShape.Point1)
                    {
                        path.MoveTo(
                            (float)quadraticBezierShape.Point1.X,
                            (float)quadraticBezierShape.Point1.Y);
                    }
                    path.QuadTo(
                        (float)quadraticBezierShape.Point2.X,
                        (float)quadraticBezierShape.Point2.Y,
                        (float)quadraticBezierShape.Point3.X,
                        (float)quadraticBezierShape.Point3.Y);
                    previous = quadraticBezierShape.Point3;
                    break;
                }
                case TextShapeViewModel textShape:
                {
                    if (textShape.TopLeft is null || textShape.BottomRight is null)
                    {
                        return null;
                    }
                    var resultPath = ToSKPath(textShape);
                    if (resultPath is { } && !resultPath.IsEmpty)
                    {
                        path.AddPath(resultPath);
                    }
                    break;
                }
                case PathShapeViewModel pathShape:
                {
                    var resultPath = ToSKPath(pathShape);
                    if (resultPath is { })
                    {
                        if (!resultPath.IsEmpty)
                        {
                            path.AddPath(resultPath);
                        }
                    }
                    else
                    {
                        return null;
                    }
                    break;
                }
                case GroupShapeViewModel groupShape:
                {
                    var resultPath = ToSKPath(groupShape.Shapes);
                    if (resultPath is { })
                    {
                        if (!resultPath.IsEmpty)
                        {
                            path.AddPath(resultPath);
                        }
                    }
                    else
                    {
                        return null;
                    }
                    break;
                }
            }
        }
        return path;
    }

    public static SKPath? ToSKPath(this BaseShapeViewModel shape)
    {
        return shape switch
        {
            LineShapeViewModel lineShape => ToSKPath(lineShape),
            RectangleShapeViewModel rectangleShape => ToSKPath(rectangleShape),
            EllipseShapeViewModel ellipseShape => ToSKPath(ellipseShape),
            ImageShapeViewModel imageShape => ToSKPath(imageShape),
            ArcShapeViewModel arcShape => ToSKPath(arcShape),
            CubicBezierShapeViewModel cubicBezierShape => ToSKPath(cubicBezierShape),
            QuadraticBezierShapeViewModel quadraticBezierShape => ToSKPath(quadraticBezierShape),
            TextShapeViewModel textShape => ToSKPath(textShape),
            PathShapeViewModel pathShape => ToSKPath(pathShape),
            GroupShapeViewModel groupShape => ToSKPath(groupShape.Shapes),
            _ => null,
        };
    }

    public static SKPath? ToSKPath(this LineShapeViewModel lineShape)
    {
        if (lineShape.Start is null || lineShape.End is null)
        {
            return null;
        }
        var path = new SKPath
        {
            FillType = SKPathFillType.Winding
        };
        path.MoveTo(
            (float)lineShape.Start.X,
            (float)lineShape.Start.Y);
        path.LineTo(
            (float)lineShape.End.X,
            (float)lineShape.End.Y);
        return path;
    }

    public static SKPath? ToSKPath(this RectangleShapeViewModel rectangleShape)
    {
        if (rectangleShape.TopLeft is null || rectangleShape.BottomRight is null)
        {
            return null;
        }
        var path = new SKPath
        {
            FillType = SKPathFillType.Winding
        };
        path.AddRect(
            SkiaSharpDrawUtil.CreateRect(rectangleShape.TopLeft, rectangleShape.BottomRight));
        return path;
    }

    public static SKPath? ToSKPath(this EllipseShapeViewModel ellipseShape)
    {
        if (ellipseShape.TopLeft is null || ellipseShape.BottomRight is null)
        {
            return null;
        }
        var path = new SKPath
        {
            FillType = SKPathFillType.Winding
        };
        path.AddOval(
            SkiaSharpDrawUtil.CreateRect(ellipseShape.TopLeft, ellipseShape.BottomRight));
        return path;
    }

    public static SKPath? ToSKPath(this ImageShapeViewModel imageShape)
    {
        if (imageShape.TopLeft is null || imageShape.BottomRight is null)
        {
            return null;
        }
        var path = new SKPath
        {
            FillType = SKPathFillType.Winding
        };
        path.AddRect(
            SkiaSharpDrawUtil.CreateRect(imageShape.TopLeft, imageShape.BottomRight));
        return path;
    }

    public static SKPath? ToSKPath(this ArcShapeViewModel arcShape)
    {
        if (arcShape.Point1 is null 
            || arcShape.Point2 is null 
            || arcShape.Point3 is null 
            || arcShape.Point4 is null)
        {
            return null;
        }
        var path = new SKPath
        {
            FillType = SKPathFillType.Winding
        };
        var a = new GdiArc(
            Point2.FromXY(arcShape.Point1.X, arcShape.Point1.Y),
            Point2.FromXY(arcShape.Point2.X, arcShape.Point2.Y),
            Point2.FromXY(arcShape.Point3.X, arcShape.Point3.Y),
            Point2.FromXY(arcShape.Point4.X, arcShape.Point4.Y));
        var rect = new SKRect(
            (float)a.X,
            (float)a.Y,
            (float)(a.X + a.Width),
            (float)(a.Y + a.Height));
        path.AddArc(rect, (float)a.StartAngle, (float)a.SweepAngle);
        return path;
    }

    public static SKPath? ToSKPath(this CubicBezierShapeViewModel cubicBezierShape)
    {
        if (cubicBezierShape.Point1 is null 
            || cubicBezierShape.Point2 is null 
            || cubicBezierShape.Point3 is null 
            || cubicBezierShape.Point4 is null)
        {
            return null;
        }
        var path = new SKPath
        {
            FillType = SKPathFillType.Winding
        };
        path.MoveTo(
            (float)cubicBezierShape.Point1.X,
            (float)cubicBezierShape.Point1.Y);
        path.CubicTo(
            (float)cubicBezierShape.Point2.X,
            (float)cubicBezierShape.Point2.Y,
            (float)cubicBezierShape.Point3.X,
            (float)cubicBezierShape.Point3.Y,
            (float)cubicBezierShape.Point4.X,
            (float)cubicBezierShape.Point4.Y);
        return path;
    }

    public static SKPath? ToSKPath(this QuadraticBezierShapeViewModel quadraticBezierShape)
    {
        if (quadraticBezierShape.Point1 is null 
            || quadraticBezierShape.Point2 is null 
            || quadraticBezierShape.Point3 is null)
        {
            return null;
        }
        var path = new SKPath
        {
            FillType = SKPathFillType.Winding
        };
        path.MoveTo(
            (float)quadraticBezierShape.Point1.X,
            (float)quadraticBezierShape.Point1.Y);
        path.QuadTo(
            (float)quadraticBezierShape.Point2.X,
            (float)quadraticBezierShape.Point2.Y,
            (float)quadraticBezierShape.Point3.X,
            (float)quadraticBezierShape.Point3.Y);
        return path;
    }

    public static SKPath? ToSKPath(this TextShapeViewModel textShape)
    {
        if (textShape.TopLeft is null 
            || textShape.BottomRight is null
            || textShape.Style is null)
        {
            return null;
        }
        var path = new SKPath
        {
            FillType = SKPathFillType.Winding
        };

        var boundText = textShape.GetProperty(nameof(TextShapeViewModel.Text)) as string ?? textShape.Text;
        if (boundText is null)
        {
            return null;
        }

        using var pen = SkiaSharpDrawUtil.GetSKPaint(
            boundText, 
            textShape.Style, 
            textShape.TopLeft, 
            textShape.BottomRight, 
            out var origin);

        if (pen is null)
        {
            return null;
        }

        using var outlinePath = pen.GetTextPath(boundText, origin.X, origin.Y);
        using var fillPath = pen.GetFillPath(outlinePath);

        path.AddPath(fillPath);

        return path;
    }

    public static SKPath? ToSKPath(this PathShapeViewModel pathShapeViewModel)
    {
        var fillType = pathShapeViewModel.FillRule == FillRule.EvenOdd ? SKPathFillType.EvenOdd : SKPathFillType.Winding;
        var path = new SKPath
        {
            FillType = fillType
        };

        foreach (var pathFigure in pathShapeViewModel.Figures)
        {
            if (!CreateFigure(pathFigure, path))
            {
                return null;
            }
        }

        return path;
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
        if (next is { })
        {
            result.Dispose();
            result = next;
            haveResult = true;
        }
    }

    public static void Op(IList<SKPath> paths, IList<SKPathOp> ops, out SKPath result, out bool haveResult)
    {
        using var builder = new SKPath.OpBuilder();

        for (var i = 0; i < paths.Count; i++)
        {
            builder.Add(paths[i], ops[i]);
        }

        result = new SKPath(paths[0]) { FillType = paths[0].FillType };
        haveResult = builder.Resolve(result);
    }

    public static void Op(IList<SKPath> paths, SKPathOp op, out SKPath? result, out bool haveResult)
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
            for (var i = 1; i < paths.Count; i++)
            {
                var next = result.Op(paths[i], op);
                if (next is { })
                {
                    result.Dispose();
                    result = next;
                    haveResult = true;
                }
            }
        }
    }
}
