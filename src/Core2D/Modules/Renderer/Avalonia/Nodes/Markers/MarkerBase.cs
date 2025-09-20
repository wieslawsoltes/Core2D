// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using A = Avalonia;
using AM = Avalonia.Media;

namespace Core2D.Modules.Renderer.Avalonia.Nodes.Markers;

internal abstract class MarkerBase : IMarker
{
    public BaseShapeViewModel? ShapeViewModel { get; set; }
    public ShapeStyleViewModel? ShapeStyleViewModel { get; set; }
    public ArrowStyleViewModel? Style { get; set; }
    public AM.IBrush? Brush { get; set; }
    public AM.IPen? Pen { get; set; }
    public A.Matrix Rotation { get; set; }
    public A.Point Point { get; set; }

    public abstract void Draw(object? dc);

    public virtual void UpdateStyle()
    {
        if (ShapeStyleViewModel?.Fill?.Color is { })
        {
            Brush = AvaloniaDrawUtil.ToBrush(ShapeStyleViewModel.Fill.Color);
        }

        if (ShapeStyleViewModel?.Stroke is { })
        {
            Pen = AvaloniaDrawUtil.ToPen(ShapeStyleViewModel, ShapeStyleViewModel.Stroke.Thickness);
        }
    }
}
