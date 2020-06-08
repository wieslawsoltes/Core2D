using Core2D.Shapes;

namespace Core2D.Renderer
{
    public interface IRectangleDrawNode : ITextDrawNode
    {
        IRectangleShape Rectangle { get; set; }
    }
}
