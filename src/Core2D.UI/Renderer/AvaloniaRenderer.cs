using System;
using System.Collections.Generic;
using Core2D;
using Core2D.Containers;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;
using Spatial;
using Spatial.Arc;
using A = Avalonia;
using AM = Avalonia.Media;
using AME = Avalonia.MatrixExtensions;
using AMI = Avalonia.Media.Imaging;

namespace Core2D.UI.Renderer
{
    /// <summary>
    /// Native Avalonia shape renderer.
    /// </summary>
    public class AvaloniaRenderer : ObservableObject, IShapeRenderer
    {
        private readonly IServiceProvider _serviceProvider;
        private IShapeRendererState _state;
        private readonly ICache<IShapeStyle, (AM.IBrush, AM.IPen)> _styleCache;
        private readonly ICache<IArrowStyle, (AM.IBrush, AM.IPen)> _arrowStyleCache;
        // TODO: Add LineShape cache.
        // TODO: Add EllipseShape cache.
        // TODO: Add ArcShape cache.
        // TODO: Add CubicBezierShape cache.
        // TODO: Add QuadraticBezierShape cache.
        private readonly ICache<ITextShape, (string, AM.FormattedText, IShapeStyle)> _textCache;
        private readonly ICache<string, AMI.Bitmap> _biCache;
        // TODO: Add PathShape cache.
        private readonly Func<double, float> _scaleToPage;
        private readonly double _textScaleFactor;

        /// <inheritdoc/>
        public IShapeRendererState State
        {
            get => _state;
            set => Update(ref _state, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AvaloniaRenderer"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="textScaleFactor">The text scale factor.</param>
        public AvaloniaRenderer(IServiceProvider serviceProvider, double textScaleFactor = 1.0)
        {
            _serviceProvider = serviceProvider;
            _state = _serviceProvider.GetService<IFactory>().CreateShapeRendererState();
            _styleCache = _serviceProvider.GetService<IFactory>().CreateCache<IShapeStyle, (AM.IBrush, AM.IPen)>();
            _arrowStyleCache = _serviceProvider.GetService<IFactory>().CreateCache<IArrowStyle, (AM.IBrush, AM.IPen)>();
            _textCache = _serviceProvider.GetService<IFactory>().CreateCache<ITextShape, (string, AM.FormattedText, IShapeStyle)>();
            _biCache = _serviceProvider.GetService<IFactory>().CreateCache<string, AMI.Bitmap>(bi => bi.Dispose());
            _textScaleFactor = textScaleFactor;
            _scaleToPage = (value) => (float)(value);
            ClearCache(isZooming: false);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        private A.Point GetTextOrigin(IShapeStyle style, ref Rect2 rect, ref A.Size size)
        {
            var ox = style.TextStyle.TextHAlignment switch
            {
                TextHAlignment.Left => rect.X,
                TextHAlignment.Right => rect.Right - size.Width,
                _ => (rect.Left + rect.Width / 2.0) - (size.Width / 2.0),
            };
            var oy = style.TextStyle.TextVAlignment switch
            {
                TextVAlignment.Top => rect.Y,
                TextVAlignment.Bottom => rect.Bottom - size.Height,
                _ => (rect.Bottom - rect.Height / 2f) - (size.Height / 2f),
            };
            return new A.Point(ox, oy);
        }

        private static AM.Color ToColor(IColor color) => color switch
        {
            IArgbColor argbColor => AM.Color.FromArgb(argbColor.A, argbColor.R, argbColor.G, argbColor.B),
            _ => throw new NotSupportedException($"The {color.GetType()} color type is not supported."),
        };

        private AM.IBrush ToBrush(IColor color) => color switch
        {
            IArgbColor argbColor => new AM.SolidColorBrush(ToColor(argbColor)),
            _ => throw new NotSupportedException($"The {color.GetType()} color type is not supported."),
        };

        private AM.IPen ToPen(IBaseStyle style, Func<double, float> scale, bool scaleStrokeWidth)
        {
            var dashStyle = default(AM.DashStyle);
            if (style.Dashes != null)
            {
                var dashes = StyleHelper.ConvertDashesToDoubleArray(style.Dashes, 1.0);
                var dashOffset = style.DashOffset;
                if (dashes != null)
                {
                    dashStyle = new AM.DashStyle(dashes, dashOffset);
                }
            }

            var lineCap = style.LineCap switch
            {
                LineCap.Flat => AM.PenLineCap.Flat,
                LineCap.Square => AM.PenLineCap.Square,
                LineCap.Round => AM.PenLineCap.Round,
                _ => throw new NotImplementedException()
            };

            var thickness = scaleStrokeWidth ? (style.Thickness / _state.ZoomX) : style.Thickness;
            var strokeWidth = scale(thickness);
            var brush = ToBrush(style.Stroke);
            var pen = new AM.Pen(brush, strokeWidth, dashStyle, lineCap);

            return pen;
        }

        private static Rect2 CreateRect(IPointShape tl, IPointShape br, double dx, double dy)
        {
            return Rect2.FromPoints(tl.X, tl.Y, br.X, br.Y, dx, dy);
        }

        private static void DrawLineInternal(AM.DrawingContext dc, AM.IPen pen, bool isStroked, ref A.Point p0, ref A.Point p1)
        {
            if (!isStroked)
            {
                return;
            }
            dc.DrawLine(pen, p0, p1);
        }

        private static void DrawLineCurveInternal(AM.DrawingContext _dc, AM.IPen pen, bool isStroked, ref A.Point pt1, ref A.Point pt2, double curvature, CurveOrientation orientation, PointAlignment pt1a, PointAlignment pt2a)
        {
            if (!isStroked)
            {
                return;
            }
            var sg = new AM.StreamGeometry();
            using (var sgc = sg.Open())
            {
                sgc.BeginFigure(new A.Point(pt1.X, pt1.Y), false);
                double p1x = pt1.X;
                double p1y = pt1.Y;
                double p2x = pt2.X;
                double p2y = pt2.Y;
                LineShapeExtensions.GetCurvedLineBezierControlPoints(orientation, curvature, pt1a, pt2a, ref p1x, ref p1y, ref p2x, ref p2y);
                sgc.CubicBezierTo(
                    new A.Point(p1x, p1y),
                    new A.Point(p2x, p2y),
                    new A.Point(pt2.X, pt2.Y));
                sgc.EndFigure(false);
            }
            _dc.DrawGeometry(null, pen, sg);
        }

        private void DrawLineArrowsInternal(AM.DrawingContext dc, ILineShape line, IShapeStyle style, double dx, double dy, Func<double, float> scaleToPage, bool scaleStrokeWidth, out A.Point pt1, out A.Point pt2)
        {
            GetCached(style.StartArrowStyle, out var fillStartArrow, out var strokeStartArrow, scaleToPage, scaleStrokeWidth);
            GetCached(style.EndArrowStyle, out var fillEndArrow, out var strokeEndArrow, scaleToPage, scaleStrokeWidth);

            double _x1 = line.Start.X + dx;
            double _y1 = line.Start.Y + dy;
            double _x2 = line.End.X + dx;
            double _y2 = line.End.Y + dy;
            line.GetMaxLength(ref _x1, ref _y1, ref _x2, ref _y2);

            float x1 = _scaleToPage(_x1);
            float y1 = _scaleToPage(_y1);
            float x2 = _scaleToPage(_x2);
            float y2 = _scaleToPage(_y2);
            var sas = style.StartArrowStyle;
            var eas = style.EndArrowStyle;
            double a1 = Math.Atan2(y1 - y2, x1 - x2);
            double a2 = Math.Atan2(y2 - y1, x2 - x1);

            pt1 = DrawLineArrowInternal(dc, strokeStartArrow, fillStartArrow, x1, y1, a1, sas);
            pt2 = DrawLineArrowInternal(dc, strokeEndArrow, fillEndArrow, x2, y2, a2, eas);
        }

        private static A.Point DrawLineArrowInternal(AM.DrawingContext dc, AM.IPen pen, AM.IBrush brush, float x, float y, double angle, IArrowStyle style)
        {
            var rt = AME.MatrixHelper.Rotation(angle, new A.Vector(x, y));
            double rx = style.RadiusX;
            double ry = style.RadiusY;
            double sx = 2.0 * rx;
            double sy = 2.0 * ry;
            A.Point pt;
            switch (style.ArrowType)
            {
                default:
                case ArrowType.None:
                    {
                        pt = new A.Point(x, y);
                    }
                    break;
                case ArrowType.Rectangle:
                    {
                        pt = AME.MatrixHelper.TransformPoint(rt, new A.Point(x - (float)sx, y));
                        var rect = new Rect2(x - sx, y - ry, sx, sy);
                        using var d = dc.PushPreTransform(rt);
                        DrawRectangleInternal(dc, brush, pen, style.IsStroked, style.IsFilled, ref rect);
                    }
                    break;
                case ArrowType.Ellipse:
                    {
                        pt = AME.MatrixHelper.TransformPoint(rt, new A.Point(x - (float)sx, y));
                        using var d = dc.PushPreTransform(rt);
                        var rect = new Rect2(x - sx, y - ry, sx, sy);
                        DrawEllipseInternal(dc, brush, pen, style.IsStroked, style.IsFilled, ref rect);
                    }
                    break;
                case ArrowType.Arrow:
                    {
                        var pts = new A.Point[]
                        {
                            new A.Point(x, y),
                            new A.Point(x - (float)sx, y + (float)sy),
                            new A.Point(x, y),
                            new A.Point(x - (float)sx, y - (float)sy),
                            new A.Point(x, y)
                        };
                        pt = AME.MatrixHelper.TransformPoint(rt, pts[0]);
                        var p11 = AME.MatrixHelper.TransformPoint(rt, pts[1]);
                        var p21 = AME.MatrixHelper.TransformPoint(rt, pts[2]);
                        var p12 = AME.MatrixHelper.TransformPoint(rt, pts[3]);
                        var p22 = AME.MatrixHelper.TransformPoint(rt, pts[4]);
                        DrawLineInternal(dc, pen, style.IsStroked, ref p11, ref p21);
                        DrawLineInternal(dc, pen, style.IsStroked, ref p12, ref p22);
                    }
                    break;
            }
            return pt;
        }

        private static void DrawRectangleInternal(AM.DrawingContext dc, AM.IBrush brush, AM.IPen pen, bool isStroked, bool isFilled, ref Rect2 rect)
        {
            if (!isStroked && !isFilled)
            {
                return;
            }
            var r = new A.Rect(rect.X, rect.Y, rect.Width, rect.Height);
            if (isFilled)
            {
                dc.FillRectangle(brush, r);
            }
            if (isStroked)
            {
                dc.DrawRectangle(pen, r);
            }
        }

        private static void DrawEllipseInternal(AM.DrawingContext dc, AM.IBrush brush, AM.IPen pen, bool isStroked, bool isFilled, ref Rect2 rect)
        {
            if (!isFilled && !isStroked)
            {
                return;
            }
            var r = new A.Rect(rect.X, rect.Y, rect.Width, rect.Height);
            var g = new AM.EllipseGeometry(r);
            dc.DrawGeometry(
                isFilled ? brush : null,
                isStroked ? pen : null,
                g);
        }

        private void DrawGridInternal(AM.DrawingContext dc, AM.IPen stroke, ref Rect2 rect, double offsetX, double offsetY, double cellWidth, double cellHeight, bool isStroked)
        {
            double ox = rect.X;
            double oy = rect.Y;
            double sx = ox + offsetX;
            double sy = oy + offsetY;
            double ex = ox + rect.Width;
            double ey = oy + rect.Height;
            for (double x = sx; x < ex; x += cellWidth)
            {
                var p0 = new A.Point(_scaleToPage(x), _scaleToPage(oy));
                var p1 = new A.Point(_scaleToPage(x), _scaleToPage(ey));
                DrawLineInternal(dc, stroke, isStroked, ref p0, ref p1);
            }
            for (double y = sy; y < ey; y += cellHeight)
            {
                var p0 = new A.Point(_scaleToPage(ox), _scaleToPage(y));
                var p1 = new A.Point(_scaleToPage(ex), _scaleToPage(y));
                DrawLineInternal(dc, stroke, isStroked, ref p0, ref p1);
            }
        }

        private static AM.StreamGeometry ToStreamGeometry(IArcShape arc, double dx, double dy)
        {
            var sg = new AM.StreamGeometry();
            using var sgc = sg.Open();
            var a = new WpfArc(
                Point2.FromXY(arc.Point1.X, arc.Point1.Y),
                Point2.FromXY(arc.Point2.X, arc.Point2.Y),
                Point2.FromXY(arc.Point3.X, arc.Point3.Y),
                Point2.FromXY(arc.Point4.X, arc.Point4.Y));
            sgc.BeginFigure(
                new A.Point(a.Start.X + dx, a.Start.Y + dy),
                arc.IsFilled);
            sgc.ArcTo(
                new A.Point(a.End.X + dx, a.End.Y + dy),
                new A.Size(a.Radius.Width, a.Radius.Height),
                0.0,
                a.IsLargeArc,
                AM.SweepDirection.Clockwise);
            sgc.EndFigure(false);
            return sg;
        }

        private static AM.StreamGeometry ToStreamGeometry(ICubicBezierShape cubicBezier, double dx, double dy)
        {
            var sg = new AM.StreamGeometry();
            using var sgc = sg.Open();
            sgc.BeginFigure(
                new A.Point(cubicBezier.Point1.X + dx, cubicBezier.Point1.Y + dy),
                cubicBezier.IsFilled);
            sgc.CubicBezierTo(
                new A.Point(cubicBezier.Point2.X + dx, cubicBezier.Point2.Y + dy),
                new A.Point(cubicBezier.Point3.X + dx, cubicBezier.Point3.Y + dy),
                new A.Point(cubicBezier.Point4.X + dx, cubicBezier.Point4.Y + dy));
            sgc.EndFigure(false);
            return sg;
        }

        private static AM.StreamGeometry ToStreamGeometry(IQuadraticBezierShape quadraticBezier, double dx, double dy)
        {
            var sg = new AM.StreamGeometry();
            using var sgc = sg.Open();
            sgc.BeginFigure(
                new A.Point(quadraticBezier.Point1.X + dx, quadraticBezier.Point1.Y + dy),
                quadraticBezier.IsFilled);
            sgc.QuadraticBezierTo(
                new A.Point(quadraticBezier.Point2.X + dx, quadraticBezier.Point2.Y + dy),
                new A.Point(quadraticBezier.Point3.X + dx, quadraticBezier.Point3.Y + dy));
            sgc.EndFigure(false);
            return sg;
        }

        private void GetCached(IArrowStyle style, out AM.IBrush fill, out AM.IPen stroke, Func<double, float> scaleToPage, bool scaleStrokeWidth)
        {
            (fill, stroke) = _arrowStyleCache.Get(style);
            if (fill == null || stroke == null)
            {
                fill = ToBrush(style.Fill);
                stroke = ToPen(style, scaleToPage, scaleStrokeWidth);
                _arrowStyleCache.Set(style, (fill, stroke));
            }
        }

        private void GetCached(IShapeStyle style, out AM.IBrush fill, out AM.IPen stroke, Func<double, float> scaleToPage, bool scaleStrokeWidth)
        {
            (fill, stroke) = _styleCache.Get(style);
            if (fill == null || stroke == null)
            {
                fill = ToBrush(style.Fill);
                stroke = ToPen(style, scaleToPage, scaleStrokeWidth);
                _styleCache.Set(style, (fill, stroke));
            }
        }

        /// <inheritdoc/>
        public void InvalidateCache(IShapeStyle style)
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
            _styleCache.Reset();
            _arrowStyleCache.Reset();

            if (!isZooming)
            {
                _textCache.Reset();
                _biCache.Reset();
            }
        }

        /// <inheritdoc/>
        public void Fill(object dc, double x, double y, double width, double height, IColor color)
        {
            var _dc = dc as AM.DrawingContext;
            var brush = ToBrush(color);
            var rect = new A.Rect(x, y, width, height);
            _dc.FillRectangle(brush, rect);
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
                if (shape.State.Flags.HasFlag(_state.DrawShapeState.Flags))
                {
                    shape.DrawShape(dc, this, dx, dy);
                }
            }

            foreach (var shape in layer.Shapes)
            {
                if (shape.State.Flags.HasFlag(_state.DrawShapeState.Flags))
                {
                    shape.DrawPoints(dc, this, dx, dy);
                }
            }
        }

        /// <inheritdoc/>
        public void Draw(object dc, IPointShape point, double dx, double dy)
        {
            if (point == null || _state == null)
            {
                return;
            }

            bool isSelected = _state.SelectedShapes?.Count == 1 && _state.SelectedShapes.Contains(point);

            var pointStyle = isSelected ? _state.SelectedPointStyle : _state.PointStyle;
            if (pointStyle == null)
            {
                return;
            }

            var _dc = dc as AM.DrawingContext;

            var pointSize = _state.PointSize;
            if (pointSize <= 0.0)
            {
                return;
            }

            var scaleThickness = true; // point.State.Flags.HasFlag(ShapeStateFlags.Thickness); // TODO:
            var scaleSize = true; // point.State.Flags.HasFlag(ShapeStateFlags.Size); // TODO:
            var rect = Rect2.FromPoints(point.X - pointSize, point.Y - pointSize, point.X + pointSize, point.Y + pointSize, dx, dy);

            var scale = scaleSize ? 1.0 / _state.ZoomX : 1.0;
            var scaleToPage = scale == 1.0 ? _scaleToPage : (value) => (float)(_scaleToPage(value) / scale);
            var center = point;
            var translateX = 0.0 - (center.X * scale) + center.X;
            var translateY = 0.0 - (center.Y * scale) + center.Y;

            GetCached(pointStyle, out var fill, out var stroke, scaleToPage, scaleThickness);

            var translateDisposable = scale != 1.0 ? _dc.PushPreTransform(AME.MatrixHelper.Translate(translateX, translateY)) : default(IDisposable);
            var scaleDisposable = scale != 1.0 ? _dc.PushPreTransform(AME.MatrixHelper.Scale(scale, scale)) : default(IDisposable);

            DrawRectangleInternal(_dc, fill, stroke, true, true, ref rect);

            scaleDisposable?.Dispose();
            translateDisposable?.Dispose();
        }

        /// <inheritdoc/>
        public void Draw(object dc, ILineShape line, double dx, double dy)
        {
            var _dc = dc as AM.DrawingContext;

            var style = line.Style;
            if (style == null)
            {
                return;
            }

            var scaleThickness = line.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            var scaleSize = line.State.Flags.HasFlag(ShapeStateFlags.Size);

            var scale = scaleSize ? 1.0 / _state.ZoomX : 1.0;
            var scaleToPage = scale == 1.0 ? _scaleToPage : (value) => (float)(_scaleToPage(value) / scale);
            var center = new Point2((line.Start.X + line.End.X) / 2.0, (line.Start.Y + line.End.Y) / 2.0);
            var translateX = 0.0 - (center.X * scale) + center.X;
            var translateY = 0.0 - (center.Y * scale) + center.Y;

            GetCached(style, out _, out var stroke, scaleToPage, scaleThickness);

            var translateDisposable = scale != 1.0 ? _dc.PushPreTransform(AME.MatrixHelper.Translate(translateX, translateY)) : default(IDisposable);
            var scaleDisposable = scale != 1.0 ? _dc.PushPreTransform(AME.MatrixHelper.Scale(scale, scale)) : default(IDisposable);

            DrawLineArrowsInternal(_dc, line, style, dx, dy, scaleToPage, scaleThickness, out var pt1, out var pt2);

            if (style.LineStyle.IsCurved)
            {
                DrawLineCurveInternal(
                    _dc,
                    stroke, line.IsStroked,
                    ref pt1, ref pt2,
                    style.LineStyle.Curvature,
                    style.LineStyle.CurveOrientation,
                    line.Start.Alignment,
                    line.End.Alignment);
            }
            else
            {
                DrawLineInternal(_dc, stroke, line.IsStroked, ref pt1, ref pt2);
            }

            scaleDisposable?.Dispose();
            translateDisposable?.Dispose();
        }

        /// <inheritdoc/>
        public void Draw(object dc, IRectangleShape rectangle, double dx, double dy)
        {
            var _dc = dc as AM.DrawingContext;

            var style = rectangle.Style;
            if (style == null)
            {
                return;
            }

            var scaleThickness = rectangle.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            var scaleSize = rectangle.State.Flags.HasFlag(ShapeStateFlags.Size);
            var rect = CreateRect(rectangle.TopLeft, rectangle.BottomRight, dx, dy);

            var scale = scaleSize ? 1.0 / _state.ZoomX : 1.0;
            var scaleToPage = scale == 1.0 ? _scaleToPage : (value) => (float)(_scaleToPage(value) / scale);
            var center = rect.Center;
            var translateX = 0.0 - (center.X * scale) + center.X;
            var translateY = 0.0 - (center.Y * scale) + center.Y;

            GetCached(style, out var fill, out var stroke, scaleToPage, rectangle.IsGrid ? true : scaleThickness);

            var translateDisposable = scale != 1.0 ? _dc.PushPreTransform(AME.MatrixHelper.Translate(translateX, translateY)) : default(IDisposable);
            var scaleDisposable = scale != 1.0 ? _dc.PushPreTransform(AME.MatrixHelper.Scale(scale, scale)) : default(IDisposable);

            DrawRectangleInternal(
                _dc,
                fill,
                stroke,
                rectangle.IsStroked,
                rectangle.IsFilled,
                ref rect);

            if (rectangle.IsGrid)
            {
                DrawGridInternal(
                    _dc,
                    stroke,
                    ref rect,
                    rectangle.OffsetX, rectangle.OffsetY,
                    rectangle.CellWidth, rectangle.CellHeight,
                    true);
            }

            scaleDisposable?.Dispose();
            translateDisposable?.Dispose();
        }

        /// <inheritdoc/>
        public void Draw(object dc, IEllipseShape ellipse, double dx, double dy)
        {
            var _dc = dc as AM.DrawingContext;

            var style = ellipse.Style;
            if (style == null)
            {
                return;
            }

            var scaleThickness = ellipse.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            var scaleSize = ellipse.State.Flags.HasFlag(ShapeStateFlags.Size);
            var rect = CreateRect(ellipse.TopLeft, ellipse.BottomRight, dx, dy);

            var scale = scaleSize ? 1.0 / _state.ZoomX : 1.0;
            var scaleToPage = scale == 1.0 ? _scaleToPage : (value) => (float)(_scaleToPage(value) / scale);
            var center = rect.Center;
            var translateX = 0.0 - (center.X * scale) + center.X;
            var translateY = 0.0 - (center.Y * scale) + center.Y;

            GetCached(style, out var fill, out var stroke, scaleToPage, scaleThickness);

            var translateDisposable = scale != 1.0 ? _dc.PushPreTransform(AME.MatrixHelper.Translate(translateX, translateY)) : default(IDisposable);
            var scaleDisposable = scale != 1.0 ? _dc.PushPreTransform(AME.MatrixHelper.Scale(scale, scale)) : default(IDisposable);

            DrawEllipseInternal(
                _dc,
                fill,
                stroke,
                ellipse.IsStroked,
                ellipse.IsFilled,
                ref rect);

            scaleDisposable?.Dispose();
            translateDisposable?.Dispose();
        }

        /// <inheritdoc/>
        public void Draw(object dc, IArcShape arc, double dx, double dy)
        {
            if (!arc.IsFilled && !arc.IsStroked)
            {
                return;
            }

            var _dc = dc as AM.DrawingContext;

            var style = arc.Style;
            if (style == null)
            {
                return;
            }

            var scaleThickness = arc.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            var scaleSize = arc.State.Flags.HasFlag(ShapeStateFlags.Size);
            var geometry = ToStreamGeometry(arc, dx, dy);

            var scale = scaleSize ? 1.0 / _state.ZoomX : 1.0;
            var scaleToPage = scale == 1.0 ? _scaleToPage : (value) => (float)(_scaleToPage(value) / scale);
            var center = geometry.Bounds.Center;
            var translateX = 0.0 - (center.X * scale) + center.X;
            var translateY = 0.0 - (center.Y * scale) + center.Y;

            GetCached(style, out var fill, out var stroke, scaleToPage, scaleThickness);

            var translateDisposable = scale != 1.0 ? _dc.PushPreTransform(AME.MatrixHelper.Translate(translateX, translateY)) : default(IDisposable);
            var scaleDisposable = scale != 1.0 ? _dc.PushPreTransform(AME.MatrixHelper.Scale(scale, scale)) : default(IDisposable);

            _dc.DrawGeometry(
                arc.IsFilled ? fill : null,
                arc.IsStroked ? stroke : null,
                geometry);

            scaleDisposable?.Dispose();
            translateDisposable?.Dispose();
        }

        /// <inheritdoc/>
        public void Draw(object dc, ICubicBezierShape cubicBezier, double dx, double dy)
        {
            if (!cubicBezier.IsFilled && !cubicBezier.IsStroked)
            {
                return;
            }

            var _dc = dc as AM.DrawingContext;

            var style = cubicBezier.Style;
            if (style == null)
            {
                return;
            }

            var scaleThickness = cubicBezier.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            var scaleSize = cubicBezier.State.Flags.HasFlag(ShapeStateFlags.Size);
            var geometry = ToStreamGeometry(cubicBezier, dx, dy);

            var scale = scaleSize ? 1.0 / _state.ZoomX : 1.0;
            var scaleToPage = scale == 1.0 ? _scaleToPage : (value) => (float)(_scaleToPage(value) / scale);
            var center = geometry.Bounds.Center;
            var translateX = 0.0 - (center.X * scale) + center.X;
            var translateY = 0.0 - (center.Y * scale) + center.Y;

            GetCached(style, out var fill, out var stroke, scaleToPage, scaleThickness);

            var translateDisposable = scale != 1.0 ? _dc.PushPreTransform(AME.MatrixHelper.Translate(translateX, translateY)) : default(IDisposable);
            var scaleDisposable = scale != 1.0 ? _dc.PushPreTransform(AME.MatrixHelper.Scale(scale, scale)) : default(IDisposable);

            _dc.DrawGeometry(
                cubicBezier.IsFilled ? fill : null,
                cubicBezier.IsStroked ? stroke : null,
                geometry);

            scaleDisposable?.Dispose();
            translateDisposable?.Dispose();
        }

        /// <inheritdoc/>
        public void Draw(object dc, IQuadraticBezierShape quadraticBezier, double dx, double dy)
        {
            if (!quadraticBezier.IsFilled && !quadraticBezier.IsStroked)
            {
                return;
            }

            var _dc = dc as AM.DrawingContext;

            var style = quadraticBezier.Style;
            if (style == null)
            {
                return;
            }

            var scaleThickness = quadraticBezier.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            var scaleSize = quadraticBezier.State.Flags.HasFlag(ShapeStateFlags.Size);
            var geometry = ToStreamGeometry(quadraticBezier, dx, dy);

            var scale = scaleSize ? 1.0 / _state.ZoomX : 1.0;
            var scaleToPage = scale == 1.0 ? _scaleToPage : (value) => (float)(_scaleToPage(value) / scale);
            var center = geometry.Bounds.Center;
            var translateX = 0.0 - (center.X * scale) + center.X;
            var translateY = 0.0 - (center.Y * scale) + center.Y;

            GetCached(style, out var fill, out var stroke, scaleToPage, scaleThickness);

            var translateDisposable = scale != 1.0 ? _dc.PushPreTransform(AME.MatrixHelper.Translate(translateX, translateY)) : default(IDisposable);
            var scaleDisposable = scale != 1.0 ? _dc.PushPreTransform(AME.MatrixHelper.Scale(scale, scale)) : default(IDisposable);

            _dc.DrawGeometry(
                quadraticBezier.IsFilled ? fill : null,
                quadraticBezier.IsStroked ? stroke : null,
                geometry);

            scaleDisposable?.Dispose();
            translateDisposable?.Dispose();
        }

        /// <inheritdoc/>
        public void Draw(object dc, ITextShape text, double dx, double dy)
        {
            var _dc = dc as AM.DrawingContext;

            var style = text.Style;
            if (style == null)
            {
                return;
            }

            if (!(text.GetProperty(nameof(ITextShape.Text)) is string tbind))
            {
                tbind = text.Text;
            }

            if (tbind == null)
            {
                return;
            }

            var scaleThickness = text.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            var scaleSize = text.State.Flags.HasFlag(ShapeStateFlags.Size);
            var rect = CreateRect(text.TopLeft, text.BottomRight, dx, dy);

            var scale = scaleSize ? 1.0 / _state.ZoomX : 1.0;
            var scaleToPage = scale == 1.0 ? _scaleToPage : (value) => (float)(_scaleToPage(value) / scale);
            var center = rect.Center;
            var translateX = 0.0 - (center.X * scale) + center.X;
            var translateY = 0.0 - (center.Y * scale) + center.Y;

            GetCached(style, out _, out var stroke, scaleToPage, scaleThickness);

            var translateDisposable = scale != 1.0 ? _dc.PushPreTransform(AME.MatrixHelper.Translate(translateX, translateY)) : default(IDisposable);
            var scaleDisposable = scale != 1.0 ? _dc.PushPreTransform(AME.MatrixHelper.Scale(scale, scale)) : default(IDisposable);

            (string ct, var ft, var cs) = _textCache.Get(text);
            if (string.Compare(ct, tbind) == 0 && cs == style)
            {
                var size = ft.Bounds.Size;
                var origin = GetTextOrigin(style, ref rect, ref size);
                _dc.DrawText(stroke.Brush, origin, ft);
            }
            else
            {
                var fontStyle = AM.FontStyle.Normal;
                var fontWeight = AM.FontWeight.Normal;

                if (style.TextStyle.FontStyle != null)
                {
                    if (style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Italic))
                    {
                        fontStyle |= AM.FontStyle.Italic;
                    }

                    if (style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Bold))
                    {
                        fontWeight |= AM.FontWeight.Bold;
                    }
                }

                if (style.TextStyle.FontSize >= 0.0)
                {
                    var tf = new AM.Typeface(
                        style.TextStyle.FontName,
                        fontWeight,
                        fontStyle);

                    ft = new AM.FormattedText()
                    {
                        Typeface = tf,
                        Text = tbind,
                        TextAlignment = AM.TextAlignment.Left,
                        TextWrapping = AM.TextWrapping.NoWrap,
                        FontSize = style.TextStyle.FontSize * _textScaleFactor
                    };

                    var size = ft.Bounds.Size;
                    var origin = GetTextOrigin(style, ref rect, ref size);

                    _textCache.Set(text, (tbind, ft, style));

                    _dc.DrawText(stroke.Brush, origin, ft);
                }
            }

            scaleDisposable?.Dispose();
            translateDisposable?.Dispose();
        }

        /// <inheritdoc/>
        public void Draw(object dc, IImageShape image, double dx, double dy)
        {
            if (image.Key == null)
            {
                return;
            }

            var _dc = dc as AM.DrawingContext;
            var style = image.Style;

            var scaleThickness = image.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            var scaleSize = image.State.Flags.HasFlag(ShapeStateFlags.Size);
            var rect = CreateRect(image.TopLeft, image.BottomRight, dx, dy);

            var scale = scaleSize ? 1.0 / _state.ZoomX : 1.0;
            var scaleToPage = scale == 1.0 ? _scaleToPage : (value) => (float)(_scaleToPage(value) / scale);
            var center = rect.Center;
            var translateX = 0.0 - (center.X * scale) + center.X;
            var translateY = 0.0 - (center.Y * scale) + center.Y;

            var translateDisposable = scale != 1.0 ? _dc.PushPreTransform(AME.MatrixHelper.Translate(translateX, translateY)) : default(IDisposable);
            var scaleDisposable = scale != 1.0 ? _dc.PushPreTransform(AME.MatrixHelper.Scale(scale, scale)) : default(IDisposable);

            if ((image.IsStroked || image.IsFilled) && style != null)
            {
                GetCached(style, out var fill, out var stroke, scaleToPage, scaleThickness);

                DrawRectangleInternal(
                    _dc,
                    fill,
                    stroke,
                    image.IsStroked,
                    image.IsFilled,
                    ref rect);
            }

            var imageCached = _biCache.Get(image.Key);
            if (imageCached != null)
            {
                try
                {
                    _dc.DrawImage(
                        imageCached,
                        new A.Rect(0, 0, imageCached.PixelSize.Width, imageCached.PixelSize.Height),
                        new A.Rect(rect.X, rect.Y, rect.Width, rect.Height));
                }
                catch (Exception ex)
                {
                    _serviceProvider.GetService<ILog>()?.LogException(ex);
                }
            }
            else
            {
                if (_state.ImageCache == null || string.IsNullOrEmpty(image.Key))
                {
                    return;
                }

                try
                {
                    var bytes = _state.ImageCache.GetImage(image.Key);
                    if (bytes != null)
                    {
                        using var ms = new System.IO.MemoryStream(bytes);
                        var bi = new AMI.Bitmap(ms);

                        _biCache.Set(image.Key, bi);

                        _dc.DrawImage(
                            bi,
                            new A.Rect(0, 0, bi.PixelSize.Width, bi.PixelSize.Height),
                            new A.Rect(rect.X, rect.Y, rect.Width, rect.Height));
                    }
                }
                catch (Exception ex)
                {
                    _serviceProvider.GetService<ILog>()?.LogException(ex);
                }
            }

            scaleDisposable?.Dispose();
            translateDisposable?.Dispose();
        }

        /// <inheritdoc/>
        public void Draw(object dc, IPathShape path, double dx, double dy)
        {
            if (path.Geometry == null)
            {
                return;
            }

            if (!path.IsFilled && !path.IsStroked)
            {
                return;
            }

            var _dc = dc as AM.DrawingContext;

            var style = path.Style;
            if (style == null)
            {
                return;
            }

            var scaleThickness = path.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            var scaleSize = path.State.Flags.HasFlag(ShapeStateFlags.Size);
            var geometry = PathGeometryConverter.ToGeometry(path.Geometry, dx, dy);

            var scale = scaleSize ? 1.0 / _state.ZoomX : 1.0;
            var scaleToPage = scale == 1.0 ? _scaleToPage : (value) => (float)(_scaleToPage(value) / scale);
            var center = geometry.Bounds.Center;
            var translateX = 0.0 - (center.X * scale) + center.X;
            var translateY = 0.0 - (center.Y * scale) + center.Y;

            GetCached(style, out var fill, out var stroke, scaleToPage, scaleThickness);

            var translateDisposable = scale != 1.0 ? _dc.PushPreTransform(AME.MatrixHelper.Translate(translateX, translateY)) : default(IDisposable);
            var scaleDisposable = scale != 1.0 ? _dc.PushPreTransform(AME.MatrixHelper.Scale(scale, scale)) : default(IDisposable);

            _dc.DrawGeometry(
                path.IsFilled ? fill : null,
                path.IsStroked ? stroke : null,
                geometry);

            scaleDisposable?.Dispose();
            translateDisposable?.Dispose();
        }
    }
}
