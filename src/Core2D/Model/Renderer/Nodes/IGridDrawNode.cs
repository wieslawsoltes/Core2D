// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
namespace Core2D.Model.Renderer.Nodes;

public interface IGridDrawNode : IDrawNode
{
    IGrid Grid { get; set; }

    double X { get; set; }

    double Y { get; set; }

    double Width { get; set; }

    double Height { get; set; }
}
