#nullable enable
using A = Avalonia;
using AP = Avalonia.Platform;

namespace Core2D.Modules.Renderer.Avalonia.Media;

internal static class DrawingContextImplExtensions
{
    public static PushedState PushPostTransform(this AP.IDrawingContextImpl context, A.Matrix matrix) 
        => PushSetTransform(context, context.Transform * matrix);

    public static PushedState PushPreTransform(this AP.IDrawingContextImpl context, A.Matrix matrix) 
        => PushSetTransform(context, matrix * context.Transform);

    public static PushedState PushSetTransform(this AP.IDrawingContextImpl context, A.Matrix matrix)
    {
        var oldMatrix = context.Transform;
        context.Transform = matrix;
        return new PushedState(context, oldMatrix);
    }
}