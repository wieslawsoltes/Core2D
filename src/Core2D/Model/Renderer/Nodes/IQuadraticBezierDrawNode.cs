using Core2D.Shapes;

namespace Core2D.Renderer
{
    public interface IQuadraticBezierDrawNode : IDrawNode
    {
        QuadraticBezierShapeViewModel QuadraticBezier { get; set; }
    }
}
