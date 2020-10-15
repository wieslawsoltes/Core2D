using Core2D.Shapes;

namespace Core2D.Renderer
{
    public interface ILineDrawNode : IDrawNode
    {
        LineShape Line { get; set; }
        public IMarker StartMarker { get; set; }
        public IMarker EndMarker { get; set; }
    }
}
