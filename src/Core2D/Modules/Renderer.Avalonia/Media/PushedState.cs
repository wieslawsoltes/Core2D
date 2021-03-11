#nullable disable
using System;
using A = Avalonia;
using AP = Avalonia.Platform;

namespace Core2D.Modules.Renderer.Media
{
    internal readonly struct PushedState : IDisposable
    {
        private readonly AP.IDrawingContextImpl _context;
        private readonly A.Matrix _matrix;

        public PushedState(AP.IDrawingContextImpl context, A.Matrix matrix = default)
        {
            _context = context;
            _matrix = matrix;
        }

        public void Dispose()
        {
            _context.Transform = _matrix;
        }
    }
}
