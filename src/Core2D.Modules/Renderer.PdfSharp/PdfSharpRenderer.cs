using System;
using System.Collections.Generic;
using Core2D;
using Core2D.Containers;
using Core2D.Shapes;
using Core2D.Style;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace Core2D.Renderer.PdfSharp
{
    /// <summary>
    /// Native PdfSharp shape renderer.
    /// </summary>
    public partial class PdfSharpRenderer : ObservableObject, IShapeRenderer
    {
        private readonly IServiceProvider _serviceProvider;
        private IShapeRendererState _state;
        private ICache<string, XImage> _biCache;
        private Func<double, double> _scaleToPage;
        private double _sourceDpi = 96.0;
        private double _targetDpi = 72.0;

        /// <inheritdoc/>
        public IShapeRendererState State
        {
            get => _state;
            set => Update(ref _state, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSharpRenderer"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public PdfSharpRenderer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _state = _serviceProvider.GetService<IFactory>().CreateShapeRendererState();
            _biCache = _serviceProvider.GetService<IFactory>().CreateCache<string, XImage>(bi => bi.Dispose());
            _scaleToPage = (value) => (float)(value * 1.0);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        private static XColor ToXColor(IColor color)
        {
            return color switch
            {
                IArgbColor argbColor => XColor.FromArgb(argbColor.A, argbColor.R, argbColor.G, argbColor.B),
                _ => throw new NotSupportedException($"The {color.GetType()} color type is not supported."),
            };
        }

        private static XPen ToXPen(IBaseStyle style, Func<double, double> scale, double sourceDpi, double targetDpi)
        {
            var strokeWidth = scale(style.Thickness * targetDpi / sourceDpi);
            var pen = new XPen(ToXColor(style.Stroke), strokeWidth);
            switch (style.LineCap)
            {
                case LineCap.Flat:
                    pen.LineCap = XLineCap.Flat;
                    break;
                case LineCap.Square:
                    pen.LineCap = XLineCap.Square;
                    break;
                case LineCap.Round:
                    pen.LineCap = XLineCap.Round;
                    break;
            }
            if (style.Dashes != null)
            {
                // TODO: Convert to correct dash values.
                var dashPattern = StyleHelper.ConvertDashesToDoubleArray(style.Dashes, strokeWidth);
                var dashOffset = style.DashOffset * strokeWidth;
                if (dashPattern != null)
                {
                    pen.DashPattern = dashPattern;
                    pen.DashStyle = XDashStyle.Custom;
                    pen.DashOffset = dashOffset;
                }
            }
            return pen;
        }

        private static XPen ToXPen(IColor color, double thickness, Func<double, double> scale, double sourceDpi, double targetDpi)
        {
            var strokeWidth = scale(thickness * targetDpi / sourceDpi);
            var pen = new XPen(ToXColor(color), strokeWidth);
            pen.LineCap = XLineCap.Flat;
            return pen;
        }

        private static XBrush ToXBrush(IColor color)
        {
            return color switch
            {
                IArgbColor argbColor => new XSolidBrush(ToXColor(argbColor)),
                _ => throw new NotSupportedException($"The {color.GetType()} color type is not supported."),
            };
        }

        private static void DrawLineInternal(XGraphics gfx, XPen pen, bool isStroked, ref XPoint p0, ref XPoint p1)
        {
            if (isStroked)
            {
                gfx.DrawLine(pen, p0, p1);
            }
        }

        private static void DrawLineCurveInternal(XGraphics gfx, XPen pen, bool isStroked, ref XPoint pt1, ref XPoint pt2, double curvature, CurveOrientation orientation, Core2D.Renderer.PointAlignment pt1a, Core2D.Renderer.PointAlignment pt2a)
        {
            if (isStroked)
            {
                var path = new XGraphicsPath();
                double p1x = pt1.X;
                double p1y = pt1.Y;
                double p2x = pt2.X;
                double p2y = pt2.Y;
                LineShapeExtensions.GetCurvedLineBezierControlPoints(orientation, curvature, pt1a, pt2a, ref p1x, ref p1y, ref p2x, ref p2y);
                path.AddBezier(
                    pt1.X, pt1.Y,
                    p1x, p1y,
                    p2x, p2y,
                    pt2.X, pt2.Y);
                gfx.DrawPath(pen, path);
            }
        }

        private void DrawLineArrowsInternal(XGraphics gfx, ILineShape line, out XPoint pt1, out XPoint pt2)
        {
            var fillStartArrow = ToXBrush(line.Style.StartArrowStyle.Fill);
            var strokeStartArrow = ToXPen(line.Style.StartArrowStyle, _scaleToPage, _sourceDpi, _targetDpi);

            var fillEndArrow = ToXBrush(line.Style.EndArrowStyle.Fill);
            var strokeEndArrow = ToXPen(line.Style.EndArrowStyle, _scaleToPage, _sourceDpi, _targetDpi);

            double _x1 = line.Start.X;
            double _y1 = line.Start.Y;
            double _x2 = line.End.X;
            double _y2 = line.End.Y;

            LineShapeExtensions.GetMaxLength(line, ref _x1, ref _y1, ref _x2, ref _y2);

            double x1 = _scaleToPage(_x1);
            double y1 = _scaleToPage(_y1);
            double x2 = _scaleToPage(_x2);
            double y2 = _scaleToPage(_y2);

            var sas = line.Style.StartArrowStyle;
            var eas = line.Style.EndArrowStyle;
            double a1 = Math.Atan2(y1 - y2, x1 - x2) * 180.0 / Math.PI;
            double a2 = Math.Atan2(y2 - y1, x2 - x1) * 180.0 / Math.PI;

            // Draw start arrow.
            pt1 = DrawLineArrowInternal(gfx, strokeStartArrow, fillStartArrow, x1, y1, a1, sas);

            // Draw end arrow.
            pt2 = DrawLineArrowInternal(gfx, strokeEndArrow, fillEndArrow, x2, y2, a2, eas);
        }

        private static XPoint DrawLineArrowInternal(XGraphics gfx, XPen pen, XBrush brush, double x, double y, double angle, IArrowStyle style)
        {
            XPoint pt;
            var rt = new XMatrix();
            var c = new XPoint(x, y);
            rt.RotateAtPrepend(angle, c);
            double rx = style.RadiusX;
            double ry = style.RadiusY;
            double sx = 2.0 * rx;
            double sy = 2.0 * ry;

            switch (style.ArrowType)
            {
                default:
                case ArrowType.None:
                    {
                        pt = new XPoint(x, y);
                    }
                    break;
                case ArrowType.Rectangle:
                    {
                        pt = rt.Transform(new XPoint(x - sx, y));
                        var rect = new XRect(x - sx, y - ry, sx, sy);
                        gfx.Save();
                        gfx.RotateAtTransform(angle, c);
                        DrawRectangleInternal(gfx, brush, pen, style.IsStroked, style.IsFilled, ref rect);
                        gfx.Restore();
                    }
                    break;
                case ArrowType.Ellipse:
                    {
                        pt = rt.Transform(new XPoint(x - sx, y));
                        gfx.Save();
                        gfx.RotateAtTransform(angle, c);
                        var rect = new XRect(x - sx, y - ry, sx, sy);
                        DrawEllipseInternal(gfx, brush, pen, style.IsStroked, style.IsFilled, ref rect);
                        gfx.Restore();
                    }
                    break;
                case ArrowType.Arrow:
                    {
                        pt = rt.Transform(new XPoint(x, y));
                        var p11 = rt.Transform(new XPoint(x - sx, y + sy));
                        var p21 = rt.Transform(new XPoint(x, y));
                        var p12 = rt.Transform(new XPoint(x - sx, y - sy));
                        var p22 = rt.Transform(new XPoint(x, y));
                        DrawLineInternal(gfx, pen, style.IsStroked, ref p11, ref p21);
                        DrawLineInternal(gfx, pen, style.IsStroked, ref p12, ref p22);
                    }
                    break;
            }

            return pt;
        }

        private static void DrawRectangleInternal(XGraphics gfx, XBrush brush, XPen pen, bool isStroked, bool isFilled, ref XRect rect)
        {
            if (isStroked && isFilled)
            {
                gfx.DrawRectangle(pen, brush, rect);
            }
            else if (isStroked && !isFilled)
            {
                gfx.DrawRectangle(pen, rect);
            }
            else if (!isStroked && isFilled)
            {
                gfx.DrawRectangle(brush, rect);
            }
        }

        private static void DrawEllipseInternal(XGraphics gfx, XBrush brush, XPen pen, bool isStroked, bool isFilled, ref XRect rect)
        {
            if (isStroked && isFilled)
            {
                gfx.DrawEllipse(pen, brush, rect);
            }
            else if (isStroked && !isFilled)
            {
                gfx.DrawEllipse(pen, rect);
            }
            else if (!isStroked && isFilled)
            {
                gfx.DrawEllipse(brush, rect);
            }
        }

        private void DrawGridInternal(XGraphics gfx, IGrid grid, XPen stroke, ref Spatial.Rect2 rect)
        {
            double ox = rect.X;
            double ex = rect.X + rect.Width;
            double oy = rect.Y;
            double ey = rect.Y + rect.Height;
            double cw = grid.GridCellWidth;
            double ch = grid.GridCellHeight;

            for (double x = ox + cw; x < ex; x += cw)
            {
                var p0 = new XPoint(
                    _scaleToPage(x),
                    _scaleToPage(oy));
                var p1 = new XPoint(
                    _scaleToPage(x),
                    _scaleToPage(ey));
                DrawLineInternal(gfx, stroke, true, ref p0, ref p1);
            }

            for (double y = oy + ch; y < ey; y += ch)
            {
                var p0 = new XPoint(
                    _scaleToPage(ox),
                    _scaleToPage(y));
                var p1 = new XPoint(
                    _scaleToPage(ex),
                    _scaleToPage(y));
                DrawLineInternal(gfx, stroke, true, ref p0, ref p1);
            }
        }

        /// <inheritdoc/>
        public void ClearCache()
        {
            _biCache.Reset();
        }

        /// <inheritdoc/>
        public void Fill(object dc, double x, double y, double width, double height, IColor color)
        {
            var _gfx = dc as XGraphics;
            _gfx.DrawRectangle(
                ToXBrush(color),
                _scaleToPage(x),
                _scaleToPage(y),
                _scaleToPage(width),
                _scaleToPage(height));
        }

        /// <inheritdoc/>
        public void Grid(object dc, IGrid grid, double x, double y, double width, double height)
        {
            var _gfx = dc as XGraphics;

            var strokeLine = ToXPen(grid.GridStrokeColor, grid.GridStrokeThickness, _scaleToPage, _sourceDpi, _targetDpi);

            var rect = Spatial.Rect2.FromPoints(
                x + grid.GridOffsetLeft,
                y + grid.GridOffsetTop,
                x + width - grid.GridOffsetLeft + grid.GridOffsetRight,
                y + height - grid.GridOffsetTop + grid.GridOffsetBottom,
                0, 0);

            if (grid.IsGridEnabled)
            {
                DrawGridInternal(
                    _gfx,
                    grid,
                    ToXPen(grid.GridStrokeColor, grid.GridStrokeThickness, _scaleToPage, _sourceDpi, _targetDpi),
                    ref rect);
            }

            if (grid.IsBorderEnabled)
            {
                _gfx.DrawRectangle(
                    ToXPen(grid.GridStrokeColor, grid.GridStrokeThickness, _scaleToPage, _sourceDpi, _targetDpi),
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }
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
            // TODO:
        }

        /// <inheritdoc/>
        public void DrawLine(object dc, ILineShape line)
        {
            if (!line.IsStroked)
            {
                return;
            }

            var _gfx = dc as XGraphics;

            var strokeLine = ToXPen(line.Style, _scaleToPage, _sourceDpi, _targetDpi);
            DrawLineArrowsInternal(_gfx, line, out var pt1, out var pt2);

            if (line.Style.LineStyle.IsCurved)
            {
                DrawLineCurveInternal(
                    _gfx,
                    strokeLine, line.IsStroked,
                    ref pt1, ref pt2,
                    line.Style.LineStyle.Curvature,
                    line.Style.LineStyle.CurveOrientation,
                    line.Start.Alignment,
                    line.End.Alignment);
            }
            else
            {
                DrawLineInternal(_gfx, strokeLine, line.IsStroked, ref pt1, ref pt2);
            }
        }

        /// <inheritdoc/>
        public void DrawRectangle(object dc, IRectangleShape rectangle)
        {
            var _gfx = dc as XGraphics;

            var rect = Spatial.Rect2.FromPoints(
                rectangle.TopLeft.X,
                rectangle.TopLeft.Y,
                rectangle.BottomRight.X,
                rectangle.BottomRight.Y,
                0, 0);

            if (rectangle.IsStroked && rectangle.IsFilled)
            {
                _gfx.DrawRectangle(
                    ToXPen(rectangle.Style, _scaleToPage, _sourceDpi, _targetDpi),
                    ToXBrush(rectangle.Style.Fill),
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }
            else if (rectangle.IsStroked && !rectangle.IsFilled)
            {
                _gfx.DrawRectangle(
                    ToXPen(rectangle.Style, _scaleToPage, _sourceDpi, _targetDpi),
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }
            else if (!rectangle.IsStroked && rectangle.IsFilled)
            {
                _gfx.DrawRectangle(
                    ToXBrush(rectangle.Style.Fill),
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }

            DrawText(dc, rectangle);
        }

        /// <inheritdoc/>
        public void DrawEllipse(object dc, IEllipseShape ellipse)
        {
            var _gfx = dc as XGraphics;

            var rect = Spatial.Rect2.FromPoints(
                ellipse.TopLeft.X,
                ellipse.TopLeft.Y,
                ellipse.BottomRight.X,
                ellipse.BottomRight.Y,
                0, 0);

            if (ellipse.IsStroked && ellipse.IsFilled)
            {
                _gfx.DrawEllipse(
                    ToXPen(ellipse.Style, _scaleToPage, _sourceDpi, _targetDpi),
                    ToXBrush(ellipse.Style.Fill),
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }
            else if (ellipse.IsStroked && !ellipse.IsFilled)
            {
                _gfx.DrawEllipse(
                    ToXPen(ellipse.Style, _scaleToPage, _sourceDpi, _targetDpi),
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }
            else if (!ellipse.IsStroked && ellipse.IsFilled)
            {
                _gfx.DrawEllipse(
                    ToXBrush(ellipse.Style.Fill),
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }

            DrawText(dc, ellipse);
        }

        /// <inheritdoc/>
        public void DrawArc(object dc, IArcShape arc)
        {
            var _gfx = dc as XGraphics;

            var a = new Spatial.Arc.GdiArc(
                Spatial.Point2.FromXY(arc.Point1.X, arc.Point1.Y),
                Spatial.Point2.FromXY(arc.Point2.X, arc.Point2.Y),
                Spatial.Point2.FromXY(arc.Point3.X, arc.Point3.Y),
                Spatial.Point2.FromXY(arc.Point4.X, arc.Point4.Y));

            if (arc.IsFilled)
            {
                var path = new XGraphicsPath();
                // NOTE: Not implemented in PdfSharp Core version.
                path.AddArc(
                    _scaleToPage(a.X),
                    _scaleToPage(a.Y),
                    _scaleToPage(a.Width),
                    _scaleToPage(a.Height),
                    a.StartAngle,
                    a.SweepAngle);

                if (arc.IsStroked)
                {
                    _gfx.DrawPath(
                        ToXPen(arc.Style, _scaleToPage, _sourceDpi, _targetDpi),
                        ToXBrush(arc.Style.Fill),
                        path);
                }
                else
                {
                    _gfx.DrawPath(
                        ToXBrush(arc.Style.Fill),
                        path);
                }
            }
            else
            {
                if (arc.IsStroked)
                {
                    _gfx.DrawArc(
                        ToXPen(arc.Style, _scaleToPage, _sourceDpi, _targetDpi),
                        _scaleToPage(a.X),
                        _scaleToPage(a.Y),
                        _scaleToPage(a.Width),
                        _scaleToPage(a.Height),
                        a.StartAngle,
                        a.SweepAngle);
                }
            }
        }

        /// <inheritdoc/>
        public void DrawCubicBezier(object dc, ICubicBezierShape cubicBezier)
        {
            var _gfx = dc as XGraphics;

            if (cubicBezier.IsFilled)
            {
                var path = new XGraphicsPath();
                path.AddBezier(
                    _scaleToPage(cubicBezier.Point1.X),
                    _scaleToPage(cubicBezier.Point1.Y),
                    _scaleToPage(cubicBezier.Point2.X),
                    _scaleToPage(cubicBezier.Point2.Y),
                    _scaleToPage(cubicBezier.Point3.X),
                    _scaleToPage(cubicBezier.Point3.Y),
                    _scaleToPage(cubicBezier.Point4.X),
                    _scaleToPage(cubicBezier.Point4.Y));

                if (cubicBezier.IsStroked)
                {
                    _gfx.DrawPath(
                        ToXPen(cubicBezier.Style, _scaleToPage, _sourceDpi, _targetDpi),
                        ToXBrush(cubicBezier.Style.Fill),
                        path);
                }
                else
                {
                    _gfx.DrawPath(
                        ToXBrush(cubicBezier.Style.Fill),
                        path);
                }
            }
            else
            {
                if (cubicBezier.IsStroked)
                {
                    _gfx.DrawBezier(
                        ToXPen(cubicBezier.Style, _scaleToPage, _sourceDpi, _targetDpi),
                        _scaleToPage(cubicBezier.Point1.X),
                        _scaleToPage(cubicBezier.Point1.Y),
                        _scaleToPage(cubicBezier.Point2.X),
                        _scaleToPage(cubicBezier.Point2.Y),
                        _scaleToPage(cubicBezier.Point3.X),
                        _scaleToPage(cubicBezier.Point3.Y),
                        _scaleToPage(cubicBezier.Point4.X),
                        _scaleToPage(cubicBezier.Point4.Y));
                }
            }
        }

        /// <inheritdoc/>
        public void DrawQuadraticBezier(object dc, IQuadraticBezierShape quadraticBezier)
        {
            var _gfx = dc as XGraphics;

            double x1 = quadraticBezier.Point1.X;
            double y1 = quadraticBezier.Point1.Y;
            double x2 = quadraticBezier.Point1.X + (2.0 * (quadraticBezier.Point2.X - quadraticBezier.Point1.X)) / 3.0;
            double y2 = quadraticBezier.Point1.Y + (2.0 * (quadraticBezier.Point2.Y - quadraticBezier.Point1.Y)) / 3.0;
            double x3 = x2 + (quadraticBezier.Point3.X - quadraticBezier.Point1.X) / 3.0;
            double y3 = y2 + (quadraticBezier.Point3.Y - quadraticBezier.Point1.Y) / 3.0;
            double x4 = quadraticBezier.Point3.X;
            double y4 = quadraticBezier.Point3.Y;

            if (quadraticBezier.IsFilled)
            {
                var path = new XGraphicsPath();
                path.AddBezier(
                    _scaleToPage(x1),
                    _scaleToPage(y1),
                    _scaleToPage(x2),
                    _scaleToPage(y2),
                    _scaleToPage(x3),
                    _scaleToPage(y3),
                    _scaleToPage(x4),
                    _scaleToPage(y4));

                if (quadraticBezier.IsStroked)
                {
                    _gfx.DrawPath(
                        ToXPen(quadraticBezier.Style, _scaleToPage, _sourceDpi, _targetDpi),
                        ToXBrush(quadraticBezier.Style.Fill),
                        path);
                }
                else
                {
                    _gfx.DrawPath(
                        ToXBrush(quadraticBezier.Style.Fill),
                        path);
                }
            }
            else
            {
                if (quadraticBezier.IsStroked)
                {
                    _gfx.DrawBezier(
                        ToXPen(quadraticBezier.Style, _scaleToPage, _sourceDpi, _targetDpi),
                        _scaleToPage(x1),
                        _scaleToPage(y1),
                        _scaleToPage(x2),
                        _scaleToPage(y2),
                        _scaleToPage(x3),
                        _scaleToPage(y3),
                        _scaleToPage(x4),
                        _scaleToPage(y4));
                }
            }
        }

        /// <inheritdoc/>
        public void DrawText(object dc, ITextShape text)
        {
            var _gfx = dc as XGraphics;

            if (!(text.GetProperty(nameof(ITextShape.Text)) is string tbind))
            {
                tbind = text.Text;
            }

            if (tbind == null)
            {
                return;
            }

            var options = new XPdfFontOptions(PdfFontEncoding.Unicode);

            var fontStyle = XFontStyle.Regular;
            if (text.Style.TextStyle.FontStyle != null)
            {
                if (text.Style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Bold))
                {
                    fontStyle |= XFontStyle.Bold;
                }

                if (text.Style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Italic))
                {
                    fontStyle |= XFontStyle.Italic;
                }
            }

            var font = new XFont(
                text.Style.TextStyle.FontName,
                _scaleToPage(text.Style.TextStyle.FontSize),
                fontStyle,
                options);

            var rect = Spatial.Rect2.FromPoints(
                text.TopLeft.X,
                text.TopLeft.Y,
                text.BottomRight.X,
                text.BottomRight.Y,
                0, 0);

            var srect = new XRect(
                _scaleToPage(rect.X),
                _scaleToPage(rect.Y),
                _scaleToPage(rect.Width),
                _scaleToPage(rect.Height));

            var format = new XStringFormat();
            switch (text.Style.TextStyle.TextHAlignment)
            {
                case TextHAlignment.Left:
                    format.Alignment = XStringAlignment.Near;
                    break;
                case TextHAlignment.Center:
                    format.Alignment = XStringAlignment.Center;
                    break;
                case TextHAlignment.Right:
                    format.Alignment = XStringAlignment.Far;
                    break;
            }

            switch (text.Style.TextStyle.TextVAlignment)
            {
                case TextVAlignment.Top:
                    format.LineAlignment = XLineAlignment.Near;
                    break;
                case TextVAlignment.Center:
                    format.LineAlignment = XLineAlignment.Center;
                    break;
                case TextVAlignment.Bottom:
                    format.LineAlignment = XLineAlignment.Far;
                    break;
            }

            _gfx.DrawString(
                tbind,
                font,
                ToXBrush(text.Style.Stroke),
                srect,
                format);
        }

        /// <inheritdoc/>
        public void DrawImage(object dc, IImageShape image)
        {
            var _gfx = dc as XGraphics;

            var rect = Spatial.Rect2.FromPoints(
                image.TopLeft.X,
                image.TopLeft.Y,
                image.BottomRight.X,
                image.BottomRight.Y,
                0, 0);

            var srect = new XRect(
                _scaleToPage(rect.X),
                _scaleToPage(rect.Y),
                _scaleToPage(rect.Width),
                _scaleToPage(rect.Height));

            if (image.IsStroked || image.IsFilled)
            {
                DrawRectangleInternal(
                    _gfx,
                    ToXBrush(image.Style.Fill),
                    ToXPen(image.Style, _scaleToPage, _sourceDpi, _targetDpi),
                    image.IsStroked,
                    image.IsFilled,
                    ref srect);
            }

            var imageCached = _biCache.Get(image.Key);
            if (imageCached != null)
            {
                _gfx.DrawImage(imageCached, srect);
            }
            else
            {
                if (State.ImageCache != null && !string.IsNullOrEmpty(image.Key))
                {
                    var bytes = State.ImageCache.GetImage(image.Key);
                    if (bytes != null)
                    {
                        var ms = new System.IO.MemoryStream(bytes);
#if WPF
                        var bs = new BitmapImage();
                        bs.BeginInit();
                        bs.StreamSource = ms;
                        bs.EndInit();
                        bs.Freeze();
                        var bi = XImage.FromBitmapSource(bs);
#else
                        var bi = XImage.FromStream(ms);
#endif
                        _biCache.Set(image.Key, bi);

                        _gfx.DrawImage(bi, srect);
                    }
                }
            }

            DrawText(dc, image);
        }

        /// <inheritdoc/>
        public void DrawPath(object dc, IPathShape path)
        {
            var _gfx = dc as XGraphics;

            var gp = path.Geometry.ToXGraphicsPath(_scaleToPage);

            if (path.IsFilled && path.IsStroked)
            {
                _gfx.DrawPath(
                    ToXPen(path.Style, _scaleToPage, _sourceDpi, _targetDpi),
                    ToXBrush(path.Style.Fill),
                    gp);
            }
            else if (path.IsFilled && !path.IsStroked)
            {
                _gfx.DrawPath(
                    ToXBrush(path.Style.Fill),
                    gp);
            }
            else if (!path.IsFilled && path.IsStroked)
            {
                _gfx.DrawPath(
                    ToXPen(path.Style, _scaleToPage, _sourceDpi, _targetDpi),
                    gp);
            }
        }

        /// <summary>
        /// Check whether the <see cref="State"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeState() => _state != null;
    }
}
