using Core2D.Shapes;

namespace Core2D.Renderer
{
    public interface IPathDrawNode : IDrawNode
    {
        IPathShape Path { get; set; }
    }
}
