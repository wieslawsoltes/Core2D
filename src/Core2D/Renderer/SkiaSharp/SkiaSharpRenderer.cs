using System;
using System.Collections.Generic;
using Core2D.Containers;
using Core2D.Interfaces;
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
        public SkiaSharpRenderer(IServiceProvider serviceProvider, bool isAntialias = true, double targetDpi = 72.0)
        {
            _serviceProvider = serviceProvider;
            _state = _serviceProvider.GetService<IFactory>().CreateShapeRendererState();
            _biCache = _serviceProvider.GetService<IFactory>().CreateCache<string, SKBitmap>(bi => bi.Dispose());
            _isAntialias = isAntialias;
            _scaleToPage = (value) => (float)(value * 1.0);
            _targetDpi = targetDpi;
            ClearCache(isZooming: false);
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

        internal static void GetSKPaint(string text, IShapeStyle shapeStyle, IPointShape topLeft, IPointShape bottomRight, double dx, double dy, Func<double, float> scale, double sourceDpi, double targetDpi, bool isAntialias, out SKPaint pen, out SKPoint origin)
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

                if (shapeStyle.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Underline))
                {
                    // TODO: Add support for FontStyleFlags.Underline
                }

                if (shapeStyle.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Strikeout))
                {
                    // TODO: Add support for FontStyleFlags.Strikeout
                }
            }
            pen = ToSKPaintBrush(shapeStyle.Stroke, isAntialias);
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
            var rect = CreateRect(topLeft, bottomRight, dx, dy, scale);
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

        internal static SKPaint ToSKPaintPen(IBaseStyle style, Func<double, float> scale, double sourceDpi, double targetDpi, bool isAntialias)
        {
            var strokeWidth = scale(style.Thickness * targetDpi / sourceDpi);
            var pathEffect = style.Dashes != null ? 
                SKPathEffect.CreateDash(
                    StyleHelper.ConvertDashesToFloatArray(style.Dashes, strokeWidth), 
                    (float)style.DashOffset) : 
                null;
            return new SKPaint()
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = isAntialias,
                IsStroke = true,
                StrokeWidth = strokeWidth,
                Color = ToSKColor(style.Stroke),
                StrokeCap = ToStrokeCap(style),
                PathEffect = pathEffect
            };
        }

        internal static SKPaint ToSKPaintBrush(IColor color, bool isAntialias)
        {
            return new SKPaint()
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = isAntialias,
                IsStroke = false,
                LcdRenderText = true,
                SubpixelText = true,
                Color = ToSKColor(color)
            };
        }

        internal static SKRect ToSKRect(double x, double y, double width, double height)
        {
            float left = (float)x;
            float top = (float)y;
            float right = (float)(x + width);
            float bottom = (float)(y + height);
            return new SKRect(left, top, right, bottom);
        }

        internal static SKRect CreateRect(IPointShape tl, IPointShape br, double dx, double dy, Func<double, float> scale)
        {
            double tlx = Math.Min(tl.X, br.X);
            double tly = Math.Min(tl.Y, br.Y);
            double brx = Math.Max(tl.X, br.X);
            double bry = Math.Max(tl.Y, br.Y);
            return new SKRect(
                scale(tlx + dx),
                scale(tly + dy),
                scale(brx + dx),
                scale(bry + dy));
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

        private void DrawLineArrowsInternal(SKCanvas canvas, ILineShape line, double dx, double dy, out SKPoint pt1, out SKPoint pt2)
        {
            using var fillStartArrow = ToSKPaintBrush(line.Style.StartArrowStyle.Fill, _isAntialias);
            using var strokeStartArrow = ToSKPaintPen(line.Style.StartArrowStyle, _scaleToPage, _sourceDpi, _targetDpi, _isAntialias);
            using var fillEndArrow = ToSKPaintBrush(line.Style.EndArrowStyle.Fill, _isAntialias);
            using var strokeEndArrow = ToSKPaintPen(line.Style.EndArrowStyle, _scaleToPage, _sourceDpi, _targetDpi, _isAntialias);
            double _x1 = line.Start.X + dx;
            double _y1 = line.Start.Y + dy;
            double _x2 = line.End.X + dx;
            double _y2 = line.End.Y + dy;

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
            using var brush = ToSKPaintBrush(color, _isAntialias);
            SKRect srect = SKRect.Create(
                _scaleToPage(rect.X),
                _scaleToPage(rect.Y),
                _scaleToPage(rect.Width),
                _scaleToPage(rect.Height));
            canvas.DrawRect(srect, brush);
        }

        private SKMatrix ToSKMatrix(IMatrixObject matrix)
        {
            return MatrixHelper.ToSKMatrix(
                matrix.M11, matrix.M12,
                matrix.M21, matrix.M22,
                matrix.OffsetX, matrix.OffsetY);
        }

        /// <inheritdoc/>
        public void InvalidateCache(IShapeStyle style)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void InvalidateCache(IMatrixObject matrix)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void InvalidateCache(IBaseShape shape, IShapeStyle style, double dx, double dy)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void ClearCache(bool isZooming)
        {
            if (!isZooming)
            {
                _biCache.Reset();
            }
        }

        /// <inheritdoc/>
        public void Fill(object dc, double x, double y, double width, double height, IColor color)
        {
            var canvas = dc as SKCanvas;
            var rect = SKRect.Create((float)x, (float)y, (float)width, (float)height);
            using var paint = ToSKPaintBrush(color, _isAntialias);
            canvas.DrawRect(rect, paint);
        }

        /// <inheritdoc/>
        public object PushMatrix(object dc, IMatrixObject matrix)
        {
            var canvas = dc as SKCanvas;
            int count = canvas.Save();
            canvas.SetMatrix(MatrixHelper.Multiply(ToSKMatrix(matrix), canvas.TotalMatrix));
            return count;
        }

        /// <inheritdoc/>
        public void PopMatrix(object dc, object state)
        {
            var canvas = dc as SKCanvas;
            var count = (int)state;
            canvas.RestoreToCount(count);
        }

        /// <inheritdoc/>
        public void Draw(object dc, IPageContainer container, double dx, double dy)
        {
            foreach (var layer in container.Layers)
            {
                if (layer.IsVisible)
                {
                    Draw(dc, layer, dx, dy);
                }
            }
        }

        /// <inheritdoc/>
        public void Draw(object dc, ILayerContainer layer, double dx, double dy)
        {
            foreach (var shape in layer.Shapes)
            {
                if (shape.State.Flags.HasFlag(State.DrawShapeState.Flags))
                {
                    shape.Draw(dc, this, dx, dy);
                }
            }
        }

        /// <inheritdoc/>
        public void Draw(object dc, IPointShape point, double dx, double dy)
        {
            if (point != null && _state != null && _state.PointShape != null)
            {
                _state.PointShape.Draw(
                    dc, 
                    this, 
                    point.X + dx, 
                    point.Y + dy);
            }
        }

        /// <inheritdoc/>
        public void Draw(object dc, ILineShape line, double dx, double dy)
        {
            var canvas = dc as SKCanvas;

            using var strokeLine = ToSKPaintPen(line.Style, _scaleToPage, _sourceDpi, _targetDpi, _isAntialias);
            DrawLineArrowsInternal(canvas, line, dx, dy, out var pt1, out var pt2);

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
        public void Draw(object dc, IRectangleShape rectangle, double dx, double dy)
        {
            var canvas = dc as SKCanvas;

            using var brush = ToSKPaintBrush(rectangle.Style.Fill, _isAntialias);
            using var pen = ToSKPaintPen(rectangle.Style, _scaleToPage, _sourceDpi, _targetDpi, _isAntialias);
            var rect = CreateRect(rectangle.TopLeft, rectangle.BottomRight, dx, dy, _scaleToPage);
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
        }

        /// <inheritdoc/>
        public void Draw(object dc, IEllipseShape ellipse, double dx, double dy)
        {
            var canvas = dc as SKCanvas;

            using var brush = ToSKPaintBrush(ellipse.Style.Fill, _isAntialias);
            using var pen = ToSKPaintPen(ellipse.Style, _scaleToPage, _sourceDpi, _targetDpi, _isAntialias);
            var rect = CreateRect(ellipse.TopLeft, ellipse.BottomRight, dx, dy, _scaleToPage);
            DrawEllipseInternal(canvas, brush, pen, ellipse.IsStroked, ellipse.IsFilled, ref rect);
        }

        /// <inheritdoc/>
        public void Draw(object dc, IArcShape arc, double dx, double dy)
        {
            var canvas = dc as SKCanvas;

            using var brush = ToSKPaintBrush(arc.Style.Fill, _isAntialias);
            using var pen = ToSKPaintPen(arc.Style, _scaleToPage, _sourceDpi, _targetDpi, _isAntialias);
            using var path = new SKPath();
            var a = new GdiArc(
                Point2.FromXY(arc.Point1.X, arc.Point1.Y),
                Point2.FromXY(arc.Point2.X, arc.Point2.Y),
                Point2.FromXY(arc.Point3.X, arc.Point3.Y),
                Point2.FromXY(arc.Point4.X, arc.Point4.Y));
            var rect = new SKRect(
                _scaleToPage(a.X + dx),
                _scaleToPage(a.Y + dy),
                _scaleToPage(a.X + dx + a.Width),
                _scaleToPage(a.Y + dy + a.Height));
            path.AddArc(rect, (float)a.StartAngle, (float)a.SweepAngle);
            DrawPathInternal(canvas, brush, pen, arc.IsStroked, arc.IsFilled, path);
        }

        /// <inheritdoc/>
        public void Draw(object dc, ICubicBezierShape cubicBezier, double dx, double dy)
        {
            var canvas = dc as SKCanvas;

            using var brush = ToSKPaintBrush(cubicBezier.Style.Fill, _isAntialias);
            using var pen = ToSKPaintPen(cubicBezier.Style, _scaleToPage, _sourceDpi, _targetDpi, _isAntialias);
            using var path = new SKPath();
            path.MoveTo(
                _scaleToPage(cubicBezier.Point1.X + dx),
                _scaleToPage(cubicBezier.Point1.Y + dy));
            path.CubicTo(
                _scaleToPage(cubicBezier.Point2.X + dx),
                _scaleToPage(cubicBezier.Point2.Y + dy),
                _scaleToPage(cubicBezier.Point3.X + dx),
                _scaleToPage(cubicBezier.Point3.Y + dy),
                _scaleToPage(cubicBezier.Point4.X + dx),
                _scaleToPage(cubicBezier.Point4.Y + dy));
            DrawPathInternal(canvas, brush, pen, cubicBezier.IsStroked, cubicBezier.IsFilled, path);
        }

        /// <inheritdoc/>
        public void Draw(object dc, IQuadraticBezierShape quadraticBezier, double dx, double dy)
        {
            var canvas = dc as SKCanvas;

            using var brush = ToSKPaintBrush(quadraticBezier.Style.Fill, _isAntialias);
            using var pen = ToSKPaintPen(quadraticBezier.Style, _scaleToPage, _sourceDpi, _targetDpi, _isAntialias);
            using var path = new SKPath();
            path.MoveTo(
                _scaleToPage(quadraticBezier.Point1.X + dx),
                _scaleToPage(quadraticBezier.Point1.Y + dy));
            path.QuadTo(
                _scaleToPage(quadraticBezier.Point2.X + dx),
                _scaleToPage(quadraticBezier.Point2.Y + dy),
                _scaleToPage(quadraticBezier.Point3.X + dx),
                _scaleToPage(quadraticBezier.Point3.Y + dy));
            DrawPathInternal(canvas, brush, pen, quadraticBezier.IsStroked, quadraticBezier.IsFilled, path);
        }

        /// <inheritdoc/>
        public void Draw(object dc, ITextShape text, double dx, double dy)
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

            GetSKPaint(tbind, text.Style, text.TopLeft, text.BottomRight, dx, dy, _scaleToPage, _sourceDpi, _targetDpi, _isAntialias, out var pen, out var origin);

            canvas.DrawText(tbind, origin.X, origin.Y, pen);
        }

        /// <inheritdoc/>
        public void Draw(object dc, IImageShape image, double dx, double dy)
        {
            var canvas = dc as SKCanvas;

            var rect = CreateRect(image.TopLeft, image.BottomRight, dx, dy, _scaleToPage);

            if (image.IsStroked || image.IsFilled)
            {
                using var brush = ToSKPaintBrush(image.Style.Fill, _isAntialias);
                using var pen = ToSKPaintPen(image.Style, _scaleToPage, _sourceDpi, _targetDpi, _isAntialias);
                DrawRectangleInternal(canvas, brush, pen, image.IsStroked, image.IsFilled, ref rect);
            }

            var imageCached = _biCache.Get(image.Key);
            if (imageCached != null)
            {
                canvas.DrawBitmap(imageCached, rect);
            }
            else
            {
                if (State.ImageCache == null || string.IsNullOrEmpty(image.Key))
                {
                    return;
                }

                var bytes = State.ImageCache.GetImage(image.Key);
                if (bytes != null)
                {
                    var bi = SKBitmap.Decode(bytes);

                    _biCache.Set(image.Key, bi);

                    canvas.DrawBitmap(bi, rect);
                }
            }
        }

        /// <inheritdoc/>
        public void Draw(object dc, IPathShape path, double dx, double dy)
        {
            var canvas = dc as SKCanvas;

            using var brush = ToSKPaintBrush(path.Style.Fill, _isAntialias);
            using var pen = ToSKPaintPen(path.Style, _scaleToPage, _sourceDpi, _targetDpi, _isAntialias);
            using var spath = path.Geometry.ToSKPath(dx, dy, _scaleToPage);
            DrawPathInternal(canvas, brush, pen, path.IsStroked, path.IsFilled, spath);
        }

        /// <summary>
        /// Check whether the <see cref="State"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeState() => _state != null;
    }
}
