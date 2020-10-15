using Core2D.Shapes;

namespace Core2D.Renderer
{
    public interface IArcDrawNode : IDrawNode
    {
        ArcShape Arc { get; set; }
    }
}
