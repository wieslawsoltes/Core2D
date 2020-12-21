#nullable disable
using Core2D.ViewModels.Shapes;

namespace Core2D.Model.Renderer.Nodes
{
    public interface IArcDrawNode : IDrawNode
    {
        ArcShapeViewModel Arc { get; set; }
    }
}
