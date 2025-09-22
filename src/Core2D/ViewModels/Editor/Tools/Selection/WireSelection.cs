// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Editor.Tools.Selection;

public class WireSelection : LineSelection
{
    public WireShapeViewModel Wire { get; }

    public WireSelection(IServiceProvider? serviceProvider, LayerContainerViewModel layer, WireShapeViewModel shape, ShapeStyleViewModel style)
        : base(serviceProvider, layer, shape, style)
    {
        Wire = shape;
    }
}
