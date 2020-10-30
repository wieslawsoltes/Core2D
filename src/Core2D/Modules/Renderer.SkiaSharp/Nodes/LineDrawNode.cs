using System;
using Core2D.Shapes;
using Core2D.Style;
using Spatial;
using SkiaSharp;

namespace Core2D.Renderer.SkiaSharp
{
    internal abstract class Marker : IMarker
    {
        public ArrowStyle Style { get; set; }
        public SKPaint Brush { get; set; }
        public SKPaint Pen { get; set; }
        public SKMatrix Rotation { get; set; }
        public SKPoint Point { get; set; }

        public abstract void Draw(object dc);

        public virtual void UpdateStyle()
        {
            Brush = SkiaSharpDrawUtil.ToSKPaintBrush(Style.Fill);
            Pen = SkiaSharpDrawUtil.ToSKPaintPen(Style, Style.Thickness);
        }
    }

    internal class NoneMarker : Marker
    {
        public override void Draw(object dc)
        {
        }
    }

    internal class RectangleMarker : Marker
    {
        public SKRect Rect { get; set; }

        public override void Draw(object dc)
        {
            var canvas = dc as SKCanvas;

            var count = canvas.Save();
            canvas.SetMatrix(MatrixHelper.Multiply(Rotation, canvas.TotalMatrix));

            if (Style.IsFilled)
            {
                canvas.DrawRect(Rect, Brush);
            }

            if (Style.IsStroked)
            {
                canvas.DrawRect(Rect, Pen);
            }

            canvas.RestoreToCount(count);
        }
    }

    internal class EllipseMarker : Marker
    {
        public SKRect Rect { get; set; }

        public override void Draw(object dc)
        {
            var canvas = dc as SKCanvas;

            var count = canvas.Save();
            canvas.SetMatrix(MatrixHelper.Multiply(Rotation, canvas.TotalMatrix));

            if (Style.IsFilled)
            {
                canvas.DrawOval(Rect, Brush);
            }

            if (Style.IsStroked)
            {
                canvas.DrawOval(Rect, Pen);
            }

            canvas.RestoreToCount(count);
        }
    }

    internal class ArrowMarker : Marker
    {
        public SKPoint P11;
        public SKPoint P21;
        public SKPoint P12;
        public SKPoint P22;

        public override void Draw(object dc)
        {
            var canvas = dc as SKCanvas;

            if (Style.IsStroked)
            {
                canvas.DrawLine(P11, P21, Pen);
                canvas.DrawLine(P12, P22, Pen);
            }
        }
    }

    internal class LineDrawNode : DrawNode, ILineDrawNode
    {
        public LineShape Line { get; set; }
        public SKPoint P0 { get; set; }
        public SKPoint P1 { get; set; }
        public IMarker StartMarker { get; set; }
        public IMarker EndMarker { get; set; }

        public LineDrawNode(LineShape line, ShapeStyle style)
        {
            Style = style;
            Line = line;
            UpdateGeometry();
        }

        private Marker CreatArrowMarker(double x, double y, double angle, ArrowStyle style)
        {
            switch (style.ArrowType)
            {
                default:
                case ArrowType.None:
                    {
                        var marker = new NoneMarker();

                        marker.Style = style;
                        marker.Point = new SKPoint((float)x, (float)y);

                        return marker;
                    }
                case ArrowType.Rectangle:
                    {
                        double rx = style.RadiusX;
                        double ry = style.RadiusY;
                        double sx = 2.0 * rx;
                        double sy = 2.0 * ry;

                        var marker = new RectangleMarker();

                        marker.Style = style;
                        marker.Rotation = MatrixHelper.Rotation(angle, new SKPoint((float)x, (float)y));
                        marker.Point = MatrixHelper.TransformPoint(marker.Rotation, new SKPoint((float)(x - sx), (float)y));

                        var rect2 = new Rect2(x - sx, y - ry, sx, sy);
                        marker.Rect = SKRect.Create((float)rect2.X, (float)rect2.Y, (float)rect2.Width, (float)rect2.Height);

                        return marker;
                    }
                case ArrowType.Ellipse:
                    {
                        double rx = style.RadiusX;
                        double ry = style.RadiusY;
                        double sx = 2.0 * rx;
                        double sy = 2.0 * ry;

                        var marker = new EllipseMarker();

                        marker.Style = style;
                        marker.Rotation = MatrixHelper.Rotation(angle, new SKPoint((float)x, (float)y));
                        marker.Point = MatrixHelper.TransformPoint(marker.Rotation, new SKPoint((float)(x - sx), (float)y));

                        var rect2 = new Rect2(x - sx, y - ry, sx, sy);
                        marker.Rect = SKRect.Create((float)rect2.X, (float)rect2.Y, (float)rect2.Width, (float)rect2.Height);

                        return marker;
                    }
                case ArrowType.Arrow:
                    {
                        double rx = style.RadiusX;
                        double ry = style.RadiusY;
                        double sx = 2.0 * rx;
                        double sy = 2.0 * ry;

                        var marker = new ArrowMarker();

                        marker.Style = style;
                        marker.Rotation = MatrixHelper.Rotation(angle, new SKPoint((float)x, (float)y));
                        marker.Point = MatrixHelper.TransformPoint(marker.Rotation, new SKPoint((float)x, (float)y));

                        marker.P11 = MatrixHelper.TransformPoint(marker.Rotation, new SKPoint((float)(x - sx), (float)(y + sy)));
                        marker.P21 = MatrixHelper.TransformPoint(marker.Rotation, new SKPoint((float)x, (float)y));
                        marker.P12 = MatrixHelper.TransformPoint(marker.Rotation, new SKPoint((float)(x - sx), (float)(y - sy)));
                        marker.P22 = MatrixHelper.TransformPoint(marker.Rotation, new SKPoint((float)x, (float)y));

                        return marker;
                    }
            }
        }

        private void UpdateMarkers()
        {
            double x1 = Line.Start.X;
            double y1 = Line.Start.Y;
            double x2 = Line.End.X;
            double y2 = Line.End.Y;

            if (Style.StartArrowStyle.ArrowType != ArrowType.None)
            {
                double a1 = Math.Atan2(y1 - y2, x1 - x2);
                StartMarker = CreatArrowMarker(x1, y1, a1, Style.StartArrowStyle);
                StartMarker.UpdateStyle();
                P0 = (StartMarker as Marker).Point;
            }
            else
            {
                StartMarker = null;
                P0 = new SKPoint((float)x1, (float)y1);
            }

            if (Style.EndArrowStyle.ArrowType != ArrowType.None)
            {
                double a2 = Math.Atan2(y2 - y1, x2 - x1);
                EndMarker = CreatArrowMarker(x2, y2, a2, Style.EndArrowStyle);
                EndMarker.UpdateStyle();
                P1 = (EndMarker as Marker).Point;
            }
            else
            {
                EndMarker = null;
                P1 = new SKPoint((float)x2, (float)y2);
            }
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = Line.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Line.State.Flags.HasFlag(ShapeStateFlags.Size);
            P0 = new SKPoint((float)Line.Start.X, (float)Line.Start.Y);
            P1 = new SKPoint((float)Line.End.X, (float)Line.End.Y);
            Center = new SKPoint((float)((P0.X + P1.X) / 2.0), (float)((P0.Y + P1.Y) / 2.0));
            UpdateMarkers();
        }

        public override void UpdateStyle()
        {
            base.UpdateStyle();

            if (Style.StartArrowStyle.ArrowType != ArrowType.None)
            {
                StartMarker?.UpdateStyle();
            }

            if (Style.EndArrowStyle.ArrowType != ArrowType.None)
            {
                EndMarker?.UpdateStyle();
            }
        }

        public override void OnDraw(object dc, double zoom)
        {
            var canvas = dc as SKCanvas;

            if (Line.IsStroked)
            {
                canvas.DrawLine(P0, P1, Stroke);
 
                if (Style.StartArrowStyle.ArrowType != ArrowType.None)
                {
                    StartMarker?.Draw(dc);
                }

                if (Style.EndArrowStyle.ArrowType != ArrowType.None)
                {
                    EndMarker?.Draw(dc);
                }
            }
        }
    }
}
