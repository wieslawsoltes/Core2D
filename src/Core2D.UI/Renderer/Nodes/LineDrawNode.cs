using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;
using A = Avalonia;
using AM = Avalonia.Media;

namespace Core2D.UI.Renderer
{
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
}
