using System;
using System.Collections.Generic;
using System.Diagnostics;
using Core2D.Containers;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;
using Spatial;
using A = Avalonia;
using AM = Avalonia.Media;
using AME = Avalonia.MatrixExtensions;
using AMI = Avalonia.Media.Imaging;

namespace Core2D.UI.Renderer
{
    internal abstract class DrawNode : IDisposable
    {
        public IShapeStyle Style { get; set; }
        public bool ScaleThickness { get; set; }
        public bool ScaleSize { get; set; }
        public AM.IBrush Fill { get; set; }
        public AM.IPen Stroke { get; set; }
        public A.Point Center { get; set; }

        protected AM.Color ToColor(IArgbColor argbColor)
        {
            return AM.Color.FromArgb(argbColor.A, argbColor.R, argbColor.G, argbColor.B);
        }

        protected AM.IBrush ToBrush(IColor color) => color switch
        {
            IArgbColor argbColor => new AM.Immutable.ImmutableSolidColorBrush(ToColor(argbColor)),
            _ => throw new NotSupportedException($"The {color.GetType()} color type is not supported.")
        };

        protected AM.IPen ToPen(IBaseStyle style, double thickness)
        {
            var dashStyle = default(AM.Immutable.ImmutableDashStyle);
            if (style.Dashes != null)
            {
                var dashes = StyleHelper.ConvertDashesToDoubleArray(style.Dashes, 1.0);
                var dashOffset = style.DashOffset;
                if (dashes != null)
                {
                    dashStyle = new AM.Immutable.ImmutableDashStyle(dashes, dashOffset);
                }
            }

            var lineCap = style.LineCap switch
            {
                LineCap.Flat => AM.PenLineCap.Flat,
                LineCap.Square => AM.PenLineCap.Square,
                LineCap.Round => AM.PenLineCap.Round,
                _ => throw new NotImplementedException()
            };

            var brush = ToBrush(style.Stroke);
            var pen = new AM.Immutable.ImmutablePen(brush, thickness, dashStyle, lineCap);

            return pen;
        }

        public DrawNode()
        {
        }

        public abstract void UpdateGeometry();

        public virtual void UpdateStyle()
        {
            Fill = ToBrush(Style.Fill);
            Stroke = ToPen(Style, Style.Thickness);
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

            if (Stroke.Thickness != thickness)
            {
                Stroke = ToPen(Style, thickness);
            }

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

        /*
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

        public override void UpdateGeometry()
        {
            ScaleThickness = Line.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Line.State.Flags.HasFlag(ShapeStateFlags.Size);
            P0 = new A.Point(Line.Start.X, Line.Start.Y);
            P1 = new A.Point(Line.End.X, Line.End.Y);
            Center = new A.Point((P0.X + P1.X) / 2.0, (P0.Y + P1.Y) / 2.0);

            // TODO: Curved

            // TODO: Arrows
        }

        public override void OnDraw(AM.DrawingContext context, double dx, double dy, double zoom)
        {
            if (Line.IsStroked)
            {
                context.DrawLine(Stroke, P0, P1);
            }

            // TODO: Curved

            // TODO: Arrows

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
    }

    internal class RectangleDrawNode : TextDrawNode
    {
        public IRectangleShape Rectangle { get; set; }

        public RectangleDrawNode(IRectangleShape rectangle, IShapeStyle style)
            : base()
        {
            Style = style;
            Rectangle = rectangle;
            Text = rectangle;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = Rectangle.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Rectangle.State.Flags.HasFlag(ShapeStateFlags.Size);
            var rect2 = Rect2.FromPoints(Rectangle.TopLeft.X, Rectangle.TopLeft.Y, Rectangle.BottomRight.X, Rectangle.BottomRight.Y, 0, 0);
            Rect = new A.Rect(rect2.X, rect2.Y, rect2.Width, rect2.Height);
            Center = Rect.Center;

            base.UpdateTextGeometry();
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

            base.OnDraw(context, dx, dy, zoom);
        }
    }

    internal class EllipseDrawNode : TextDrawNode
    {
        public IEllipseShape Ellipse { get; set; }
        public AM.Geometry Geometry { get; set; }

        public EllipseDrawNode(IEllipseShape ellipse, IShapeStyle style)
            : base()
        {
            Style = style;
            Ellipse = ellipse;
            Text = ellipse;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = Ellipse.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Ellipse.State.Flags.HasFlag(ShapeStateFlags.Size);
            Geometry = PathGeometryConverter.ToGeometry(Ellipse, 0, 0);
            Rect = Geometry.Bounds;
            Center = Geometry.Bounds.Center;

            base.UpdateTextGeometry();
        }

        public override void OnDraw(AM.DrawingContext context, double dx, double dy, double zoom)
        {
            context.DrawGeometry(Ellipse.IsFilled ? Fill : null, Ellipse.IsStroked ? Stroke : null, Geometry);

            base.OnDraw(context, dx, dy, zoom);
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

    internal class TextDrawNode : DrawNode
    {
        public ITextShape Text { get; set; }
        public A.Rect Rect { get; set; }
        public A.Point Origin { get; set; }
        public AM.Typeface Typeface { get; set; }
        public AM.FormattedText FormattedText { get; set; }
        public string BoundText { get; set; }

        protected TextDrawNode()
        {
        }

        public TextDrawNode(ITextShape text, IShapeStyle style)
        {
            Style = style;
            Text = text;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = Text.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Text.State.Flags.HasFlag(ShapeStateFlags.Size);
            var rect2 = Rect2.FromPoints(Text.TopLeft.X, Text.TopLeft.Y, Text.BottomRight.X, Text.BottomRight.Y, 0, 0);
            Rect = new A.Rect(rect2.X, rect2.Y, rect2.Width, rect2.Height);
            Center = Rect.Center;

            UpdateTextGeometry();
        }

        protected void UpdateTextGeometry()
        {
            BoundText = Text.GetProperty(nameof(ITextShape.Text)) is string boundText ? boundText : Text.Text;

            if (BoundText == null)
            {
                return;
            }

            if (Style.TextStyle.FontSize < 0.0)
            {
                return;
            }

            var fontStyle = AM.FontStyle.Normal;
            var fontWeight = AM.FontWeight.Normal;

            if (Style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Italic))
            {
                fontStyle |= AM.FontStyle.Italic;
            }

            if (Style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Bold))
            {
                fontWeight |= AM.FontWeight.Bold;
            }

            // TODO: Cache Typeface
            // TODO: Cache FormattedText

            Typeface = new AM.Typeface(Style.TextStyle.FontName, fontWeight, fontStyle);

            var textAlignment = Style.TextStyle.TextHAlignment switch
            {
                TextHAlignment.Right => AM.TextAlignment.Right,
                TextHAlignment.Center => AM.TextAlignment.Center,
                _ => AM.TextAlignment.Left,
            };

            FormattedText = new AM.FormattedText()
            {
                Typeface = Typeface,
                Text = BoundText,
                TextAlignment = textAlignment,
                TextWrapping = AM.TextWrapping.NoWrap,
                FontSize = Style.TextStyle.FontSize,
                Constraint = Rect.Size
            };

            var size = FormattedText.Bounds.Size;
            var rect = Rect;

            var originX = rect.X; // NOTE: Using AM.TextAlignment
            //var originX = Style.TextStyle.TextHAlignment switch
            //{
            //    TextHAlignment.Left => rect.X,
            //    TextHAlignment.Right => rect.Right - size.Width,
            //    _ => (rect.Left + rect.Width / 2.0) - (size.Width / 2.0)
            //};

            var originY = Style.TextStyle.TextVAlignment switch
            {
                TextVAlignment.Top => rect.Y,
                TextVAlignment.Bottom => rect.Bottom - size.Height,
                _ => (rect.Bottom - rect.Height / 2f) - (size.Height / 2f)
            };

            Origin = new A.Point(originX, originY);
        }

        public override void OnDraw(AM.DrawingContext context, double dx, double dy, double zoom)
        {
            if (FormattedText != null)
            {
                context.DrawText(Stroke.Brush, Origin, FormattedText); 
            }
        }
    }

    internal class ImageDrawNode : TextDrawNode
    {
        public IImageShape Image { get; set; }
        public IImageCache ImageCache { get; set; }
        public ICache<string, AMI.Bitmap> BitmapCache { get; set; }
        public AMI.Bitmap ImageCached { get; set; }
        public A.Rect SourceRect { get; set; }
        public A.Rect DestRect { get; set; }

        public ImageDrawNode(IImageShape image, IShapeStyle style, IImageCache imageCache, ICache<string, AMI.Bitmap> bitmapCache)
            : base()
        {
            Style = style;
            Image = image;
            Text = image;
            ImageCache = imageCache;
            BitmapCache = bitmapCache;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = Image.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Image.State.Flags.HasFlag(ShapeStateFlags.Size);

            if (!string.IsNullOrEmpty(Image.Key))
            {
                ImageCached = BitmapCache.Get(Image.Key);
                if (ImageCached == null && ImageCache != null)
                {
                    try
                    {
                        var bytes = ImageCache.GetImage(Image.Key);
                        if (bytes != null)
                        {
                            using var ms = new System.IO.MemoryStream(bytes);
                            ImageCached = new AMI.Bitmap(ms);
                            BitmapCache.Set(Image.Key, ImageCached);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"{ex.Message}");
                        Debug.WriteLine($"{ex.StackTrace}");
                    }
                }

                if (ImageCached != null)
                {
                    SourceRect = new A.Rect(0, 0, ImageCached.PixelSize.Width, ImageCached.PixelSize.Height);
                }
            }
            else
            {
                ImageCached = null;
            }

            var rect2 = Rect2.FromPoints(Image.TopLeft.X, Image.TopLeft.Y, Image.BottomRight.X, Image.BottomRight.Y, 0, 0);
            DestRect = new A.Rect(rect2.X, rect2.Y, rect2.Width, rect2.Height);
            Center = DestRect.Center;

            base.UpdateTextGeometry();
        }

        public override void OnDraw(AM.DrawingContext context, double dx, double dy, double zoom)
        {
            if (Image.IsFilled)
            {
                context.FillRectangle(Fill, DestRect);
            }

            if (Image.IsStroked)
            {
                context.DrawRectangle(Stroke, DestRect);
            }

            if (ImageCached != null)
            {
                try
                {
                    context.DrawImage(ImageCached, SourceRect, DestRect);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{ex.Message}");
                    Debug.WriteLine($"{ex.StackTrace}");
                }
            }

            base.OnDraw(context, dx, dy, zoom);
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
        private readonly ICache<string, AMI.Bitmap> _biCache;
        private readonly ICache<object, DrawNode> _drawNodeCache;

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
        public AvaloniaRenderer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _state = _serviceProvider.GetService<IFactory>().CreateShapeRendererState();
            _biCache = _serviceProvider.GetService<IFactory>().CreateCache<string, AMI.Bitmap>(x => x.Dispose());
            _drawNodeCache = _serviceProvider.GetService<IFactory>().CreateCache<object, DrawNode>(x => x.Dispose());
            ClearCache(isZooming: false);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
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
            if (!isZooming)
            {
                // TODO: _biCache.Reset();
                // TODO: _drawNodeCache.Reset();
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
        public void DrawPage(object dc, IPageContainer container, double dx, double dy)
        {
            foreach (var layer in container.Layers)
            {
                if (layer.IsVisible)
                {
                    DrawLayer(dc, layer, dx, dy);
                }
            }
        }

        /// <inheritdoc/>
        public void DrawLayer(object dc, ILayerContainer layer, double dx, double dy)
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
        public void DrawPoint(object dc, IPointShape point, double dx, double dy)
        {
            var isSelected = _state.SelectedShapes?.Count > 0 && _state.SelectedShapes.Contains(point);
            var pointStyle = isSelected ? _state.SelectedPointStyle : _state.PointStyle;
            var pointSize = _state.PointSize;
            if (pointStyle == null || pointSize <= 0.0)
            {
                return;
            }

            var context = dc as AM.DrawingContext;

            var drawNodeCached = _drawNodeCache.Get(point);
            if (drawNodeCached != null)
            {
                if (pointStyle.IsDirty() || drawNodeCached.Style != pointStyle)
                {
                    drawNodeCached.Style = pointStyle;
                    drawNodeCached.UpdateStyle();
                }

                if (point.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                }

                if (_state.DrawPoints == true)
                {
                    drawNodeCached.Draw(context, dx, dy, _state.ZoomX);
                }
            }
            else
            {
                var drawNode = new PointDrawNode(point, pointStyle, pointSize);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(point, drawNode);

                if (_state.DrawPoints == true)
                {
                    drawNode.Draw(context, dx, dy, _state.ZoomX);
                }
            }
        }

        /// <inheritdoc/>
        public void DrawLine(object dc, ILineShape line, double dx, double dy)
        {
            var context = dc as AM.DrawingContext;

            var drawNodeCached = _drawNodeCache.Get(line);
            if (drawNodeCached != null)
            {
                if (line.Style.IsDirty() || drawNodeCached.Style != line.Style)
                {
                    drawNodeCached.Style = line.Style;
                    drawNodeCached.UpdateStyle();
                    line.Style.Invalidate();
                }

                if (line.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
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
        }

        /// <inheritdoc/>
        public void DrawRectangle(object dc, IRectangleShape rectangle, double dx, double dy)
        {
            var context = dc as AM.DrawingContext;

            var drawNodeCached = _drawNodeCache.Get(rectangle);
            if (drawNodeCached != null)
            {
                if (rectangle.Style.IsDirty() || drawNodeCached.Style != rectangle.Style)
                {
                    drawNodeCached.Style = rectangle.Style;
                    drawNodeCached.UpdateStyle();
                    rectangle.Style.Invalidate();
                }

                if (rectangle.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
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
        public void DrawEllipse(object dc, IEllipseShape ellipse, double dx, double dy)
        {
            var context = dc as AM.DrawingContext;

            var drawNodeCached = _drawNodeCache.Get(ellipse);
            if (drawNodeCached != null)
            {
                if (ellipse.Style.IsDirty() || drawNodeCached.Style != ellipse.Style)
                {
                    drawNodeCached.Style = ellipse.Style;
                    drawNodeCached.UpdateStyle();
                    ellipse.Style.Invalidate();
                }

                if (ellipse.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
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
        public void DrawArc(object dc, IArcShape arc, double dx, double dy)
        {
            var context = dc as AM.DrawingContext;

            var drawNodeCached = _drawNodeCache.Get(arc);
            if (drawNodeCached != null)
            {
                if (arc.Style.IsDirty() || drawNodeCached.Style != arc.Style)
                {
                    drawNodeCached.Style = arc.Style;
                    drawNodeCached.UpdateStyle();
                    arc.Style.Invalidate();
                }

                if (arc.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
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
        public void DrawCubicBezier(object dc, ICubicBezierShape cubicBezier, double dx, double dy)
        {
            var context = dc as AM.DrawingContext;

            var drawNodeCached = _drawNodeCache.Get(cubicBezier);
            if (drawNodeCached != null)
            {
                if (cubicBezier.Style.IsDirty() || drawNodeCached.Style != cubicBezier.Style)
                {
                    drawNodeCached.Style = cubicBezier.Style;
                    drawNodeCached.UpdateStyle();
                    cubicBezier.Style.Invalidate();
                }

                if (cubicBezier.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
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
        public void DrawQuadraticBezier(object dc, IQuadraticBezierShape quadraticBezier, double dx, double dy)
        {
            var context = dc as AM.DrawingContext;

            var drawNodeCached = _drawNodeCache.Get(quadraticBezier);
            if (drawNodeCached != null)
            {
                if (quadraticBezier.Style.IsDirty() || drawNodeCached.Style != quadraticBezier.Style)
                {
                    drawNodeCached.Style = quadraticBezier.Style;
                    drawNodeCached.UpdateStyle();
                    quadraticBezier.Style.Invalidate();
                }

                if (quadraticBezier.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
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
        public void DrawText(object dc, ITextShape text, double dx, double dy)
        {
            var context = dc as AM.DrawingContext;

            var drawNodeCached = _drawNodeCache.Get(text);
            if (drawNodeCached != null)
            {
                if (text.Style.IsDirty() || drawNodeCached.Style != text.Style)
                {
                    drawNodeCached.Style = text.Style;
                    drawNodeCached.UpdateStyle();
                    drawNodeCached.UpdateGeometry();
                    text.Style.Invalidate();
                }

                if (text.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                }

                drawNodeCached.Draw(context, dx, dy, _state.ZoomX);
            }
            else
            {
                var drawNode = new TextDrawNode(text, text.Style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(text, drawNode);

                drawNode.Draw(context, dx, dy, _state.ZoomX);
            }
        }

        /// <inheritdoc/>
        public void DrawImage(object dc, IImageShape image, double dx, double dy)
        {
            var context = dc as AM.DrawingContext;

            var drawNodeCached = _drawNodeCache.Get(image);
            if (drawNodeCached != null)
            {
                if (image.Style.IsDirty() || drawNodeCached.Style != image.Style)
                {
                    drawNodeCached.Style = image.Style;
                    drawNodeCached.UpdateStyle();
                    image.Style.Invalidate();
                }

                if (image.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                }

                drawNodeCached.Draw(context, dx, dy, _state.ZoomX);
            }
            else
            {
                var drawNode = new ImageDrawNode(image, image.Style, _state.ImageCache, _biCache);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(image, drawNode);

                drawNode.Draw(context, dx, dy, _state.ZoomX);
            }
        }

        /// <inheritdoc/>
        public void DrawPath(object dc, IPathShape path, double dx, double dy)
        {
            var context = dc as AM.DrawingContext;

            var drawNodeCached = _drawNodeCache.Get(path);
            if (drawNodeCached != null)
            {
                if (path.Style.IsDirty() || drawNodeCached.Style != path.Style)
                {
                    drawNodeCached.Style = path.Style;
                    drawNodeCached.UpdateStyle();
                    path.Style.Invalidate();
                }

                if (path.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
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
}
