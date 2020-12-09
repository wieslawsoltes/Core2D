using Core2D.Shapes;

namespace Core2D.Renderer
{
    public interface IPointDrawNode : IDrawNode
    {
        PointShapeViewModel Point { get; set; }
        double PointSize { get; set; }
    }
}
