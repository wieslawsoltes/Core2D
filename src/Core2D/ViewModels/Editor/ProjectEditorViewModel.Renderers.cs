// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.Collections.Generic;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;

namespace Core2D.ViewModels.Editor;

public partial class ProjectEditorViewModel
{
    private readonly Dictionary<FrameContainerViewModel, IShapeRenderer> _containerRenderers = new();
    private IImageCache? _currentImageCache;

    private IShapeRenderer? EnsureRenderer(FrameContainerViewModel? container)
    {
        if (container is null)
        {
            return null;
        }

        if (_containerRenderers.TryGetValue(container, out var existing))
        {
            if (!ReferenceEquals(container.Renderer, existing))
            {
                container.Renderer = existing;
            }

            return existing;
        }

        var created = ServiceProvider?.GetService<IShapeRenderer>();
        if (created is null)
        {
            return null;
        }

        ApplyImageCache(created, _currentImageCache);
        _containerRenderers[container] = created;
        container.Renderer = created;
        return created;
    }

    private static void SetImageCache(IShapeRenderer renderer, IImageCache? cache)
    {
        renderer.ClearCache();

        if (renderer.State is { } state)
        {
            state.ImageCache = cache;
        }
    }

    private void ApplyImageCache(IShapeRenderer? renderer, IImageCache? cache)
    {
        if (renderer is null)
        {
            return;
        }

        SetImageCache(renderer, cache);
    }

    private void ApplyImageCache(IShapeRenderer? renderer)
    {
        ApplyImageCache(renderer, _currentImageCache);
    }

    private void ReleaseRenderer(FrameContainerViewModel? container)
    {
        if (container is null)
        {
            return;
        }

        if (_containerRenderers.Remove(container, out var renderer))
        {
            renderer.ClearCache();
        }

        container.Renderer = null;
    }

    private void ReleaseAllRenderers()
    {
        foreach (var pair in _containerRenderers)
        {
            pair.Value.ClearCache();
            pair.Key.Renderer = null;
        }

        _containerRenderers.Clear();
    }
}
