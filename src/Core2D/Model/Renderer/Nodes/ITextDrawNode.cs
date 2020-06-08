using Core2D.Shapes;

namespace Core2D.Renderer
{
    public interface ITextDrawNode : IDrawNode
    {
        ITextShape Text { get; set; }
        string BoundText { get; set; }
    }
}
