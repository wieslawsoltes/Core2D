// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using Core2D.ViewModels.Containers;

namespace Core2D.Model.Renderer;

public interface IContainerPresenter
{
    void Render(object? dc, IShapeRenderer? renderer, ISelection? selection, FrameContainerViewModel? container, double dx, double dy);
}
