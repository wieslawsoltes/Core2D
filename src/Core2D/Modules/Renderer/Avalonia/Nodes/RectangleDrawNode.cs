// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.Spatial;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using A = Avalonia;
using AM = Avalonia.Media;
using AP = Avalonia.Platform;

namespace Core2D.Modules.Renderer.Avalonia.Nodes;

internal class RectangleDrawNode : DrawNode, IRectangleDrawNode
{
    public RectangleShapeViewModel Rectangle { get; set; }
    public A.Rect Rect { get; set; }

    public RectangleDrawNode(RectangleShapeViewModel rectangle, ShapeStyleViewModel? style)
    {
        Style = style;
        Rectangle = rectangle;
        UpdateGeometry();
    }

    public sealed override void UpdateGeometry()
    {
        ScaleThickness = Rectangle.State.HasFlag(ShapeStateFlags.Thickness);
        ScaleSize = Rectangle.State.HasFlag(ShapeStateFlags.Size);
        
        if (Rectangle.TopLeft is { } && Rectangle.BottomRight is { })
        {
            var rect2 = Rect2.FromPoints(Rectangle.TopLeft.X, Rectangle.TopLeft.Y, Rectangle.BottomRight.X, Rectangle.BottomRight.Y);
            Rect = new A.Rect(rect2.X, rect2.Y, rect2.Width, rect2.Height);
            Center = Rect.Center;
        }
        else
        {
            Rect = new A.Rect();
            Center = new A.Point();
        }
    }

    public override void OnDraw(object? dc, double zoom)
    {
        if (dc is not AM.DrawingContext context)
        {
            return;
        }

        if (Rectangle.IsFilled)
        {
            context.DrawRectangle(Fill, null, Rect);
        }

        if (Rectangle.IsStroked)
        {
            context.DrawRectangle(null, Stroke, Rect);
        }
    }
}
