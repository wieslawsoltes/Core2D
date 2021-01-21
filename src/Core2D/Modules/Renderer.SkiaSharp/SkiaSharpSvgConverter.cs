#nullable disable
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
using SP = Svg.Model;

namespace Core2D.Modules.Renderer.SkiaSharp
{
    public class SkiaSharpSvgConverter : ISvgConverter
    {
        private static readonly SP.IAssetLoader _assetLoader = new SkiaAssetLoader();
        private readonly IServiceProvider _serviceProvider;

        public SkiaSharpSvgConverter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private static bool IsStroked(SP.Painting.Paint paint)
        {
            if (paint is null)
            {
                return false;
            }
            return paint.Style == SP.Painting.PaintStyle.Stroke || paint.Style == SP.Painting.PaintStyle.StrokeAndFill;
        }

        private static bool IsFilled(SP.Painting.Paint paint)
        {
            if (paint is null)
            {
                return false;
            }
            return paint.Style == SP.Painting.PaintStyle.Fill || paint.Style == SP.Painting.PaintStyle.StrokeAndFill;
        }

        private static ArgbColorViewModel ToArgbColor(SP.Painting.Shaders.ColorShader colorShader, IFactory factory)
        {
            return factory.CreateArgbColor(
                colorShader.Color.Alpha,
                colorShader.Color.Red,
                colorShader.Color.Green,
                colorShader.Color.Blue);
        }

        private static LineCap ToLineCap(SP.Painting.StrokeCap strokeCap)
        {
            switch (strokeCap)
            {
                default:
                case SP.Painting.StrokeCap.Butt:
                    return LineCap.Flat;

                case SP.Painting.StrokeCap.Round:
                    return LineCap.Round;

                case SP.Painting.StrokeCap.Square:
                    return LineCap.Square;
            }
        }

        public static TextHAlignment ToTextHAlignment(SP.Painting.TextAlign textAlign)
        {
            switch (textAlign)
            {
                default:
                case SP.Painting.TextAlign.Left:
                    return TextHAlignment.Left;

                case SP.Painting.TextAlign.Center:
                    return TextHAlignment.Center;

                case SP.Painting.TextAlign.Right:
                    return TextHAlignment.Right;
            }
        }

        private static ShapeStyleViewModel ToStyle(SP.Painting.Paint paint, IFactory factory)
        {
            var style = factory.CreateShapeStyle("Style");

            if (paint is null)
            {
                return style;
            }

            switch (paint.Shader)
            {
                case SP.Painting.Shaders.ColorShader colorShader:
                    style.Stroke.Color = ToArgbColor(colorShader, factory);
                    style.Fill.Color = ToArgbColor(colorShader, factory);
                    break;

                case SP.Painting.Shaders.LinearGradientShader linearGradientShader:
                    // TODO:
                    break;

                case SP.Painting.Shaders.TwoPointConicalGradientShader twoPointConicalGradientShader:
                    // TODO:
                    break;

                case SP.Painting.Shaders.PictureShader pictureShader:
                    // TODO:
                    break;

                default:
                    break;
            }

            style.Stroke.Thickness = paint.StrokeWidth;

            style.Stroke.LineCap = ToLineCap(paint.StrokeCap);

            if (paint.PathEffect is SP.Painting.PathEffects.DashPathEffect dashPathEffect && dashPathEffect.Intervals is { })
            {
                style.Stroke.Dashes = StyleHelper.ConvertFloatArrayToDashes(dashPathEffect.Intervals);
                style.Stroke.DashOffset = dashPathEffect.Phase;
            }

            if (paint.Typeface is { })
            {
                if (paint.Typeface.FamilyName is { })
                {
                    style.TextStyle.FontName = paint.Typeface.FamilyName;
                }

                style.TextStyle.FontSize = paint.TextSize;

                style.TextStyle.TextHAlignment = ToTextHAlignment(paint.TextAlign);

                if (paint.Typeface.Weight == SP.Painting.FontStyleWeight.Bold)
                {
                    style.TextStyle.FontStyle = style.TextStyle.FontStyle | FontStyleFlags.Bold;
                }

                if (paint.Typeface.Style == SP.Painting.FontStyleSlant.Italic)
                {
                    style.TextStyle.FontStyle = style.TextStyle.FontStyle | FontStyleFlags.Italic;
                }
            }

            return style;
        }

        public static PathGeometryViewModel ToPathGeometry(SP.Primitives.Path path, bool isFilled, IFactory factory)
        {
            if (path.Commands is null)
            {
                return null;
            }

            var geometry = factory.CreatePathGeometry(
                ImmutableArray.Create<PathFigureViewModel>(),
                path.FillType == SP.Primitives.PathFillType.EvenOdd ? FillRule.EvenOdd : FillRule.Nonzero);

            var context = factory.CreateGeometryContext(geometry);

            bool endFigure = false;
            bool haveFigure = false;

            for (int i = 0; i < path.Commands.Count; i++)
            {
                var pathCommand = path.Commands[i];
                var isLast = i == path.Commands.Count - 1;

                switch (pathCommand)
                {
                    case SP.Primitives.PathCommands.MoveToPathCommand moveToPathCommand:
                        {
                            if (endFigure == true && haveFigure == false)
                            {
                                return null;
                            }
                            if (haveFigure == true)
                            {
                                context.SetClosedState(false);
                            }
                            if (isLast == true)
                            {
                                return geometry;
                            }
                            else
                            {
                                if (path.Commands[i + 1] is SP.Primitives.PathCommands.MoveToPathCommand)
                                {
                                    return geometry;
                                }

                                if (path.Commands[i + 1] is SP.Primitives.PathCommands.ClosePathCommand)
                                {
                                    return geometry;
                                }
                            }
                            endFigure = true;
                            haveFigure = false;
                            var x = moveToPathCommand.X;
                            var y = moveToPathCommand.Y;
                            var point = factory.CreatePointShape(x, y);
                            context.BeginFigure(point, false);
                        }
                        break;

                    case SP.Primitives.PathCommands.LineToPathCommand lineToPathCommand:
                        {
                            if (endFigure == false)
                            {
                                return null;
                            }
                            haveFigure = true;
                            var x = lineToPathCommand.X;
                            var y = lineToPathCommand.Y;
                            var point = factory.CreatePointShape(x, y);
                            context.LineTo(point);
                        }
                        break;

                    case SP.Primitives.PathCommands.ArcToPathCommand arcToPathCommand:
                        {
                            if (endFigure == false)
                            {
                                return null;
                            }
                            haveFigure = true;
                            var x = arcToPathCommand.X;
                            var y = arcToPathCommand.Y;
                            var point = factory.CreatePointShape(x, y);
                            var rx = arcToPathCommand.Rx;
                            var ry = arcToPathCommand.Ry;
                            var size = factory.CreatePathSize(rx, ry);
                            var rotationAngle = arcToPathCommand.XAxisRotate;
                            var isLargeArc = arcToPathCommand.LargeArc == SP.Primitives.PathArcSize.Large;
                            var sweep = arcToPathCommand.Sweep == SP.Primitives.PathDirection.Clockwise ? SweepDirection.Clockwise : SweepDirection.Counterclockwise;
                            context.ArcTo(point, size, rotationAngle, isLargeArc, sweep);
                        }
                        break;

                    case SP.Primitives.PathCommands.QuadToPathCommand quadToPathCommand:
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
                            var control = factory.CreatePointShape(x0, y0);
                            var endPoint = factory.CreatePointShape(x1, y1);
                            context.QuadraticBezierTo(control, endPoint);
                        }
                        break;

                    case SP.Primitives.PathCommands.CubicToPathCommand cubicToPathCommand:
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
                            var point1 = factory.CreatePointShape(x0, y0);
                            var point2 = factory.CreatePointShape(x1, y1);
                            var point3 = factory.CreatePointShape(x2, y2);
                            context.CubicBezierTo(point1, point2, point3);
                        }
                        break;

                    case SP.Primitives.PathCommands.ClosePathCommand _:
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

                    default:
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

        public static PathGeometryViewModel ToPathGeometry(SP.Primitives.PathCommands.AddPolyPathCommand addPolyPathCommand, SP.Primitives.PathFillType fillType, bool isFilled, bool isClosed, IFactory factory)
        {
            if (addPolyPathCommand.Points is null || addPolyPathCommand.Points.Count < 2)
            {
                return null;
            }

            var geometry = factory.CreatePathGeometry(
                ImmutableArray.Create<PathFigureViewModel>(),
                fillType == SP.Primitives.PathFillType.EvenOdd ? FillRule.EvenOdd : FillRule.Nonzero);

            var context = factory.CreateGeometryContext(geometry);

            var startX = addPolyPathCommand.Points[0].X;
            var startY = addPolyPathCommand.Points[0].Y;
            var startPoint = factory.CreatePointShape(startX, startY);
            context.BeginFigure(startPoint, false);

            for (int i = 1; i < addPolyPathCommand.Points.Count; i++)
            {
                var x = addPolyPathCommand.Points[i].X;
                var y = addPolyPathCommand.Points[i].Y;
                var point = factory.CreatePointShape(x, y);
                context.LineTo(point);
            }

            context.SetClosedState(isClosed);

            return geometry;
        }

        private static void ToShape(SP.Primitives.Picture picture, List<BaseShapeViewModel> shapes, IFactory factory)
        {
            foreach (var canvasCommand in picture.Commands)
            {
                switch (canvasCommand)
                {
                    case SP.Primitives.CanvasCommands.ClipPathCanvasCommand clipPathCanvasCommand:
                        {
                            // TODO:
                        }
                        break;

                    case SP.Primitives.CanvasCommands.ClipRectCanvasCommand clipRectCanvasCommand:
                        {
                            // TODO:
                        }
                        break;

                    case SP.Primitives.CanvasCommands.SaveCanvasCommand _:
                        {
                            // TODO:
                        }
                        break;

                    case SP.Primitives.CanvasCommands.RestoreCanvasCommand _:
                        {
                            // TODO:
                        }
                        break;

                    case SP.Primitives.CanvasCommands.SetMatrixCanvasCommand setMatrixCanvasCommand:
                        {
                            // TODO:
                        }
                        break;

                    case SP.Primitives.CanvasCommands.SaveLayerCanvasCommand saveLayerCanvasCommand:
                        {
                            // TODO:
                        }
                        break;

                    case SP.Primitives.CanvasCommands.DrawImageCanvasCommand drawImageCanvasCommand:
                        {
                            // TODO:
                        }
                        break;

                    case SP.Primitives.CanvasCommands.DrawPathCanvasCommand drawPathCanvasCommand:
                        {
                            if (drawPathCanvasCommand.Path is { } && drawPathCanvasCommand.Paint is { })
                            {
                                if (drawPathCanvasCommand.Path.Commands?.Count == 1)
                                {
                                    var pathCommand = drawPathCanvasCommand.Path.Commands[0];
                                    var success = false;

                                    switch (pathCommand)
                                    {
                                        case SP.Primitives.PathCommands.AddRectPathCommand addRectPathCommand:
                                            {
                                                var style = ToStyle(drawPathCanvasCommand.Paint, factory);
                                                var rectangleShape = factory.CreateRectangleShape(
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

                                        case SP.Primitives.PathCommands.AddRoundRectPathCommand addRoundRectPathCommand:
                                            {
                                                // TODO:
                                            }
                                            break;

                                        case SP.Primitives.PathCommands.AddOvalPathCommand addOvalPathCommand:
                                            {
                                                var style = ToStyle(drawPathCanvasCommand.Paint, factory);
                                                var ellipseShape = factory.CreateEllipseShape(
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

                                        case SP.Primitives.PathCommands.AddCirclePathCommand addCirclePathCommand:
                                            {
                                                var style = ToStyle(drawPathCanvasCommand.Paint, factory);
                                                var x = addCirclePathCommand.X;
                                                var y = addCirclePathCommand.Y;
                                                var radius = addCirclePathCommand.Radius;
                                                var ellipseShape = factory.CreateEllipseShape(
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

                                        case SP.Primitives.PathCommands.AddPolyPathCommand addPolyPathCommand:
                                            {
                                                if (addPolyPathCommand.Points is { })
                                                {
                                                    var polyGeometry = ToPathGeometry(
                                                        addPolyPathCommand,
                                                        drawPathCanvasCommand.Path.FillType,
                                                        IsFilled(drawPathCanvasCommand.Paint),
                                                        addPolyPathCommand.Close, factory);
                                                    if (polyGeometry is { })
                                                    {
                                                        var style = ToStyle(drawPathCanvasCommand.Paint, factory);
                                                        var pathShape = factory.CreatePathShape(
                                                            "Path",
                                                            style,
                                                            polyGeometry,
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

                                if (drawPathCanvasCommand.Path.Commands?.Count == 2)
                                {
                                    var pathCommand1 = drawPathCanvasCommand.Path.Commands[0];
                                    var pathCommand2 = drawPathCanvasCommand.Path.Commands[1];

                                    if (pathCommand1 is SP.Primitives.PathCommands.MoveToPathCommand moveTo && pathCommand2 is SP.Primitives.PathCommands.LineToPathCommand lineTo)
                                    {
                                        var style = ToStyle(drawPathCanvasCommand.Paint, factory);
                                        var pathShape = factory.CreateLineShape(
                                            moveTo.X, moveTo.Y,
                                            lineTo.X, lineTo.Y,
                                            style,
                                            IsStroked(drawPathCanvasCommand.Paint));
                                        shapes.Add(pathShape);
                                        break;
                                    }
                                }

                                var geometry = ToPathGeometry(drawPathCanvasCommand.Path, IsFilled(drawPathCanvasCommand.Paint), factory);
                                if (geometry is { })
                                {
                                    var style = ToStyle(drawPathCanvasCommand.Paint, factory);
                                    var pathShape = factory.CreatePathShape(
                                        "Path",
                                        style,
                                        geometry,
                                        IsStroked(drawPathCanvasCommand.Paint),
                                        IsFilled(drawPathCanvasCommand.Paint));
                                    shapes.Add(pathShape);
                                }
                            }
                        }
                        break;

                    case SP.Primitives.CanvasCommands.DrawTextBlobCanvasCommand drawTextBlobCanvasCommand:
                        {
                            // TODO:
                        }
                        break;

                    case SP.Primitives.CanvasCommands.DrawTextCanvasCommand drawTextCanvasCommand:
                        {
                            if (drawTextCanvasCommand.Paint is { })
                            {
                                var style = ToStyle(drawTextCanvasCommand.Paint, factory);
                                var pathShape = factory.CreateTextShape(
                                    drawTextCanvasCommand.X,
                                    drawTextCanvasCommand.Y,
                                    style,
                                    drawTextCanvasCommand.Text,
                                    IsFilled(drawTextCanvasCommand.Paint));
                                shapes.Add(pathShape);
                            }
                        }
                        break;

                    case SP.Primitives.PathCommands.DrawTextOnPathCanvasCommand drawTextOnPathCanvasCommand:
                        {
                            // TODO:
                        }
                        break;

                    default:
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

        private IList<BaseShapeViewModel> Convert(Svg.SvgDocument document, out double width, out double height)
        {
            var picture = SP.SvgModelExtensions.ToModel(document, _assetLoader);
            if (picture is null)
            {
                width = double.NaN;
                height = double.NaN;
                return null;
            }

            var shapes = new List<BaseShapeViewModel>();
            var factory = _serviceProvider.GetService<IFactory>();

            ToShape(picture, shapes, factory);

            var group = factory.CreateGroupShape("svg");

            group.Shapes = group.Shapes.AddRange(shapes);

            width = picture.CullRect.Width;
            height = picture.CullRect.Height;
            return Enumerable.Repeat<BaseShapeViewModel>(group, 1).ToList();
        }

        public IList<BaseShapeViewModel> Convert(string path, out double width, out double height)
        {
            var document = SP.SvgModelExtensions.Open(path);
            if (document is null)
            {
                width = double.NaN;
                height = double.NaN;
                return null;
            }

            return Convert(document, out width, out height);
        }

        public IList<BaseShapeViewModel> FromString(string text, out double width, out double height)
        {
            using var stream = ToStream(text);
            var document = SP.SvgModelExtensions.Open(stream);
            if (document is null)
            {
                width = double.NaN;
                height = double.NaN;
                return null;
            }
            return Convert(document, out width, out height);
        }
    }
}
