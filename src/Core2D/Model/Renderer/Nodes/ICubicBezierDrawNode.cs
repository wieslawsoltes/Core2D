using Core2D.Shapes;

namespace Core2D.Renderer
{
    public interface ICubicBezierDrawNode : IDrawNode
    {
        CubicBezierShape CubicBezier { get; set; }
    }
}
