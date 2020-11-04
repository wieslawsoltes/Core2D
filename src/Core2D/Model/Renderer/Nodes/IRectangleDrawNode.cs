using Core2D.Shapes;

namespace Core2D.Renderer
{
    public interface IRectangleDrawNode : IDrawNode
    {
        RectangleShape Rectangle { get; set; }
    }
}
