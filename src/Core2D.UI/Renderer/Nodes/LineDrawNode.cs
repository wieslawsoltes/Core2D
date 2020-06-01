using System;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;
using Spatial;
using A = Avalonia;
using AM = Avalonia.Media;
using AME = Avalonia.MatrixExtensions;

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

        private void DrawLineInternal(AM.DrawingContext context, AM.IPen pen, bool isStroked, ref A.Point p0, ref A.Point p1)
        {
            if (isStroked)
            {
                context.DrawLine(pen, p0, p1);
            }
        }

        private void DrawRectangleInternal(AM.DrawingContext context, AM.IBrush brush, AM.IPen pen, bool isStroked, bool isFilled, ref A.Rect rect)
        {
            if (isFilled)
            {
                context.FillRectangle(brush, rect);
            }

            if (isStroked)
            {
                context.DrawRectangle(pen, rect);
            }
        }

        private void DrawEllipseInternal(AM.DrawingContext context, AM.IBrush brush, AM.IPen pen, bool isStroked, bool isFilled, AM.EllipseGeometry ellipseGeometry)
        {
            context.DrawGeometry(isFilled ? brush : null, isStroked ? pen : null, ellipseGeometry);
        }

        private void DrawLineCurveInternal(AM.DrawingContext context, AM.IPen pen, bool isStroked, AM.StreamGeometry curveGeometry)
        {
            context.DrawGeometry(null, isStroked ? pen : null, curveGeometry);
        }

        private AM.StreamGeometry CreateLineCurveGeometry(ref A.Point p0, ref A.Point p1, double curvature, CurveOrientation orientation, PointAlignment pt0a, PointAlignment pt1a)
        {
            var curveGeometry = new AM.StreamGeometry();

            using (var geometryContext = curveGeometry.Open())
            {
                geometryContext.BeginFigure(new A.Point(p0.X, p0.Y), false);
                double p0x = p0.X;
                double p0y = p0.Y;
                double p1x = p1.X;
                double p1y = p1.Y;
                LineShapeExtensions.GetCurvedLineBezierControlPoints(orientation, curvature, pt0a, pt1a, ref p0x, ref p0y, ref p1x, ref p1y);
                var point1 = new A.Point(p0x, p0y);
                var point2 = new A.Point(p1x, p1y);
                var point3 = new A.Point(p1.X, p1.Y);
                geometryContext.CubicBezierTo(point1, point2, point3);
                geometryContext.EndFigure(false);
            }

            return curveGeometry;
        }

        private A.Point DrawLineArrowInternal(AM.DrawingContext dc, AM.IPen pen, AM.IBrush brush, double x, double y, double angle, IArrowStyle style)
        {
            var rotation = AME.MatrixHelper.Rotation(angle, new A.Vector(x, y));
            double rx = style.RadiusX;
            double ry = style.RadiusY;
            double sx = 2.0 * rx;
            double sy = 2.0 * ry;
            A.Point point;

            switch (style.ArrowType)
            {
                default:
                case ArrowType.None:
                    {
                        point = new A.Point(x, y);
                    }
                    break;
                case ArrowType.Rectangle:
                    {
                        point = AME.MatrixHelper.TransformPoint(rotation, new A.Point(x - (float)sx, y));

                        var rect2 = new Rect2(x - sx, y - ry, sx, sy);
                        var rect = new A.Rect(rect2.X, rect2.Y, rect2.Width, rect2.Height);

                        using var rotationDisposable = dc.PushPreTransform(rotation);
                        DrawRectangleInternal(dc, brush, pen, style.IsStroked, style.IsFilled, ref rect);
                    }
                    break;
                case ArrowType.Ellipse:
                    {
                        point = AME.MatrixHelper.TransformPoint(rotation, new A.Point(x - (float)sx, y));

                        var rect2 = new Rect2(x - sx, y - ry, sx, sy);
                        var rect = new A.Rect(rect2.X, rect2.Y, rect2.Width, rect2.Height);
                        var ellipseGeometry = new AM.EllipseGeometry(rect);

                        using var rotationDisposable = dc.PushPreTransform(rotation);
                        DrawEllipseInternal(dc, brush, pen, style.IsStroked, style.IsFilled, ellipseGeometry);
                    }
                    break;
                case ArrowType.Arrow:
                    {
                        point = AME.MatrixHelper.TransformPoint(rotation, new A.Point(x, y));

                        var p11 = AME.MatrixHelper.TransformPoint(rotation, new A.Point(x - (float)sx, y + (float)sy));
                        var p21 = AME.MatrixHelper.TransformPoint(rotation, new A.Point(x, y));
                        var p12 = AME.MatrixHelper.TransformPoint(rotation, new A.Point(x - (float)sx, y - (float)sy));
                        var p22 = AME.MatrixHelper.TransformPoint(rotation, new A.Point(x, y));

                        DrawLineInternal(dc, pen, style.IsStroked, ref p11, ref p21);
                        DrawLineInternal(dc, pen, style.IsStroked, ref p12, ref p22);
                    }
                    break;
            }

            return point;
        }

        private void DrawLineInternal(AM.DrawingContext dc, ILineShape line, IShapeStyle style, out A.Point p0, out A.Point p1)
        {
            double x1 = line.Start.X;
            double y1 = line.Start.Y;
            double x2 = line.End.X;
            double y2 = line.End.Y;
            line.GetMaxLength(ref x1, ref y1, ref x2, ref y2);

            var fillStartArrow = ToBrush(style.StartArrowStyle.Fill);
            var strokeStartArrow = ToPen(style.StartArrowStyle, style.StartArrowStyle.Thickness);
            double a1 = Math.Atan2(y1 - y2, x1 - x2);
            p0 = DrawLineArrowInternal(dc, strokeStartArrow, fillStartArrow, x1, y1, a1, style.StartArrowStyle);

            var fillEndArrow = ToBrush(style.EndArrowStyle.Fill);
            var strokeEndArrow = ToPen(style.EndArrowStyle, style.EndArrowStyle.Thickness);
            double a2 = Math.Atan2(y2 - y1, x2 - x1);
            p1 = DrawLineArrowInternal(dc, strokeEndArrow, fillEndArrow, x2, y2, a2, style.EndArrowStyle);
        }

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
                if (Style.StartArrowStyle.ArrowType != ArrowType.None || Style.EndArrowStyle.ArrowType != ArrowType.None || Style.LineStyle.IsCurved)
                {
                    // TODO: Cache Arrows
                    DrawLineInternal(context, Line, Style, out var p0, out var p1);

                    if (Style.LineStyle.IsCurved)
                    {
                        var curveGeometry = CreateLineCurveGeometry(ref p0, ref p1, Style.LineStyle.Curvature, Style.LineStyle.CurveOrientation, Line.Start.Alignment, Line.End.Alignment);

                        // TODO: Cache Line Curve
                        DrawLineCurveInternal(context, Stroke, Line.IsStroked, curveGeometry);
                    }
                    else
                    {
                        DrawLineInternal(context, Stroke, Line.IsStroked, ref p0, ref p1);
                    }
                }
                else
                {
                    context.DrawLine(Stroke, P0, P1);
                }
            }
        }
    }
}
