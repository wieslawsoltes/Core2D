// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Model;
using Core2D.Model.Path;
using Core2D.Model.Style;
using Core2D.ViewModels.Path;
using Core2D.ViewModels.Path.Segments;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.Procedural.WaveFunctionCollapse;

internal static class WaveFunctionShapeHelpers
{
    public static ShapeStyleViewModel CreateStrokeStyle(
        this WaveFunctionTileBuilderContext context,
        string key,
        uint stroke,
        double thicknessMultiplier = 1.0,
        LineCap lineCap = LineCap.Round,
        string? dashes = null,
        double dashOffset = 0.0)
    {
        var cacheKey = $"stroke:{key}:{stroke:X8}:{thicknessMultiplier:F2}:{lineCap}:{dashes}:{dashOffset}";
        return context.Styles.GetOrCreate(cacheKey, () =>
        {
            var (sa, sr, sg, sb) = ToArgb(stroke);
            return context.Factory.CreateShapeStyle(
                name: key,
                sa: sa,
                sr: sr,
                sg: sg,
                sb: sb,
                fa: 0,
                fr: 0,
                fg: 0,
                fb: 0,
                thickness: Math.Max(0.5, context.StrokeThickness * thicknessMultiplier),
                textStyleViewModel: context.Factory.CreateTextStyle(fontSize: 12.0),
                startArrowStyleViewModel: context.Factory.CreateArrowStyle(),
                endArrowStyleViewModel: context.Factory.CreateArrowStyle(),
                lineCap: lineCap,
                dashes: dashes,
                dashOffset: dashOffset);
        });
    }

    public static ShapeStyleViewModel CreateFillStyle(
        this WaveFunctionTileBuilderContext context,
        string key,
        uint stroke,
        uint fill,
        double thicknessMultiplier = 1.0)
    {
        var cacheKey = $"fill:{key}:{stroke:X8}:{fill:X8}:{thicknessMultiplier:F2}";
        return context.Styles.GetOrCreate(cacheKey, () =>
        {
            var (sa, sr, sg, sb) = ToArgb(stroke);
            var (fa, fr, fg, fb) = ToArgb(fill);
            return context.Factory.CreateShapeStyle(
                name: key,
                sa: sa,
                sr: sr,
                sg: sg,
                sb: sb,
                fa: fa,
                fr: fr,
                fg: fg,
                fb: fb,
                thickness: Math.Max(0.5, context.StrokeThickness * thicknessMultiplier),
                textStyleViewModel: context.Factory.CreateTextStyle(fontSize: 12.0),
                startArrowStyleViewModel: context.Factory.CreateArrowStyle(),
                endArrowStyleViewModel: context.Factory.CreateArrowStyle());
        });
    }

    public static IEnumerable<BaseShapeViewModel> CreateLine(
        this WaveFunctionTileBuilderContext context,
        double x1,
        double y1,
        double x2,
        double y2,
        ShapeStyleViewModel style)
    {
        yield return context.Factory.CreateLineShape(x1, y1, x2, y2, style, isStroked: true);
    }

    public static IEnumerable<BaseShapeViewModel> CreateRectangle(
        this WaveFunctionTileBuilderContext context,
        double left,
        double top,
        double right,
        double bottom,
        ShapeStyleViewModel style,
        bool isFilled)
    {
        yield return context.Factory.CreateRectangleShape(left, top, right, bottom, style, isStroked: true, isFilled: isFilled);
    }

    public static IEnumerable<BaseShapeViewModel> CreateEllipse(
        this WaveFunctionTileBuilderContext context,
        double left,
        double top,
        double right,
        double bottom,
        ShapeStyleViewModel style,
        bool isFilled)
    {
        yield return context.Factory.CreateEllipseShape(left, top, right, bottom, style, isStroked: true, isFilled: isFilled);
    }

    public static IEnumerable<BaseShapeViewModel> CreateText(
        this WaveFunctionTileBuilderContext context,
        double left,
        double top,
        double right,
        double bottom,
        ShapeStyleViewModel style,
        string text)
    {
        yield return context.Factory.CreateTextShape(left, top, right, bottom, style, text, isStroked: false);
    }

    public static IEnumerable<BaseShapeViewModel> CreateDiamond(
        this WaveFunctionTileBuilderContext context,
        double left,
        double top,
        double right,
        double bottom,
        ShapeStyleViewModel style,
        bool isFilled)
    {
        var centerX = (left + right) / 2.0;
        var centerY = (top + bottom) / 2.0;
        var topPoint = context.Factory.CreatePointShape(centerX, top);
        var rightPoint = context.Factory.CreatePointShape(right, centerY);
        var bottomPoint = context.Factory.CreatePointShape(centerX, bottom);
        var leftPoint = context.Factory.CreatePointShape(left, centerY);
        var figure = context.Factory.CreatePathFigure(topPoint, isClosed: true);
        figure.Segments = ImmutableArray.Create<PathSegmentViewModel>(
            context.Factory.CreateLineSegment(rightPoint),
            context.Factory.CreateLineSegment(bottomPoint),
            context.Factory.CreateLineSegment(leftPoint));
        var path = context.Factory.CreatePathShape(context.Tile.Id, style, ImmutableArray.Create(figure), FillRule.Nonzero, isStroked: true, isFilled: isFilled);
        yield return path;
    }

    private static (byte A, byte R, byte G, byte B) ToArgb(uint color)
    {
        var a = (byte)((color >> 24) & 0xFF);
        var r = (byte)((color >> 16) & 0xFF);
        var g = (byte)((color >> 8) & 0xFF);
        var b = (byte)(color & 0xFF);
        if (a == 0)
        {
            a = 0xFF;
        }

        return (a, r, g, b);
    }
}
