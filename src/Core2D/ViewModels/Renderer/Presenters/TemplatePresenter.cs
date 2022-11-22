﻿#nullable enable
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;

namespace Core2D.ViewModels.Renderer.Presenters;

public class TemplatePresenter : IContainerPresenter
{
    public void Render(object? dc, IShapeRenderer? renderer, ISelection? selection, FrameContainerViewModel? container, double dx, double dy)
    {
        if (dc is null || renderer is null || container is null)
        {
            return;
        }

        if (container is PageContainerViewModel page && page.Template is { })
        {
            renderer.Fill(dc, dx, dy, page.Template.Width, page.Template.Height, page.Template.Background);
            renderer.Grid(dc, page.Template, 0, 0, page.Template.Width, page.Template.Height);
            DrawContainer(dc, renderer, selection, page.Template);
        }
    }

    private void DrawContainer(object dc, IShapeRenderer renderer, ISelection? selection, FrameContainerViewModel container)
    {
        foreach (var layer in container.Layers)
        {
            if (layer.IsVisible)
            {
                DrawLayer(dc, renderer, selection, layer);
            }
        }
    }

    private void DrawLayer(object dc, IShapeRenderer renderer, ISelection? selection, LayerContainerViewModel layer)
    {
        if (renderer.State is null)
        {
            return;
        }

        foreach (var shape in layer.Shapes)
        {
            if (shape.State.HasFlag(renderer.State.DrawShapeState))
            {
                shape.DrawShape(dc, renderer, selection);
            }
        }

        foreach (var shape in layer.Shapes)
        {
            if (shape.State.HasFlag(renderer.State.DrawShapeState))
            {
                shape.DrawPoints(dc, renderer, selection);
            }
        }
    }
}
