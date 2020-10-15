using Core2D.Shapes;

namespace Core2D.Renderer
{
    public interface IRectangleDrawNode : ITextDrawNode
    {
        RectangleShape Rectangle { get; set; }
    }
}
