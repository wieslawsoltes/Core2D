// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using Core2D.Modules.Renderer.SkiaSharp;
using Core2D.ViewModels.Shapes;
using SkiaSharp;
using VelloSharp;
using PathFillRule = Core2D.Model.Path.FillRule;

namespace Core2D.Modules.Renderer.Common;

internal readonly record struct PathGeometryData(PathBuilder Builder, PathFillRule FillRule);

internal static class PathBuilderFactory
{
    public static PathGeometryData? Create(BaseShapeViewModel shape)
    {
        using var path = PathGeometryConverter.ToSKPath(shape);
        return Create(path);
    }

    public static PathGeometryData? Create(SKPath? path)
    {
        if (path is null)
        {
            return null;
        }

        var builder = new PathBuilder();
        using var iterator = path.CreateRawIterator();
        var points = new SKPoint[4];

        while (iterator.Next(points) is var verb && verb != SKPathVerb.Done)
        {
            switch (verb)
            {
                case SKPathVerb.Move:
                    builder.MoveTo(points[0].X, points[0].Y);
                    break;
                case SKPathVerb.Line:
                    builder.LineTo(points[1].X, points[1].Y);
                    break;
                case SKPathVerb.Cubic:
                    builder.CubicTo(
                        points[1].X, points[1].Y,
                        points[2].X, points[2].Y,
                        points[3].X, points[3].Y);
                    break;
                case SKPathVerb.Quad:
                    builder.QuadraticTo(
                        points[1].X, points[1].Y,
                        points[2].X, points[2].Y);
                    break;
                case SKPathVerb.Conic:
                {
                    var weight = iterator.ConicWeight();
                    var quadPoints = SKPath.ConvertConicToQuads(points[0], points[1], points[2], weight, 1);
                    if (quadPoints.Length >= 5)
                    {
                        builder.QuadraticTo(
                            quadPoints[1].X, quadPoints[1].Y,
                            quadPoints[2].X, quadPoints[2].Y);
                        builder.QuadraticTo(
                            quadPoints[3].X, quadPoints[3].Y,
                            quadPoints[4].X, quadPoints[4].Y);
                    }
                    break;
                }
                case SKPathVerb.Close:
                    builder.Close();
                    break;
            }
        }

        if (builder.Count == 0)
        {
            return null;
        }

        var fillRule = path.FillType == SKPathFillType.EvenOdd ? PathFillRule.EvenOdd : PathFillRule.Nonzero;
        return new PathGeometryData(builder, fillRule);
    }
}
