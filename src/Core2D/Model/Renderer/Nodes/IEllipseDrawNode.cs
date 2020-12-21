#nullable disable
using Core2D.ViewModels.Shapes;

namespace Core2D.Model.Renderer.Nodes
{
    public interface IEllipseDrawNode : IDrawNode
    {
        EllipseShapeViewModel Ellipse { get; set; }
    }
}
