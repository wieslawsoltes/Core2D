#nullable disable
using Core2D.ViewModels.Shapes;

namespace Core2D.Model.Renderer.Nodes
{
    public interface IQuadraticBezierDrawNode : IDrawNode
    {
        QuadraticBezierShapeViewModel QuadraticBezier { get; set; }
    }
}
