// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.Model.Style;
using Core2D.Modules.Renderer.SkiaSharp.Nodes.Markers;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using SkiaSharp;
using Core2D.Spatial;

namespace Core2D.Modules.Renderer.SkiaSharp.Nodes;

internal class LineDrawNode : DrawNode, ILineDrawNode
{
    public LineShapeViewModel Line { get; set; }
    public SKPoint P0 { get; set; }
    public SKPoint P1 { get; set; }
    public IMarker? StartMarker { get; set; }
    public IMarker? EndMarker { get; set; }

    public LineDrawNode(LineShapeViewModel line, ShapeStyleViewModel? style)
    {
        Style = style;
        Line = line;
        UpdateGeometry();
    }

    private MarkerBase CreateArrowMarker(double x, double y, double angle, ShapeStyleViewModel shapeStyleViewModel, ArrowStyleViewModel style)
    {
        switch (style.ArrowType)
        {
            default:
            {
                var marker = new NoneMarker();

                marker.ShapeViewModel = Line;
                marker.ShapeStyleViewModel = shapeStyleViewModel;
                marker.Style = style;
                marker.Point = new SKPoint((float)x, (float)y);

                return marker;
            }
            case ArrowType.Rectangle:
            {
                var rx = style.RadiusX;
                var ry = style.RadiusY;
                var sx = 2.0 * rx;
                var sy = 2.0 * ry;

                var marker = new RectangleMarker();

                marker.ShapeViewModel = Line;
                marker.ShapeStyleViewModel = shapeStyleViewModel;
                marker.Style = style;
                marker.Rotation = MatrixHelper.Rotation(angle, new SKPoint((float)x, (float)y));
                marker.Point = MatrixHelper.TransformPoint(marker.Rotation, new SKPoint((float)(x - sx), (float)y));

                var rect2 = new Rect2(x - sx, y - ry, sx, sy);
                marker.Rect = SKRect.Create((float)rect2.X, (float)rect2.Y, (float)rect2.Width, (float)rect2.Height);

                return marker;
            }
            case ArrowType.Ellipse:
            {
                var rx = style.RadiusX;
                var ry = style.RadiusY;
                var sx = 2.0 * rx;
                var sy = 2.0 * ry;

                var marker = new EllipseMarker();

                marker.ShapeViewModel = Line;
                marker.ShapeStyleViewModel = shapeStyleViewModel;
                marker.Style = style;
                marker.Rotation = MatrixHelper.Rotation(angle, new SKPoint((float)x, (float)y));
                marker.Point = MatrixHelper.TransformPoint(marker.Rotation, new SKPoint((float)(x - sx), (float)y));

                var rect2 = new Rect2(x - sx, y - ry, sx, sy);
                marker.Rect = SKRect.Create((float)rect2.X, (float)rect2.Y, (float)rect2.Width, (float)rect2.Height);

                return marker;
            }
            case ArrowType.Arrow:
            {
                var rx = style.RadiusX;
                var ry = style.RadiusY;
                var sx = 2.0 * rx;
                var sy = 2.0 * ry;

                var marker = new ArrowMarker();

                marker.ShapeViewModel = Line;
                marker.ShapeStyleViewModel = shapeStyleViewModel;
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
        if (Line.Start is null || Line.End is null)
        {
            StartMarker = null;
            P0 = new SKPoint();

            EndMarker = null;
            P1 = new SKPoint();
        }
        else
        {
            var x1 = Line.Start.X;
            var y1 = Line.Start.Y;
            var x2 = Line.End.X;
            var y2 = Line.End.Y;

            if (Style?.Stroke?.StartArrow is not null && Style.Stroke.StartArrow.ArrowType != ArrowType.None)
            {
                var a1 = Math.Atan2(y1 - y2, x1 - x2);
                var marker = CreateArrowMarker(x1, y1, a1, Style, Style.Stroke.StartArrow);
                StartMarker = marker;
                marker.UpdateStyle();
                P0 = marker.Point;
            }
            else
            {
                StartMarker = null;
                P0 = new SKPoint((float)x1, (float)y1);
            }

            if (Style?.Stroke?.EndArrow is not null && Style.Stroke.EndArrow.ArrowType != ArrowType.None)
            {
                var a2 = Math.Atan2(y2 - y1, x2 - x1);
                var marker = CreateArrowMarker(x2, y2, a2, Style, Style.Stroke.EndArrow);
                marker.UpdateStyle();
                EndMarker = marker;
                P1 = marker.Point;
            }
            else
            {
                EndMarker = null;
                P1 = new SKPoint((float)x2, (float)y2);
            }
        }
    }

    public sealed override void UpdateGeometry()
    {
        ScaleThickness = Line.State.HasFlag(ShapeStateFlags.Thickness);
        ScaleSize = Line.State.HasFlag(ShapeStateFlags.Size);
        if (Line.Start is null || Line.End is null)
        {
            P0 = new SKPoint();
            P1 = new SKPoint();
        }
        else
        {
            P0 = new SKPoint((float)Line.Start.X, (float)Line.Start.Y);
            P1 = new SKPoint((float)Line.End.X, (float)Line.End.Y);
        }

        Center = new SKPoint((float)((P0.X + P1.X) / 2.0), (float)((P0.Y + P1.Y) / 2.0));
        UpdateMarkers();
    }

    public override void UpdateStyle()
    {
        base.UpdateStyle();

        if (Style?.Stroke?.StartArrow is not null && Style.Stroke.StartArrow.ArrowType != ArrowType.None)
        {
            StartMarker?.UpdateStyle();
        }

        if (Style?.Stroke?.EndArrow is not null && Style.Stroke.EndArrow.ArrowType != ArrowType.None)
        {
            EndMarker?.UpdateStyle();
        }
    }

    public override void OnDraw(object? dc, double zoom)
    {
        if (dc is not SKCanvas canvas)
        {
            return;
        }

        if (Line.Start is null || Line.End is null)
        {
            return;
        }

        if (Line.IsStroked)
        {
            canvas.DrawLine(P0, P1, Stroke);

            if (Style?.Stroke?.StartArrow is not null && Style.Stroke.StartArrow.ArrowType != ArrowType.None)
            {
                StartMarker?.Draw(dc);
            }

            if (Style?.Stroke?.EndArrow is not null && Style.Stroke.EndArrow.ArrowType != ArrowType.None)
            {
                EndMarker?.Draw(dc);
            }
        }
    }
}
