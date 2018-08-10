// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using Core2D.Data;
using Core2D.Shapes.Interfaces;
using Core2D.Style;
using SkiaSharp;
using Spatial;
using Spatial.Arc;

namespace Core2D.Renderer.SkiaSharp
{
    /// <summary>
    /// Native SkiaSharp shape renderer.
    /// </summary>
    public class SkiaSharpRenderer : ShapeRenderer
    {
        private bool _isAntialias = true;
        private ICache<string, SKBitmap> _biCache = Cache<string, SKBitmap>.Create(bi => bi.Dispose());
        private Func<double, float> _scaleToPage;
        private double _sourceDpi = 96.0;
        private double _targetDpi = 72.0;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkiaSharpRenderer"/> class.
        /// </summary>
        /// <param name="isAntialias">The flag indicating whether paint is antialiased.</param>
        /// <param name="targetDpi">The target renderer dpi.</param>
        public SkiaSharpRenderer(bool isAntialias = true, double targetDpi = 72.0)
        {
            ClearCache(isZooming: false);
            _isAntialias = isAntialias;
            _scaleToPage = (value) => (float)(value * 1.0);
            _targetDpi = targetDpi;
        }

        /// <summary>
        /// Creates a new <see cref="SkiaSharpRenderer"/> instance.
        /// </summary>
        /// <returns>The new instance of the <see cref="SkiaSharpRenderer"/> class.</returns>
        public static ShapeRenderer Create() => new SkiaSharpRenderer();

        private SKPoint GetTextOrigin(ShapeStyle style, ref SKRect rect, ref SKRect size)
        {
            double rwidth = Math.Abs(rect.Right - rect.Left);
            double rheight = Math.Abs(rect.Bottom - rect.Top);
            double swidth = Math.Abs(size.Right - size.Left);
            double sheight = Math.Abs(size.Bottom - size.Top);
            double ox, oy;

            switch (style.TextStyle.TextHAlignment)
            {
                case TextHAlignment.Left:
                    ox = rect.Left;
                    break;
                case TextHAlignment.Right:
                    ox = rect.Right - swidth;
                    break;
                case TextHAlignment.Center:
                default:
                    ox = (rect.Left + rwidth / 2f) - (swidth / 2f);
                    break;
            }

            switch (style.TextStyle.TextVAlignment)
            {
                case TextVAlignment.Top:
                    oy = rect.Top;
                    break;
                case TextVAlignment.Bottom:
                    oy = rect.Bottom - sheight;
                    break;
                case TextVAlignment.Center:
                default:
                    oy = (rect.Bottom - rheight / 2f) - (sheight / 2f);
                    break;
            }

            return new SKPoint((float)ox, (float)oy);
        }

        private SKColor ToSKColor(ArgbColor color) => new SKColor(color.R, color.G, color.B, color.A);

        private static SKStrokeCap ToStrokeCap(BaseStyle style)
        {
            switch (style.LineCap)
            {
                default:
                case LineCap.Flat:
                    return SKStrokeCap.Butt;
                case LineCap.Square:
                    return SKStrokeCap.Square;
                case LineCap.Round:
                    return SKStrokeCap.Round;
            }
        }

        private SKPaint ToSKPaintPen(BaseStyle style, Func<double, float> scale, double sourceDpi, double targetDpi)
        {
            return new SKPaint()
            {
                IsAntialias = _isAntialias,
                IsStroke = true,
                StrokeWidth = scale(style.Thickness * targetDpi / sourceDpi),
                Color = ToSKColor(style.Stroke),
                StrokeCap = ToStrokeCap(style),
                PathEffect = style.Dashes != null ? SKPathEffect.CreateDash(BaseStyle.ConvertDashesToFloatArray(style.Dashes), (float)style.DashOffset) : null
            };
        }

        private SKPaint ToSKPaintBrush(ArgbColor color)
        {
            return new SKPaint()
            {
                IsAntialias = _isAntialias,
                IsStroke = false,
                LcdRenderText = true,
                SubpixelText = true,
                Color = ToSKColor(color)
            };
        }

        private SKRect ToSKRect(double x, double y, double width, double height)
        {
            float left = (float)x;
            float top = (float)y;
            float right = (float)(x + width);
            float bottom = (float)(y + height);
            return new SKRect(left, top, right, bottom);
        }

        private SKRect CreateRect(IPointShape tl, IPointShape br, double dx, double dy, Func<double, float> scale)
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
                using (var path = new SKPath())
                {
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
        }

        private void DrawLineArrowsInternal(SKCanvas canvas, ILineShape line, double dx, double dy, out SKPoint pt1, out SKPoint pt2)
        {
            using (var fillStartArrow = ToSKPaintBrush(line.Style.StartArrowStyle.Fill))
            using (var strokeStartArrow = ToSKPaintPen(line.Style.StartArrowStyle, _scaleToPage, _sourceDpi, _targetDpi))
            using (var fillEndArrow = ToSKPaintBrush(line.Style.EndArrowStyle.Fill))
            using (var strokeEndArrow = ToSKPaintPen(line.Style.EndArrowStyle, _scaleToPage, _sourceDpi, _targetDpi))
            {
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
        }

        private SKPoint DrawLineArrowInternal(SKCanvas canvas, SKPaint pen, SKPaint brush, float x, float y, double angle, ArrowStyle style)
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

        private void DrawBackgroundInternal(SKCanvas canvas, ArgbColor color, Rect2 rect)
        {
            using (var brush = ToSKPaintBrush(color))
            {
                SKRect srect = SKRect.Create(
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
                canvas.DrawRect(srect, brush);
            }
        }

        private SKMatrix ToSKMatrix(MatrixObject matrix)
        {
            return MatrixHelper.ToSKMatrix(
                matrix.M11, matrix.M12,
                matrix.M21, matrix.M22,
                matrix.OffsetX, matrix.OffsetY);
        }

        /// <inheritdoc/>
        public override void ClearCache(bool isZooming)
        {
            if (!isZooming)
            {
                _biCache.Reset();
            }
        }

        /// <inheritdoc/>
        public override void Fill(object dc, double x, double y, double width, double height, ArgbColor color)
        {
            var canvas = dc as SKCanvas;
            var rect = SKRect.Create((float)x, (float)y, (float)width, (float)height);
            using (var paint = ToSKPaintBrush(color))
            {
                canvas.DrawRect(rect, paint);
            }
        }

        /// <inheritdoc/>
        public override object PushMatrix(object dc, MatrixObject matrix)
        {
            var canvas = dc as SKCanvas;
            int count = canvas.Save();
            canvas.SetMatrix(MatrixHelper.Multiply(ToSKMatrix(matrix), canvas.TotalMatrix));
            return count;
        }

        /// <inheritdoc/>
        public override void PopMatrix(object dc, object state)
        {
            var canvas = dc as SKCanvas;
            var count = (int)state;
            canvas.RestoreToCount(count);
        }

        /// <inheritdoc/>
        public override void Draw(object dc, ILineShape line, double dx, double dy, object db, object r)
        {
            var canvas = dc as SKCanvas;

            using (var strokeLine = ToSKPaintPen(line.Style, _scaleToPage, _sourceDpi, _targetDpi))
            {
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
        }

        /// <inheritdoc/>
        public override void Draw(object dc, IRectangleShape rectangle, double dx, double dy, object db, object r)
        {
            var canvas = dc as SKCanvas;

            using (var brush = ToSKPaintBrush(rectangle.Style.Fill))
            using (var pen = ToSKPaintPen(rectangle.Style, _scaleToPage, _sourceDpi, _targetDpi))
            {
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
        }

        /// <inheritdoc/>
        public override void Draw(object dc, IEllipseShape ellipse, double dx, double dy, object db, object r)
        {
            var canvas = dc as SKCanvas;

            using (var brush = ToSKPaintBrush(ellipse.Style.Fill))
            using (var pen = ToSKPaintPen(ellipse.Style, _scaleToPage, _sourceDpi, _targetDpi))
            {
                var rect = CreateRect(ellipse.TopLeft, ellipse.BottomRight, dx, dy, _scaleToPage);
                DrawEllipseInternal(canvas, brush, pen, ellipse.IsStroked, ellipse.IsFilled, ref rect);
            }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, IArcShape arc, double dx, double dy, object db, object r)
        {
            var canvas = dc as SKCanvas;

            using (var brush = ToSKPaintBrush(arc.Style.Fill))
            using (var pen = ToSKPaintPen(arc.Style, _scaleToPage, _sourceDpi, _targetDpi))
            using (var path = new SKPath())
            {
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
        }

        /// <inheritdoc/>
        public override void Draw(object dc, ICubicBezierShape cubicBezier, double dx, double dy, object db, object r)
        {
            var canvas = dc as SKCanvas;

            using (var brush = ToSKPaintBrush(cubicBezier.Style.Fill))
            using (var pen = ToSKPaintPen(cubicBezier.Style, _scaleToPage, _sourceDpi, _targetDpi))
            using (var path = new SKPath())
            {
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
        }

        /// <inheritdoc/>
        public override void Draw(object dc, IQuadraticBezierShape quadraticBezier, double dx, double dy, object db, object r)
        {
            var canvas = dc as SKCanvas;

            using (var brush = ToSKPaintBrush(quadraticBezier.Style.Fill))
            using (var pen = ToSKPaintPen(quadraticBezier.Style, _scaleToPage, _sourceDpi, _targetDpi))
            using (var path = new SKPath())
            {
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
        }

        /// <inheritdoc/>
        public override void Draw(object dc, ITextShape text, double dx, double dy, object db, object r)
        {
            var canvas = dc as SKCanvas;

            var properties = (ImmutableArray<Property>)db;
            var record = (Record)r;
            var tbind = text.BindText(properties, record);
            if (string.IsNullOrEmpty(tbind))
                return;

            var style = SKTypefaceStyle.Normal;
            if (text.Style.TextStyle.FontStyle != null)
            {
                if (text.Style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Bold))
                {
                    style |= SKTypefaceStyle.Bold;
                }

                if (text.Style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Italic))
                {
                    style |= SKTypefaceStyle.Italic;
                }

                if (text.Style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Underline))
                {
                    // TODO: Add support for FontStyleFlags.Underline
                }

                if (text.Style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Strikeout))
                {
                    // TODO: Add support for FontStyleFlags.Strikeout
                }
            }

            using (var pen = ToSKPaintBrush(text.Style.Stroke))
            using (var tf = SKTypeface.FromFamilyName(text.Style.TextStyle.FontName, style))
            {
                pen.TextEncoding = SKTextEncoding.Utf16;
                pen.TextSize = _scaleToPage(text.Style.TextStyle.FontSize * _targetDpi / _sourceDpi);

                var fm = pen.FontMetrics;
                float offset = -(fm.Top + fm.Bottom);

                var rect = CreateRect(text.TopLeft, text.BottomRight, dx, dy, _scaleToPage);
                SKRect bounds = new SKRect();
                pen.MeasureText(tbind, ref bounds);
                var origin = GetTextOrigin(text.Style, ref rect, ref bounds);

                canvas.DrawText(tbind, origin.X, origin.Y + offset, pen);
            }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, IImageShape image, double dx, double dy, object db, object r)
        {
            var canvas = dc as SKCanvas;

            var rect = CreateRect(image.TopLeft, image.BottomRight, dx, dy, _scaleToPage);

            if (image.IsStroked || image.IsFilled)
            {
                using (var brush = ToSKPaintBrush(image.Style.Fill))
                using (var pen = ToSKPaintPen(image.Style, _scaleToPage, _sourceDpi, _targetDpi))
                {
                    DrawRectangleInternal(canvas, brush, pen, image.IsStroked, image.IsFilled, ref rect);
                }
            }

            var imageCached = _biCache.Get(image.Key);
            if (imageCached != null)
            {
                canvas.DrawBitmap(imageCached, rect);
            }
            else
            {
                if (State.ImageCache == null || string.IsNullOrEmpty(image.Key))
                    return;

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
        public override void Draw(object dc, IPathShape path, double dx, double dy, object db, object r)
        {
            var canvas = dc as SKCanvas;

            using (var brush = ToSKPaintBrush(path.Style.Fill))
            using (var pen = ToSKPaintPen(path.Style, _scaleToPage, _sourceDpi, _targetDpi))
            using (var spath = path.Geometry.ToSKPath(dx, dy, _scaleToPage))
            {
                DrawPathInternal(canvas, brush, pen, path.IsStroked, path.IsFilled, spath);
            }
        }
    }
}
