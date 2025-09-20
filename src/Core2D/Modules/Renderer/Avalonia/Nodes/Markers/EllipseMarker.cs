// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using AP = Avalonia.Platform;
using AM = Avalonia.Media;

namespace Core2D.Modules.Renderer.Avalonia.Nodes.Markers;

internal class EllipseMarker : MarkerBase
{
    public AM.EllipseGeometry? EllipseGeometry { get; set; }

    public override void Draw(object? dc)
    {
        if (dc is not AM.DrawingContext context)
        {
            return;
        }

        if (ShapeViewModel is { } && EllipseGeometry is { })
        {
            using var rotationDisposable = context.PushTransform(Rotation);
            context.DrawGeometry(ShapeViewModel.IsFilled ? Brush : null, ShapeViewModel.IsStroked ? Pen : null, EllipseGeometry);
        }
    }
}
