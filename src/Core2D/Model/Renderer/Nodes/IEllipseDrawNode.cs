using Core2D.Shapes;

namespace Core2D.Renderer
{
    public interface IEllipseDrawNode : IDrawNode
    {
        EllipseShape Ellipse { get; set; }
    }
}
