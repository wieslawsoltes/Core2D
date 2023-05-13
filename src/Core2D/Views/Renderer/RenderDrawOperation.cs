#nullable enable
using Avalonia;
using Avalonia.Media;
using Avalonia.Rendering.SceneGraph;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Renderer.Presenters;

namespace Core2D.Views.Renderer;

public class RenderDrawOperation : ICustomDrawOperation
{
    private static readonly IContainerPresenter s_editorPresenter = new EditorPresenter();

    private static readonly IContainerPresenter s_templatePresenter = new TemplatePresenter();

    private static readonly IContainerPresenter s_exportPresenter = new ExportPresenter();

    // ReSharper disable once UnusedParameter.Local
    private static void DrawData(RenderState renderState, object context)
    {
        if (renderState.Container is null || renderState.DataFlow is null)
        {
            return;
        }

        var db = (object) renderState.Container.Properties;
        var record = (object?) renderState.Container.Record;

        if (renderState.Container is PageContainerViewModel page)
        {
            renderState.DataFlow.Bind(page.Template, db, record);
        }

        renderState.DataFlow.Bind(renderState.Container, db, record);
    }

    private static void DrawTemplate(RenderState renderState, object context)
    {
        if (renderState.Container is null || renderState.Renderer is null)
        {
            return;
        }
 
        s_templatePresenter.Render(context, renderState.Renderer, renderState.Selection, renderState.Container, 0.0, 0.0);
 
        if (renderState.Container is PageContainerViewModel page)
        {
            page.Template?.Invalidate();
        }
    }

    private static void DrawEditor(RenderState renderState, object context)
    {
        if (renderState.Container is null || renderState.Renderer is null)
        {
            return;
        }
 
        s_editorPresenter.Render(context, renderState.Renderer, renderState.Selection, renderState.Container, 0.0, 0.0);

        renderState.Container?.Invalidate();
        renderState.Renderer.State?.PointStyle?.Invalidate();
        renderState.Renderer.State?.SelectedPointStyle?.Invalidate();
    }

    private static void DrawExport(RenderState renderState, object context)
    {
        if (renderState.Container is null || renderState.Renderer is null)
        {
            return;
        }

        s_exportPresenter.Render(context, renderState.Renderer, renderState.Selection, renderState.Container, 0.0, 0.0);
    }

    public static void Draw(RenderState? renderState, object context)
    {
        switch (renderState?.RenderType)
        {
            case RenderType.None:
                break;
            case RenderType.Data:
                DrawData(renderState, context);
                break;
            case RenderType.Template:
                DrawTemplate(renderState, context);
                break;
            case RenderType.Editor:
                DrawEditor(renderState, context);
                break;
            case RenderType.Export:
                DrawExport(renderState, context);
                break;
        }
    }

    public RenderState? RenderState { get; set; }

    public Rect Bounds { get; set; }

    public void Dispose()
    {
    }

    public bool HitTest(Point p) => false;

    public bool Equals(ICustomDrawOperation? other) => false;

    public void Render(ImmediateDrawingContext context)
    {
        Draw(RenderState, context);
    }
}
