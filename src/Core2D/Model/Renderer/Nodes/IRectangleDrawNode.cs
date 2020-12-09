using Core2D.Shapes;

namespace Core2D.Renderer
{
    public interface IRectangleDrawNode : IDrawNode
    {
        RectangleShapeViewModel Rectangle { get; set; }
    }
}
