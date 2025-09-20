// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using Core2D.Model.Renderer.Nodes;
using Core2D.Spatial;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using A = Avalonia;
using AM = Avalonia.Media;
using AP = Avalonia.Platform;

namespace Core2D.Modules.Renderer.Avalonia.Nodes;

internal class PointDrawNode : DrawNode, IPointDrawNode
{
    public PointShapeViewModel Point { get; set; }
    public double PointSize { get; set; }
    public A.Rect Rect { get; set; }

    public PointDrawNode(PointShapeViewModel point, ShapeStyleViewModel? pointStyleViewModel, double pointSize)
    {
        Style = pointStyleViewModel;
        Point = point;
        PointSize = pointSize;
        UpdateGeometry();
    }

    public sealed override void UpdateGeometry()
    {
        ScaleThickness = true; // Point.State.HasFlag(ShapeStateFlags.Thickness);
        ScaleSize = true; // Point.State.HasFlag(ShapeStateFlags.Size);
        var rect2 = Rect2.FromPoints(Point.X - PointSize, Point.Y - PointSize, Point.X + PointSize, Point.Y + PointSize);
        Rect = new A.Rect(rect2.X, rect2.Y, rect2.Width, rect2.Height);
        Center = Rect.Center;
    }

    public override void OnDraw(object? dc, double zoom)
    {
        if (dc is not AM.DrawingContext context)
        {
            return;
        }

        context.DrawRectangle(Fill, null, Rect);
        context.DrawRectangle(null, Stroke, Rect);
    }
}
