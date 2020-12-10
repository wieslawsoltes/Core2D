using Core2D.ViewModels.Shapes;

namespace Core2D.Model.Renderer.Nodes
{
    public interface IPathDrawNode : IDrawNode
    {
        PathShapeViewModel Path { get; set; }
    }
}
