// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Core2D.Model;
using Core2D.Model.Path;
using Core2D.Model.Style;
using Core2D.ViewModels;
using Core2D.ViewModels.Path;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using Svg.Skia;
using SP = ShimSkiaSharp;

namespace Core2D.Modules.Renderer.SkiaSharp;

public class SkiaSharpSvgConverter : ISvgConverter
{
    private static readonly Svg.Model.ISvgAssetLoader s_assetLoader = new SkiaSvgAssetLoader(new SkiaModel(new SKSvgSettings()));
    private readonly IServiceProvider? _serviceProvider;

    public SkiaSharpSvgConverter(IServiceProvider? serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private static bool IsStroked(SP.SKPaint? paint)
    {
        if (paint is null)
        {
            return false;
        }
        return paint.Style == SP.SKPaintStyle.Stroke || paint.Style == SP.SKPaintStyle.StrokeAndFill;
    }

    private static bool IsFilled(SP.SKPaint? paint)
    {
        if (paint is null)
        {
            return false;
        }
        return paint.Style == SP.SKPaintStyle.Fill || paint.Style == SP.SKPaintStyle.StrokeAndFill;
    }

    private static ArgbColorViewModel ToArgbColor(SP.ColorShader colorShader, IViewModelFactory viewModelFactory)
    {
        return viewModelFactory.CreateArgbColor(
            colorShader.Color.Alpha,
            colorShader.Color.Red,
            colorShader.Color.Green,
            colorShader.Color.Blue);
    }

    private static LineCap ToLineCap(SP.SKStrokeCap strokeCap)
    {
        switch (strokeCap)
        {
            default:
            // ReSharper disable once RedundantCaseLabel
            case SP.SKStrokeCap.Butt:
                return LineCap.Flat;

            case SP.SKStrokeCap.Round:
                return LineCap.Round;

            case SP.SKStrokeCap.Square:
                return LineCap.Square;
        }
    }

    public static TextHAlignment ToTextHAlignment(SP.SKTextAlign textAlign)
    {
        switch (textAlign)
        {
            default:
            // ReSharper disable once RedundantCaseLabel
            case SP.SKTextAlign.Left:
                return TextHAlignment.Left;

            case SP.SKTextAlign.Center:
                return TextHAlignment.Center;

            case SP.SKTextAlign.Right:
                return TextHAlignment.Right;
        }
    }

    private static ShapeStyleViewModel ToStyle(SP.SKPaint? paint, IViewModelFactory viewModelFactory)
    {
        var style = viewModelFactory.CreateShapeStyle("Style");

        if (paint is null)
        {
            return style;
        }

        switch (paint.Shader)
        {
            case SP.ColorShader colorShader:
            {
                if (style.Stroke is { })
                {
                    style.Stroke.Color = ToArgbColor(colorShader, viewModelFactory);
                }

                if (style.Fill is { })
                {
                    style.Fill.Color = ToArgbColor(colorShader, viewModelFactory);
                }

                break;
            }
            case SP.LinearGradientShader _:
            {
                // TODO:
                break;
            }
            case SP.TwoPointConicalGradientShader _:
            {
                // TODO:
                break;
            }
            case SP.PictureShader _:
            {
                // TODO:
                break;
            }
        }

        if (style.Stroke is { })
        {
            style.Stroke.Thickness = paint.StrokeWidth;

            style.Stroke.LineCap = ToLineCap(paint.StrokeCap);

            if (paint.PathEffect is SP.DashPathEffect dashPathEffect && dashPathEffect.Intervals is { })
            {
                style.Stroke.Dashes = StyleHelper.ConvertFloatArrayToDashes(dashPathEffect.Intervals);
                style.Stroke.DashOffset = dashPathEffect.Phase;
            }
        }

        if (paint.Typeface is { } && style.TextStyle is { })
        {
            if (paint.Typeface.FamilyName is { })
            {
                style.TextStyle.FontName = paint.Typeface.FamilyName;
            }

            style.TextStyle.FontSize = paint.TextSize;

            style.TextStyle.TextHAlignment = ToTextHAlignment(paint.TextAlign);

            if (paint.Typeface.FontWeight == SP.SKFontStyleWeight.Bold)
            {
                style.TextStyle.FontStyle = style.TextStyle.FontStyle | FontStyleFlags.Bold;
            }

            if (paint.Typeface.FontSlant == SP.SKFontStyleSlant.Italic)
            {
                style.TextStyle.FontStyle = style.TextStyle.FontStyle | FontStyleFlags.Italic;
            }
        }

        return style;
    }

    public static PathShapeViewModel? ToPathGeometry(SP.SKPath? path, bool isFilled, IViewModelFactory viewModelFactory)
    {
        if (path?.Commands is null)
        {
            return null;
        }

        var geometry = viewModelFactory.CreatePathShape(
            null,
            ImmutableArray.Create<PathFigureViewModel>(),
            path.FillType == SP.SKPathFillType.EvenOdd ? FillRule.EvenOdd : FillRule.Nonzero);

        var context = viewModelFactory.CreateGeometryContext(geometry);

        bool endFigure = false;
        bool haveFigure = false;

        for (int i = 0; i < path.Commands.Count; i++)
        {
            var pathCommand = path.Commands[i];
            var isLast = i == path.Commands.Count - 1;

            switch (pathCommand)
            {
                case SP.MoveToPathCommand moveToPathCommand:
                {
                    if (endFigure && haveFigure == false)
                    {
                        return null;
                    }
                    if (haveFigure)
                    {
                        context.SetClosedState(false);
                    }
                    if (isLast)
                    {
                        return geometry;
                    }
                    else
                    {
                        if (path.Commands[i + 1] is SP.MoveToPathCommand)
                        {
                            return geometry;
                        }

                        if (path.Commands[i + 1] is SP.ClosePathCommand)
                        {
                            return geometry;
                        }
                    }
                    endFigure = true;
                    haveFigure = false;
                    var x = moveToPathCommand.X;
                    var y = moveToPathCommand.Y;
                    var point = viewModelFactory.CreatePointShape(x, y);
                    context.BeginFigure(point, false);
                }
                    break;

                case SP.LineToPathCommand lineToPathCommand:
                {
                    if (endFigure == false)
                    {
                        return null;
                    }
                    haveFigure = true;
                    var x = lineToPathCommand.X;
                    var y = lineToPathCommand.Y;
                    var point = viewModelFactory.CreatePointShape(x, y);
                    context.LineTo(point);
                }
                    break;

                case SP.ArcToPathCommand arcToPathCommand:
                {
                    if (endFigure == false)
                    {
                        return null;
                    }
                    haveFigure = true;
                    var x = arcToPathCommand.X;
                    var y = arcToPathCommand.Y;
                    var point = viewModelFactory.CreatePointShape(x, y);
                    var rx = arcToPathCommand.Rx;
                    var ry = arcToPathCommand.Ry;
                    var size = viewModelFactory.CreatePathSize(rx, ry);
                    var rotationAngle = arcToPathCommand.XAxisRotate;
                    var isLargeArc = arcToPathCommand.LargeArc == SP.SKPathArcSize.Large;
                    var sweep = arcToPathCommand.Sweep == SP.SKPathDirection.Clockwise ? SweepDirection.Clockwise : SweepDirection.Counterclockwise;
                    context.ArcTo(point, size, rotationAngle, isLargeArc, sweep);
                }
                    break;

                case SP.QuadToPathCommand quadToPathCommand:
                {
                    if (endFigure == false)
                    {
                        return null;
                    }
                    haveFigure = true;
                    var x0 = quadToPathCommand.X0;
                    var y0 = quadToPathCommand.Y0;
                    var x1 = quadToPathCommand.X1;
                    var y1 = quadToPathCommand.Y1;
                    var control = viewModelFactory.CreatePointShape(x0, y0);
                    var endPoint = viewModelFactory.CreatePointShape(x1, y1);
                    context.QuadraticBezierTo(control, endPoint);
                }
                    break;

                case SP.CubicToPathCommand cubicToPathCommand:
                {
                    if (endFigure == false)
                    {
                        return null;
                    }
                    haveFigure = true;
                    var x0 = cubicToPathCommand.X0;
                    var y0 = cubicToPathCommand.Y0;
                    var x1 = cubicToPathCommand.X1;
                    var y1 = cubicToPathCommand.Y1;
                    var x2 = cubicToPathCommand.X2;
                    var y2 = cubicToPathCommand.Y2;
                    var point1 = viewModelFactory.CreatePointShape(x0, y0);
                    var point2 = viewModelFactory.CreatePointShape(x1, y1);
                    var point3 = viewModelFactory.CreatePointShape(x2, y2);
                    context.CubicBezierTo(point1, point2, point3);
                }
                    break;

                case SP.ClosePathCommand _:
                {
                    if (endFigure == false)
                    {
                        return null;
                    }
                    if (haveFigure == false)
                    {
                        return null;
                    }
                    endFigure = false;
                    haveFigure = false;
                    context.SetClosedState(true);
                }
                    break;
            }
        }

        if (endFigure)
        {
            if (haveFigure == false)
            {
                return null;
            }
            context.SetClosedState(false);
        }

        return geometry;
    }

    public static PathShapeViewModel? ToPathGeometry(SP.AddPolyPathCommand? addPolyPathCommand, SP.SKPathFillType fillType, bool isFilled, bool isClosed, IViewModelFactory viewModelFactory)
    {
        if (addPolyPathCommand?.Points is null || addPolyPathCommand.Points.Count < 2)
        {
            return null;
        }

        var geometry = viewModelFactory.CreatePathShape(
            null,
            ImmutableArray.Create<PathFigureViewModel>(),
            fillType == SP.SKPathFillType.EvenOdd ? FillRule.EvenOdd : FillRule.Nonzero);

        var context = viewModelFactory.CreateGeometryContext(geometry);

        var startX = addPolyPathCommand.Points[0].X;
        var startY = addPolyPathCommand.Points[0].Y;
        var startPoint = viewModelFactory.CreatePointShape(startX, startY);
        context.BeginFigure(startPoint, false);

        for (int i = 1; i < addPolyPathCommand.Points.Count; i++)
        {
            var x = addPolyPathCommand.Points[i].X;
            var y = addPolyPathCommand.Points[i].Y;
            var point = viewModelFactory.CreatePointShape(x, y);
            context.LineTo(point);
        }

        context.SetClosedState(isClosed);

        return geometry;
    }

    private static void ToShape(SP.SKPicture? picture, List<BaseShapeViewModel> shapes, IViewModelFactory viewModelFactory)
    {
        if (picture?.Commands is null)
        {
            return;
        }
        foreach (var canvasCommand in picture.Commands)
        {
            switch (canvasCommand)
            {
                case SP.ClipPathCanvasCommand _:
                {
                    // TODO:
                }
                    break;

                case SP.ClipRectCanvasCommand _:
                {
                    // TODO:
                }
                    break;

                case SP.SaveCanvasCommand _:
                {
                    // TODO:
                }
                    break;

                case SP.RestoreCanvasCommand _:
                {
                    // TODO:
                }
                    break;

                case SP.SetMatrixCanvasCommand _:
                {
                    // TODO:
                }
                    break;

                case SP.SaveLayerCanvasCommand _:
                {
                    // TODO:
                }
                    break;

                case SP.DrawImageCanvasCommand _:
                {
                    // TODO:
                }
                    break;

                case SP.DrawPathCanvasCommand drawPathCanvasCommand:
                {
                    if (drawPathCanvasCommand.Path?.Commands?.Count == 1)
                    {
                        var pathCommand = drawPathCanvasCommand.Path.Commands[0];
                        var success = false;

                        switch (pathCommand)
                        {
                            case SP.AddRectPathCommand addRectPathCommand:
                            {
                                var style = ToStyle(drawPathCanvasCommand.Paint, viewModelFactory);
                                var rectangleShape = viewModelFactory.CreateRectangleShape(
                                    addRectPathCommand.Rect.Left,
                                    addRectPathCommand.Rect.Top,
                                    addRectPathCommand.Rect.Right,
                                    addRectPathCommand.Rect.Bottom,
                                    style,
                                    IsStroked(drawPathCanvasCommand.Paint),
                                    IsFilled(drawPathCanvasCommand.Paint));
                                shapes.Add(rectangleShape);
                                success = true;
                            }
                                break;

                            case SP.AddRoundRectPathCommand _:
                            {
                                // TODO:
                            }
                                break;

                            case SP.AddOvalPathCommand addOvalPathCommand:
                            {
                                var style = ToStyle(drawPathCanvasCommand.Paint, viewModelFactory);
                                var ellipseShape = viewModelFactory.CreateEllipseShape(
                                    addOvalPathCommand.Rect.Left,
                                    addOvalPathCommand.Rect.Top,
                                    addOvalPathCommand.Rect.Right,
                                    addOvalPathCommand.Rect.Bottom,
                                    style,
                                    IsStroked(drawPathCanvasCommand.Paint),
                                    IsFilled(drawPathCanvasCommand.Paint));
                                shapes.Add(ellipseShape);
                                success = true;
                            }
                                break;

                            case SP.AddCirclePathCommand addCirclePathCommand:
                            {
                                var style = ToStyle(drawPathCanvasCommand.Paint, viewModelFactory);
                                var x = addCirclePathCommand.X;
                                var y = addCirclePathCommand.Y;
                                var radius = addCirclePathCommand.Radius;
                                var ellipseShape = viewModelFactory.CreateEllipseShape(
                                    x - radius,
                                    y - radius,
                                    x + radius,
                                    y + radius,
                                    style,
                                    IsStroked(drawPathCanvasCommand.Paint),
                                    IsFilled(drawPathCanvasCommand.Paint));
                                shapes.Add(ellipseShape);
                                success = true;
                            }
                                break;

                            case SP.AddPolyPathCommand addPolyPathCommand:
                            {
                                if (addPolyPathCommand.Points is { })
                                {
                                    var polyGeometry = ToPathGeometry(
                                        addPolyPathCommand,
                                        drawPathCanvasCommand.Path.FillType,
                                        IsFilled(drawPathCanvasCommand.Paint),
                                        addPolyPathCommand.Close, viewModelFactory);
                                    if (polyGeometry is { })
                                    {
                                        var style = ToStyle(drawPathCanvasCommand.Paint, viewModelFactory);
                                        var pathShape = viewModelFactory.CreatePathShape(
                                            "Path",
                                            style,
                                            ImmutableArray.Create<PathFigureViewModel>(),
                                            FillRule.Nonzero,
                                            IsStroked(drawPathCanvasCommand.Paint),
                                            IsFilled(drawPathCanvasCommand.Paint));
                                        shapes.Add(pathShape);
                                        success = true;
                                    }
                                }
                            }
                                break;
                        }

                        if (success)
                        {
                            break;
                        }
                    }

                    if (drawPathCanvasCommand.Path?.Commands?.Count == 2)
                    {
                        var pathCommand1 = drawPathCanvasCommand.Path.Commands[0];
                        var pathCommand2 = drawPathCanvasCommand.Path.Commands[1];

                        if (pathCommand1 is SP.MoveToPathCommand moveTo &&
                            pathCommand2 is SP.LineToPathCommand lineTo)
                        {
                            var style = ToStyle(drawPathCanvasCommand.Paint, viewModelFactory);
                            var pathShape = viewModelFactory.CreateLineShape(
                                moveTo.X, moveTo.Y,
                                lineTo.X, lineTo.Y,
                                style,
                                IsStroked(drawPathCanvasCommand.Paint));
                            shapes.Add(pathShape);
                            break;
                        }
                    }

                    var path = ToPathGeometry(drawPathCanvasCommand.Path, IsFilled(drawPathCanvasCommand.Paint), viewModelFactory);
                    if (path is { })
                    {
                        path.Name = "Path";
                        path.Style = ToStyle(drawPathCanvasCommand.Paint, viewModelFactory);
                        path.IsStroked = IsStroked(drawPathCanvasCommand.Paint);
                        path.IsStroked = IsFilled(drawPathCanvasCommand.Paint);
                        shapes.Add(path);
                    }
                }
                    break;

                case SP.DrawTextBlobCanvasCommand _:
                {
                    // TODO:
                }
                    break;

                case SP.DrawTextCanvasCommand drawTextCanvasCommand:
                {
                    var style = ToStyle(drawTextCanvasCommand.Paint, viewModelFactory);
                    var pathShape = viewModelFactory.CreateTextShape(
                        drawTextCanvasCommand.X,
                        drawTextCanvasCommand.Y,
                        style,
                        drawTextCanvasCommand.Text,
                        IsFilled(drawTextCanvasCommand.Paint));
                    shapes.Add(pathShape);
                }
                    break;

                case SP.DrawTextOnPathCanvasCommand _:
                {
                    // TODO:
                }
                    break;
            }
        }
    }

    private static Stream ToStream(string text)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(text);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    private IList<BaseShapeViewModel>? Convert(Svg.SvgDocument document, out double width, out double height)
    {
        var picture = Svg.Model.Services.SvgService.ToModel(document, s_assetLoader, out _, out _);
        if (picture is null)
        {
            width = double.NaN;
            height = double.NaN;
            return null;
        }

        var shapes = new List<BaseShapeViewModel>();
        var factory = _serviceProvider.GetService<IViewModelFactory>();
        if (factory is null)
        {
            width = double.NaN;
            height = double.NaN;
            return null;
        }

        ToShape(picture, shapes, factory);

        var group = factory.CreateBlockShape("svg");

        group.Shapes = group.Shapes.AddRange(shapes);

        width = picture.CullRect.Width;
        height = picture.CullRect.Height;
        return Enumerable.Repeat<BaseShapeViewModel>(group, 1).ToList();
    }

    public IList<BaseShapeViewModel>? Convert(Stream stream, out double width, out double height)
    {
        var document = Svg.Model.Services.SvgService.Open(stream);
        if (document is null)
        {
            width = double.NaN;
            height = double.NaN;
            return null;
        }

        return Convert(document, out width, out height);
    }

    public IList<BaseShapeViewModel>? FromString(string text, out double width, out double height)
    {
        using var stream = ToStream(text);
        var document = Svg.Model.Services.SvgService.Open(stream);
        if (document is null)
        {
            width = double.NaN;
            height = double.NaN;
            return null;
        }
        return Convert(document, out width, out height);
    }
}
