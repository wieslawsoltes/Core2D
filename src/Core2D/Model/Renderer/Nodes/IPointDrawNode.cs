using Core2D.ViewModels.Shapes;

namespace Core2D.Model.Renderer.Nodes
{
    public interface IPointDrawNode : IDrawNode
    {
        PointShapeViewModel Point { get; set; }
        double PointSize { get; set; }
    }
}
