using Core2D.Shapes;

namespace Core2D.Renderer
{
    public interface IPathDrawNode : IDrawNode
    {
        PathShape Path { get; set; }
    }
}
