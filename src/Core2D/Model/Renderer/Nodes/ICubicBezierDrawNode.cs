using Core2D.Shapes;

namespace Core2D.Renderer
{
    public interface ICubicBezierDrawNode : IDrawNode
    {
        ICubicBezierShape CubicBezier { get; set; }
    }
}
