#nullable disable
using Core2D.ViewModels.Shapes;

namespace Core2D.Model.Renderer.Nodes
{
    public interface IRectangleDrawNode : IDrawNode
    {
        RectangleShapeViewModel Rectangle { get; set; }
    }
}
