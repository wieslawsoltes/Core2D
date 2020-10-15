using Core2D.Shapes;

namespace Core2D.Renderer
{
    public interface IEllipseDrawNode : ITextDrawNode
    {
        EllipseShape Ellipse { get; set; }
    }
}
