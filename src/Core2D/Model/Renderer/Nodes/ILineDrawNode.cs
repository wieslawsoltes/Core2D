// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using Core2D.ViewModels.Shapes;

namespace Core2D.Model.Renderer.Nodes;

public interface ILineDrawNode : IDrawNode
{
    LineShapeViewModel Line { get; set; }

    public IMarker? StartMarker { get; set; }

    public IMarker? EndMarker { get; set; }
}
