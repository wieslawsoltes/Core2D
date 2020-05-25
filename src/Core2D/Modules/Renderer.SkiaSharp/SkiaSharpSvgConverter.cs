using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using Core2D.Editor;
using Core2D.Path;
using Core2D.Shapes;
using Core2D.Style;
using Svg.Skia;
using SP = Svg.Picture;

namespace Core2D.Renderer.SkiaSharp
{
    /// <summary>
    /// Svg converter.
    /// </summary>
    public class SkiaSharpSvgConverter : ISvgConverter
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkiaSharpSvgConverter"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public SkiaSharpSvgConverter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private static bool IsStroked(SP.Paint paint)
        {
            if (paint == null)
            {
                return false;
            }
            return paint.Style == SP.PaintStyle.Stroke || paint.Style == SP.PaintStyle.StrokeAndFill;
        }

        private static bool IsFilled(SP.Paint paint)
        {
            if (paint == null)
            {
                return false;
            }
            return paint.Style == SP.PaintStyle.Fill || paint.Style == SP.PaintStyle.StrokeAndFill;
        }

        private static IArgbColor ToArgbColor(SP.ColorShader colorShader, IFactory factory)
        {
            return factory.CreateArgbColor(
                colorShader.Color.Alpha,
                colorShader.Color.Red,
                colorShader.Color.Green,
                colorShader.Color.Blue);
        }

        private static LineCap ToLineCap(SP.StrokeCap strokeCap)
        {
            switch (strokeCap)
            {
                default:
                case SP.StrokeCap.Butt:
                    return LineCap.Flat;
                case SP.StrokeCap.Round:
                    return LineCap.Round;
                case SP.StrokeCap.Square:
                    return LineCap.Square;
            }
        }

        public static TextHAlignment ToTextHAlignment(SP.TextAlign textAlign)
        {
            switch (textAlign)
            {
                default:
                case SP.TextAlign.Left:
                    return TextHAlignment.Left;
                case SP.TextAlign.Center:
                    return TextHAlignment.Center;
                case SP.TextAlign.Right:
                    return TextHAlignment.Right;
            }
        }

        private static IShapeStyle ToStyle(SP.Paint paint, IFactory factory)
        {
            var style = factory.CreateShapeStyle("Style");

            if (paint == null)
            {
                return style;
            }

            switch (paint.Shader)
            {
                case SP.ColorShader colorShader:
                    style.Stroke = ToArgbColor(colorShader, factory);
                    style.Fill = ToArgbColor(colorShader, factory);
                    break;
                case SP.LinearGradientShader linearGradientShader:
                    // TODO:
                    break;
                case SP.TwoPointConicalGradientShader twoPointConicalGradientShader:
                    // TODO:
                    break;
                case SP.PictureShader pictureShader:
                    // TODO:
                    break;
                default:
                    break;
            }

            style.Thickness = paint.StrokeWidth;

            style.LineCap = ToLineCap(paint.StrokeCap);

            if (paint.PathEffect is SP.DashPathEffect dashPathEffect && dashPathEffect.Intervals != null)
            {
                style.Dashes = StyleHelper.ConvertFloatArrayToDashes(dashPathEffect.Intervals);
                style.DashOffset = dashPathEffect.Phase;
            }

            if (paint.Typeface != null)
            {
                style.TextStyle.FontName = paint.Typeface.FamilyName;
                style.TextStyle.FontSize = paint.TextSize;
                style.TextStyle.TextHAlignment = ToTextHAlignment(paint.TextAlign);

                if (paint.Typeface.Weight == SP.FontStyleWeight.Bold)
                {
                    style.TextStyle.FontStyle.Bold = true;
                }

                if (paint.Typeface.Style == SP.FontStyleSlant.Italic)
                {
                    style.TextStyle.FontStyle.Italic = true;
                }
            }

            return style;
        }

        public static IPathGeometry ToPathGeometry(SP.Path path, bool isFilled, IFactory factory)
        {
            if (path.Commands == null)
            {
                return null;
            }

            var geometry = factory.CreatePathGeometry(
                ImmutableArray.Create<IPathFigure>(),
                path.FillType == SP.PathFillType.EvenOdd ? FillRule.EvenOdd : FillRule.Nonzero);

            var context = factory.CreateGeometryContext(geometry);

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
                            var point = factory.CreatePointShape(x, y);
                            context.BeginFigure(point, isFilled, false);
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
                            var point = factory.CreatePointShape(x, y);
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
                            var point = factory.CreatePointShape(x, y);
                            var rx = arcToPathCommand.Rx;
                            var ry = arcToPathCommand.Ry;
                            var size = factory.CreatePathSize(rx, ry);
                            var rotationAngle = arcToPathCommand.XAxisRotate;
                            var isLargeArc = arcToPathCommand.LargeArc == SP.PathArcSize.Large;
                            var sweep = arcToPathCommand.Sweep == SP.PathDirection.Clockwise ? SweepDirection.Clockwise : SweepDirection.Counterclockwise;
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
                            var control = factory.CreatePointShape(x0, y0);
                            var endPoint = factory.CreatePointShape(x1, y1);
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
                            var point1 = factory.CreatePointShape(x0, y0);
                            var point2 = factory.CreatePointShape(x1, y1);
                            var point3 = factory.CreatePointShape(x2, y2);
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

        public static IPathGeometry ToPathGeometry(SP.AddPolyPathCommand addPolyPathCommand, SP.PathFillType fillType, bool isFilled, bool isClosed, IFactory factory)
        {
            if (addPolyPathCommand.Points == null || addPolyPathCommand.Points.Count < 2)
            {
                return null;
            }

            var geometry = factory.CreatePathGeometry(
                ImmutableArray.Create<IPathFigure>(),
                fillType == SP.PathFillType.EvenOdd ? FillRule.EvenOdd : FillRule.Nonzero);

            var context = factory.CreateGeometryContext(geometry);

            var startX = addPolyPathCommand.Points[0].X;
            var startY = addPolyPathCommand.Points[0].Y;
            var startPoint = factory.CreatePointShape(startX, startY);
            context.BeginFigure(startPoint, isFilled, false);

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

        private static void ToShape(SP.Picture picture, List<IBaseShape> shapes, IFactory factory)
        {
            foreach (var canvasCommand in picture.Commands)
            {
                switch (canvasCommand)
                {
                    case SP.ClipPathCanvasCommand clipPathCanvasCommand:
                        {
                            // TODO:
                        }
                        break;
                    case SP.ClipRectCanvasCommand clipRectCanvasCommand:
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
                    case SP.SetMatrixCanvasCommand setMatrixCanvasCommand:
                        {
                            // TODO:
                        }
                        break;
                    case SP.SaveLayerCanvasCommand saveLayerCanvasCommand:
                        {
                            // TODO:
                        }
                        break;
                    case SP.DrawImageCanvasCommand drawImageCanvasCommand:
                        {
                            // TODO:
                        }
                        break;
                    case SP.DrawPathCanvasCommand drawPathCanvasCommand:
                        {
                            if (drawPathCanvasCommand.Path != null && drawPathCanvasCommand.Paint != null)
                            {
                                if (drawPathCanvasCommand.Path.Commands?.Count == 1)
                                {
                                    var pathCommand = drawPathCanvasCommand.Path.Commands[0];
                                    var success = false;

                                    switch (pathCommand)
                                    {
                                        case SP.AddRectPathCommand addRectPathCommand:
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
                                        case SP.AddRoundRectPathCommand addRoundRectPathCommand:
                                            {
                                                // TODO:
                                            }
                                            break;
                                        case SP.AddOvalPathCommand addOvalPathCommand:
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
                                        case SP.AddCirclePathCommand addCirclePathCommand:
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
                                        case SP.AddPolyPathCommand addPolyPathCommand:
                                            {
                                                if (addPolyPathCommand.Points != null)
                                                {
                                                    var polyGeometry = ToPathGeometry(
                                                        addPolyPathCommand,
                                                        drawPathCanvasCommand.Path.FillType,
                                                        IsFilled(drawPathCanvasCommand.Paint),
                                                        addPolyPathCommand.Close, factory);
                                                    if (polyGeometry != null)
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

                                    if (pathCommand1 is SP.MoveToPathCommand moveTo && pathCommand2 is SP.LineToPathCommand lineTo)
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
                                if (geometry != null)
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
                    case SP.DrawPositionedTextCanvasCommand drawPositionedTextCanvasCommand:
                        {
                            // TODO:
                        }
                        break;
                    case SP.DrawTextCanvasCommand drawTextCanvasCommand:
                        {
                            if (drawTextCanvasCommand.Paint != null)
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
                    case SP.DrawTextOnPathCanvasCommand drawTextOnPathCanvasCommand:
                        {
                            // TODO:
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        /// <inheritdoc/>
        public IList<IBaseShape> Convert(string path)
        {
            var document = SKSvg.Open(path);
            if (document == null)
            {
                return null;
            }

            var picture = SKSvg.ToModel(document);
            if (picture == null)
            {
                return null;
            }

            var shapes = new List<IBaseShape>();
            var factory = _serviceProvider.GetService<IFactory>();

            ToShape(picture, shapes, factory);

            var group = factory.CreateGroupShape("svg");

            group.Shapes = group.Shapes.AddRange(shapes);

            return Enumerable.Repeat<IBaseShape>(group, 1).ToList();
        }
    }
}
