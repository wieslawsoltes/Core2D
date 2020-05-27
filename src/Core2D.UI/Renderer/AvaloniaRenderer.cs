#define USE_DRAW_NODES
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
#if USE_DRAW_NODES
    internal abstract class DrawNode : IDisposable
    {
        public IShapeStyle Style { get; set; }
        public bool ScaleThickness { get; set; }
        public bool ScaleSize { get; set; }
        public AM.IBrush Fill { get; set; }
        public AM.Pen Stroke { get; set; }
        public A.Point Center { get; set; }

        protected AM.Color ToColor(IArgbColor argbColor)
        {
            return AM.Color.FromArgb(argbColor.A, argbColor.R, argbColor.G, argbColor.B);
        }

        protected AM.IBrush ToBrush(IColor color) => color switch
        {
            IArgbColor argbColor => new AM.SolidColorBrush(ToColor(argbColor)),
            _ => throw new NotSupportedException($"The {color.GetType()} color type is not supported."),
        };

        protected AM.Pen ToPen(IBaseStyle style)
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

            var thickness = style.Thickness;
            var brush = ToBrush(style.Stroke);
            var pen = new AM.Pen(brush, thickness, dashStyle, lineCap);

            return pen;
        }

        public DrawNode()
        {
        }

        public abstract void UpdateGeometry();

        public virtual void UpdateStyle()
        {
            Fill = ToBrush(Style.Fill);
            Stroke = ToPen(Style);
        }

        public virtual void Draw(AM.DrawingContext context, double dx, double dy, double zoom)
        {
            var scale = ScaleSize ? 1.0 / zoom : 1.0;
            var translateX = 0.0 - (Center.X * scale) + Center.X;
            var translateY = 0.0 - (Center.Y * scale) + Center.Y;

            double thickness = Style.Thickness;

            if (ScaleThickness)
            {
                thickness /= zoom;
            }

            if (scale != 1.0)
            {
                thickness /= scale;
            }

            Stroke.Thickness = thickness;

            var offsetDisposable = dx != 0.0 || dy != 0.0 ? context.PushPreTransform(AME.MatrixHelper.Translate(dx, dy)) : default(IDisposable);
            var translateDisposable = scale != 1.0 ? context.PushPreTransform(AME.MatrixHelper.Translate(translateX, translateY)) : default(IDisposable);
            var scaleDisposable = scale != 1.0 ? context.PushPreTransform(AME.MatrixHelper.Scale(scale, scale)) : default(IDisposable);

            OnDraw(context, dx, dy, zoom);

            scaleDisposable?.Dispose();
            translateDisposable?.Dispose();
            offsetDisposable?.Dispose();
        }

        public abstract void OnDraw(AM.DrawingContext context, double dx, double dy, double zoom);

        public virtual void Dispose()
        {
        }
    }

    internal class FillDrawNode : DrawNode
    {
        public A.Rect Rect { get; set; }
        public IColor Color { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public FillDrawNode(double x, double y, double width, double height, IColor color)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Color = color;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = false;
            ScaleSize = false;
            Rect = new A.Rect(X, Y, Width, Height);
            Center = Rect.Center;
        }

        public override void UpdateStyle()
        {
            Fill = ToBrush(Color);
        }

        public override void Draw(AM.DrawingContext context, double dx, double dy, double zoom)
        {
            OnDraw(context, dx, dy, zoom);
        }

        public override void OnDraw(AM.DrawingContext context, double dx, double dy, double zoom)
        {
            context.FillRectangle(Fill, Rect);
        }
    }

    internal class PointDrawNode : DrawNode
    {
        public IPointShape Point { get; set; }
        public double PointSize { get; set; }
        public A.Rect Rect { get; set; }

        public PointDrawNode(IPointShape point, IShapeStyle style, double pointSize)
        {
            Style = style;
            Point = point;
            PointSize = pointSize;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = true; // Point.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = true; // Point.State.Flags.HasFlag(ShapeStateFlags.Size);
            var rect2 = Rect2.FromPoints(Point.X - PointSize, Point.Y - PointSize, Point.X + PointSize, Point.Y + PointSize, 0, 0);
            Rect = new A.Rect(rect2.X, rect2.Y, rect2.Width, rect2.Height);
            Center = Rect.Center;
        }

        public override void OnDraw(AM.DrawingContext context, double dx, double dy, double zoom)
        {
            context.FillRectangle(Fill, Rect);
            context.DrawRectangle(Stroke, Rect);
        }
    }


    internal class LineDrawNode : DrawNode
    {
        public ILineShape Line { get; set; }
        public A.Point P0 { get; set; }
        public A.Point P1 { get; set; }

        public LineDrawNode(ILineShape line, IShapeStyle style)
        {
            Style = style;
            Line = line;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = Line.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Line.State.Flags.HasFlag(ShapeStateFlags.Size);
            P0 = new A.Point(Line.Start.X, Line.Start.Y);
            P1 = new A.Point(Line.End.X, Line.End.Y);
            Center = new A.Point((P0.X + P1.X) / 2.0, (P0.Y + P1.Y) / 2.0);
        }

        public override void OnDraw(AM.DrawingContext context, double dx, double dy, double zoom)
        {
            if (Line.IsStroked)
            {
                context.DrawLine(Stroke, P0, P1);
            }
        }
    }

    internal class RectangleDrawNode : DrawNode
    {
        public IRectangleShape Rectangle { get; set; }
        public A.Rect Rect { get; set; }

        public RectangleDrawNode(IRectangleShape rectangle, IShapeStyle style)
        {
            Style = style;
            Rectangle = rectangle;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = Rectangle.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Rectangle.State.Flags.HasFlag(ShapeStateFlags.Size);
            var rect2 = Rect2.FromPoints(Rectangle.TopLeft.X, Rectangle.TopLeft.Y, Rectangle.BottomRight.X, Rectangle.BottomRight.Y, 0, 0);
            Rect = new A.Rect(rect2.X, rect2.Y, rect2.Width, rect2.Height);
            Center = Rect.Center;
        }

        public override void OnDraw(AM.DrawingContext context, double dx, double dy, double zoom)
        {
            if (Rectangle.IsFilled)
            {
                context.FillRectangle(Fill, Rect);
            }

            if (Rectangle.IsStroked)
            {
                context.DrawRectangle(Stroke, Rect);
            }

            if (Rectangle.IsStroked && Rectangle.IsGrid)
            {
                double ox = Rect.X;
                double oy = Rect.Y;
                double sx = ox + Rectangle.OffsetX;
                double sy = oy + Rectangle.OffsetY;
                double ex = ox + Rect.Width;
                double ey = oy + Rect.Height;

                for (double x = sx; x < ex; x += Rectangle.CellWidth)
                {
                    var p0 = new A.Point(x, oy);
                    var p1 = new A.Point(x, ey);
                    context.DrawLine(Stroke, p0, p1);

                }

                for (double y = sy; y < ey; y += Rectangle.CellHeight)
                {
                    var p0 = new A.Point(ox, y);
                    var p1 = new A.Point(ex, y);
                    context.DrawLine(Stroke, p0, p1);
                }
            }
        }
    }

    internal class EllipseDrawNode : DrawNode
    {
        public IEllipseShape Ellipse { get; set; }
        public AM.Geometry Geometry { get; set; }

        public EllipseDrawNode(IEllipseShape ellipse, IShapeStyle style)
        {
            Style = style;
            Ellipse = ellipse;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = Ellipse.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Ellipse.State.Flags.HasFlag(ShapeStateFlags.Size);
            Geometry = PathGeometryConverter.ToGeometry(Ellipse, 0, 0);
            Center = Geometry.Bounds.Center;
        }

        public override void OnDraw(AM.DrawingContext context, double dx, double dy, double zoom)
        {
            context.DrawGeometry(Ellipse.IsFilled ? Fill : null, Ellipse.IsStroked ? Stroke : null, Geometry);
        }
    }

    internal class ArcDrawNode : DrawNode
    {
        public IArcShape Arc { get; set; }
        public AM.Geometry Geometry { get; set; }

        public ArcDrawNode(IArcShape arc, IShapeStyle style)
        {
            Style = style;
            Arc = arc;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = Arc.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Arc.State.Flags.HasFlag(ShapeStateFlags.Size);
            Geometry = PathGeometryConverter.ToGeometry(Arc, 0, 0);
            Center = Geometry.Bounds.Center;
        }

        public override void OnDraw(AM.DrawingContext context, double dx, double dy, double zoom)
        {
            context.DrawGeometry(Arc.IsFilled ? Fill : null, Arc.IsStroked ? Stroke : null, Geometry);
        }
    }

    internal class CubicBezierDrawNode : DrawNode
    {
        public ICubicBezierShape CubicBezier { get; set; }
        public AM.Geometry Geometry { get; set; }

        public CubicBezierDrawNode(ICubicBezierShape cubicBezier, IShapeStyle style)
        {
            Style = style;
            CubicBezier = cubicBezier;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = CubicBezier.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = CubicBezier.State.Flags.HasFlag(ShapeStateFlags.Size);
            Geometry = PathGeometryConverter.ToGeometry(CubicBezier, 0, 0);
            Center = Geometry.Bounds.Center;
        }

        public override void OnDraw(AM.DrawingContext context, double dx, double dy, double zoom)
        {
            context.DrawGeometry(CubicBezier.IsFilled ? Fill : null, CubicBezier.IsStroked ? Stroke : null, Geometry);
        }
    }

    internal class QuadraticBezierDrawNode : DrawNode
    {
        public IQuadraticBezierShape QuadraticBezier { get; set; }
        public AM.Geometry Geometry { get; set; }

        public QuadraticBezierDrawNode(IQuadraticBezierShape quadraticBezier, IShapeStyle style)
        {
            Style = style;
            QuadraticBezier = quadraticBezier;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = QuadraticBezier.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = QuadraticBezier.State.Flags.HasFlag(ShapeStateFlags.Size);
            Geometry = PathGeometryConverter.ToGeometry(QuadraticBezier, 0, 0);
            Center = Geometry.Bounds.Center;
        }

        public override void OnDraw(AM.DrawingContext context, double dx, double dy, double zoom)
        {
            context.DrawGeometry(QuadraticBezier.IsFilled ? Fill : null, QuadraticBezier.IsStroked ? Stroke : null, Geometry);
        }
    }

    internal class PathDrawNode : DrawNode
    {
        public IPathShape Path { get; set; }
        public AM.Geometry Geometry { get; set; }

        public PathDrawNode(IPathShape path, IShapeStyle style)
        {
            Style = style;
            Path = path;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = Path.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Path.State.Flags.HasFlag(ShapeStateFlags.Size);
            Geometry = PathGeometryConverter.ToGeometry(Path.Geometry, 0, 0);
            Center = Geometry.Bounds.Center;
        }

        public override void OnDraw(AM.DrawingContext context, double dx, double dy, double zoom)
        {
            context.DrawGeometry(Path.IsFilled ? Fill : null, Path.IsStroked ? Stroke : null, Geometry);
        }
    }

    /// <summary>
    /// Native Avalonia shape renderer.
    /// </summary>
    public class AvaloniaRenderer : ObservableObject, IShapeRenderer
    {
        private readonly IServiceProvider _serviceProvider;
        private IShapeRendererState _state;
        private readonly ICache<ITextShape, (string, AM.FormattedText, IShapeStyle)> _textCache;
        private readonly ICache<string, AMI.Bitmap> _biCache;
        private readonly ICache<object, DrawNode> _drawNodeCache;
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
            _textCache = _serviceProvider.GetService<IFactory>().CreateCache<ITextShape, (string, AM.FormattedText, IShapeStyle)>();
            _biCache = _serviceProvider.GetService<IFactory>().CreateCache<string, AMI.Bitmap>(x => x.Dispose());
            _drawNodeCache = _serviceProvider.GetService<IFactory>().CreateCache<object, DrawNode>(x => x.Dispose());
            _textScaleFactor = textScaleFactor;
            ClearCache(isZooming: false);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /*
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
            GetCached(style.StartArrowStyle, out var fillStartArrow, out var strokeStartArrow, scaleStrokeWidth);
            GetCached(style.EndArrowStyle, out var fillEndArrow, out var strokeEndArrow, scaleStrokeWidth);

            double _x1 = line.Start.X + dx;
            double _y1 = line.Start.Y + dy;
            double _x2 = line.End.X + dx;
            double _y2 = line.End.Y + dy;
            line.GetMaxLength(ref _x1, ref _y1, ref _x2, ref _y2);

            float x1 = (float)_x1;
            float y1 = (float)_y1;
            float x2 = (float)_x2;
            float y2 = (float)_y2;
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
        */

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
            if (!isZooming)
            {
                _textCache.Reset();
                _biCache.Reset();
                //_drawNodeCache.Reset();
            }
        }

        /// <inheritdoc/>
        public void Fill(object dc, double x, double y, double width, double height, IColor color)
        {
            var context = dc as AM.DrawingContext;

            var drawNodeCached = _drawNodeCache.Get(color);
            if (drawNodeCached != null)
            {
                if (drawNodeCached is FillDrawNode drawNode)
                {
                    drawNode.X = x;
                    drawNode.Y = y;
                    drawNode.Width = width;
                    drawNode.Height = height;
                    drawNode.UpdateGeometry();
                    if (color.IsDirty())
                    {
                        drawNode.Color = color;
                        drawNode.UpdateStyle();
                        color.Invalidate();
                    }
                    drawNode.Draw(context, 0, 0, _state.ZoomX);
                }
            }
            else
            {
                var drawNode = new FillDrawNode(x, y, width, height, color);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(color, drawNode);

                drawNode.Draw(context, 0, 0, _state.ZoomX);
            }
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
            bool isSelected = _state.SelectedShapes?.Count > 0 && _state.SelectedShapes.Contains(point);
            var pointStyle = isSelected ? _state.SelectedPointStyle : _state.PointStyle;
            if (pointStyle == null)
            {
                return;
            }

            var pointSize = _state.PointSize;
            if (pointSize <= 0.0)
            {
                return;
            }

            var context = dc as AM.DrawingContext;

            var drawNodeCached = _drawNodeCache.Get(point);
            if (drawNodeCached != null)
            {
                // TODO:
                //if (pointStyle.IsDirty())
                //{
                //    drawNodeCached.Style = pointStyle;
                //    drawNodeCached.UpdateStyle();
                //    pointStyle.Invalidate();
                //}

                if (point.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                    //point.Invalidate();
                }

                drawNodeCached.Draw(context, dx, dy, _state.ZoomX);
            }
            else
            {
                var drawNode = new PointDrawNode(point, pointStyle, pointSize);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(point, drawNode);

                drawNode.Draw(context, dx, dy, _state.ZoomX);
            }
        }

        /// <inheritdoc/>
        public void Draw(object dc, ILineShape line, double dx, double dy)
        {
            var context = dc as AM.DrawingContext;

            var drawNodeCached = _drawNodeCache.Get(line);
            if (drawNodeCached != null)
            {
                if (line.Style.IsDirty())
                {
                    drawNodeCached.Style = line.Style;
                    drawNodeCached.UpdateStyle();
                    line.Style.Invalidate();
                }

                if (line.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                    //line.Invalidate();
                }

                drawNodeCached.Draw(context, dx, dy, _state.ZoomX);
            }
            else
            {
                var drawNode = new LineDrawNode(line, line.Style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(line, drawNode);

                drawNode.Draw(context, dx, dy, _state.ZoomX);
            }
            /*
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

            GetCached(style, out _, out var stroke, scaleThickness);

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
            */
        }

        /// <inheritdoc/>
        public void Draw(object dc, IRectangleShape rectangle, double dx, double dy)
        {
            var context = dc as AM.DrawingContext;

            var drawNodeCached = _drawNodeCache.Get(rectangle);
            if (drawNodeCached != null)
            {
                if (rectangle.Style.IsDirty())
                {
                    drawNodeCached.Style = rectangle.Style;
                    drawNodeCached.UpdateStyle();
                    rectangle.Style.Invalidate();
                }

                if (rectangle.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                    //rectangle.Invalidate();
                }

                drawNodeCached.Draw(context, dx, dy, _state.ZoomX);
            }
            else
            {
                var drawNode = new RectangleDrawNode(rectangle, rectangle.Style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(rectangle, drawNode);

                drawNode.Draw(context, dx, dy, _state.ZoomX);
            }
        }

        /// <inheritdoc/>
        public void Draw(object dc, IEllipseShape ellipse, double dx, double dy)
        {
            var context = dc as AM.DrawingContext;

            var drawNodeCached = _drawNodeCache.Get(ellipse);
            if (drawNodeCached != null)
            {
                if (ellipse.Style.IsDirty())
                {
                    drawNodeCached.Style = ellipse.Style;
                    drawNodeCached.UpdateStyle();
                    ellipse.Style.Invalidate();
                }

                if (ellipse.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                    //ellipse.Invalidate();
                }

                drawNodeCached.Draw(context, dx, dy, _state.ZoomX);
            }
            else
            {
                var drawNode = new EllipseDrawNode(ellipse, ellipse.Style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(ellipse, drawNode);

                drawNode.Draw(context, dx, dy, _state.ZoomX);
            }
        }

        /// <inheritdoc/>
        public void Draw(object dc, IArcShape arc, double dx, double dy)
        {
            var context = dc as AM.DrawingContext;

            var drawNodeCached = _drawNodeCache.Get(arc);
            if (drawNodeCached != null)
            {
                if (arc.Style.IsDirty())
                {
                    drawNodeCached.Style = arc.Style;
                    drawNodeCached.UpdateStyle();
                    arc.Style.Invalidate();
                }

                if (arc.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                    //arc.Invalidate();
                }

                drawNodeCached.Draw(context, dx, dy, _state.ZoomX);
            }
            else
            {
                var drawNode = new ArcDrawNode(arc, arc.Style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(arc, drawNode);

                drawNode.Draw(context, dx, dy, _state.ZoomX);
            }
        }

        /// <inheritdoc/>
        public void Draw(object dc, ICubicBezierShape cubicBezier, double dx, double dy)
        {
            var context = dc as AM.DrawingContext;

            var drawNodeCached = _drawNodeCache.Get(cubicBezier);
            if (drawNodeCached != null)
            {
                if (cubicBezier.Style.IsDirty())
                {
                    drawNodeCached.Style = cubicBezier.Style;
                    drawNodeCached.UpdateStyle();
                    cubicBezier.Style.Invalidate();
                }

                if (cubicBezier.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                    //cubicBezier.Invalidate();
                }

                drawNodeCached.Draw(context, dx, dy, _state.ZoomX);
            }
            else
            {
                var drawNode = new CubicBezierDrawNode(cubicBezier, cubicBezier.Style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(cubicBezier, drawNode);

                drawNode.Draw(context, dx, dy, _state.ZoomX);
            }
        }

        /// <inheritdoc/>
        public void Draw(object dc, IQuadraticBezierShape quadraticBezier, double dx, double dy)
        {
            var context = dc as AM.DrawingContext;

            var drawNodeCached = _drawNodeCache.Get(quadraticBezier);
            if (drawNodeCached != null)
            {
                if (quadraticBezier.Style.IsDirty())
                {
                    drawNodeCached.Style = quadraticBezier.Style;
                    drawNodeCached.UpdateStyle();
                    quadraticBezier.Style.Invalidate();
                }

                if (quadraticBezier.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                    //quadraticBezier.Invalidate();
                }

                drawNodeCached.Draw(context, dx, dy, _state.ZoomX);
            }
            else
            {
                var drawNode = new QuadraticBezierDrawNode(quadraticBezier, quadraticBezier.Style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(quadraticBezier, drawNode);

                drawNode.Draw(context, dx, dy, _state.ZoomX);
            }
        }

        /// <inheritdoc/>
        public void Draw(object dc, ITextShape text, double dx, double dy)
        {
            /*
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
            var rect = Rect2.FromPoints(text.TopLeft.X, text.TopLeft.Y, text.BottomRight.X, text.BottomRight.Y, dx, dy);

            var scale = scaleSize ? 1.0 / _state.ZoomX : 1.0;
            var scaleToPage = scale == 1.0 ? _scaleToPage : (value) => (float)(_scaleToPage(value) / scale);
            var center = rect.Center;
            var translateX = 0.0 - (center.X * scale) + center.X;
            var translateY = 0.0 - (center.Y * scale) + center.Y;

            GetCached(style, out _, out var stroke, scaleThickness);

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
            */
        }

        /// <inheritdoc/>
        public void Draw(object dc, IImageShape image, double dx, double dy)
        {
            /*
            if (image.Key == null)
            {
                return;
            }

            var _dc = dc as AM.DrawingContext;
            var style = image.Style;

            var scaleThickness = image.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            var scaleSize = image.State.Flags.HasFlag(ShapeStateFlags.Size);
            var rect = Rect2.FromPoints(image.TopLeft.X, image.TopLeft.Y, image.BottomRight.X, image.BottomRight.Y, dx, dy);

            var scale = scaleSize ? 1.0 / _state.ZoomX : 1.0;
            var scaleToPage = scale == 1.0 ? _scaleToPage : (value) => (float)(_scaleToPage(value) / scale);
            var center = rect.Center;
            var translateX = 0.0 - (center.X * scale) + center.X;
            var translateY = 0.0 - (center.Y * scale) + center.Y;

            var translateDisposable = scale != 1.0 ? _dc.PushPreTransform(AME.MatrixHelper.Translate(translateX, translateY)) : default(IDisposable);
            var scaleDisposable = scale != 1.0 ? _dc.PushPreTransform(AME.MatrixHelper.Scale(scale, scale)) : default(IDisposable);

            if ((image.IsStroked || image.IsFilled) && style != null)
            {
                GetCached(style, out var fill, out var stroke, scaleThickness);

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
            */
        }

        /// <inheritdoc/>
        public void Draw(object dc, IPathShape path, double dx, double dy)
        {
            var context = dc as AM.DrawingContext;

            var drawNodeCached = _drawNodeCache.Get(path);
            if (drawNodeCached != null)
            {
                if (path.Style.IsDirty())
                {
                    drawNodeCached.Style = path.Style;
                    drawNodeCached.UpdateStyle();
                    path.Style.Invalidate();
                }

                if (path.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                    //path.Invalidate();
                }

                drawNodeCached.Draw(context, dx, dy, _state.ZoomX);
            }
            else
            {
                var drawNode = new PathDrawNode(path, path.Style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(path, drawNode);

                drawNode.Draw(context, dx, dy, _state.ZoomX);
            }
        }
    }
#else
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

            bool isSelected = _state.SelectedShapes?.Count > 0 && _state.SelectedShapes.Contains(point);

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
            var geometry = PathGeometryConverter.ToGeometry(arc, dx, dy);

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
            var geometry = PathGeometryConverter.ToGeometry(cubicBezier, dx, dy);

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
            var geometry = PathGeometryConverter.ToGeometry(quadraticBezier, dx, dy);

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
#endif
}
