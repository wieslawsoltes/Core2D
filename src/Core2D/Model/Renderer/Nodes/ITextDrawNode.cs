using Core2D.Shapes;

namespace Core2D.Renderer
{
    public interface ITextDrawNode : IDrawNode
    {
        TextShape Text { get; set; }
        string BoundText { get; set; }
    }
}
