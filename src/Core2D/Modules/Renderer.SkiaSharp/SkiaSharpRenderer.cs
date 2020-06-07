using System;
using System.Collections.Generic;
using Core2D;
using Core2D.Containers;
using Core2D.Shapes;
using Core2D.Style;
using SkiaSharp;
using Spatial;
using Spatial.Arc;

namespace Core2D.Renderer.SkiaSharp
{
    /// <summary>
    /// Native SkiaSharp shape renderer.
    /// </summary>
    public class SkiaSharpRenderer : ObservableObject, IShapeRenderer
    {
        private readonly IServiceProvider _serviceProvider;
        private IShapeRendererState _state;
        private bool _isAntialias = true;
        private ICache<string, SKBitmap> _biCache;
        private ICache<IColor, SKPaint> _fillCache;
        private ICache<IShapeStyle, SKPaint> _strokeCache;
        private ICache<IArrowStyle, SKPaint> _arrowCache;
        private ICache<IShapeStyle, SKPaint> _textCache;
        private readonly Func<double, float> _scaleToPage;
        private readonly double _sourceDpi = 96.0;
        private readonly double _targetDpi = 72.0;

        /// <inheritdoc/>
        public IShapeRendererState State
        {
            get => _state;
            set => Update(ref _state, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkiaSharpRenderer"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="isAntialias">The flag indicating whether paint is antialiased.</param>
        /// <param name="targetDpi">The target renderer dpi.</param>
        public SkiaSharpRenderer(IServiceProvider serviceProvider, bool isAntialias = true, double targetDpi = 96.0)
        {
            _serviceProvider = serviceProvider;
            _state = _serviceProvider.GetService<IFactory>().CreateShapeRendererState();
            _biCache = _serviceProvider.GetService<IFactory>().CreateCache<string, SKBitmap>(bi => bi.Dispose());
            _fillCache = _serviceProvider.GetService<IFactory>().CreateCache<IColor, SKPaint>(paint => paint.Dispose());
            _strokeCache = _serviceProvider.GetService<IFactory>().CreateCache<IShapeStyle, SKPaint>(paint => paint.Dispose());
            _arrowCache = _serviceProvider.GetService<IFactory>().CreateCache<IArrowStyle, SKPaint>(paint => paint.Dispose());
            _textCache = _serviceProvider.GetService<IFactory>().CreateCache<IShapeStyle, SKPaint>(paint => paint.Dispose());
            _isAntialias = isAntialias;
            _scaleToPage = (value) => (float)(value);
            _targetDpi = targetDpi;
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        internal static SKPoint GetTextOrigin(IShapeStyle style, ref SKRect rect, ref SKRect size)
        {
            double rwidth = Math.Abs(rect.Right - rect.Left);
            double rheight = Math.Abs(rect.Bottom - rect.Top);
            double swidth = Math.Abs(size.Right - size.Left);
            double sheight = Math.Abs(size.Bottom - size.Top);
            var ox = style.TextStyle.TextHAlignment switch
            {
                TextHAlignment.Left => rect.Left,
                TextHAlignment.Right => rect.Right - swidth,
                _ => (rect.Left + rwidth / 2f) - (swidth / 2f),
            };
            var oy = style.TextStyle.TextVAlignment switch
            {
                TextVAlignment.Top => rect.Top,
                TextVAlignment.Bottom => rect.Bottom - sheight,
                _ => (rect.Bottom - rheight / 2f) - (sheight / 2f),
            };
            return new SKPoint((float)ox, (float)oy);
        }

        internal static void GetSKPaint(string text, IShapeStyle shapeStyle, IPointShape topLeft, IPointShape bottomRight, Func<double, float> scale, double sourceDpi, double targetDpi, bool isAntialias, SKPaint pen, out SKPoint origin)
        {
            var weight = SKFontStyleWeight.Normal;
            if (shapeStyle.TextStyle.FontStyle != null)
            {
                if (shapeStyle.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Bold))
                {
                    weight |= SKFontStyleWeight.Bold;
                }
            }

            var style = SKFontStyleSlant.Upright;
            if (shapeStyle.TextStyle.FontStyle != null)
            {
                if (shapeStyle.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Italic))
                {
                    style |= SKFontStyleSlant.Italic;
                }
            }
            ToSKPaintBrush(shapeStyle.Stroke, isAntialias, pen);
            // TODO: Cache SKTypeface.
            var tf = SKTypeface.FromFamilyName(shapeStyle.TextStyle.FontName, weight, SKFontStyleWidth.Normal, style);
            pen.Typeface = tf;
            pen.TextEncoding = SKTextEncoding.Utf16;
            pen.TextSize = scale(shapeStyle.TextStyle.FontSize * targetDpi / sourceDpi);

            pen.TextAlign = shapeStyle.TextStyle.TextHAlignment switch
            {
                TextHAlignment.Center => SKTextAlign.Center,
                TextHAlignment.Right => SKTextAlign.Right,
                _ => SKTextAlign.Left,
            };

            var metrics = pen.FontMetrics;
            var mAscent = metrics.Ascent;
            var mDescent = metrics.Descent;
            var rect = CreateRect(topLeft, bottomRight, scale);
            float x = rect.Left;
            float y = rect.Top;
            float width = rect.Width;
            float height = rect.Height;

            switch (shapeStyle.TextStyle.TextVAlignment)
            {
                default:
                case TextVAlignment.Top:
                    y -= mAscent;
                    break;
                case TextVAlignment.Center:
                    y += (height / 2.0f) - (mAscent / 2.0f) - mDescent / 2.0f;
                    break;
                case TextVAlignment.Bottom:
                    y += height - mDescent;
                    break;
            }

            switch (shapeStyle.TextStyle.TextHAlignment)
            {
                default:
                case TextHAlignment.Left:
                    // x = x;
                    break;
                case TextHAlignment.Center:
                    x += width / 2.0f;
                    break;
                case TextHAlignment.Right:
                    x += width;
                    break;
            }

            origin = new SKPoint(x, y);
        }

        internal static SKColor ToSKColor(IColor color)
        {
            return color switch
            {
                IArgbColor argbColor => new SKColor(argbColor.R, argbColor.G, argbColor.B, argbColor.A),
                _ => throw new NotSupportedException($"The {color.GetType()} color type is not supported."),
            };
        }

        internal static SKStrokeCap ToStrokeCap(IBaseStyle style)
        {
            return style.LineCap switch
            {
                LineCap.Square => SKStrokeCap.Square,
                LineCap.Round => SKStrokeCap.Round,
                _ => SKStrokeCap.Butt,
            };
        }

        internal static void ToSKPaintPen(IBaseStyle style, Func<double, float> scale, double sourceDpi, double targetDpi, bool isAntialias, SKPaint pen)
        {
            var strokeWidth = scale(style.Thickness * targetDpi / sourceDpi);
            var pathEffect = default(SKPathEffect);
            if (style.Dashes != null)
            {
                var intervals = StyleHelper.ConvertDashesToFloatArray(style.Dashes, strokeWidth);
                var phase = (float)(style.DashOffset * strokeWidth);
                if (intervals != null)
                {
                    pathEffect = SKPathEffect.CreateDash(intervals, phase);
                }
            }

            pen.Style = SKPaintStyle.Stroke;
            pen.IsAntialias = isAntialias;
            pen.IsStroke = true;
            pen.StrokeWidth = strokeWidth;
            pen.Color = ToSKColor(style.Stroke);
            pen.StrokeCap = ToStrokeCap(style);
            pen.PathEffect = pathEffect;
        }

        internal static void ToSKPaintBrush(IColor color, bool isAntialias, SKPaint brush)
        {
            brush.Style = SKPaintStyle.Fill;
            brush.IsAntialias = isAntialias;
            brush.IsStroke = false;
            brush.LcdRenderText = true;
            brush.SubpixelText = true;
            brush.Color = ToSKColor(color);
        }

        internal static SKRect ToSKRect(double x, double y, double width, double height)
        {
            float left = (float)x;
            float top = (float)y;
            float right = (float)(x + width);
            float bottom = (float)(y + height);
            return new SKRect(left, top, right, bottom);
        }

        internal static SKRect CreateRect(IPointShape tl, IPointShape br, Func<double, float> scale)
        {
            double tlx = Math.Min(tl.X, br.X);
            double tly = Math.Min(tl.Y, br.Y);
            double brx = Math.Max(tl.X, br.X);
            double bry = Math.Max(tl.Y, br.Y);
            return new SKRect(
                scale(tlx),
                scale(tly),
                scale(brx),
                scale(bry));
        }

        private void DrawLineInternal(SKCanvas canvas, SKPaint pen, bool isStroked, ref SKPoint p0, ref SKPoint p1)
        {
            if (isStroked)
            {
                canvas.DrawLine(p0.X, p0.Y, p1.X, p1.Y, pen);
            }
        }

        private void DrawLineCurveInternal(SKCanvas canvas, SKPaint pen, bool isStroked, ref SKPoint pt1, ref SKPoint pt2, double curvature, CurveOrientation orientation, PointAlignment pt1a, PointAlignment pt2a)
        {
            if (isStroked)
            {
                using var path = new SKPath();
                path.MoveTo(pt1.X, pt1.Y);
                double p1x = pt1.X;
                double p1y = pt1.Y;
                double p2x = pt2.X;
                double p2y = pt2.Y;
                LineShapeExtensions.GetCurvedLineBezierControlPoints(orientation, curvature, pt1a, pt2a, ref p1x, ref p1y, ref p2x, ref p2y);
                path.CubicTo(
                    (float)p1x,
                    (float)p1y,
                    (float)p2x,
                    (float)p2y,
                    pt2.X, pt2.Y);
                canvas.DrawPath(path, pen);
            }
        }

        private void DrawLineArrowsInternal(SKCanvas canvas, ILineShape line, out SKPoint pt1, out SKPoint pt2)
        {
            var fillStartArrow = _fillCache.Get(line.Style.StartArrowStyle.Fill);
            if (fillStartArrow == null)
            {
                fillStartArrow = new SKPaint();
                _fillCache.Set(line.Style.StartArrowStyle.Fill, fillStartArrow);
            }
            ToSKPaintBrush(line.Style.StartArrowStyle.Fill, _isAntialias, fillStartArrow);

            var strokeStartArrow = _arrowCache.Get(line.Style.StartArrowStyle);
            if (strokeStartArrow == null)
            {
                strokeStartArrow = new SKPaint();
                _arrowCache.Set(line.Style.StartArrowStyle, strokeStartArrow);
            }
            ToSKPaintPen(line.Style.StartArrowStyle, _scaleToPage, _sourceDpi, _targetDpi, _isAntialias, strokeStartArrow);

            var fillEndArrow = _fillCache.Get(line.Style.EndArrowStyle.Fill);
            if (fillEndArrow == null)
            {
                fillEndArrow = new SKPaint();
                _fillCache.Set(line.Style.EndArrowStyle.Fill, fillEndArrow);
            }
            ToSKPaintBrush(line.Style.EndArrowStyle.Fill, _isAntialias, fillEndArrow);

            var strokeEndArrow = _arrowCache.Get(line.Style.EndArrowStyle);
            if (strokeEndArrow == null)
            {
                strokeEndArrow = new SKPaint();
                _arrowCache.Set(line.Style.EndArrowStyle, strokeEndArrow);
            }
            ToSKPaintPen(line.Style.EndArrowStyle, _scaleToPage, _sourceDpi, _targetDpi, _isAntialias, strokeEndArrow);

            double _x1 = line.Start.X;
            double _y1 = line.Start.Y;
            double _x2 = line.End.X;
            double _y2 = line.End.Y;

            line.GetMaxLength(ref _x1, ref _y1, ref _x2, ref _y2);

            float x1 = _scaleToPage(_x1);
            float y1 = _scaleToPage(_y1);
            float x2 = _scaleToPage(_x2);
            float y2 = _scaleToPage(_y2);

            var sas = line.Style.StartArrowStyle;
            var eas = line.Style.EndArrowStyle;
            double a1 = Math.Atan2(y1 - y2, x1 - x2);
            double a2 = Math.Atan2(y2 - y1, x2 - x1);

            // Draw start arrow.
            pt1 = DrawLineArrowInternal(canvas, strokeStartArrow, fillStartArrow, x1, y1, a1, sas);

            // Draw end arrow.
            pt2 = DrawLineArrowInternal(canvas, strokeEndArrow, fillEndArrow, x2, y2, a2, eas);
        }

        private SKPoint DrawLineArrowInternal(SKCanvas canvas, SKPaint pen, SKPaint brush, float x, float y, double angle, IArrowStyle style)
        {
            SKPoint pt = default;
            var rt = MatrixHelper.Rotation(angle, new SKPoint(x, y));
            double rx = style.RadiusX;
            double ry = style.RadiusY;
            double sx = 2.0 * rx;
            double sy = 2.0 * ry;

            switch (style.ArrowType)
            {
                default:
                case ArrowType.None:
                    {
                        pt = new SKPoint(x, y);
                    }
                    break;
                case ArrowType.Rectangle:
                    {
                        pt = MatrixHelper.TransformPoint(rt, new SKPoint(x - (float)sx, y));
                        var rect = ToSKRect(x - sx, y - ry, sx, sy);
                        int count = canvas.Save();
                        canvas.SetMatrix(MatrixHelper.Multiply(rt, canvas.TotalMatrix));
                        DrawRectangleInternal(canvas, brush, pen, style.IsStroked, style.IsFilled, ref rect);
                        canvas.RestoreToCount(count);
                    }
                    break;
                case ArrowType.Ellipse:
                    {
                        pt = MatrixHelper.TransformPoint(rt, new SKPoint(x - (float)sx, y));
                        int count = canvas.Save();
                        canvas.SetMatrix(MatrixHelper.Multiply(rt, canvas.TotalMatrix));
                        var rect = ToSKRect(x - sx, y - ry, sx, sy);
                        DrawEllipseInternal(canvas, brush, pen, style.IsStroked, style.IsFilled, ref rect);
                        canvas.RestoreToCount(count);
                    }
                    break;
                case ArrowType.Arrow:
                    {
                        var pts = new SKPoint[]
                        {
                            new SKPoint(x, y),
                            new SKPoint(x - (float)sx, y + (float)sy),
                            new SKPoint(x, y),
                            new SKPoint(x - (float)sx, y - (float)sy),
                            new SKPoint(x, y)
                        };
                        pt = MatrixHelper.TransformPoint(rt, pts[0]);
                        var p11 = MatrixHelper.TransformPoint(rt, pts[1]);
                        var p21 = MatrixHelper.TransformPoint(rt, pts[2]);
                        var p12 = MatrixHelper.TransformPoint(rt, pts[3]);
                        var p22 = MatrixHelper.TransformPoint(rt, pts[4]);
                        DrawLineInternal(canvas, pen, style.IsStroked, ref p11, ref p21);
                        DrawLineInternal(canvas, pen, style.IsStroked, ref p12, ref p22);
                    }
                    break;
            }

            return pt;
        }

        private void DrawRectangleInternal(SKCanvas canvas, SKPaint brush, SKPaint pen, bool isStroked, bool isFilled, ref SKRect rect)
        {
            if (isFilled)
            {
                canvas.DrawRect(rect, brush);
            }

            if (isStroked)
            {
                canvas.DrawRect(rect, pen);
            }
        }

        private void DrawEllipseInternal(SKCanvas canvas, SKPaint brush, SKPaint pen, bool isStroked, bool isFilled, ref SKRect rect)
        {
            if (isFilled)
            {
                canvas.DrawOval(rect, brush);
            }

            if (isStroked)
            {
                canvas.DrawOval(rect, pen);
            }
        }

        private void DrawPathInternal(SKCanvas canvas, SKPaint brush, SKPaint pen, bool isStroked, bool isFilled, SKPath path)
        {
            if (isFilled)
            {
                canvas.DrawPath(path, brush);
            }

            if (isStroked)
            {
                canvas.DrawPath(path, pen);
            }
        }

        private void DrawGridInternal(SKCanvas canvas, SKPaint stroke, ref SKRect rect, double offsetX, double offsetY, double cellWidth, double cellHeight, bool isStroked)
        {
            float ox = rect.Left;
            float oy = rect.Top;
            float sx = (float)(ox + offsetX);
            float sy = (float)(oy + offsetY);
            float ex = ox + (rect.Right - rect.Left);
            float ey = oy + (rect.Bottom - rect.Top);

            for (float x = sx; x < ex; x += (float)cellWidth)
            {
                var p0 = new SKPoint(x, oy);
                var p1 = new SKPoint(x, ey);
                DrawLineInternal(canvas, stroke, isStroked, ref p0, ref p1);
            }

            for (float y = sy; y < ey; y += (float)cellHeight)
            {
                var p0 = new SKPoint(ox, y);
                var p1 = new SKPoint(ex, y);
                DrawLineInternal(canvas, stroke, isStroked, ref p0, ref p1);
            }
        }

        private void DrawBackgroundInternal(SKCanvas canvas, IColor color, Rect2 rect)
        {
            var brush = _fillCache.Get(color);
            if (brush == null)
            {
                brush = new SKPaint();
                _fillCache.Set(color, brush);
            }
            ToSKPaintBrush(color, _isAntialias, brush);

            SKRect srect = SKRect.Create(
                _scaleToPage(rect.X),
                _scaleToPage(rect.Y),
                _scaleToPage(rect.Width),
                _scaleToPage(rect.Height));
            canvas.DrawRect(srect, brush);
        }

        /// <inheritdoc/>
        public void ClearCache()
        {
            _biCache.Reset();
            _fillCache.Reset();
            _strokeCache.Reset();
            _arrowCache.Reset();
            _textCache.Reset();
        }

        /// <inheritdoc/>
        public void Fill(object dc, double x, double y, double width, double height, IColor color)
        {
            var canvas = dc as SKCanvas;
            var rect = SKRect.Create((float)x, (float)y, (float)width, (float)height);

            var paint = _fillCache.Get(color);
            if (paint == null)
            {
                paint = new SKPaint();
                _fillCache.Set(color, paint);
            }
            ToSKPaintBrush(color, _isAntialias, paint);

            canvas.DrawRect(rect, paint);
        }

        /// <inheritdoc/>
        public void DrawPage(object dc, IPageContainer container)
        {
            foreach (var layer in container.Layers)
            {
                if (layer.IsVisible)
                {
                    DrawLayer(dc, layer);
                }
            }
        }

        /// <inheritdoc/>
        public void DrawLayer(object dc, ILayerContainer layer)
        {
            foreach (var shape in layer.Shapes)
            {
                if (shape.State.Flags.HasFlag(State.DrawShapeState.Flags))
                {
                    shape.DrawShape(dc, this);
                }
            }

            foreach (var shape in layer.Shapes)
            {
                if (shape.State.Flags.HasFlag(_state.DrawShapeState.Flags))
                {
                    shape.DrawPoints(dc, this);
                }
            }
        }

        /// <inheritdoc/>
        public void DrawPoint(object dc, IPointShape point)
        {
            if (point == null || _state == null)
            {
                return;
            }

            bool isSelected = _state.SelectedShapes?.Count > 0 && _state.SelectedShapes.Contains(point);

            var pointStyle = isSelected ? _state.SelectedPointStyle : _state.PointStyle;
            if (pointStyle == null)
            {
                return;
            }

            var canvas = dc as SKCanvas;

            var pointSize = _state.PointSize;
            if (pointSize <= 0.0)
            {
                return;
            }

            //var scaleThickness = true; // point.State.Flags.HasFlag(ShapeStateFlags.Thickness); // TODO:
            var scaleSize = true; // point.State.Flags.HasFlag(ShapeStateFlags.Size); // TODO:
            var rect2 = Rect2.FromPoints(point.X - pointSize, point.Y - pointSize, point.X + pointSize, point.Y + pointSize, 0, 0);
            var rect = new SKRect((float)rect2.Left, (float)rect2.Top, (float)rect2.Right, (float)rect2.Bottom);

            var scale = scaleSize ? 1.0 / _state.ZoomX : 1.0;
            var scaleToPage = scale == 1.0 ? _scaleToPage : (value) => (float)(_scaleToPage(value) / scale);
            var center = point;
            var translateX = 0.0 - (center.X * scale) + center.X;
            var translateY = 0.0 - (center.Y * scale) + center.Y;

            var fill = _fillCache.Get(pointStyle.Fill);
            if (fill == null)
            {
                fill = new SKPaint();
                _fillCache.Set(pointStyle.Fill, fill);
            }
            ToSKPaintBrush(pointStyle.Fill, _isAntialias, fill);

            var stroke = _strokeCache.Get(pointStyle);
            if (stroke == null)
            {
                stroke = new SKPaint();
                _strokeCache.Set(pointStyle, stroke);
            }
            ToSKPaintPen(pointStyle, _scaleToPage, _sourceDpi, _targetDpi, _isAntialias, stroke);

            if (scale != 1.0)
            {
                canvas.Save();
                canvas.Translate((float)translateX, (float)translateY);
                canvas.Scale((float)scale, (float)scale);
            }

            DrawRectangleInternal(canvas, fill, stroke, true, true, ref rect);

            if (scale != 1.0)
            {
                canvas.Restore();
            }
        }

        /// <inheritdoc/>
        public void DrawLine(object dc, ILineShape line)
        {
            var canvas = dc as SKCanvas;

            var strokeLine = _strokeCache.Get(line.Style);
            if (strokeLine == null)
            {
                strokeLine = new SKPaint();
                _strokeCache.Set(line.Style, strokeLine);
            }
            ToSKPaintPen(line.Style, _scaleToPage, _sourceDpi, _targetDpi, _isAntialias, strokeLine);

            DrawLineArrowsInternal(canvas, line, out var pt1, out var pt2);

            if (line.Style.LineStyle.IsCurved)
            {
                DrawLineCurveInternal(
                    canvas,
                    strokeLine, line.IsStroked,
                    ref pt1, ref pt2,
                    line.Style.LineStyle.Curvature,
                    line.Style.LineStyle.CurveOrientation,
                    line.Start.Alignment,
                    line.End.Alignment);
            }
            else
            {
                DrawLineInternal(canvas, strokeLine, line.IsStroked, ref pt1, ref pt2);
            }
        }

        /// <inheritdoc/>
        public void DrawRectangle(object dc, IRectangleShape rectangle)
        {
            var canvas = dc as SKCanvas;

            var brush = _fillCache.Get(rectangle.Style.Fill);
            if (brush == null)
            {
                brush = new SKPaint();
                _fillCache.Set(rectangle.Style.Fill, brush);
            }
            ToSKPaintBrush(rectangle.Style.Fill, _isAntialias, brush);

            var pen = _strokeCache.Get(rectangle.Style);
            if (pen == null)
            {
                pen = new SKPaint();
                _strokeCache.Set(rectangle.Style, pen);
            }
            ToSKPaintPen(rectangle.Style, _scaleToPage, _sourceDpi, _targetDpi, _isAntialias, pen);

            var rect = CreateRect(rectangle.TopLeft, rectangle.BottomRight, _scaleToPage);
            DrawRectangleInternal(canvas, brush, pen, rectangle.IsStroked, rectangle.IsFilled, ref rect);
            if (rectangle.IsGrid)
            {
                DrawGridInternal(
                    canvas,
                    pen,
                    ref rect,
                    rectangle.OffsetX,
                    rectangle.OffsetY,
                    rectangle.CellWidth,
                    rectangle.CellHeight,
                    isStroked: true);
            }

            DrawText(dc, rectangle);
        }

        /// <inheritdoc/>
        public void DrawEllipse(object dc, IEllipseShape ellipse)
        {
            var canvas = dc as SKCanvas;

            var brush = _fillCache.Get(ellipse.Style.Fill);
            if (brush == null)
            {
                brush = new SKPaint();
                _fillCache.Set(ellipse.Style.Fill, brush);
            }
            ToSKPaintBrush(ellipse.Style.Fill, _isAntialias, brush);

            var pen = _strokeCache.Get(ellipse.Style);
            if (pen == null)
            {
                pen = new SKPaint();
                _strokeCache.Set(ellipse.Style, pen);
            }
            ToSKPaintPen(ellipse.Style, _scaleToPage, _sourceDpi, _targetDpi, _isAntialias, pen);

            var rect = CreateRect(ellipse.TopLeft, ellipse.BottomRight, _scaleToPage);
            DrawEllipseInternal(canvas, brush, pen, ellipse.IsStroked, ellipse.IsFilled, ref rect);

            DrawText(dc, ellipse);
        }

        /// <inheritdoc/>
        public void DrawArc(object dc, IArcShape arc)
        {
            var canvas = dc as SKCanvas;

            var brush = _fillCache.Get(arc.Style.Fill);
            if (brush == null)
            {
                brush = new SKPaint();
                _fillCache.Set(arc.Style.Fill, brush);
            }
            ToSKPaintBrush(arc.Style.Fill, _isAntialias, brush);

            var pen = _strokeCache.Get(arc.Style);
            if (pen == null)
            {
                pen = new SKPaint();
                _strokeCache.Set(arc.Style, pen);
            }
            ToSKPaintPen(arc.Style, _scaleToPage, _sourceDpi, _targetDpi, _isAntialias, pen);

            using var path = new SKPath();
            var a = new GdiArc(
                Point2.FromXY(arc.Point1.X, arc.Point1.Y),
                Point2.FromXY(arc.Point2.X, arc.Point2.Y),
                Point2.FromXY(arc.Point3.X, arc.Point3.Y),
                Point2.FromXY(arc.Point4.X, arc.Point4.Y));
            var rect = new SKRect(
                _scaleToPage(a.X),
                _scaleToPage(a.Y),
                _scaleToPage(a.X + a.Width),
                _scaleToPage(a.Y + a.Height));
            path.AddArc(rect, (float)a.StartAngle, (float)a.SweepAngle);
            DrawPathInternal(canvas, brush, pen, arc.IsStroked, arc.IsFilled, path);
        }

        /// <inheritdoc/>
        public void DrawCubicBezier(object dc, ICubicBezierShape cubicBezier)
        {
            var canvas = dc as SKCanvas;

            var brush = _fillCache.Get(cubicBezier.Style.Fill);
            if (brush == null)
            {
                brush = new SKPaint();
                _fillCache.Set(cubicBezier.Style.Fill, brush);
            }
            ToSKPaintBrush(cubicBezier.Style.Fill, _isAntialias, brush);

            var pen = _strokeCache.Get(cubicBezier.Style);
            if (pen == null)
            {
                pen = new SKPaint();
                _strokeCache.Set(cubicBezier.Style, pen);
            }
            ToSKPaintPen(cubicBezier.Style, _scaleToPage, _sourceDpi, _targetDpi, _isAntialias, pen);

            using var path = new SKPath();
            path.MoveTo(
                _scaleToPage(cubicBezier.Point1.X),
                _scaleToPage(cubicBezier.Point1.Y));
            path.CubicTo(
                _scaleToPage(cubicBezier.Point2.X),
                _scaleToPage(cubicBezier.Point2.Y),
                _scaleToPage(cubicBezier.Point3.X),
                _scaleToPage(cubicBezier.Point3.Y),
                _scaleToPage(cubicBezier.Point4.X),
                _scaleToPage(cubicBezier.Point4.Y));
            DrawPathInternal(canvas, brush, pen, cubicBezier.IsStroked, cubicBezier.IsFilled, path);
        }

        /// <inheritdoc/>
        public void DrawQuadraticBezier(object dc, IQuadraticBezierShape quadraticBezier)
        {
            var canvas = dc as SKCanvas;

            var brush = _fillCache.Get(quadraticBezier.Style.Fill);
            if (brush == null)
            {
                brush = new SKPaint();
                _fillCache.Set(quadraticBezier.Style.Fill, brush);
            }
            ToSKPaintBrush(quadraticBezier.Style.Fill, _isAntialias, brush);

            var pen = _strokeCache.Get(quadraticBezier.Style);
            if (pen == null)
            {
                pen = new SKPaint();
                _strokeCache.Set(quadraticBezier.Style, pen);
            }
            ToSKPaintPen(quadraticBezier.Style, _scaleToPage, _sourceDpi, _targetDpi, _isAntialias, pen);

            using var path = new SKPath();
            path.MoveTo(
                _scaleToPage(quadraticBezier.Point1.X),
                _scaleToPage(quadraticBezier.Point1.Y));
            path.QuadTo(
                _scaleToPage(quadraticBezier.Point2.X),
                _scaleToPage(quadraticBezier.Point2.Y),
                _scaleToPage(quadraticBezier.Point3.X),
                _scaleToPage(quadraticBezier.Point3.Y));
            DrawPathInternal(canvas, brush, pen, quadraticBezier.IsStroked, quadraticBezier.IsFilled, path);
        }

        /// <inheritdoc/>
        public void DrawText(object dc, ITextShape text)
        {
            var canvas = dc as SKCanvas;

            if (!(text.GetProperty(nameof(ITextShape.Text)) is string tbind))
            {
                tbind = text.Text;
            }

            if (tbind == null)
            {
                return;
            }

            var pen = _textCache.Get(text.Style);
            if (pen == null)
            {
                pen = new SKPaint();
                _textCache.Set(text.Style, pen);
            }
            GetSKPaint(tbind, text.Style, text.TopLeft, text.BottomRight, _scaleToPage, _sourceDpi, _targetDpi, _isAntialias, pen, out var origin);

            canvas.DrawText(tbind, origin.X, origin.Y, pen);
        }

        /// <inheritdoc/>
        public void DrawImage(object dc, IImageShape image)
        {
            var canvas = dc as SKCanvas;

            var rect = CreateRect(image.TopLeft, image.BottomRight, _scaleToPage);

            if (image.IsStroked || image.IsFilled)
            {
                var brush = _fillCache.Get(image.Style.Fill);
                if (brush == null)
                {
                    brush = new SKPaint();
                    _fillCache.Set(image.Style.Fill, brush);
                }
                ToSKPaintBrush(image.Style.Fill, _isAntialias, brush);

                var pen = _strokeCache.Get(image.Style);
                if (pen == null)
                {
                    pen = new SKPaint();
                    _strokeCache.Set(image.Style, pen);
                }
                ToSKPaintPen(image.Style, _scaleToPage, _sourceDpi, _targetDpi, _isAntialias, pen);

                DrawRectangleInternal(canvas, brush, pen, image.IsStroked, image.IsFilled, ref rect);
            }

            var imageCached = _biCache.Get(image.Key);
            if (imageCached != null)
            {
                canvas.DrawBitmap(imageCached, rect);
            }
            else
            {
                if (State.ImageCache != null && !string.IsNullOrEmpty(image.Key))
                {
                    var bytes = State.ImageCache.GetImage(image.Key);
                    if (bytes != null)
                    {
                        var bi = SKBitmap.Decode(bytes);

                        _biCache.Set(image.Key, bi);

                        canvas.DrawBitmap(bi, rect);
                    }
                }
            }

            DrawText(dc, image);
        }

        /// <inheritdoc/>
        public void DrawPath(object dc, IPathShape path)
        {
            var canvas = dc as SKCanvas;

            var brush = _fillCache.Get(path.Style.Fill);
            if (brush == null)
            {
                brush = new SKPaint();
                _fillCache.Set(path.Style.Fill, brush);
            }
            ToSKPaintBrush(path.Style.Fill, _isAntialias, brush);

            var pen = _strokeCache.Get(path.Style);
            if (pen == null)
            {
                pen = new SKPaint();
                _strokeCache.Set(path.Style, pen);
            }
            ToSKPaintPen(path.Style, _scaleToPage, _sourceDpi, _targetDpi, _isAntialias, pen);

            using var spath = path.Geometry.ToSKPath(_scaleToPage);
            DrawPathInternal(canvas, brush, pen, path.IsStroked, path.IsFilled, spath);
        }

        /// <summary>
        /// Check whether the <see cref="State"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeState() => _state != null;
    }
}
