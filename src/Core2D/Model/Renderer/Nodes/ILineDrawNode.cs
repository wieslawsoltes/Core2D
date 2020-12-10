using Core2D.ViewModels.Shapes;

namespace Core2D.Model.Renderer.Nodes
{
    public interface ILineDrawNode : IDrawNode
    {
        LineShapeViewModel Line { get; set; }
        public IMarker StartMarker { get; set; }
        public IMarker EndMarker { get; set; }
    }
}
