using Core2D.ViewModels.Shapes;

namespace Core2D.Model.Renderer.Nodes
{
    public interface IArcDrawNode : IDrawNode
    {
        ArcShapeViewModelViewModel Arc { get; set; }
    }
}
