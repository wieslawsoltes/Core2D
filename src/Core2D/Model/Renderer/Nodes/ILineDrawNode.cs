// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using Core2D.ViewModels.Shapes;

namespace Core2D.Model.Renderer.Nodes;

public interface ILineDrawNode : IDrawNode
{
    LineShapeViewModel Line { get; set; }

    public IMarker? StartMarker { get; set; }

    public IMarker? EndMarker { get; set; }
}