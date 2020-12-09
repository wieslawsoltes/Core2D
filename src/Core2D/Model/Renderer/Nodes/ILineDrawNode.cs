using Core2D.Shapes;

namespace Core2D.Renderer
{
    public interface ILineDrawNode : IDrawNode
    {
        LineShapeViewModel Line { get; set; }
        public IMarker StartMarker { get; set; }
        public IMarker EndMarker { get; set; }
    }
}
