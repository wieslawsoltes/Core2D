// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using SkiaSharp;

namespace Core2D.Modules.Renderer.SkiaSharp.Nodes.Markers;

internal class EllipseMarker : MarkerBase
{
    public SKRect Rect { get; set; }

    public override void Draw(object? dc)
    {
        if (dc is not SKCanvas canvas)
        {
            return;
        }

        if (ShapeViewModel is null)
        {
            return;
        }
        
        var count = canvas.Save();
        canvas.SetMatrix(MatrixHelper.Multiply(Rotation, canvas.TotalMatrix));

        if (ShapeViewModel.IsFilled)
        {
            canvas.DrawOval(Rect, Brush);
        }

        if (ShapeViewModel.IsStroked)
        {
            canvas.DrawOval(Rect, Pen);
        }

        canvas.RestoreToCount(count);
    }
}
