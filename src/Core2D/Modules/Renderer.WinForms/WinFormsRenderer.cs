using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using Core2D;
using Core2D.Containers;
using Core2D.Shapes;
using Core2D.Style;
using Spatial;
using Spatial.Arc;

namespace Core2D.Renderer.WinForms
{
    public class WinFormsRenderer : ViewModelBase, IShapeRenderer
    {
        private readonly IServiceProvider _serviceProvider;
        private ShapeRendererState _state;
        private ICache<string, Image> _biCache;
        private readonly Func<double, float> _scaleToPage;
        private readonly double _textScaleFactor;

        public ShapeRendererState State
        {
            get => _state;
            set => RaiseAndSetIfChanged(ref _state, value);
        }

        public WinFormsRenderer(IServiceProvider serviceProvider, double textScaleFactor = 1.0)
        {
            _serviceProvider = serviceProvider;
            _state = _serviceProvider.GetService<IFactory>().CreateShapeRendererState();
            _biCache = _serviceProvider.GetService<IFactory>().CreateCache<string, Image>(bi => bi.Dispose());
            _textScaleFactor = textScaleFactor;
            _scaleToPage = (value) => (float)(value);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        private static Color ToColor(BaseColor color)
        {
            return color switch
            {
                ArgbColor argbColor => Color.FromArgb(argbColor.A, argbColor.R, argbColor.G, argbColor.B),
                _ => throw new NotSupportedException($"The {color.GetType()} color type is not supported."),
            };
        }

        private Brush ToBrush(BaseColor color)
        {
            return color switch
            {
                ArgbColor argbColor => new SolidBrush(ToColor(argbColor)),
                _ => throw new NotSupportedException($"The {color.GetType()} color type is not supported."),
            };
        }

        private Pen ToPen(BaseStyle style, Func<double, float> scale)
        {
            var pen = new Pen(ToColor(style.Stroke), (float)(style.Thickness / State.ZoomX));
            switch (style.LineCap)
            {
                case Core2D.Style.LineCap.Flat:
                    pen.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
                    pen.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
                    pen.DashCap = System.Drawing.Drawing2D.DashCap.Flat;
                    break;

                case Core2D.Style.LineCap.Square:
                    pen.StartCap = System.Drawing.Drawing2D.LineCap.Square;
                    pen.EndCap = System.Drawing.Drawing2D.LineCap.Square;
                    pen.DashCap = System.Drawing.Drawing2D.DashCap.Flat;
                    break;

                case Core2D.Style.LineCap.Round:
                    pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                    pen.DashCap = System.Drawing.Drawing2D.DashCap.Round;
                    break;
            }
            if (style.Dashes != null)
            {
                // TODO: Convert to correct dash values.
                var dashPattern = StyleHelper.ConvertDashesToFloatArray(style.Dashes, 1);
                var dashOffset = (float)style.DashOffset;
                if (dashPattern != null)
                {
                    pen.DashPattern = dashPattern;
                    pen.DashStyle = DashStyle.Custom;
                    pen.DashOffset = dashOffset;
                }
            }
            return pen;
        }

        private Pen ToPen(BaseColor color, double thickness, Func<double, float> scale)
        {
            var pen = new Pen(ToColor(color), (float)(thickness / State.ZoomX));

            pen.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
            pen.DashCap = System.Drawing.Drawing2D.DashCap.Flat;

            return pen;
        }

        private static Rect2 CreateRect(PointShape tl, PointShape br, double dx, double dy) => Rect2.FromPoints(tl.X, tl.Y, br.X, br.Y, dx, dy);

        private static void DrawLineInternal(Graphics gfx, Pen pen, bool isStroked, ref PointF p0, ref PointF p1)
        {
            if (isStroked)
            {
                gfx.DrawLine(pen, p0, p1);
            }
        }

        private void DrawLineArrowsInternal(LineShape line, Graphics gfx, out PointF pt1, out PointF pt2)
        {
            var fillStartArrow = ToBrush(line.Style.Fill);
            var strokeStartArrow = ToPen(line.Style, _scaleToPage);

            var fillEndArrow = ToBrush(line.Style.Fill);
            var strokeEndArrow = ToPen(line.Style, _scaleToPage);

            double _x1 = line.Start.X;
            double _y1 = line.Start.Y;
            double _x2 = line.End.X;
            double _y2 = line.End.Y;

            float x1 = _scaleToPage(_x1);
            float y1 = _scaleToPage(_y1);
            float x2 = _scaleToPage(_x2);
            float y2 = _scaleToPage(_y2);

            var sas = line.Style.StartArrowStyle;
            var eas = line.Style.EndArrowStyle;
            float a1 = (float)(Math.Atan2(y1 - y2, x1 - x2) * 180.0 / Math.PI);
            float a2 = (float)(Math.Atan2(y2 - y1, x2 - x1) * 180.0 / Math.PI);

            // Draw start arrow.
            pt1 = DrawLineArrowInternal(gfx, strokeStartArrow, fillStartArrow, x1, y1, a1, sas, line);

            // Draw end arrow.
            pt2 = DrawLineArrowInternal(gfx, strokeEndArrow, fillEndArrow, x2, y2, a2, eas, line);

            fillStartArrow.Dispose();
            strokeStartArrow.Dispose();

            fillEndArrow.Dispose();
            strokeEndArrow.Dispose();
        }

        private static PointF DrawLineArrowInternal(Graphics gfx, Pen pen, Brush brush, float x, float y, float angle, ArrowStyle style, BaseShape shape)
        {
            PointF pt;
            var rt = new Matrix();
            rt.RotateAt(angle, new PointF(x, y));
            double rx = style.RadiusX;
            double ry = style.RadiusY;
            double sx = 2.0 * rx;
            double sy = 2.0 * ry;

            switch (style.ArrowType)
            {
                default:
                case ArrowType.None:
                    {
                        pt = new PointF(x, y);
                    }
                    break;

                case ArrowType.Rectangle:
                    {
                        var pts = new PointF[] { new PointF(x - (float)sx, y) };
                        rt.TransformPoints(pts);
                        pt = pts[0];
                        var rect = new Rect2(x - sx, y - ry, sx, sy);
                        var gs = gfx.Save();
                        gfx.MultiplyTransform(rt);
                        DrawRectangleInternal(gfx, brush, pen, shape.IsStroked, shape.IsFilled, ref rect);
                        gfx.Restore(gs);
                    }
                    break;

                case ArrowType.Ellipse:
                    {
                        var pts = new PointF[] { new PointF(x - (float)sx, y) };
                        rt.TransformPoints(pts);
                        pt = pts[0];
                        var gs = gfx.Save();
                        gfx.MultiplyTransform(rt);
                        var rect = new Rect2(x - sx, y - ry, sx, sy);
                        DrawEllipseInternal(gfx, brush, pen, shape.IsStroked, shape.IsFilled, ref rect);
                        gfx.Restore(gs);
                    }
                    break;

                case ArrowType.Arrow:
                    {
                        var pts = new PointF[]
                        {
                            new PointF(x, y),
                            new PointF(x - (float)sx, y + (float)sy),
                            new PointF(x, y),
                            new PointF(x - (float)sx, y - (float)sy),
                            new PointF(x, y)
                        };
                        rt.TransformPoints(pts);
                        pt = pts[0];
                        var p11 = pts[1];
                        var p21 = pts[2];
                        var p12 = pts[3];
                        var p22 = pts[4];
                        DrawLineInternal(gfx, pen, shape.IsStroked, ref p11, ref p21);
                        DrawLineInternal(gfx, pen, shape.IsStroked, ref p12, ref p22);
                    }
                    break;
            }

            return pt;
        }

        private static void DrawRectangleInternal(Graphics gfx, Brush brush, Pen pen, bool isStroked, bool isFilled, ref Rect2 rect)
        {
            if (isFilled)
            {
                gfx.FillRectangle(
                    brush,
                    (float)rect.X,
                    (float)rect.Y,
                    (float)rect.Width,
                    (float)rect.Height);
            }

            if (isStroked)
            {
                gfx.DrawRectangle(
                    pen,
                    (float)rect.X,
                    (float)rect.Y,
                    (float)rect.Width,
                    (float)rect.Height);
            }
        }

        private static void DrawEllipseInternal(Graphics gfx, Brush brush, Pen pen, bool isStroked, bool isFilled, ref Rect2 rect)
        {
            if (isFilled)
            {
                gfx.FillEllipse(
                    brush,
                    (float)rect.X,
                    (float)rect.Y,
                    (float)rect.Width,
                    (float)rect.Height);
            }

            if (isStroked)
            {
                gfx.DrawEllipse(
                    pen,
                    (float)rect.X,
                    (float)rect.Y,
                    (float)rect.Width,
                    (float)rect.Height);
            }
        }

        private void DrawGridInternal(Graphics gfx, IGrid grid, Pen stroke, ref Rect2 rect)
        {
            double ox = rect.X;
            double ex = rect.X + rect.Width;
            double oy = rect.Y;
            double ey = rect.Y + rect.Height;
            double cw = grid.GridCellWidth;
            double ch = grid.GridCellHeight;

            for (double x = ox + ch; x < ex; x += cw)
            {
                var p0 = new PointF(
                    _scaleToPage(x),
                    _scaleToPage(oy));
                var p1 = new PointF(
                    _scaleToPage(x),
                    _scaleToPage(ey));
                DrawLineInternal(gfx, stroke, true, ref p0, ref p1);
            }

            for (double y = oy + ch; y < ey; y += ch)
            {
                var p0 = new PointF(
                    _scaleToPage(ox),
                    _scaleToPage(y));
                var p1 = new PointF(
                    _scaleToPage(ex),
                    _scaleToPage(y));
                DrawLineInternal(gfx, stroke, true, ref p0, ref p1);
            }
        }

        public void ClearCache()
        {
            _biCache.Reset();
        }

        public void Fill(object dc, double x, double y, double width, double height, BaseColor color)
        {
            var _gfx = dc as Graphics;
            var brush = ToBrush(color);
            _gfx.FillRectangle(
                brush,
                (float)x,
                (float)y,
                (float)width,
                (float)height);
            brush.Dispose();
        }

        public void Grid(object dc, IGrid grid, double x, double y, double width, double height)
        {
            var _gfx = dc as Graphics;

            var pen = ToPen(grid.GridStrokeColor, grid.GridStrokeThickness, _scaleToPage);

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
                    pen,
                    ref rect);
            }

            if (grid.IsBorderEnabled)
            {
                _gfx.DrawRectangle(
                    pen,
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }

            pen.Dispose();
        }

        public void DrawPage(object dc, PageContainer container)
        {
            foreach (var layer in container.Layers)
            {
                if (layer.IsVisible)
                {
                    DrawLayer(dc, layer);
                }
            }
        }

        public void DrawLayer(object dc, LayerContainer layer)
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

        public void DrawPoint(object dc, PointShape point)
        {
            // TODO:
        }

        public void DrawLine(object dc, LineShape line)
        {
            var _gfx = dc as Graphics;

            var strokeLine = ToPen(line.Style, _scaleToPage);
            DrawLineArrowsInternal(line, _gfx, out var pt1, out var pt2);
            DrawLineInternal(_gfx, strokeLine, line.IsStroked, ref pt1, ref pt2);
            strokeLine.Dispose();
        }

        public void DrawRectangle(object dc, RectangleShape rectangle)
        {
            var _gfx = dc as Graphics;

            var brush = ToBrush(rectangle.Style.Fill);
            var pen = ToPen(rectangle.Style, _scaleToPage);

            var rect = CreateRect(
                rectangle.TopLeft,
                rectangle.BottomRight,
                0, 0);

            if (rectangle.IsFilled)
            {
                _gfx.FillRectangle(
                    brush,
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }

            if (rectangle.IsStroked)
            {
                _gfx.DrawRectangle(
                    pen,
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }

            brush.Dispose();
            pen.Dispose();
        }

        public void DrawEllipse(object dc, EllipseShape ellipse)
        {
            var _gfx = dc as Graphics;

            var brush = ToBrush(ellipse.Style.Fill);
            var pen = ToPen(ellipse.Style, _scaleToPage);

            var rect = CreateRect(
                ellipse.TopLeft,
                ellipse.BottomRight,
                0, 0);

            if (ellipse.IsFilled)
            {
                _gfx.FillEllipse(
                    brush,
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }

            if (ellipse.IsStroked)
            {
                _gfx.DrawEllipse(
                    pen,
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }

            brush.Dispose();
            pen.Dispose();
        }

        public void DrawArc(object dc, ArcShape arc)
        {
            var a = new GdiArc(
                Point2.FromXY(arc.Point1.X, arc.Point1.Y),
                Point2.FromXY(arc.Point2.X, arc.Point2.Y),
                Point2.FromXY(arc.Point3.X, arc.Point3.Y),
                Point2.FromXY(arc.Point4.X, arc.Point4.Y));

            if (a.Width > 0.0 && a.Height > 0.0)
            {
                var _gfx = dc as Graphics;

                var brush = ToBrush(arc.Style.Fill);
                var pen = ToPen(arc.Style, _scaleToPage);

                if (arc.IsFilled)
                {
                    var path = new GraphicsPath();
                    path.AddArc(
                        _scaleToPage(a.X),
                        _scaleToPage(a.Y),
                        _scaleToPage(a.Width),
                        _scaleToPage(a.Height),
                        (float)a.StartAngle,
                        (float)a.SweepAngle);
                    _gfx.FillPath(brush, path);
                }

                if (arc.IsStroked)
                {
                    _gfx.DrawArc(
                        pen,
                        _scaleToPage(a.X),
                        _scaleToPage(a.Y),
                        _scaleToPage(a.Width),
                        _scaleToPage(a.Height),
                        (float)a.StartAngle,
                        (float)a.SweepAngle);
                }

                brush.Dispose();
                pen.Dispose();
            }
        }

        public void DrawCubicBezier(object dc, CubicBezierShape cubicBezier)
        {
            var _gfx = dc as Graphics;

            var brush = ToBrush(cubicBezier.Style.Fill);
            var pen = ToPen(cubicBezier.Style, _scaleToPage);

            if (cubicBezier.IsFilled)
            {
                var path = new GraphicsPath();
                path.AddBezier(
                    _scaleToPage(cubicBezier.Point1.X),
                    _scaleToPage(cubicBezier.Point1.Y),
                    _scaleToPage(cubicBezier.Point2.X),
                    _scaleToPage(cubicBezier.Point2.Y),
                    _scaleToPage(cubicBezier.Point3.X),
                    _scaleToPage(cubicBezier.Point3.Y),
                    _scaleToPage(cubicBezier.Point4.X),
                    _scaleToPage(cubicBezier.Point4.Y));
                _gfx.FillPath(brush, path);
            }

            if (cubicBezier.IsStroked)
            {
                _gfx.DrawBezier(
                    pen,
                    _scaleToPage(cubicBezier.Point1.X),
                    _scaleToPage(cubicBezier.Point1.Y),
                    _scaleToPage(cubicBezier.Point2.X),
                    _scaleToPage(cubicBezier.Point2.Y),
                    _scaleToPage(cubicBezier.Point3.X),
                    _scaleToPage(cubicBezier.Point3.Y),
                    _scaleToPage(cubicBezier.Point4.X),
                    _scaleToPage(cubicBezier.Point4.Y));
            }

            brush.Dispose();
            pen.Dispose();
        }

        public void DrawQuadraticBezier(object dc, QuadraticBezierShape quadraticBezier)
        {
            var _gfx = dc as Graphics;

            var brush = ToBrush(quadraticBezier.Style.Fill);
            var pen = ToPen(quadraticBezier.Style, _scaleToPage);

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
                var path = new GraphicsPath();
                path.AddBezier(
                    _scaleToPage(x1),
                    _scaleToPage(y1),
                    _scaleToPage(x2),
                    _scaleToPage(y2),
                    _scaleToPage(x3),
                    _scaleToPage(y3),
                    _scaleToPage(x4),
                    _scaleToPage(y4));
                _gfx.FillPath(brush, path);
            }

            if (quadraticBezier.IsStroked)
            {
                _gfx.DrawBezier(
                    pen,
                    _scaleToPage(x1),
                    _scaleToPage(y1),
                    _scaleToPage(x2),
                    _scaleToPage(y2),
                    _scaleToPage(x3),
                    _scaleToPage(y3),
                    _scaleToPage(x4),
                    _scaleToPage(y4));
            }

            brush.Dispose();
            pen.Dispose();
        }

        public void DrawText(object dc, TextShape text)
        {
            var _gfx = dc as Graphics;

            if (!(text.GetProperty(nameof(TextShape.Text)) is string tbind))
            {
                tbind = text.Text;
            }

            if (tbind == null)
            {
                return;
            }

            var brush = ToBrush(text.Style.Stroke);

            var fontStyle = System.Drawing.FontStyle.Regular;
            if (text.Style.TextStyle.FontStyle != null)
            {
                if (text.Style.TextStyle.FontStyle.Flags.HasFlag(Core2D.Style.FontStyleFlags.Bold))
                {
                    fontStyle |= System.Drawing.FontStyle.Bold;
                }

                if (text.Style.TextStyle.FontStyle.Flags.HasFlag(Core2D.Style.FontStyleFlags.Italic))
                {
                    fontStyle |= System.Drawing.FontStyle.Italic;
                }
            }

            Font font = new Font(
                text.Style.TextStyle.FontName,
                (float)(text.Style.TextStyle.FontSize * _textScaleFactor),
                fontStyle);

            var rect = CreateRect(
                text.TopLeft,
                text.BottomRight,
                0, 0);

            var srect = new RectangleF(
                _scaleToPage(rect.X),
                _scaleToPage(rect.Y),
                _scaleToPage(rect.Width),
                _scaleToPage(rect.Height));

            var format = new StringFormat();
            switch (text.Style.TextStyle.TextHAlignment)
            {
                case TextHAlignment.Left:
                    format.Alignment = StringAlignment.Near;
                    break;

                case TextHAlignment.Center:
                    format.Alignment = StringAlignment.Center;
                    break;

                case TextHAlignment.Right:
                    format.Alignment = StringAlignment.Far;
                    break;
            }

            switch (text.Style.TextStyle.TextVAlignment)
            {
                case TextVAlignment.Top:
                    format.LineAlignment = StringAlignment.Near;
                    break;

                case TextVAlignment.Center:
                    format.LineAlignment = StringAlignment.Center;
                    break;

                case TextVAlignment.Bottom:
                    format.LineAlignment = StringAlignment.Far;
                    break;
            }

            format.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.NoClip;
            format.Trimming = StringTrimming.None;

            _gfx.DrawString(
                tbind,
                font,
                ToBrush(text.Style.Stroke),
                srect,
                format);

            brush.Dispose();
            font.Dispose();
        }

        public void DrawImage(object dc, ImageShape image)
        {
            var _gfx = dc as Graphics;

            var brush = ToBrush(image.Style.Stroke);

            var rect = CreateRect(
                image.TopLeft,
                image.BottomRight,
                0, 0);

            var srect = new RectangleF(
                _scaleToPage(rect.X),
                _scaleToPage(rect.Y),
                _scaleToPage(rect.Width),
                _scaleToPage(rect.Height));

            if (image.IsFilled)
            {
                _gfx.FillRectangle(
                    ToBrush(image.Style.Fill),
                    srect);
            }

            if (image.IsStroked)
            {
                _gfx.DrawRectangle(
                    ToPen(image.Style, _scaleToPage),
                    srect.X,
                    srect.Y,
                    srect.Width,
                    srect.Height);
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
                        var bi = Image.FromStream(ms);
                        ms.Dispose();

                        _biCache.Set(image.Key, bi);

                        _gfx.DrawImage(bi, srect);
                    }
                }
            }

            brush.Dispose();
        }

        public void DrawPath(object dc, PathShape path)
        {
            var _gfx = dc as Graphics;

            var gp = path.Geometry.ToGraphicsPath(_scaleToPage);

            if (path.IsFilled && path.IsStroked)
            {
                var brush = ToBrush(path.Style.Fill);
                var pen = ToPen(path.Style, _scaleToPage);
                _gfx.FillPath(
                    brush,
                    gp);
                _gfx.DrawPath(
                    pen,
                    gp);
                brush.Dispose();
                pen.Dispose();
            }
            else if (path.IsFilled && !path.IsStroked)
            {
                var brush = ToBrush(path.Style.Fill);
                _gfx.FillPath(
                    brush,
                    gp);
                brush.Dispose();
            }
            else if (!path.IsFilled && path.IsStroked)
            {
                var pen = ToPen(path.Style, _scaleToPage);
                _gfx.DrawPath(
                    pen,
                    gp);
                pen.Dispose();
            }
        }
    }
}
