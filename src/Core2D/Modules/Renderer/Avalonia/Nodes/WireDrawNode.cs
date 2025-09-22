// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.Modules.Renderer.Avalonia.Nodes;

internal class WireDrawNode : LineDrawNode, IWireDrawNode
{
    public WireShapeViewModel Wire { get; set; }

    public WireDrawNode(WireShapeViewModel wire, ShapeStyleViewModel? style)
        : base(wire, style)
    {
        Wire = wire;
    }
}
