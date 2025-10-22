// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.Model.Style;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using SkiaSharp;

namespace Core2D.Modules.Renderer.SkiaSharp.Nodes;

internal class WireDrawNode : LineDrawNode, IWireDrawNode
{
    public WireShapeViewModel Wire { get; set; }
    private SKPath? _bezierPath;
    private WireGeometry _wireGeometry;
    private SKPoint _startPoint;
    private SKPoint _endPoint;

    public WireDrawNode(WireShapeViewModel wire, ShapeStyleViewModel? style)
        : base(wire, style)
    {
        Wire = wire;
        UpdateGeometry();
    }

    public override void UpdateGeometry()
    {
        if (!WireGeometryHelper.TryCreateGeometry(Wire, out var geometry) || !geometry.IsBezier)
        {
            _bezierPath = null;
            base.UpdateGeometry();
            return;
        }

        _wireGeometry = geometry;
        ScaleThickness = Wire.State.HasFlag(ShapeStateFlags.Thickness);
        ScaleSize = Wire.State.HasFlag(ShapeStateFlags.Size);

        _startPoint = new SKPoint((float)geometry.Start.X, (float)geometry.Start.Y);
        _endPoint = new SKPoint((float)geometry.End.X, (float)geometry.End.Y);

        var path = new SKPath();
        path.MoveTo(_startPoint);
        path.CubicTo(
            (float)geometry.Control1.X,
            (float)geometry.Control1.Y,
            (float)geometry.Control2.X,
            (float)geometry.Control2.Y,
            _endPoint.X,
            _endPoint.Y);
        _bezierPath = path;

        Center = new SKPoint((_startPoint.X + _endPoint.X) / 2f, (_startPoint.Y + _endPoint.Y) / 2f);
        P0 = _startPoint;
        P1 = _endPoint;

        UpdateBezierMarkers(geometry);
    }

    private void UpdateBezierMarkers(WireGeometry geometry)
    {
        if (Style?.Stroke?.StartArrow is { } startArrow && startArrow.ArrowType != ArrowType.None)
        {
            var angle = Math.Atan2(geometry.Start.Y - geometry.Control1.Y, geometry.Start.X - geometry.Control1.X);
            var marker = CreateArrowMarker(geometry.Start.X, geometry.Start.Y, angle, Style, startArrow);
            marker.UpdateStyle();
            StartMarker = marker;
        }
        else
        {
            StartMarker = null;
        }

        if (Style?.Stroke?.EndArrow is { } endArrow && endArrow.ArrowType != ArrowType.None)
        {
            var angle = Math.Atan2(geometry.End.Y - geometry.Control2.Y, geometry.End.X - geometry.Control2.X);
            var marker = CreateArrowMarker(geometry.End.X, geometry.End.Y, angle, Style, endArrow);
            marker.UpdateStyle();
            EndMarker = marker;
        }
        else
        {
            EndMarker = null;
        }
    }

    public override void OnDraw(object? dc, double zoom)
    {
        if (Wire.RendererKey == WireRendererKeys.Bezier)
        {
            if (dc is not SKCanvas canvas || _bezierPath is null)
            {
                return;
            }

            if (Wire.IsStroked && Stroke is { })
            {
                canvas.DrawPath(_bezierPath, Stroke);
            }

            if (Style?.Stroke?.StartArrow is { } startArrow && startArrow.ArrowType != ArrowType.None)
            {
                StartMarker?.Draw(dc);
            }

            if (Style?.Stroke?.EndArrow is { } endArrow && endArrow.ArrowType != ArrowType.None)
            {
                EndMarker?.Draw(dc);
            }

            return;
        }

        base.OnDraw(dc, zoom);
    }
}
