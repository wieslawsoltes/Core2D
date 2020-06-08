using System;
using Core2D.Style;

namespace Core2D.Renderer
{
    public interface IDrawNode : IDisposable
    {
        IShapeStyle Style { get; set; }
        bool ScaleThickness { get; set; }
        bool ScaleSize { get; set; }
        void UpdateGeometry();
        void UpdateStyle();
        void Draw(object dc, double zoom);
        void OnDraw(object dc, double zoom);
    }
}
