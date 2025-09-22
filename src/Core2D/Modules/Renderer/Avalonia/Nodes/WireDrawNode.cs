// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.Model.Style;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using A = Avalonia;
using AM = Avalonia.Media;

namespace Core2D.Modules.Renderer.Avalonia.Nodes;

internal class WireDrawNode : LineDrawNode, IWireDrawNode
{
    public WireShapeViewModel Wire { get; set; }
    private AM.PathGeometry? _bezierGeometry;
    private WireGeometry _wireGeometry;
    private A.Point _startPoint;
    private A.Point _endPoint;

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
            _bezierGeometry = null;
            base.UpdateGeometry();
            return;
        }

        _wireGeometry = geometry;
        ScaleThickness = Wire.State.HasFlag(ShapeStateFlags.Thickness);
        ScaleSize = Wire.State.HasFlag(ShapeStateFlags.Size);

        _startPoint = new A.Point(geometry.Start.X, geometry.Start.Y);
        _endPoint = new A.Point(geometry.End.X, geometry.End.Y);

        var figure = new AM.PathFigure
        {
            StartPoint = _startPoint,
            IsFilled = false,
            IsClosed = false,
            Segments = new AM.PathSegments
            {
                new AM.BezierSegment
                {
                    Point1 = new A.Point(geometry.Control1.X, geometry.Control1.Y),
                    Point2 = new A.Point(geometry.Control2.X, geometry.Control2.Y),
                    Point3 = _endPoint
                }
            }
        };

        var path = new AM.PathGeometry
        {
            Figures = new AM.PathFigures { figure }
        };
        _bezierGeometry = path;

        Center = new A.Point((_startPoint.X + _endPoint.X) / 2.0, (_startPoint.Y + _endPoint.Y) / 2.0);
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
            if (dc is not AM.DrawingContext context || _bezierGeometry is null)
            {
                return;
            }

            if (Wire.IsStroked && Stroke is { })
            {
                context.DrawGeometry(null, Stroke, _bezierGeometry);
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
