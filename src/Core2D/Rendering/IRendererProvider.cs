// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.Collections.Immutable;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Renderer;

namespace Core2D.Rendering;

public interface IRendererProvider
{
    ImmutableArray<RendererOption> Options { get; }

    RendererOption? GetOption(string id);

    IShapeRenderer CreateRenderer(string id);
}
