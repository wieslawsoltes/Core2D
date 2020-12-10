using Core2D.ViewModels.Shapes;

namespace Core2D.Model.Renderer.Nodes
{
    public interface ITextDrawNode : IDrawNode
    {
        TextShapeViewModel Text { get; set; }
        string BoundText { get; set; }
    }
}
