using Core2D.Shapes;

namespace Core2D.Renderer
{
    public interface IEllipseDrawNode : ITextDrawNode
    {
        IEllipseShape Ellipse { get; set; }
    }
}
