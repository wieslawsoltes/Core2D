using System;
using Core2D.ViewModels.Style;

namespace Core2D.Model.Renderer.Nodes
{
    public interface IDrawNode : IDisposable
    {
        ShapeStyleViewModel Style { get; set; }
        bool ScaleThickness { get; set; }
        bool ScaleSize { get; set; }
        void UpdateGeometry();
        void UpdateStyle();
        void Draw(object dc, double zoom);
        void OnDraw(object dc, double zoom);
    }
}
