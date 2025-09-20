// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using Core2D.ViewModels.Style;

namespace Core2D.Model.Renderer.Nodes;

public interface IMarker
{
    ArrowStyleViewModel? Style { get; set; }

    void Draw(object? dc);

    void UpdateStyle();
}
