// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using SkiaSharp;

namespace Core2D.Modules.Renderer.SkiaSharp.Nodes;

internal class ArcDrawNode : DrawNode, IArcDrawNode
{
    public ArcShapeViewModel Arc { get; set; }
    public SKPath? Geometry { get; set; }

    public ArcDrawNode(ArcShapeViewModel arc, ShapeStyleViewModel? style)
    {
        Style = style;
        Arc = arc;
        UpdateGeometry();
    }

    public sealed override void UpdateGeometry()
    {
        ScaleThickness = Arc.State.HasFlag(ShapeStateFlags.Thickness);
        ScaleSize = Arc.State.HasFlag(ShapeStateFlags.Size);
        Geometry = PathGeometryConverter.ToSKPath(Arc);
        if (Geometry is { })
        {
            Center = new SKPoint(Geometry.Bounds.MidX, Geometry.Bounds.MidY);
        }
        else
        {
            Center = SKPoint.Empty;
        }
    }

    public override void OnDraw(object? dc, double zoom)
    {
        if (dc is not SKCanvas canvas)
        {
            return;
        }

        if (Arc.IsFilled)
        {
            canvas.DrawPath(Geometry, Fill);
        }

        if (Arc.IsStroked)
        {
            canvas.DrawPath(Geometry, Stroke);
        }
    }
}
