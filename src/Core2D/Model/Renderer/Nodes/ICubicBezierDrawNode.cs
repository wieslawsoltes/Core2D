using Core2D.Shapes;

namespace Core2D.Renderer
{
    public interface ICubicBezierDrawNode : IDrawNode
    {
        CubicBezierShapeViewModel CubicBezier { get; set; }
    }
}
