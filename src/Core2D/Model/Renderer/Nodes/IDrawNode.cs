// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using Core2D.ViewModels.Style;

namespace Core2D.Model.Renderer.Nodes;

public interface IDrawNode : IDisposable
{
    ShapeStyleViewModel? Style { get; set; }

    bool ScaleThickness { get; set; }

    bool ScaleSize { get; set; }

    void UpdateGeometry();

    void UpdateStyle();

    void Draw(object? dc, double zoom);

    void OnDraw(object? dc, double zoom);
}
