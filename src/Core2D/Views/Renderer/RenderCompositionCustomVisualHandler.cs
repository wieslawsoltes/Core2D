using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Rendering.Composition;

namespace Core2D.Views.Renderer;

public class RenderCompositionCustomVisualHandler : CompositionCustomVisualHandler
{
    private RenderState? _drawState;
    
    public override void OnMessage(object message)
    {
        if (message is RenderState drawState)
        {
            _drawState = drawState;
            Invalidate();
            // RegisterForNextAnimationFrameUpdate();
        }
    }

    public override void OnAnimationFrameUpdate()
    {
        // Invalidate();
        // RegisterForNextAnimationFrameUpdate();
    }

    public override void OnRender(ImmediateDrawingContext drawingContext)
    {
        // drawingContext.DrawRectangle(new ImmutableSolidColorBrush(Colors.Cyan), null, GetRenderBounds());
        if (_drawState is { })
        {
            RenderDrawOperation.Draw(_drawState, drawingContext);
        }
    }
}
